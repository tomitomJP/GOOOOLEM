using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullHunter : Monsters
{
    // Start is called before the first frame update
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

        Vector2 direction = transform.right;
        Monsters target = null;
        //Debug.DrawRay(transform.position + rayOrigin, direction * enemyDistance, Color.red, 0.5f); // 可視化（長さに注意）

        RaycastHit2D hit = Physics2D.Raycast(transform.position + rayOrigin, direction, Mathf.Infinity, enemyLayer);
        if (hit.collider != null)
        {
            Monsters targetData = hit.collider.gameObject.GetComponent<Monsters>();
            if (targetData != null && gameObject != hit.collider.gameObject)
            {
                if (targetData.player != player)
                {
                    target = targetData;
                }
            }
        }

        if (target == null)
        {
            //yield break;
        }

        mode = Mode.atk;

        for (int i = 0; i < 15; i++)
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

        AudioManager.PlaySEWithPitch(shootSE, Random.Range(0.8f, 1.2f));
        Attack(target, -810, false);
        ApplyStatusTarget(target, new StatusManager("SKullHunterSpdRateDown", true, StatusManager.StatusType.spdRate, 0.7f, -1f));

        spriteRenderer.sprite = atkSprites[6];
        yield return Wait(0.05f, 0);

        spriteRenderer.sprite = atkSprites[7];
        yield return Wait(0.7f, 0);
        mode = Mode.move;
    }

}
