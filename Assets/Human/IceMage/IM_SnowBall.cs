using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IM_SnowBall : MonoBehaviour
{



    [SerializeField] GameObject snowballExpPar;

    public void Slowing(Monsters Monsters, float damage, IceMage iceMage)
    {
        if (Monsters == null || !Monsters.gameObject.activeSelf || !gameObject.activeSelf)
        {
            Destroy(gameObject);
            return;
        }
        StartCoroutine(ArcMove(gameObject, Monsters.transform, 4, 0.5f, Monsters, damage, iceMage));

    }

    IEnumerator ArcMove(GameObject obj, Transform target, float height, float duration, Monsters monsters, float damage, IceMage iceMage)
    {
        obj.transform.SetParent(null);

        Vector3 startPos = transform.position;
        Vector3 midPos = (startPos + target.position) / 2 + Vector3.up * height; // 弧の頂点
        Vector3 endPos = target.position;


        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;

            // 2次ベジェ曲線 (start → mid → end)
            Vector3 m1 = Vector3.Lerp(startPos, midPos, t);
            Vector3 m2 = Vector3.Lerp(midPos, endPos, t);
            obj.transform.position = Vector3.Lerp(m1, m2, t);

            time += Time.deltaTime;
            yield return null;
        }

        // 最後に位置をピタッと合わせる
        Instantiate(snowballExpPar, obj.transform.position, Quaternion.identity);
        iceMage.Attack(monsters, damage);
        Destroy(obj);
    }
}
