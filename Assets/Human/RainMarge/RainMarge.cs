using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainMarge : Human
{
    [SerializeField] GameObject cloud;
    [SerializeField] GameObject cloudSummonPar;

    [SerializeField] Sprite[] summonSprite;

    void Start()
    {
        StartSetup();
        HumanSetUp();


    }

    public override void FirstSkill()
    {
        if (Random.Range(0, 12) == 0 || level >= 15) AtkTriggerRate = 90;

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

    public override IEnumerator AtkMotion(Monsters target)//攻撃アニメーションなど
    {
        Debug.Log("ATK" + gameObject.name);

        int num = 0;
        if (atkSprites.Length < 2)
        {
            Debug.LogWarning("攻撃スプライトが足りません");
            yield break;
        }
        mode = Mode.atk;

        spriteRenderer.sprite = atkSprites[0];
        yield return Wait(0.4f);

        spriteRenderer.sprite = atkSprites[1];
        yield return Wait(0.055f);

        Attack(target);

        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.055f);
        spriteRenderer.sprite = atkSprites[3];
        yield return Wait(0.07f);
        spriteRenderer.sprite = atkSprites[4];
        yield return Wait(0.07f);
        spriteRenderer.sprite = atkSprites[5];
        yield return Wait(0.5f);


        mode = Mode.move;
    }



    public IEnumerator AtkMotion()
    {
        mode = Mode.atk;

        float animDuration = 0.3f;
        float duration = 3;
        int n = 0;

        spriteRenderer.sprite = summonSprite[0];
        yield return Wait(0.7f);

        spriteRenderer.sprite = summonSprite[1];
        yield return Wait(0.05f);
        Shoot();

        spriteRenderer.sprite = summonSprite[2];
        yield return Wait(0.05f);
        spriteRenderer.sprite = summonSprite[3];
        yield return Wait(0.5f);


        mode = Mode.move;

        yield break;
    }


    void Shoot()
    {
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
            AtkTriggerRate = 50;
            return;
        }

        var c = Instantiate(cloud, transform.position, Quaternion.identity).GetComponent<Cloud>();
        StartCoroutine(c.ArcMove(target.transform));
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
            transform.Translate(direction * Time.deltaTime * 0.7f, Space.World);
        }

    }



}
