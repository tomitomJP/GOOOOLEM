using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goolem : Monsters
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
        yield return Wait(0.1f, 0);

        float t = 1;
        float t2 = 0;
        int i = 0;
        while (t >= t2)
        {
            i++;
            int q = (i % 2) + 1;
            spriteRenderer.sprite = atkSprites[q];
            t2 += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.sprite = atkSprites[1];
        yield return Wait(0.1f, 0);

        spriteRenderer.sprite = atkSprites[3];
        yield return Wait(0.1f, 0);

        Attack(target);
        spriteRenderer.sprite = atkSprites[4];
        yield return Wait(0.3f, 0);

        spriteRenderer.sprite = atkSprites[0];
        yield return Wait(0.3f, 2);

        mode = Mode.move;
    }

    int n = 0;
    public void GolemBeamCharge()
    {
        n = (n + 1) % 2;
        spriteRenderer.sprite = atkSprites[n + 1];
    }
}
