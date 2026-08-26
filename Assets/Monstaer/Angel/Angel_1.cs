using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angel_1 : Monsters
{
    [SerializeField] GameObject impact;
    [SerializeField] AudioClip trumpetSE;

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
        Debug.Log("ATK" + gameObject.name);

        int num = 0;
        if (atkSprites.Length < 2)
        {
            Debug.LogWarning("攻撃スプライトが足りません");
            yield break;
        }
        mode = Mode.atk;

        for (int i = 0; i < 10; i++)
        {

            int q = (i % 2);
            spriteRenderer.sprite = atkSprites[q];
            yield return Wait(0.1f);

        }
        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.4f);

        AudioManager.PlaySEWithPitch(trumpetSE, 1f, 0.3f);
        yield return Wait(0.05f);
        AudioManager.PlaySEWithPitch(trumpetSE, 1.2f, 0.3f);
        yield return Wait(0.05f);
        AudioManager.PlaySEWithPitch(trumpetSE, 1.4f, 0.3f);


        InstantImpact();
        spriteRenderer.sprite = atkSprites[3];
        yield return Wait(0.2f, 2);

        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.1f, 2);


        mode = Mode.move;
    }

    void InstantImpact()
    {

        GameObject I = Instantiate(impact, transform.position, Quaternion.Euler(transform.eulerAngles));
        //I.layer = Mathf.RoundToInt(Mathf.Log(this.myLayer.value, 2));

        AngelImpact_1 angelImpact = I.GetComponent<AngelImpact_1>();
        angelImpact.angel = this;
    }
}
