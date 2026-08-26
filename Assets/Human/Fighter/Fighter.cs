using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fighter : Human
{
    Rigidbody2D rb;
    public Sprite[] dashSprite;//歩く用のスプライトを入れる
    [SerializeField] AudioClip upperSE;
    [SerializeField] AudioClip kawasuSE;
    [SerializeField] GameObject dashPar;

    void Start()
    {
        StartSetup();
        HumanSetUp();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Updating();
    }
    public override IEnumerator AtkMotion(Monsters target)//攻撃アニメーションなど
    {
        if (atkSprites.Length < 2)
        {
            Debug.LogWarning("攻撃スプライトが足りません");
            yield break;
        }
        mode = Mode.atk;

        spriteRenderer.sprite = atkSprites[0];
        yield return Wait(0.2f);

        spriteRenderer.sprite = atkSprites[1];
        Punch(target, 0.33f);
        yield return Wait(0.05f);

        spriteRenderer.sprite = atkSprites[0];
        yield return Wait(0.1f);

        spriteRenderer.sprite = atkSprites[1];
        Punch(target, 0.33f);
        yield return Wait(0.05f);

        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.4f);

        spriteRenderer.sprite = atkSprites[3];
        Punch(target, 0.6f);
        yield return Wait(0.1f);

        spriteRenderer.sprite = atkSprites[0];
        yield return Wait(0.4f);

        float upperDamageRte = 0.6f;

        if (hp > 0 && target.hp > 0)
        {
            AudioManager.PlaySE(upperSE, 1f);
            spriteRenderer.sprite = atkSprites[4];
            Punch(target, 0.6f);//アッパー
            float upperPower = 9;

            if (target.hp - (atk * upperDamageRte * atkRate) > 0)
            {
                target.GetComponent<Rigidbody2D>().AddForce(new Vector2(-0.1f, 1) * upperPower, ForceMode2D.Impulse);
            }
            else
            {
                Punch(target, upperDamageRte);//アッパー
                GameObject obj = new GameObject("MyObject");
                obj.transform.position = target.transform.position;
                SpriteRenderer objSpr = obj.AddComponent<SpriteRenderer>();
                SpriteRenderer targetSpr = target.GetComponent<SpriteRenderer>();
                objSpr.sprite = targetSpr.sprite;
                objSpr.material = targetSpr.material;
                objSpr.sortingOrder = targetSpr.sortingOrder;
                objSpr.sortingLayerID = targetSpr.sortingLayerID;

                target.GetComponent<SpriteRenderer>().enabled = false;

                obj.AddComponent<Rigidbody2D>().AddForce(new Vector2(-0.1f, 1) * upperPower, ForceMode2D.Impulse);
                obj.GetComponent<Rigidbody2D>().angularVelocity = upperPower * 100;
                obj.GetComponent<Rigidbody2D>().gravityScale = 0;

                Destroy(obj, 2f);
            }
            rb.AddForce(Vector2.up * upperPower, ForceMode2D.Impulse);
            float duration = 0.5f;
            int i = 0;
            float timer = 0f;
            float frameTime = 0.1f;
            int hit = 6;

            for (int j = 0; j < hit; j++)
            {
                spriteRenderer.sprite = atkSprites[4 + i];
                i = (i + 1) % 2;
                Punch(target, upperDamageRte / hit);//アッパー
                yield return new WaitForSeconds(frameTime);
            }
            spriteRenderer.sprite = atkSprites[0];
            yield return Wait(1.3f, 2);
        }

        mode = Mode.move;
        yield break;
    }

    void Punch(Monsters target, float rate)
    {
        Attack(target, atk * rate, true, GetEffect("FighterATKSlow", true, StatusManager.StatusType.atkSpdRate, 0.2f, -1));
        Attack(target, 0, true, GetEffect("FighterSPDSlow", true, StatusManager.StatusType.spdRate, 0.1f, -1));

    }

    IEnumerator Dash()
    {
        if (mode == Mode.atk) yield break;
        mode = Mode.atk;

        float upSpd = 15;
        spdRate += upSpd;

        spriteRenderer.sprite = dashSprite[0];
        GameObject _dashpar = Instantiate(dashPar, transform.position, Quaternion.Euler(0, 180, 0), transform);


        float duration = 0.2f;
        while (duration > 0 && DashMoving())
        {
            duration -= Time.deltaTime;
            yield return null;
        }
        spriteRenderer.sprite = dashSprite[1];
        spdRate -= upSpd;

        upSpd = 6;
        spdRate += upSpd;

        duration = 1f;
        float frameTime = 0.15f;
        float timer = 0f;
        int i = 0;

        while (duration > 0 && DashMoving())
        {
            timer += Time.deltaTime;
            duration -= Time.deltaTime;

            if (timer >= frameTime)
            {
                timer = 0f;
                i = (i + 1) % 4;
                spriteRenderer.sprite = dashSprite[2 + i];

                Debug.Log("切り替え: " + (2 + i));
            }

            yield return null;
        }
        _dashpar.GetComponent<ParticleSystem>().Stop();
        Destroy(_dashpar, 1);
        spdRate -= upSpd;
        mode = Mode.move;
        yield break;
    }

    bool DashMoving()
    {
        Vector2 direction;
        if (player == 0)
        {
            direction = Vector2.right;
        }
        else
        {
            direction = -Vector2.right;

        }

        if (!Physics2D.Raycast(transform.position + rayOrigin, direction, 0.8f, enemyLayer))
        {
            transform.Translate(direction * Time.deltaTime * spd * Mathf.Clamp(spdRate, 0, 100), Space.World);
            return true;
        }
        else
        {
            return false;
        }

    }


    public override void Damaged(float damage, Monsters attacker, bool Melee = true, StatusManager newStatus = null)
    {
        if (Melee == false)
        {
            if (mode != Mode.atk)
            {
                AudioManager.PlaySE(kawasuSE, 0.6f);
                StartCoroutine(Dash());
            }
            return;
        }

        if (newStatus != null) ApplyStatus(newStatus);
        if (damage > 0)
        {
            AudioManager.PlaySE(defaultAtkSE, 0.3f);
        }
        else
        {
            return;
        }

        if (hp > 0)
        {
            float _damage = damage / defRate;
            hp -= _damage;
            Text _damageText = Instantiate(damageText, transform.position, Quaternion.identity, canvas.transform).GetComponent<Text>();

            if (_damage >= 1)
            {
                _damageText.text = (damage / defRate).ToString("F0");

            }
            else
            {
                _damageText.text = (damage / defRate).ToString("F1");
            }
        }

    }
}
