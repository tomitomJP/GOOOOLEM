using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileControl : MonoBehaviour
{
    [SerializeField] ControlManager controlManager;
    [SerializeField] PazzleManager pazzleManager;
    [SerializeField] Transform pazzleField;
    [SerializeField] LayerMask peaceLayer;


    public int peaceNumber = -1;
    public List<GameObject> checkingPeace = new List<GameObject>();


    void Update()
    {
        if (!controlManager.CanCheckPeace) return;

        if (Input.touchCount <= 0)
            return;

        Touch touch = Input.GetTouch(0);

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(touch.position);
        worldPos.z = 0f;

        Touching((Vector2)worldPos, touch.phase);
    }



    void Touching(Vector2 pos, TouchPhase phase)
    {

        // 指を離した処理は最優先
        if (phase == TouchPhase.Ended || phase == TouchPhase.Canceled)
        {
            Debug.Log("Ended");

            if (checkingPeace.Count > 0)
            {
                controlManager._DeletePeace(new List<GameObject>(checkingPeace));
            }

            checkingPeace.Clear();
            peaceNumber = -1;

            return;
        }


        Collider2D col = Physics2D.OverlapPoint(pos, peaceLayer);

        if (col == null)
            return;


        GameObject selectingPeace = col.gameObject;

        Peace p = selectingPeace.GetComponent<Peace>();

        if (p == null)
            return;



        // 押した瞬間
        if (phase == TouchPhase.Began)
        {
            Debug.Log("Began");

            p.check = true;

            peaceNumber = p.peaceNumber;

            checkingPeace.Add(selectingPeace);

            pazzleManager.BrickCount(checkingPeace.Count);
            pazzleManager.HilightPeace(checkingPeace, peaceNumber);
        }



        // ドラッグ中
        else if (phase == TouchPhase.Moved ||
         phase == TouchPhase.Stationary)
        {
            if (checkingPeace.Count == 0)
            {
                p.check = true;
                peaceNumber = p.peaceNumber;
                checkingPeace.Add(selectingPeace);
            }
            else
            {
                GameObject lastPeace = checkingPeace[checkingPeace.Count - 1];


                // 戻り操作
                if (checkingPeace.Count > 1 &&
                    selectingPeace == checkingPeace[checkingPeace.Count - 2])
                {
                    GameObject removePeace = checkingPeace[checkingPeace.Count - 1];

                    Peace remove = removePeace.GetComponent<Peace>();

                    if (remove != null)
                        remove.check = false;


                    checkingPeace.RemoveAt(checkingPeace.Count - 1);

                    peaceNumber = NumberCheck(checkingPeace);
                }


                // 新規追加
                else if (
                    !p.check && IsNeighbor(selectingPeace, lastPeace)
                )
                {
                    if (p.peaceNumber == peaceNumber ||
                       p.peaceNumber == 4 ||
                       peaceNumber == 4)
                    {
                        p.check = true;

                        if (peaceNumber == 4) peaceNumber = p.peaceNumber;

                        checkingPeace.Add(selectingPeace);
                    }
                }
            }


            pazzleManager.BrickCount(checkingPeace.Count);
            pazzleManager.HilightPeace(checkingPeace, peaceNumber);
        }
    }

    [SerializeField] float peaceDistance = 2;
    bool IsNeighbor(GameObject a, GameObject b)
    {
        float x = Mathf.Abs(a.transform.position.x - b.transform.position.x);
        float y = Mathf.Abs(a.transform.position.y - b.transform.position.y);

        // 縦横1マスのみ
        bool horizontal = x > 0 && y == 0;
        bool vertical = x == 0 && y > 0;

        return (horizontal || vertical) && (x + y <= peaceDistance);
    }



    int NumberCheck(List<GameObject> peaces)
    {
        int num = -1;


        foreach (GameObject peace in peaces)
        {
            Peace p = peace.GetComponent<Peace>();

            if (p == null)
                continue;


            num = p.peaceNumber;


            if (p.peaceNumber != 4)
            {
                break;
            }
        }


        return num;
    }

}