using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceMage : Human
{
    [SerializeField] GameObject snowball;

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
        if (atkSprites.Length < 2)
        {
            Debug.LogWarning("攻撃スプライトが足りません");
            yield break;
        }
        mode = Mode.atk;

        IM_SnowBall _snowBall = Instantiate(snowball, transform.position, Quaternion.identity, transform).GetComponent<IM_SnowBall>();
        _snowBall.transform.localPosition = new Vector3(-0.5f, 1f);
        float duration = 1f;
        int i = 0;
        float animDuration = 0;

        while (duration > 0)
        {

            if (animDuration < 0)
            {
                animDuration = 0.1f;
                i = (i + 1) % 2;
            }

            spriteRenderer.sprite = atkSprites[i];
            yield return null;
            duration -= Time.deltaTime;
            animDuration -= Time.deltaTime;
        }

        _snowBall.Slowing(target, atk, this);
        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.05f);

        spriteRenderer.sprite = atkSprites[3];
        yield return Wait(0.8f, 2);
        mode = Mode.move;

    }




}
