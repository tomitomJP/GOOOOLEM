using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkullHunter_1 : Monsters
{
    [SerializeField] int shootCount = 20;
    [SerializeField] AudioClip shootSE;

    void Start()
    {
        StartSetup();
    }

    // Update is called once per frame
    void Update()
    {
        RandomAtkCharaUpdate();
    }
    public override void RandomAtkTrigger()
    {
        StartCoroutine(AtkMotion());
    }

    public IEnumerator AtkMotion()//攻撃アニメーションなど
    {
        int num = 0;
        if (atkSprites.Length < 2)
        {
            Debug.LogWarning("攻撃スプライトが足りません");
            yield break;
        }

        mode = Mode.atk;

        for (int i = 0; i < 20; i++)
        {
            int q = (i % 3);
            spriteRenderer.sprite = atkSprites[q];
            yield return Wait(0.2f, 0);

        }
        spriteRenderer.sprite = atkSprites[3];
        yield return Wait(0.1f, 0);

        spriteRenderer.sprite = atkSprites[4];
        yield return Wait(0.1f, 0);


        spriteRenderer.sprite = atkSprites[5];
        yield return Wait(0.1f, 0);

        spriteRenderer.sprite = atkSprites[7];
        yield return Wait(0.15f, 0);

        for (int i = 0; i < shootCount; i++)
        {
            if (hp <= 0) break;
            spriteRenderer.sprite = atkSprites[6];
            yield return Wait(0.05f, 0);
            Shoot();
            AudioManager.PlaySEWithPitch(shootSE, Random.Range(0.8f, 1.2f));
            spriteRenderer.sprite = atkSprites[7];
            yield return Wait(0.15f, 0);
        }


        mode = Mode.move;
    }


    void Shoot()
    {
        Vector2 direction = transform.right;
        Monsters target = null;

        // 可視化（長さは enemyDistance を使う方が直感的）
        Debug.DrawRay(transform.position + rayOrigin, direction * Mathf.Infinity, Color.red, 0.5f);

        // RaycastAll 実行
        RaycastHit2D[] hits = Physics2D.RaycastAll(
            transform.position + rayOrigin,
            direction,
            Mathf.Infinity,        // Mathf.Infinity より「指定距離」の方が安全
            enemyLayer
        );

        // 配列が空でないか確認
        if (hits.Length > 0)
        {
            // Monsters を持っているヒットだけを抽出
            List<Monsters> validTargets = new List<Monsters>();
            foreach (var h in hits)
            {
                Monsters m = h.collider.GetComponent<Monsters>();
                if (m != null)
                    validTargets.Add(m);
            }

            if (validTargets.Count > 0)
            {

                // ランダムで 1 体選ぶ
                target = validTargets[Random.Range(0, validTargets.Count)];
            }
        }

        // ターゲットが決まったら攻撃
        if (target != null)
        {
            Attack(target, atk / shootCount, false);
            ApplyStatusTarget(target, new StatusManager("SKullHunter1SpdRateDown", true, StatusManager.StatusType.spdRate, 0.1f, -0.5f));

        }

    }

}
