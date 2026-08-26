using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PazzleManager_1 : MonoBehaviour
{
    [SerializeField] int fieldWidth = 10;
    [SerializeField] int fieldHeight = 10;
    public GameObject[] peaces;
    [SerializeField] float spacing = 1.1f; // ピースの間隔
    [SerializeField] GameObject peacePearent;

    Dictionary<Vector2Int, GameObject> grid = new Dictionary<Vector2Int, GameObject>();
    List<GameObject> destroyPeace = new List<GameObject>();
    public GameObject blackScreen;

    public Transform pointer;
    public GameObject[] Houses;
    public Transform BattleField;

    void Start()
    {
        StartCoroutine(PeaceSet());
    }

    [SerializeField] bool movepeace = false;
    void Update()
    {
        // PeaceSet();

        if (movepeace)
        {
            movepeace = false;
        }
    }

    public IEnumerator PeaceSet()
    {
        int check = 1;
        while (check > 0)
        {
            check = 0;
            for (int i = 0; i < 11; i++)
            {
                Vector2 localPos = new Vector2((i * 0.5f) - 2.5f, 2.5f);
                Vector2 worldPos = (Vector2)transform.TransformPoint(localPos) - new Vector2(0, 0.25f);
                DrawCircle2D(worldPos, 0.25f);
                if (Physics2D.OverlapPoint(worldPos, LayerMask.GetMask("Peace")) == null)
                {
                    if (Random.Range(0, 20) == 0)
                    {
                        Instantiate(peaces[4], worldPos, Quaternion.identity, peacePearent.transform);

                    }
                    else
                    {
                        Instantiate(peaces[Random.Range(0, peaces.Length - 1)], worldPos, Quaternion.identity, peacePearent.transform);

                    }
                    check++;
                    yield return null;
                }
            }
            if (check > 0)
            {
                yield return new WaitForSeconds(0.5f);
            }

        }
        blackScreen.SetActive(false);


    }



    void DrawCircle2D(Vector3 center, float radius, int segments = 30, Color color = default)
    {
        if (color == default) color = Color.green;

        float angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float angleA = Mathf.Deg2Rad * angleStep * i;
            float angleB = Mathf.Deg2Rad * angleStep * (i + 1);

            Vector3 pointA = center + new Vector3(Mathf.Cos(angleA), Mathf.Sin(angleA), 0) * radius;
            Vector3 pointB = center + new Vector3(Mathf.Cos(angleB), Mathf.Sin(angleB), 0) * radius;

            Debug.DrawLine(pointA, pointB, color, 0f, false);
        }
    }

}
