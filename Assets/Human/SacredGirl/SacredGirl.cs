using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacredGirl : Human
{
    [SerializeField] GameObject impact;
    [SerializeField] AudioClip trumpetSE;
    [SerializeField] Sprite[] holySprite;
    [SerializeField] GameObject holyBeam;
    [SerializeField] GameObject chargePar;

    [SerializeField] float purificationRate = -25;
    void Start()
    {
        StartSetup();
        HumanSetUp();


    }

    public override void FirstSkill()
    {
        if (Random.Range(0, 10) == 0 || level >= 18) purificationRate = 25f;

    }

    // Update is called once per frame
    void Update()
    {
        Updating();
    }

    public override IEnumerator AtkMotion(Monsters target)//攻撃アニメーションなど
    {
        Debug.Log("ATK" + gameObject.name);

        int num = 0;
        if (atkSprites.Length < 2)
        {
            Debug.LogWarning("攻撃スプライトが足りません");
            yield break;
        }
        mode = Mode.atk;

        if (Random.Range(0, 100) <= purificationRate)
        {
            StartCoroutine(Purification());
            purificationRate = -25;
            yield break;

        }

        spriteRenderer.sprite = atkSprites[0];
        yield return Wait(0.5f);

        int shootCount = 0;

        do
        {
            shootCount++;
            purificationRate += shootCount * 3;

            spriteRenderer.sprite = atkSprites[0];
            yield return Wait(0.2f);

            InstantImpact();
            AudioManager.PlaySEWithPitch(trumpetSE, 1f, 0.3f + (0.15f * (shootCount - 1)));
            spriteRenderer.sprite = atkSprites[1];
            yield return Wait(0.1f, 2);

            spriteRenderer.sprite = atkSprites[2];
            yield return Wait(0.1f, 2);

            spriteRenderer.sprite = atkSprites[0];
            yield return Wait(0.2f);
            if (Random.Range(0, 5) != 0)
            {
                break;
            }
        } while (true);



        mode = Mode.move;
    }

    void InstantImpact()
    {

        GameObject I = Instantiate(impact, transform.position, Quaternion.Euler(transform.eulerAngles));
        //I.layer = Mathf.RoundToInt(Mathf.Log(this.myLayer.value, 2));

        AngelImpact angelImpact = I.GetComponent<AngelImpact>();
        angelImpact.angel = this;
    }

    public IEnumerator Purification()
    {
        mode = Mode.atk;

        GameObject _chargePar = Instantiate(chargePar, transform.position, Quaternion.identity, transform);
        ParticleSystem par = _chargePar.GetComponent<ParticleSystem>();

        float chargeTime = 8;
        float animTime = 0.2f;
        int n = 0;

        while (chargeTime > 0)
        {
            if (hp <= 0)
            {
                yield break;
            }

            var shape = par.shape;
            float t = Mathf.Clamp01(chargeTime / 8f);
            shape.arc = Mathf.Lerp(360f, 1f, t);

            if (animTime < 0)
            {
                n++;
                int q = n % 3;
                spriteRenderer.sprite = holySprite[q];
                animTime = 0.2f;
            }

            chargeTime -= Time.deltaTime;
            animTime -= Time.deltaTime;
            yield return null;
        }

        spriteRenderer.sprite = holySprite[4];
        yield return Wait(0.1f, 2);

        spriteRenderer.sprite = holySprite[5];
        yield return Wait(0.1f, 2);

        Destroy(_chargePar);


        if (hp <= 0)
        {
            yield break;
        }
        List<Monsters> targets = GameManager.GetMonsters(GameManager.type.mon);

        StartCoroutine(PlaySE());
        foreach (var target in targets)
        {

            if (target == null || !target.gameObject.activeSelf || target.hp <= 0) continue;

            if (!target.gameObject.TryGetComponent<House>(out House house))
            {
                Instantiate(holyBeam, target.transform.position, Quaternion.identity);
                target.Damaged(0, this, false, new StatusManager("holy", false, StatusManager.StatusType.spdRate, 5, -1));
                target.Damaged(0, this, false, new StatusManager("holy1", false, StatusManager.StatusType.atkSpdRate, 5, -1));
            }

        }

        for (int i = 0; i < 17; i++)
        {

            int q = i % 3;
            spriteRenderer.sprite = holySprite[q];
            yield return Wait(0.2f);

        }

        foreach (var target in targets)
        {
            if (target == null || !target.gameObject.activeSelf || target.hp <= 0) continue;

            if (!target.gameObject.TryGetComponent<House>(out House house))
            {
                target.Damaged(10000, this, false);

            }
        }

        mode = Mode.move;

        yield break;
    }

    [SerializeField] AudioClip se;
    [SerializeField] AudioClip se2;

    IEnumerator PlaySE(int steps = 12, float duration = 3f, float startPitch = 0.7f, float endPitch = 2.5f)
    {
        yield return new WaitForSeconds(0.14f);

        StartCoroutine(PlaySE2());
        float stepDuration = duration / steps;  // 各ステップの間隔
        for (int i = 0; i < steps; i++)
        {
            // ピッチを段階的に上げる
            float pitch = Mathf.Lerp(startPitch, endPitch, (float)i / (steps - 1));

            // 1回だけ再生
            AudioManager.PlaySEWithPitch(se, pitch, 0.5f);
            // 後半ほど早くなるようにステップ時間を短くする
            float dynamicStep = stepDuration * Mathf.Pow(0.5f, (float)i / steps); // 徐々に早く
            yield return new WaitForSeconds(dynamicStep);
        }

        // 最後の音を少し長めに残す
        yield return new WaitForSeconds(se.length);
    }

    IEnumerator PlaySE2()
    {
        yield return new WaitForSeconds(1.4f);
        AudioManager.PlaySE(se2, 1f);

    }



}
