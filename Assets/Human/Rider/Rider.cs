using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Rider : Human
{
    public Sprite[] driveingSprite;//攻撃用のスプライトを入れる
    [SerializeField] ParticleSystem superBackFire;
    [SerializeField] AudioClip audioClip1;
    [SerializeField] AudioClip audioClip2;

    void Start()
    {
        superBackFire.Stop();

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

        spriteRenderer.sprite = atkSprites[0];
        yield return Wait(0.15f, 0);

        spriteRenderer.sprite = atkSprites[1];
        yield return Wait(0.2f, 0);

        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.1f, 0);
        Attack(target, atk);

        spriteRenderer.sprite = atkSprites[3];
        if (Random.Range(0, 3) < 1)
        {
            StartCoroutine(SuperAtk());
            yield break;
        }
        yield return Wait(0.1f, 0);



        spriteRenderer.sprite = atkSprites[4];
        yield return Wait(0.2f, 0);
        mode = Mode.move;

    }

    IEnumerator SuperAtk()
    {
        Coroutine anim = StartCoroutine(Anim());
        transform.GetChild(3).gameObject.SetActive(true);
        superBackFire.Play();
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;

        AudioManager.PlaySEWithPitch(audioClip2, 1.5f, 0.3f);
        AudioManager.PlaySEWithPitch(audioClip1, 0.8f, 0.3f);
        yield return transform.DOMoveX(-13, 0.3f).SetEase(Ease.InQuart).WaitForCompletion();
        transform.position = new Vector2(13, transform.position.y);
        SuperAtkDamage();


        for (int i = 0; i < 4; i++)
        {
            if (hp <= 0) break;

            AudioManager.PlaySEWithPitch(audioClip2, 1.5f, 0.2f);
            AudioManager.PlaySEWithPitch(audioClip1, 0.8f, 0.2f);
            yield return transform.DOMoveX(-13, 0.4f).WaitForCompletion();
            transform.position = new Vector2(13, transform.position.y);
            SuperAtkDamage();
        }


        ApplyStatus(GetEffect("finishSuperAtk", true, StatusManager.StatusType.spdRate, 0.5f, 5));
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Collider2D>().enabled = true;
        StopCoroutine(anim);
        superBackFire.Stop();
        transform.GetChild(3).gameObject.SetActive(false);
        mode = Mode.move;

    }

    void SuperAtkDamage()
    {


        foreach (var mon in GameManager.GetMonsters(GameManager.type.mon0))
        {
            if (mon.gameObject.GetComponent<Collider2D>().enabled)
            {
                Attack(mon, atk / 3, false);
            }
        }
    }

    IEnumerator Anim()
    {
        int index = 0;
        while (true)
        {
            spriteRenderer.sprite = driveingSprite[index];
            index = (index + 1) % driveingSprite.Length;
            yield return null;
        }
    }
}
