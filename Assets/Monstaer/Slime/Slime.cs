using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Slime : Monsters
{
    // Start is called before the first frame update
    [SerializeField] AudioClip attackSE;
    public float slimeSize = 2;
    public float spawnTime;
    [SerializeField] float mergeRadius = 1;
    [SerializeField] BoxCollider2D boxCollider2D;
    [SerializeField] GameObject slimeSplit;
    [SerializeField] float martgeCT = 0;
    float mutekiTime = 0;
    void Start()
    {
        StartSetup();

        spawnTime = Time.time;

        StartCoroutine(CheckMergeDelay());
    }


    IEnumerator CheckMergeDelay()
    {
        yield return new WaitForSeconds(0.2f);
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.right, 20f, myLayer);
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject == gameObject) continue;
            Slime other = hit.collider.GetComponent<Slime>();
            if (other != null && other.martgeCT <= 0)
            {
                Debug.Log("JUMP");
                StartCoroutine(JumpMarge(other));
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Updating();
        ScaleChange();

        if (isDead) return;

        if (martgeCT <= 0)
        {
            CheckMerge();
        }
        else
        {
            martgeCT -= Time.deltaTime;
        }


        if (mutekiTime > 0) mutekiTime -= Time.deltaTime;
    }

    void ScaleChange()
    {
        transform.localScale = Vector3.one * (Mathf.Min(slimeSize, 6) / 2);
        //mergeRadius = slimeSize / 2;
        enemyDistance = 1 + ((Mathf.Min(slimeSize, 6) - 2) * 0.2222f);

        if (slimeSize == 1)
        {
            spdRate = 2;
            atkRate = 0.2f;

        }
        else if (slimeSize == 2)
        {
            spdRate = 1;
            atkRate = 1f;

        }
        else
        {
            spdRate = 0.7f;
            atkRate = Mathf.Log(slimeSize + 1, 4) / Mathf.Log(3, 4);

        }

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

        spriteRenderer.sprite = atkSprites[0];
        yield return Wait(0.3f, 0);

        Attack(target);
        AudioManager.PlaySE(attackSE, 0.3f);

        spriteRenderer.sprite = atkSprites[1];
        yield return Wait(0.3f, 0);

        mode = Mode.move;

    }

    IEnumerator JumpMarge(Monsters target)
    {
        mode = Mode.atk;

        spriteRenderer.sprite = atkSprites[0];
        yield return Wait(1f, 0);

        spriteRenderer.sprite = atkSprites[1];

        float height = 3f; // 弧の高さ
        float duration = 1f;

        Vector3 startPos = transform.position;

        // 中間点（弧の頂点）
        Vector3 midPos = (startPos + target.transform.position) / 2 + Vector3.up * height;

        // パスを作成（弧の頂点を通る）
        Vector3[] path = new Vector3[] { startPos, midPos, target.transform.position };

        // パス移動
        transform.DOPath(path, duration, PathType.CatmullRom)
               .SetEase(Ease.OutCubic);

        yield return Wait(2f, 0);
        mode = Mode.move;
    }

    public override void Damaged(float damage, Monsters attacker, bool Melee = true, StatusManager newStatus = null)
    {
        // newStatus が null じゃないときだけ適用する
        if (newStatus != null)
            ApplyStatus(newStatus);

        if (damage > 0)
        {
            AudioManager.PlaySE(defaultAtkSE, 0.3f);
            InsHitPar(attacker);
        }
        else
        {
            return;
        }

        if (slimeSize > 1 && damage < 10000)
        {
            Split();
        }
        else
        {
            if (hp > 0)
            {
                hp -= damage;
                Text _damageText = Instantiate(damageText, transform.position, Quaternion.identity, canvas.transform).GetComponent<Text>();
                _damageText.text = damage.ToString("F0");
            }
        }
    }


    void Split()
    {
        slimeSize /= 2;
        martgeCT = 7;
        mutekiTime = 0.25f;
        Text _damageText = Instantiate(damageText, transform.position, Quaternion.identity, canvas.transform).GetComponent<Text>();
        _damageText.text = "ブンレツ";

        Slime splitSlime = Instantiate(slimeSplit, transform.position, transform.rotation, transform.parent).GetComponent<Slime>();
        splitSlime.player = player;
        splitSlime.slimeSize = slimeSize;
        splitSlime.martgeCT = 7;
        splitSlime.mutekiTime = 0.5f;

        KnockBack(splitSlime.gameObject, 2);
        KnockBack(gameObject, 0.5f);

    }

    void KnockBack(GameObject hit, float knockBackPower = 10)
    {
        Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
        GameObject mon = hit;

        Vector2 monUp = mon.transform.up;
        Vector2 myLeft = -transform.right;
        Vector2 dire;
        if (player == 1)
        {
            dire = new Vector2(1, 0.5f).normalized;
        }
        else
        {
            dire = new Vector2(-1, 0.5f).normalized;

        }

        rb.AddForce(dire * knockBackPower, ForceMode2D.Impulse);
    }
    void CheckMerge()
    {
        // 半径 mergeRadius の円形範囲にいるコライダーを取得
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, mergeRadius, myLayer);

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue; // 自分自身は無視

            Slime other = hit.GetComponent<Slime>();
            if (other != null && other.martgeCT <= 0)
            {
                Merge(other);
                break;
            }
        }
    }

    void Merge(Slime other)
    {
        if (spawnTime > other.spawnTime) return;

        Text _damageText = Instantiate(damageText, transform.position, Quaternion.identity, canvas.transform).GetComponent<Text>();
        _damageText.color = Color.green;
        _damageText.text = "ガッタイ";

        slimeSize += other.slimeSize;
        Destroy(other.gameObject);
    }
}
