using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : Human
{
    [SerializeField] float healRate = 0.2f;
    [SerializeField] float healCooldown = 3f;  // 回復の間隔（秒）
    private float healTimer = 0f;
    [SerializeField] AudioClip healSE;

    void Start()
    {
        StartSetup();
        HumanSetUp();
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

    // 同じ player の味方で一番HPが低いキャラを探す
    private Monsters FindAllyToHeal()
    {
        Monsters lowestHpAlly = null;
        float lowestHpRate = 1f;

        List<Monsters> targets = new List<Monsters>();
        targets = GameManager.GetMonsters(GameManager.type.human);

        foreach (var h in targets)
        {
            if (h != null && h != this && h.player == player && h.hp > 0)
            {
                float hpRate = h.hp / h.maxHp;
                if (hpRate < lowestHpRate)
                {
                    lowestHpRate = hpRate;
                    lowestHpAlly = h;
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
