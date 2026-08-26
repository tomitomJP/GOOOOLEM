using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Beamanim : MonoBehaviour
{
    [SerializeField] float scalePulse = 0.1f;   // Xスケールの上下幅
    [SerializeField] float pulseTime = 0.08f;   // 上下の速さ
    [SerializeField] float lifeTime = 0.3f;     // 消えるまでの時間

    void Start()
    {
        // Xスケールを上下
        transform.DOScaleX(transform.localScale.x + scalePulse, pulseTime)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

        // だんだん透明に
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.DOFade(0f, lifeTime).OnComplete(() => Destroy(gameObject));
        }
        else
        {
            // SpriteRendererが無い場合はそのまま消える
            Destroy(gameObject, lifeTime);
        }
    }
}
