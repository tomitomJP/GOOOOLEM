using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Majyo : Human
{
    [Header("Blaster Attack")]
    [SerializeField] GameObject blasterPrefab;
    [SerializeField] GameObject beamPrefab;
    [SerializeField] float appearYOffset = 4.5f;
    [SerializeField] float appearTime = 0.3f;
    [SerializeField] float chargeTime = 0.2f;
    [SerializeField] float beamTime = 0.3f;
    [SerializeField] float beamDamageRate = 0.7f;

    [Header("Beam Hitbox")]
    [SerializeField] Vector2 beamSize = new Vector2(6f, 0.8f);

    [SerializeField] AudioClip beamSE;
    [SerializeField] float beamSEVolume = 1f;


    void Start()
    {
        StartSetup();
        HumanSetUp();
        hp = 1;
        maxHp = 1;
        ApplyStatus(new StatusManager("MAJOATK", true, StatusManager.StatusType.atkRate, Mathf.Infinity, (level - 1) * 0.01f));

    }

    void Update()
    {
        Updating();
    }

    public override IEnumerator AtkMotion(Monsters target)
    {
        if (blasterPrefab == null || beamPrefab == null)
        {
            Debug.LogWarning("Blaster/Beam prefab is not assigned.");
            yield break;
        }
        if (atkSprites == null || atkSprites.Length == 0)
        {
            Debug.LogWarning("atkSpritesが未設定です");
            yield break;
        }

        mode = Mode.atk;

        // 攻撃スプライト（1枚だけ）
        spriteRenderer.sprite = atkSprites[0];

        Vector3 basePos = transform.position;
        float targetX = (target != null) ? target.transform.position.x : basePos.x;

        // 上からスライドして出現 → 停止
        Vector3 startPos = new Vector3(targetX, basePos.y + appearYOffset, 0);
        Vector3 stopPos = new Vector3(targetX, basePos.y + appearYOffset * 0.4f, 0);

        GameObject blaster = Instantiate(blasterPrefab, startPos, Quaternion.identity);

        yield return blaster.transform
            .DOMoveY(stopPos.y, appearTime)
            .SetEase(Ease.OutQuad)
            .WaitForCompletion();

        // 溜め
        yield return new WaitForSeconds(chargeTime);

        // SE
        if (beamSE != null) AudioManager.PlaySE(beamSE, beamSEVolume);

        // 相手位置にビームをボーン
        Vector3 beamPos = (target != null) ? target.transform.position : basePos;
        GameObject beam = Instantiate(beamPrefab, beamPos, Quaternion.identity);
        DealBeamDamage(beamPos);

        yield return new WaitForSeconds(beamTime);

        Destroy(beam);

        // 上にスライドして退場
        yield return blaster.transform
            .DOMoveY(startPos.y, appearTime)
            .SetEase(Ease.InQuad)
            .WaitForCompletion();

        Destroy(blaster);

        mode = Mode.move;
    }

    void DealBeamDamage(Vector3 pos)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(pos, beamSize, 0, enemyLayer);
        for (int i = 0; i < hits.Length; i++)
        {
            Monsters m = hits[i].GetComponent<Monsters>();
            if (m != null)
            {
                Attack(m, atk * beamDamageRate, false);
            }
        }
    }
}
