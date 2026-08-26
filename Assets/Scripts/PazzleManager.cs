using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PazzleManager : MonoBehaviour
{
    [SerializeField] int fieldWidth = 10;
    [SerializeField] int fieldHeight = 10;
    public GameObject[] peaces;
    [SerializeField] float spacing = 1.1f; // ピースの間隔
    public GameObject peacePearent;

    Dictionary<Vector2Int, GameObject> grid = new Dictionary<Vector2Int, GameObject>();
    List<GameObject> destroyPeace = new List<GameObject>();
    public GameObject blackScreen;

    public Transform pointer;
    public GameObject[] Houses;
    public Transform BattleField;
    public Transform MonstersPearent;
    public ControlManager controllManager { get; set; }
    public House house;

    public Transform MonstarsRule;
    public Vector3[] MonstarsRulePos = new Vector3[2];
    public int soulSpawnRate = 10;
    public bool soloMode = false;
    public Slider levelGage;
    public Image levelGageFull;
    public GameObject levelUpText;
    void Start()
    {
        //StartCoroutine(PeaceSet());
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

    int[] CountPeace()
    {
        int[] counts = new int[11];

        foreach (Transform child in peacePearent.transform)
        {
            Vector3 local = transform.InverseTransformPoint(child.position);

            int x = Mathf.RoundToInt((local.x + 2.5f) / 0.5f);

            if (x >= 0 && x < 11)
                counts[x]++;
        }

        return counts;
    }

    [SerializeField] float fallSpeed = 10f;
    public IEnumerator PeaceSet()
    {
        // ① 落下
        PeaceFall();

        // ② 落下完了待ち（距離から最大時間計算でもいいけど簡易版）
        // yield return new WaitForSeconds(0.3f);

        // ③ 最新状態取得
        int[] counts = CountPeace();

        blackScreen.SetActive(true);

        int peaceCount = peacePearent.transform.childCount;

        while (peaceCount < 121)
        {
            for (int i = 0; i < 11; i++)
            {
                if (counts[i] >= 11) continue;

                Vector2 localPos = new Vector2((i * 0.5f) - 2.5f, 2.5f);

                GameObject newPiece;

                if (Random.Range(0, 100) <= soulSpawnRate)
                    newPiece = Instantiate(peaces[4], peacePearent.transform);
                else
                    newPiece = Instantiate(peaces[Random.Range(0, peaces.Length - 1)], peacePearent.transform);

                newPiece.transform.localPosition = localPos;

                // ④ 生成したら即カウント更新
                counts[i]++;
                peaceCount++;

                // ⑤ その場で落とす
                Vector2 targetLocal = new Vector2((i * 0.5f) - 2.5f, -2.5f + ((counts[i] - 1) * 0.5f));

                DOLocalMoveWithSpeed(newPiece.transform, targetLocal);
            }

            yield return new WaitForSeconds(0.05f);
        }

        ResetHilightPeace();
        blackScreen.SetActive(false);
    }

    void PeaceFall()
    {
        List<Transform>[] columns = new List<Transform>[11];
        for (int i = 0; i < 11; i++)
            columns[i] = new List<Transform>();

        // 列ごとに振り分け
        foreach (Transform child in peacePearent.transform)
        {
            Vector3 local = transform.InverseTransformPoint(child.position);
            int x = Mathf.RoundToInt((local.x + 2.5f) / 0.5f);

            if (x >= 0 && x < 11)
                columns[x].Add(child);
        }

        // 各列ごとに落下処理
        for (int x = 0; x < 11; x++)
        {
            // 下に詰めるためにYでソート（下から順）
            columns[x].Sort((a, b) => a.position.y.CompareTo(b.position.y));

            for (int y = 0; y < columns[x].Count; y++)
            {
                Vector2 localPos = new Vector2((x * 0.5f) - 2.5f, -2.5f + (y * 0.5f));
                Vector3 target = transform.TransformPoint(localPos);

                DOLocalMoveWithSpeed(columns[x][y], localPos);
            }
        }
    }

    public Tweener DOLocalMoveWithSpeed(Transform tr, Vector3 localTarget)
    {
        if (fallSpeed <= 0f) fallSpeed = 0.01f;

        float distance = Vector3.Distance(tr.localPosition, localTarget);
        float duration = distance / fallSpeed;

        return tr.DOLocalMove(localTarget, duration).SetEase(Ease.Linear);
    }

    [SerializeField] Text brickCountText;
    public void BrickCount(int num)
    {
        StartCoroutine(BrickCountAnim(num));
    }
    public void BrickCountOff()
    {
        brickCountText.enabled = false;
    }

    IEnumerator BrickCountAnim(int num)
    {
        brickCountText.enabled = true;
        float time;
        float timer;

        brickCountText.text = num.ToString();
        timer = 0;
        time = 0.15f;
        while (time >= timer)
        {
            float t = timer / time;
            brickCountText.fontSize = (int)Mathf.Lerp(40, 50, t);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        time = 0.1f;
        while (time >= timer)
        {
            float t = timer / time;
            brickCountText.fontSize = (int)Mathf.Lerp(50, 40, t);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public void SoultBonus(int num)
    {
        List<GameObject> availablePeaces = new List<GameObject>();

        foreach (Transform child in peacePearent.transform)
        {
            GameObject peaceObject = child.gameObject;
            Peace peace = peaceObject.GetComponent<Peace>();

            if (peace == null)
                continue;

            if (peace.peaceNumber == 4)
                continue;

            if (peace.check)
                continue;

            availablePeaces.Add(peaceObject);
        }

        // num個以下なら存在する分だけ使う
        int count = Mathf.Min(num, availablePeaces.Count);

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, availablePeaces.Count);

            GameObject peaceObject = availablePeaces[randomIndex];

            availablePeaces.RemoveAt(randomIndex);

            GameObject newPiece = Instantiate(peaces[4], peacePearent.transform);

            newPiece.transform.localPosition = peaceObject.transform.localPosition;

            Instantiate(
                controllManager.soulSpwanPar,
                newPiece.transform.position,
                Quaternion.identity,
                newPiece.transform
            );

            Destroy(peaceObject);
        }
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

    public void ResetHilightPeace()
    {
        for (int i = 0; i < peacePearent.transform.childCount; i++)
        {
            GameObject peaceG = peacePearent.transform.GetChild(i).gameObject;
            Peace peaceS = peaceG.GetComponent<Peace>();

            SpriteRenderer spr = peaceG.GetComponent<SpriteRenderer>();

            spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 1);
            if (peaceS.peaceNumber == 4)
            {
                peaceS.StartParticle(4);

            }
        }
    }


    [SerializeField] float peaceDistance = 0.6f;
    public Vector2 HilightPeace(List<GameObject> peces, int number)
    {
        Debug.Log("HilightPeace");
        ResetHilightPeace();

        Vector2 canDire = Vector2.zero;

        List<GameObject> hilightPeaces = new List<GameObject>();
        hilightPeaces.AddRange(peces);
        int firstCount = hilightPeaces.Count - 1;
        for (int n = firstCount; n < hilightPeaces.Count; n++)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 direction = Vector2.right;
                direction = GetDirection(i);

                Debug.DrawRay(hilightPeaces[n].transform.position, direction * peaceDistance, Color.red, 0.5f); // 可視化（長さに注意）
                RaycastHit2D[] hit = Physics2D.RaycastAll(hilightPeaces[n].transform.position, direction, peaceDistance, LayerMask.GetMask("Peace"));

                if (!(hit != null && hit.Length > 0)) continue;

                for (int j = 0; j < hit.Length; j++)
                {
                    Peace peace = hit[j].collider.gameObject.GetComponent<Peace>();
                    Peace Apeace = hilightPeaces[n].GetComponent<Peace>();

                    if (peace == null) continue;

                    if ((number == peace.peaceNumber || 4 == peace.peaceNumber || number == 4) && peace.check == false)
                    {
                        if (!hilightPeaces.Contains(peace.gameObject))
                        {
                            hilightPeaces.Add(peace.gameObject);
                            if (n == firstCount)
                            {
                                canDire = SetCanDir(i, canDire);
                            }
                        }
                    }
                    else if (hilightPeaces.Count >= 2 && firstCount >= 1)
                    {
                        if (n == firstCount && peace.gameObject == hilightPeaces[firstCount - 1])
                        {

                            canDire = SetCanDir(i, canDire);
                        }
                    }

                }

            }
        }

        for (int i = 0; i < peacePearent.transform.childCount; i++)
        {
            GameObject peaceG = peacePearent.transform.GetChild(i).gameObject;
            Peace peaceS = peaceG.GetComponent<Peace>();
            if (!hilightPeaces.Contains(peaceG) && peaceS.check == false)
            {
                SpriteRenderer spr = peaceG.gameObject.GetComponent<SpriteRenderer>();
                spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 0.1f);
                peaceS.StartParticle(-1);

            }
            else
            {
                SpriteRenderer spr = peaceG.gameObject.GetComponent<SpriteRenderer>();
                spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 1f);
                if (peaceS.peaceNumber == 4)
                {
                    peaceS.StartParticle(number);
                }

            }


        }
        return canDire;
    }

    Vector2 SetCanDir(int dir, Vector2 cd)
    {
        switch (dir)
        {
            case 0: // 右
                if (cd.x == -1 || cd.x == 2) cd.x = 2;
                else cd.x = 1;
                break;
            case 2: // 左
                if (cd.x == 1 || cd.x == 2) cd.x = 2;
                else cd.x = -1;
                break;
            case 1: // 上
                if (cd.y == -1 || cd.y == 2) cd.y = 2;
                else cd.y = 1;
                break;
            case 3: // 下
                if (cd.y == 1 || cd.y == 2) cd.y = 2;
                else cd.y = -1;
                break;
        }

        return cd;
    }

    Vector2 GetDirection(int n)
    {
        Vector2 direction = Vector2.zero;
        switch (n)
        {
            case 0:
                direction = Vector2.right;
                break;
            case 1:
                direction = Vector2.up;
                break;
            case 2:
                direction = Vector2.left;
                break;
            case 3:
                direction = Vector2.down;
                break;
        }
        return direction;
    }

}
