using UnityEngine;

public class CannonController : MonoBehaviour
{
    [SerializeField] int playerNum = 0;
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


        // 反動から元の位置に戻す
        if (isRecoiling)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime / recoilDuration);

            if (Vector3.Distance(transform.localPosition, originalPosition) < 0.01f)
            {
                transform.localPosition = originalPosition;
                isRecoiling = false;
            }
        }
    }

    public void Shoot()
    {
        // 弾を生成
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<CannonBallStatus>().player = playerNum;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 左方向へ発射
            rb.velocity = -transform.right * bulletSpeed;
        }

        // 砲身の反動（右にちょっとズレる）
        transform.localPosition += new Vector3(recoilDistance, 0f, 0f);
        isRecoiling = true;
    }
}
