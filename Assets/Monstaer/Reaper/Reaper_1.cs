using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaper_1 : Monsters
{
    [SerializeField] GameObject repaersReaper;
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
        Vector2 pos = target.gameObject.transform.position;
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
        yield return Wait(0.04f, 2);

        spriteRenderer.sprite = atkSprites[3];

        ReapersReaper obj = Instantiate(repaersReaper, transform.position, transform.rotation).GetComponent<ReapersReaper>();
        obj.startPos = transform.position;
        obj.endPos = pos;
        obj.damage = (atk) / 12;
        obj.rate = atkSpdRate;
        obj.playerNum = player;
        obj.reaper = this;

        while (obj != null && obj.gameObject.activeSelf)
        {

            yield return null;
        }

        spriteRenderer.sprite = atkSprites[0];
        yield return Wait(0.6f, 0);

        mode = Mode.move;
    }
}
