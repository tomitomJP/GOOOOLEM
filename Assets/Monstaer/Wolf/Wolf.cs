using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Monsters
{

    [SerializeField] GameObject bites;
    void Start()
    {
        StartSetup();
    }

    // Update is called once per frame
    void Update()
    {
        Updating();
    }

    [SerializeField] Vector2 bithsRayOrigin;
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
