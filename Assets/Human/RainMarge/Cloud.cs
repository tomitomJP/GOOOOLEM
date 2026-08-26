using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Cloud : MonoBehaviour
{

    [SerializeField] GameObject par0;
    [SerializeField] GameObject par1;
    [SerializeField] GameObject cloud;
    [SerializeField] GameObject rain;


    [SerializeField] List<Monsters> targets = new List<Monsters>();

    int playerNum = 1;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator ArcMove(Transform target)
    {

        float height = 4;
        Vector3 targetPos = target.position;

        targetPos.x *= 0.6f;
        Vector3 startPos = transform.position;
        Vector3 midPos = (startPos + targetPos) / 2 + Vector3.up * height; // 弧の頂点
        Vector3 endPos = targetPos;


        float time = 0f;
        float duration = 0.8f;
        while (time < duration)
        {
            float t = time / duration;

            // 2次ベジェ曲線 (start → mid → end)
            Vector3 m1 = Vector3.Lerp(startPos, midPos, t);
            Vector3 m2 = Vector3.Lerp(midPos, endPos, t);
            transform.position = Vector3.Lerp(m1, m2, t);

            time += Time.deltaTime;
            yield return null;
        }
        par0.SetActive(false);
        par1.SetActive(true);

        float targetY = targetPos.y + Random.Range(0.5f, 3);

        yield return transform.DOMoveY(targetY, 0.7f).SetEase(Ease.InQuad).WaitForCompletion();
        par1.SetActive(false);
        cloud.SetActive(true);

        yield return cloud.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBounce).WaitForCompletion();

        yield return new WaitForSeconds(2f);
        rain.SetActive(true);

        yield return transform.DOMoveX(-13, 30f);

        for (int i = 0; i < 30; i++)
        {
            Attack();
            yield return new WaitForSeconds(1f);
        }
        Destroy(gameObject);

    }
    void Attack()
    {
        for (int i = targets.Count - 1; i >= 0; i--)
        {
            targets[i].Damaged(3, null, false);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster") || collision.CompareTag("House"))
        {
            Monsters monsters = collision.gameObject.GetComponent<Monsters>();
            if (playerNum != monsters.player)
            {

                if (!monsters.gameObject.TryGetComponent<House>(out House house))
                {
                    targets.Add(monsters);
                }

            }
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster") || collision.CompareTag("House"))
        {
            Monsters monsters = collision.gameObject.GetComponent<Monsters>();
            if (playerNum != monsters.player)
            {
                if (targets.Contains(monsters)) targets.Remove(monsters);

            }
        }
    }
}
