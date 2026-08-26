using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallStatus : MonoBehaviour
{
    public float lifeTime = 10f;          // 弾の寿命
    public GameObject explosionPrefab;   // 爆発エフェクトのPrefab
    [SerializeField] float attackRadius = 2f; // 攻撃半径
    [SerializeField] float damage = 30f; // 攻撃半径
    [SerializeField] LayerMask[] targetLayer;

    [SerializeField] LayerMask myLayer0;
    [SerializeField] LayerMask myLayer1;
    public int player = 0;
    void Start()
    {
        // 5秒後に自動で消える（撃ちっぱなし対策）
        Destroy(gameObject, lifeTime);

        if (player == 0)
        {
            gameObject.layer = 15;
        }
        else
        {
            gameObject.layer = 16;

        }
    }

    private bool hasExploded = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasExploded) return; // すでに処理したなら無視
        hasExploded = true;

        // 爆発エフェクト
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // 範囲ダメージ
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRadius, targetLayer[player]);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Monster") || hit.CompareTag("House"))
            {
                Monsters monsters = hit.GetComponent<Monsters>();
                if (monsters == null) continue;

                if (hit.CompareTag("House"))
                {
                    monsters.Damaged(damage * 0.45f, null);
                }
                else
                {
                    KnockBack(monsters);
                    monsters.Damaged(damage, null);
                    monsters.ApplyStatus(new Monsters.StatusManager("cannonspd", true, Monsters.StatusManager.StatusType.spdRate, 1f, -1));
                    monsters.ApplyStatus(new Monsters.StatusManager("annonatkspd", true, Monsters.StatusManager.StatusType.atkSpdRate, 1f, -1f));
                }
            }
        }

        // 弾を削除
        Destroy(gameObject);
    }

    [SerializeField] float knockBackPower = 7;
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
            dire = new Vector2(0.7f, 3f).normalized;
        }
        else
        {
            dire = new Vector2(-0.7f, 3f).normalized;

        }

        rb.AddForce(dire * knockBackPower, ForceMode2D.Impulse);
    }

}

