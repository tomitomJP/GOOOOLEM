using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Shielder : Human
{
    [SerializeField] LayerMask hitLayer;
    // Start is called before the first frame update
    void Start()
    {
        StartSetup();
        HumanSetUp();
    }

    // Update is called once per frame
    void Update()
    {
        Updating();
    }

    public override IEnumerator AtkMotion(Monsters target)//攻撃アニメーションなど
    {
        int num = 0;
        if (atkSprites.Length < 2)
        {
            Debug.LogWarning("攻撃スプライトが足りません");
            yield break;
        }
        mode = Mode.atk;
        Vector2 endPos = target.transform.position;
        spriteRenderer.sprite = atkSprites[0];
        yield return Wait(0.05f);

        spriteRenderer.sprite = atkSprites[1];
        yield return Wait(0.1f, 2);

        if ((target != null && target.gameObject.activeSelf) && Vector2.Distance(target.transform.position, transform.position) < 1)
        {
            Attack(target, atk * 0.45f);
        }
        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.15f, 2);

        spriteRenderer.sprite = atkSprites[3];
        yield return Wait(0.15f, 2);

        spriteRenderer.sprite = atkSprites[4];
        yield return Wait(0.3f, 2);

        float duration = 0.4f;
        yield return StartCoroutine(Charge(duration, endPos));


        duration = 7f;
        int i = 0;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        defRate += 1.5f;
        float animDuration = 0;
        while (duration > 0)
        {

            if (animDuration < 0)
            {
                animDuration = 0.6f;
                i = (i + 1) % 2;
            }
            int j = 7 + i;
            spriteRenderer.sprite = atkSprites[j];

            ShieldingMove();

            yield return null;
            duration -= Time.deltaTime;
            animDuration -= Time.deltaTime;
        }

        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        defRate -= 1.5f;
        mode = Mode.move;

    }

    IEnumerator Charge(float duration, Vector2 endPos)
    {
        float time = duration;
        int i = 0;

        Vector2 startPos = transform.position;
        endPos.y = startPos.y;
        while (duration > 0)
        {
            float t = duration / time;
            transform.position = Vector2.Lerp(startPos, endPos, 1 - t);
            if (i % 2 == 0)
            {
                spriteRenderer.sprite = atkSprites[5];

            }
            else
            {
                spriteRenderer.sprite = atkSprites[6];

            }
            i++;
            yield return null;

            Collider2D targetCol = ChargeRayHit();
            if (targetCol != null)
            {
                if (targetCol.gameObject.TryGetComponent<Monsters>(out Monsters target))
                {
                    KnockBack(target);
                    Attack(target, atk);
                }
                break;
            }
            duration -= Time.deltaTime;
            yield return null;
        }
    }

    Collider2D ChargeRayHit()
    {
        int mask = enemyLayer.value | hitLayer.value;

        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            transform.right,
            0.8f,
            mask
        );

        return hit.collider; // ヒットしなければ null
    }

    void ShieldingMove()
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

        }

    }

    [SerializeField] float knockBackPower = 2;
    void KnockBack(Monsters hit)
    {

        if (hit == null || !hit.gameObject.activeSelf) return;
        GameObject mon = hit.gameObject;
        Rigidbody2D rb = mon.GetComponent<Rigidbody2D>();

        Vector2 monUp = mon.transform.up;
        Vector2 myLeft = -transform.right;
        Vector2 dire;
        if (player == 0)
        {
            dire = new Vector2(1, 0.7f).normalized;
        }
        else
        {
            dire = new Vector2(-1, 0.7f).normalized;

        }

        rb.AddForce(dire * knockBackPower, ForceMode2D.Impulse);
    }
}
