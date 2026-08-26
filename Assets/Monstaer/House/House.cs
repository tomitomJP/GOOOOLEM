using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class House : Monsters
{
    [SerializeField] Slider hpBar;
    [SerializeField] Slider hpBar_Red;

    [SerializeField] Text hpBarText;
    public float hpMax;
    [SerializeField] Transform monstersPearent;
    [SerializeField] BoxCollider2D boxCollider2D;

    [SerializeField] Sprite[] houseSprites;

    [SerializeField] SpriteRenderer DoorSpr;
    [SerializeField] Sprite[] DoorSprite;
    [SerializeField] GameObject[] castleDamageParticles;
    [SerializeField] GameObject knockBackMon;
    [SerializeField] SpriteRenderer viewSpr;
    [SerializeField] Transform vireTra;
    public bool NonDamage = false;
    int monstaerablock = 1;
    Vector3 startPos;
    void Start()
    {
        startPos = transform.position;
        StartCoroutine(DamageSliderMove());
        StartSetup();
        hpMax = hp;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position + rayOrigin, transform.right * (enemyDistance), Color.yellow, 0.3f);

        hp = Mathf.Clamp(hp, 0, hpMax);
        hpBar.value = hp / hpMax;
        hpBarText.text = Mathf.Floor(hp).ToString() + "HP";

        if (monstersPearent.childCount > 0)
        {
            NonDamage = true;
            gameObject.layer = 18;
        }
        else
        {
            NonDamage = false;
            gameObject.layer = (int)Mathf.Log(myLayer.value, 2);

        }

        if (hp <= 0)
        {
            boxCollider2D.enabled = false;
        }

        if (hp / hpMax <= 0.75f)
        {
            spriteRenderer.sprite = houseSprites[0];
        }
        if (hp / hpMax < -0.5f)
        {
            spriteRenderer.sprite = houseSprites[1];



        }
        if (hp / hpMax <= 0.25f)
        {
            spriteRenderer.sprite = houseSprites[2];
            castleDamageParticles[0].SetActive(true);
            castleDamageParticles[1].SetActive(true);


        }
        if (hp / hpMax <= 0)
        {
            spriteRenderer.sprite = houseSprites[3];
            castleDamageParticles[2].SetActive(true);
            DoorSpr.enabled = false;
            for (int i = 0; i < castleDamageParticles[2].transform.childCount; i++)
            {
                castleDamageParticles[2].transform.GetChild(i).gameObject.SetActive(false);
            }
            spriteRenderer.sprite = houseSprites[4];
            spriteRenderer.color = Color.white;

        }
        viewSpr.sprite = spriteRenderer.sprite;
    }

    public void DoorAnimTrigger()
    {
        KnockBack();

        StartCoroutine(DoorAnim());
    }

    IEnumerator DoorAnim()
    {

        for (int i = 0; i < DoorSprite.Length; i++)
        {
            DoorSpr.sprite = DoorSprite[i];
            yield return Wait(0.1f, 2);
        }
        yield return Wait(0.3f, 2);

        for (int i = 0; i < DoorSprite.Length; i++)
        {
            DoorSpr.sprite = DoorSprite[(DoorSprite.Length - 1) - i];
            yield return Wait(0.1f, 2);
        }

    }

    public override void Damaged(float damage, Monsters attacker, bool Melee = true, StatusManager newStatus = null)

    {
        if (damage > 0)
        {
        }
        else
        {
            return;
        }

        if (NonDamage && damage < 10000)
        {
            // Text _damageText = Instantiate(damageText, transform.position, Quaternion.identity, canvas.transform).GetComponent<Text>();
            // _damageText.text = "NULLILED";
            // _damageText.color = Color.gray;
        }
        else
        {

            damage = Mathf.Min(damage, hpMax * 0.25f);

            InsHitPar(attacker);
            AudioManager.PlaySE(defaultAtkSE, 0.4f);
            StartCoroutine(DamageAnimHouse(player));
            damages.Add(new Damage(hp, damage));
            hp -= damage;

            Text _damageText = Instantiate(damageText, transform.position, Quaternion.identity, canvas.transform).GetComponent<Text>();
            _damageText.text = damage.ToString("F0");
        }

    }


    class Damage
    {
        public float beforeHP;
        public float afterHP;

        public Damage(float nowHP, float damage)
        {
            beforeHP = nowHP;
            afterHP = nowHP - damage;
        }

    }
    List<Damage> damages = new List<Damage>();



    IEnumerator DamageAnimHouse(int playerNum = 0)
    {
        Vector3 A = startPos;
        Vector3 B = startPos + new Vector3(0.5f, 0);
        Vector3 C = startPos + new Vector3(-0.2f, 0);

        if (playerNum == 1)
        {
            B = startPos + new Vector3(-0.5f, 0);
            C = startPos + new Vector3(0.2f, 0);
        }

        float timer = 0;
        float time = 0.1f;
        while (time > timer)
        {
            vireTra.position = Vector3.Lerp(A, B, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        time = 0.05f;
        while (time > timer)
        {
            vireTra.position = Vector3.Lerp(B, C, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        time = 0.2f;
        while (time > timer)
        {
            vireTra.position = Vector3.Lerp(C, A, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }

        vireTra.position = A;
    }

    IEnumerator DamageSliderMove()
    {
        while (true)
        {
            if (damages.Count == 0)
            {
                yield return null;
                continue;
            }

            float A = damages[0].beforeHP / hpMax;
            float B = damages[0].afterHP / hpMax;
            float timer = 0;

            yield return Wait(0.5f, 2);
            while (timer < 0.5f)
            {
                hpBar_Red.value = math.lerp(A, B, timer / 0.5f);
                timer += Time.deltaTime;
                yield return null;
            }

            hpBar_Red.value = B; // 最終的にぴったり合わせる
            damages.RemoveAt(0); // 次のダメージへ
        }
    }


    float knockBackPower = 4;
    void KnockBack()
    {
        Instantiate(knockBackMon, transform.position, Quaternion.identity);
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + rayOrigin, transform.right, enemyDistance, enemyLayer);
        Debug.DrawRay(transform.position + rayOrigin, transform.right * (enemyDistance), Color.yellow, 0.3f);

        foreach (RaycastHit2D hit in hits)
        {

            Attack(hit.collider.gameObject.GetComponent<Monsters>(), 18);
            Rigidbody2D rb = hit.collider.gameObject.GetComponent<Rigidbody2D>();
            GameObject mon = hit.collider.gameObject;

            Vector2 monUp = mon.transform.up;
            Vector2 myLeft = -transform.right;
            Vector2 dire;
            if (player == 0)
            {
                dire = new Vector2(1, 1).normalized;
            }
            else
            {
                dire = new Vector2(-1, 1).normalized;

            }

            rb.AddForce(dire * knockBackPower, ForceMode2D.Impulse);
        }

    }
}
