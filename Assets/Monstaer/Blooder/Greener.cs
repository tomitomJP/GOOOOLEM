using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Greener : Monsters
{
    [SerializeField] float healRate = 0.2f;
    [SerializeField] float healCooldown = 3f;  // 回復の間隔（秒）
    private float healTimer = 0f;
    [SerializeField] AudioClip healSE;

    void Start()
    {
        StartSetup();

    }

    void Update()
    {
        Updating();

        healTimer -= Time.deltaTime;
        if (healTimer <= 0f && !isDead)
        {
            Monsters target = FindAllyToHeal();
            if (target != null)
            {

                StartCoroutine(HealMotion(target));
                healTimer = healCooldown;
            }
            else
            {
                healTimer = healCooldown * 0.5f;//対象者がいない場合通常の半分のクールタイム

            }
        }
    }


    private Monsters FindAllyToHeal()
    {
        Monsters lowestHpAlly = null;
        float lowestHpRate = 1f;

        List<Monsters> targets = new List<Monsters>();

        // 自分の player に応じて対象を切り替え
        if (player == 0)
        {
            targets = GameManager.GetMonsters(GameManager.type.mon0);
        }
        else if (player == 1)
        {
            targets = GameManager.GetMonsters(GameManager.type.mon1);
        }

        foreach (var ally in targets)
        {
            if (ally != null && ally != this && ally.player == player && ally.hp > 0 && !ally.gameObject.TryGetComponent<House>(out House house) && !ally.gameObject.TryGetComponent<GoblinBuild>(out GoblinBuild goblinBuild))
            {
                float hpRate = ally.hp / ally.maxHp;
                if (hpRate < lowestHpRate)
                {
                    lowestHpRate = hpRate;
                    lowestHpAlly = ally;
                }
            }
        }
        return lowestHpAlly;
    }

    // 回復アニメーションと処理
    private IEnumerator HealMotion(Monsters target)
    {
        mode = Mode.atk;

        spriteRenderer.sprite = atkSprites[0];
        yield return Wait(0.2f, 2);
        spriteRenderer.sprite = atkSprites[1];
        yield return Wait(0.2f, 2);
        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.2f, 2);
        spriteRenderer.sprite = atkSprites[3];
        yield return Wait(0.2f, 2);
        spriteRenderer.sprite = atkSprites[4];
        yield return Wait(0.4f, 2);

        if (target != null)
        {
            float healAmount = target.maxHp * healRate; // 回復量を自由に設定
            target.Healed(healAmount); // 同じ player の味方を回復
            AudioManager.PlaySE(healSE, 0.6f);
        }

        mode = Mode.move; // 終わったら移動状態に戻す
    }
}
