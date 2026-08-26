using System.Collections;
using UnityEngine;

public class Goblin : Monsters
{
    // Start is called before the first frame update
    public bool building = false;
    public float buildPosX;
    public GoblinBuild goblinBuild;
    public bool nowBuiding;

    [SerializeField] Sprite[] buildMotion;
    [SerializeField] AudioClip tonkachi;
    [SerializeField] bool canBuild = true;

    void Start()
    {
        StartSetup();

        if (canBuild) goblinBuild = gameManager.GoblinBuilds[player];
    }

    // Update is called once per frame
    void Update()
    {
        if (canBuild)
        {
            if (BuildingDist() < 0.3f && goblinBuild.hp < goblinBuild.hpMax && hp > 0)
            {
                if (!nowBuiding)
                {
                    nowBuiding = true;
                    StartCoroutine(BuildMotion(goblinBuild));
                }
            }
            else
            {
                Updating();
            }
        }
        else
        {
            Updating();
        }

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
        yield return Wait(0.45f);

        spriteRenderer.sprite = atkSprites[1];
        yield return Wait(0.1f);

        Attack(target);
        spriteRenderer.sprite = atkSprites[2];
        yield return Wait(0.05f);

        mode = Mode.move;

    }

    float BuildingDist()
    {
        float dist = Vector2.Distance(transform.position, goblinBuild.transform.position);
        return dist;
    }

    public IEnumerator BuildMotion(Monsters target)//攻撃アニメーションなど
    {
        int num = 0;
        if (atkSprites.Length < 2)
        {
            Debug.LogWarning("攻撃スプライトが足りません");
            yield break;
        }
        mode = Mode.atk;

        spriteRenderer.sprite = buildMotion[0];
        yield return Wait(0.4f);

        spriteRenderer.sprite = buildMotion[1];
        yield return Wait(0.2f);

        target.hp += 2.5f;
        AudioManager.PlaySEWithPitch(tonkachi, Random.Range(0.8f, 1.2f), 0.4f);

        spriteRenderer.sprite = buildMotion[2];
        yield return Wait(0.05f);

        spriteRenderer.sprite = buildMotion[0];
        yield return Wait(0.2f);

        spriteRenderer.sprite = buildMotion[1];
        yield return Wait(0.2f);

        target.hp += 2.5f;
        AudioManager.PlaySEWithPitch(tonkachi, Random.Range(0.8f, 1.2f), 0.4f);
        spriteRenderer.sprite = buildMotion[2];
        yield return Wait(0.05f);

        yield return Wait(1.5f);

        mode = Mode.move;
        nowBuiding = false;
    }
}
