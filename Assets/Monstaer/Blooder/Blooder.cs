using System.Collections;
using UnityEngine;

public class Blooder : Monsters
{
    // Start is called before the first frame update
    [SerializeField] GameObject thunder;
    [SerializeField] AudioClip masicSE;
    [SerializeField] AudioClip kaminariSE;

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
        int num = 0;
        if (atkSprites.Length < 2)
        {
            Debug.LogWarning("攻撃スプライトが足りません");
            yield break;
        }
        mode = Mode.atk;

        spriteRenderer.sprite = atkSprites[0];
        AudioManager.PlaySE(masicSE);

        yield return Wait(0.1f);

        float t = 1;
        float t2 = 0;
        int i = 0;
        while (t >= t2)
        {
            i++;
            int q = (i % 2);
            spriteRenderer.sprite = atkSprites[q];
            t2 += Time.deltaTime * atkSpdRate;
            yield return null;
        }
        RaycastHit2D[] hit;
        if (Physics2D.Raycast(rayOrigin + transform.position, transform.right, Mathf.Infinity, enemyLayer))
        {
            spriteRenderer.sprite = atkSprites[3];
            yield return Wait(0.1f, 2);

            spriteRenderer.sprite = atkSprites[4];
            yield return Wait(0.1f, 2);

            if (Physics2D.Raycast(rayOrigin + transform.position, transform.right, Mathf.Infinity, enemyLayer))
            {
                hit = Physics2D.RaycastAll(rayOrigin + transform.position, transform.right, Mathf.Infinity, enemyLayer);
                for (int j = 0; j < hit.Length; j++)
                {
                    GameObject monster = hit[j].collider.gameObject;
                    Instantiate(thunder, hit[j].point, Quaternion.identity);
                    AudioManager.PlaySE(kaminariSE, 0.15f);

                    Attack(monster.GetComponent<Monsters>(), -810, false);
                    // スピード1.5倍、5秒間
                    Attack(monster.GetComponent<Monsters>(), 0, false, new StatusManager("BlooderSpdRateDown", true, StatusManager.StatusType.spdRate, 0.1f, -0.8f));
                    Attack(monster.GetComponent<Monsters>(), 0, false, new StatusManager("BlooderAtkSpdRateDown", true, StatusManager.StatusType.atkSpdRate, 0.1f, -0.8f));

                    Attack(monster.GetComponent<Monsters>(), 0, false, new StatusManager("BlooderSpdRateDown1", true, StatusManager.StatusType.spdRate, 3f, -0.3f));
                    Attack(monster.GetComponent<Monsters>(), 0, false, new StatusManager("BlooderAtkSpdRateDown1", true, StatusManager.StatusType.atkSpdRate, 3f, -0.4f));

                }
            }
            spriteRenderer.sprite = atkSprites[4];
            yield return Wait(0.1f, 2);
        }



        mode = Mode.move;
    }
}
