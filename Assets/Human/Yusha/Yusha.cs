using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yusha : Human
{
    void Start()
    {
        StartSetup();
        HumanSetUp();

        ApplyStatus(new StatusManager("YushaUp", true, StatusManager.StatusType.atkRate, Mathf.Infinity, (level - 1) * 0.025f));
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

        spriteRenderer.sprite = atkSprites[0];
        yield return Wait(0.6f, 2);

        AreaAtk();
        spriteRenderer.sprite = atkSprites[1];
        yield return Wait(0.1f, 2);

        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.1f, 2);

        spriteRenderer.sprite = atkSprites[3];
        yield return Wait(0.1f, 2);

        AreaAtk();
        spriteRenderer.sprite = atkSprites[4];
        yield return Wait(0.1f, 2);

        spriteRenderer.sprite = atkSprites[5];
        yield return Wait(0.1f, 2);

        spriteRenderer.sprite = atkSprites[6];
        yield return Wait(0.1f, 2);

        spriteRenderer.sprite = atkSprites[7];
        yield return Wait(0.5f, 2);


        mode = Mode.move;

    }


    void AreaAtk()
    {
        if (Physics2D.Raycast(rayOrigin + transform.position, transform.right, Mathf.Infinity, enemyLayer))
        {
            RaycastHit2D[] hit;
            hit = Physics2D.RaycastAll(rayOrigin + transform.position, transform.right, enemyDistance * 2, enemyLayer);
            for (int j = 0; j < hit.Length; j++)
            {
                GameObject monster = hit[j].collider.gameObject;

                Attack(monster.GetComponent<Monsters>(), atk / 1.3f);
            }
        }
    }
}
