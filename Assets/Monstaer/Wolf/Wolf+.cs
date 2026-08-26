using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfPlus : Monsters
{

    [SerializeField] GameObject bites;
    [SerializeField] SpriteRenderer[] zanzou;
    void Start()
    {
        StartSetup();
    }

    // Update is called once per frame
    void Update()
    {
        Updating();
    }

    Vector3 lastPos;

    void ZanzouView(int spriteNum)
    {
        float moved = (transform.position - lastPos).sqrMagnitude;
        lastPos = transform.position;

        if (moved < 0.0001f)
        {
            // 動いていないので残像を消す
            foreach (var z in zanzou)
            {
                z.sprite = null;
            }
            return;
        }

        // 残像更新処理
        for (int i = 0; i < zanzou.Length; i++)
        {
            int sprNum = (spriteNum - 1 - i + moveSprites.Length) % moveSprites.Length;
            zanzou[i].sprite = moveSprites[sprNum];
        }
    }

    void ZanzouOff()
    {
        for (int i = 0; i < zanzou.Length; i++)
        {
            zanzou[i].sprite = null;
        }
    }




    public override void Move()//移動と異動アニメーションを管理
    {
        if (mode == Mode.move && stanTIme <= 0 && MoveOk)
        {
            if (player == 0)
            {
                transform.Translate(new Vector3(1, 0, 0) * Time.deltaTime * spd * Mathf.Clamp(spdRate, 0, 100), Space.World);

            }
            else
            {
                transform.Translate(new Vector3(-1, 0, 0) * Time.deltaTime * spd * Mathf.Clamp(spdRate, 0, 100), Space.World);

            }
        }
        else
        {
            ZanzouOff();
        }

        if (stanTIme > 0)
        {
            stanTIme -= Time.deltaTime;
        }

        if (MoveAniTimer >= MoveAniTime)
        {
            MoveAniSpriteNum = (MoveAniSpriteNum + 1) % moveSprites.Length;
            spriteRenderer.sprite = moveSprites[MoveAniSpriteNum];
            MoveAniTimer = 0;
            spriteRenderer.color = Color.white;

            ZanzouView(MoveAniSpriteNum);
        }
        else
        {
            MoveAniTimer += Time.deltaTime * Mathf.Clamp(spdRate, 0, 2);
        }

    }

    [SerializeField] Vector2 bithsRayOrigin;
    public override IEnumerator AtkMotion(Monsters target)//攻撃アニメーションなど
    {
        ZanzouOff();
        int num = 0;
        if (atkSprites.Length < 2)
        {
            Debug.LogWarning("攻撃スプライトが足りません");
            yield break;
        }
        mode = Mode.atk;

        spriteRenderer.sprite = atkSprites[0];
        yield return Wait(0.15f, 0);

        Vector3 offset = new Vector2(3f, 0);

        if (player == 1) offset.x *= -1;

        Vector3 pos = transform.position + offset;


        Debug.DrawRay((Vector2)transform.position + bithsRayOrigin, transform.right * enemyDistance, Color.red, 0.5f); // 可視化（長さに注意）

        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + bithsRayOrigin, transform.right, enemyDistance, enemyLayer);

        if (player == 0)
        {
            Instantiate(bites, hit.point, Quaternion.Euler(new Vector3(0, 0, -20)));

        }
        else
        {
            Instantiate(bites, hit.point, Quaternion.Euler(new Vector3(0, 0, 20)));

        }

        spriteRenderer.sprite = atkSprites[1];
        yield return Wait(0.15f, 0);

        Attack(target);
        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.15f, 0);



        mode = Mode.move;

    }
}
