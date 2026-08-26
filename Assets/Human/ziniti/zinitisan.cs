using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class zinitisan : Human
{
    [Header("Fist Attack")]
    [SerializeField] GameObject fistPrefab;
    [SerializeField] float jumpHeight = 2.2f;
    [SerializeField] float jumpUpTime = 0.18f;
    [SerializeField] float fallTime = 0.12f;
    [SerializeField] int slamCount = 10;
    [SerializeField] float slamRadius = 1.2f;
    [SerializeField] float fistSpawnYOffset = 2.5f;
    [SerializeField] float fistSpawnRandomX = 0.4f;
    [SerializeField] float fistLandOffsetTowardTarget = 1.0f;
    [SerializeField] float fistLandRandomX = 0.5f;

    [Header("Crouch")]
    [SerializeField] Sprite crouchSprite;
    [SerializeField] float crouchTime = 1.0f;

    void Start()
    {
        StartSetup();
        HumanSetUp();
    }

    void Update()
    {
        Updating();
    }

    public override IEnumerator AtkMotion(Monsters target)
    {
        if (fistPrefab == null)
        {
            Debug.LogWarning("Fist prefab is not assigned.");
            yield break;
        }

        mode = Mode.atk;

        Vector3 basePos = transform.position;

        if (crouchSprite != null)
        {
            spriteRenderer.sprite = crouchSprite;
            yield return new WaitForSeconds(crouchTime);
        }

        float totalAirTime = jumpUpTime + fallTime;
        float interval = totalAirTime / Mathf.Max(1, slamCount);

        // jump up
        Tweener jumpUp = transform.DOMoveY(basePos.y + jumpHeight, jumpUpTime).SetEase(Ease.OutQuad);
        yield return jumpUp.WaitForCompletion();

        // fall down while dropping fists
        Tweener fallDown = transform.DOMoveY(basePos.y, fallTime).SetEase(Ease.InQuad);

        for (int i = 0; i < slamCount; i++)
        {
            if (hp <= 0) break;
            SpawnAndDropFist(basePos, target);
            yield return new WaitForSeconds(interval);
        }

        yield return fallDown.WaitForCompletion();

        mode = Mode.move;
    }

    void SpawnAndDropFist(Vector3 center, Monsters target)
    {
        float randX = Random.Range(-fistSpawnRandomX, fistSpawnRandomX);
        Vector3 spawnPos = new Vector3(center.x + randX, center.y + fistSpawnYOffset, center.z);

        // target direction (right=+1, left=-1)
        float targetDir = 1f;
        if (target != null)
        {
            targetDir = Mathf.Sign(target.transform.position.x - center.x);
            if (targetDir == 0f) targetDir = 1f;
        }

        float landRandX = Random.Range(-fistLandRandomX, fistLandRandomX);
        Vector3 landPos = new Vector3(
            center.x + (targetDir * fistLandOffsetTowardTarget) + landRandX,
            center.y,
            center.z
        );

        // -180 degrees rotation
        GameObject fist = Instantiate(fistPrefab, spawnPos, Quaternion.Euler(0, 0, -190));

        fist.transform
            .DOMove(landPos, fallTime)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                GroundSlam(landPos, slamRadius);
                Destroy(fist);
            });
    }

    void GroundSlam(Vector3 center, float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius, enemyLayer);
        for (int i = 0; i < hits.Length; i++)
        {
            Monsters m = hits[i].GetComponent<Monsters>();
            if (m != null)
            {
                Attack(m, atk / 6f, false);
            }
        }
    }
}
