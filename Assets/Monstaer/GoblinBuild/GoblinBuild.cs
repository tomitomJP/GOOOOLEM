using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoblinBuild : Monsters
{

    [SerializeField] Slider hpBar;
    public float hpMax = 150;
    [SerializeField] BoxCollider2D boxCollider2D;
    [SerializeField] Sprite[] houseSprites;
    Vector3 startPos;
    bool buildOK = false;
    void Start()
    {
        startPos = transform.position;
        StartSetup();
    }

    // Update is called once per frame
    void Update()
    {
        hp = Mathf.Clamp(hp, -30, hpMax);
        //hpBar.value = hp / hpMax;


        boxCollider2D.enabled = hp > 0;
        //spriteRenderer.enabled = hp > 0;

        if (buildOK && hp <= 0)
        {
            buildOK = false;
            hp = -hpMax;
        }
        if (hp > 0 && !buildOK)
        {
            hp = hpMax * 0.8f;

            buildOK = true;
        }


        SpriteChanger();


    }

    public override void Damaged(float damage, Monsters attacker, bool Melee = true, StatusManager newStatus = null)
    {

        if (damage > 0)
        {
            AudioManager.PlaySE(defaultAtkSE, 0.3f);
            InsHitPar(attacker);
        }
        else
        {
            return;
        }
        StartCoroutine(DamageAnimHouse(player));
        hp -= damage;

        Text _damageText = Instantiate(damageText, transform.position, Quaternion.identity, canvas.transform).GetComponent<Text>();
        _damageText.text = damage.ToString("F0");
    }

    IEnumerator DamageAnimHouse(int playerNum = 0)
    {
        Vector3 A = startPos;
        Vector3 B = startPos + new Vector3(-0.5f, 0);
        Vector3 C = startPos + new Vector3(0.2f, 0);

        if (playerNum == 1)
        {
            B = startPos + new Vector3(0.5f, 0);
            C = startPos + new Vector3(-0.2f, 0);
        }

        float timer = 0;
        float time = 0.1f;
        while (time > timer)
        {
            transform.position = Vector3.Lerp(A, B, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        time = 0.05f;
        while (time > timer)
        {
            transform.position = Vector3.Lerp(B, C, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        time = 0.2f;
        while (time > timer)
        {
            transform.position = Vector3.Lerp(C, A, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = A;
    }

    void SpriteChanger()
    {
        float hpRate = (float)hp / hpMax;

        if (hpRate >= 1.0f)
        {
            spriteRenderer.sprite = houseSprites[0];
        }
        else if (hpRate >= 0.75f)
        {
            spriteRenderer.sprite = houseSprites[1];
        }
        else if (hpRate >= 0.5f)
        {
            spriteRenderer.sprite = houseSprites[2];
        }
        else if (hpRate >= 0.25f)
        {
            spriteRenderer.sprite = houseSprites[3];
        }
        else if (hpRate >= 0f)
        {
            spriteRenderer.sprite = houseSprites[4];

        }
        else if (hpRate >= -5)
        {
            spriteRenderer.sprite = houseSprites[5];
        }
        else if (hpRate >= -10f)
        {
            spriteRenderer.sprite = houseSprites[6];
        }
        else if (hpRate >= -20f)
        {
            spriteRenderer.sprite = houseSprites[7];
        }
        else if (hpRate >= -30f)
        {
            spriteRenderer.sprite = houseSprites[8];
        }

        spriteRenderer.enabled = true;


    }

}
