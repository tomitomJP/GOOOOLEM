using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaper : Monsters
{
    // Start is called before the first frame update
    void Start()
    {
        StartSetup();
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
        yield return Wait(0.5f, 0);

        spriteRenderer.sprite = atkSprites[1];
        yield return Wait(0.1f, 2);

        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.1f, 2);

        if (Attack(target) > 0f)
        {
            ApplyStatus(new StatusManager("ReaperHeal", false, StatusManager.StatusType.heal, 0.0f, atk * atkRate * 0.5f));

        }

        spriteRenderer.sprite = atkSprites[3];
        yield return Wait(0.3f, 0);

        mode = Mode.move;
    }
}
