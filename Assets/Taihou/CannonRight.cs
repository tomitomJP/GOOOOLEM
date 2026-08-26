using UnityEngine;

public class CannonLeft : MonoBehaviour
{
    [Header("弾の設定")]
    public GameObject bulletPrefab;   // 弾プレハブ
    public Transform firePoint;       // 発射位置
    public float bulletSpeed = 10f;   // 弾速

    [Header("反動の設定")]
    public float recoilDistance = 0.2f; // 反動の移動量
    public float recoilDuration = 0.1f; // 元に戻るまでの時間

    private Vector3 originalPosition; // 砲身の初期位置
    private bool isRecoiling = false;

    void Start()
    {
        originalPosition = transform.localPosition; // 砲身の初期位置を記録
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

        // 反動から元の位置に戻す
        if (isRecoiling)
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                originalPosition,
                Time.deltaTime / recoilDuration
            );

            if (Vector3.Distance(transform.localPosition, originalPosition) < 0.01f)
            {
                transform.localPosition = originalPosition;
                isRecoiling = false;
            }
        }
    }

    void Shoot()
    {
        // 弾を生成
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 右方向へ発射
            rb.velocity = Vector2.right * bulletSpeed;
        }

        // 砲身の反動（左にちょっとズレる）
        transform.localPosition += new Vector3(-recoilDistance, 0f, 0f);
        isRecoiling = true;
    }
}
