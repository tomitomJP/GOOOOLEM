using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joker : Human
{
    public Sprite[] magicAtkSprites;//攻撃用のスプライトを入れる
    [SerializeField] GameObject trickParticle;
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
        mode = Mode.atk;


        if (Vector2.Distance(target.transform.position, transform.position) < 3)
        {
            yield return NomalAtk(target);

        }
        else
        {
            yield return MagicAtk(target);
            aktCTimer = atkCT * 2;

        }

        mode = Mode.move;

    }

    public IEnumerator NomalAtk(Monsters target)//攻撃アニメーションなど
    {
        int num = 0;
        if (atkSprites.Length < 2)
        {
            Debug.LogWarning("攻撃スプライトが足りません");
            yield break;
        }


        spriteRenderer.sprite = atkSprites[0];
        yield return Wait(0.3f, 0);

        Attack(target, atk);
        spriteRenderer.sprite = atkSprites[1];
        yield return Wait(0.1f, 0);

        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.1f, 0);

        spriteRenderer.sprite = atkSprites[3];
        yield return Wait(0.1f, 0);

        spriteRenderer.sprite = magicAtkSprites[4];
        yield return Wait(0.5f, 0);
    }

    IEnumerator MagicAtk(Monsters target)//攻撃アニメーションなど
    {
        Vector2 originPos = transform.position;
        transform.localScale = new Vector3(1, -1, 1);
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;


        Instantiate(trickParticle, transform.position, Quaternion.identity);
        GetComponent<SpriteRenderer>().enabled = false;
        yield return Wait(0.3f, 2);
        GetComponent<SpriteRenderer>().enabled = true;

        ApplyStatusTarget(target, new StatusManager("JokerMagic", true, StatusManager.StatusType.spdRate, 1.5f, -0.8f));

        Vector2 targetPos = target.transform.position;
        targetPos.x *= 0.6f;
        transform.position = targetPos;
        Instantiate(trickParticle, transform.position, Quaternion.identity);

        spriteRenderer.sprite = magicAtkSprites[0];
        yield return Wait(0.3f, 2);

        Attack(target, atk / 3);
        spriteRenderer.sprite = magicAtkSprites[1];
        yield return Wait(0.1f, 2);

        spriteRenderer.sprite = magicAtkSprites[2];
        yield return Wait(0.1f, 2);

        spriteRenderer.sprite = magicAtkSprites[3];
        yield return Wait(0.1f, 2);

        spriteRenderer.sprite = magicAtkSprites[4];
        yield return Wait(0.5f, 2);

        Instantiate(trickParticle, transform.position, Quaternion.identity);
        GetComponent<SpriteRenderer>().enabled = false;
        yield return Wait(0.3f, 2);
        GetComponent<SpriteRenderer>().enabled = true;

        transform.position = originPos;
        transform.localScale = new Vector3(1, 1, 1);
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GetComponent<BoxCollider2D>().enabled = true;
        Instantiate(trickParticle, transform.position, Quaternion.identity);

    }
}
