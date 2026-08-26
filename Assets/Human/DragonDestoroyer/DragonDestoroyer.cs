using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonDestoroyer : Human
{
   void Start()
    {
        StartSetup();
        HumanSetUp();
    }

    // Update is called once per frame
    void Update()
    {
        Updating();
    }

    public override IEnumerator AtkMotion(Monsters target)//攻撃アニメーションなど
    {
        int num = 0;

        mode = Mode.atk;

        spriteRenderer.sprite = atkSprites[0];
        yield return Wait(0.4f, 2);

        DoragonCheck(target);
        spriteRenderer.sprite = atkSprites[1];
        yield return Wait(0.1f, 2);

        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.1f, 2);

        spriteRenderer.sprite = atkSprites[3];
        yield return Wait(0.1f, 2);

       




        mode = Mode.move;

    }

     void DoragonCheck(Monsters target)
    {
        GameObject monsterObj = target.gameObject;

        if (target != null)
        {
        float damage = atk;

        if (monsterObj.TryGetComponent<Doragon>(out _) ||
            monsterObj.TryGetComponent<Doragon_1>(out _))
        {
            damage *= 2f;
        }

        Attack(target, damage);
        }
    }

}

