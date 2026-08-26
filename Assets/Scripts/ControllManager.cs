using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using UnityEngine.Timeline;


public class ControlManager : MonoBehaviour
{
    public int playerNumber = 0;
    public Vector2 PlayerPointer;
    [SerializeField] float speed = 10f;

    Transform pointer;
    public PazzleManager pazzleManager;
    GameObject[] Houses;
    Transform BattleField;
    Transform MonstersPearent;

    [SerializeField] GameObject[] golemHead;
    [SerializeField] GameObject[] Monsters;
    [SerializeField] GameObject[] MonstersPlus;

    private GameInput controls;  // InputSystem用のコントロール
    private Vector2 moveInput;
    private bool choiceInput;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction choiceAction;
    private InputAction peaceDeleteAction;
    private InputAction openRule;
    [SerializeField] bool choiseMode = false;
    [SerializeField] private bool choiceDown = false;
    [SerializeField] private bool choiceUp = false;
    [SerializeField] GameObject brokenText;
    [SerializeField] LineRenderer lineRenderer;
    public GameObject soulSpwanPar;
    [SerializeField] GameObject brokenPeaceParticle;

    Canvas canvas;

    GameManager gameManager;
    [SerializeField] AudioClip peaceDeleteSound;

    CannonController cannon;

    [SerializeField] bool mobileMode = false;
    void Awake()
    {
        if (mobileMode) return;
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        choiceAction = playerInput.actions["Choice"];
        peaceDeleteAction = playerInput.actions["PeaceDelete"];

        openRule = playerInput.actions["OpenRule"];

        choiceAction.started += ctx => choiceDown = true;   // 押した瞬間
        peaceDeleteAction.started += ctx => choiceUp = true;     // 離した瞬間
    }

    IEnumerator Start()
    {
        canvas = GameObject.FindWithTag("Canvas").GetComponent<Canvas>();
        pazzleManager = GameObject.Find("Field_" + playerNumber.ToString()).GetComponent<PazzleManager>();
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();

        gameManager.playerInputs[playerNumber] = playerInput;
        cannon = gameManager.cannons[playerNumber];

        transform.SetParent(pazzleManager.transform);
        pointer = pazzleManager.pointer;
        Houses = pazzleManager.Houses;
        BattleField = pazzleManager.BattleField;
        MonstersPearent = pazzleManager.MonstersPearent;
        pazzleManager.controllManager = this;

        while (pazzleManager.enabled == false)
        {
            yield return null;
        }
        yield return pazzleManager.PeaceSet();

        CanCheckPeace = true;

        if (mobileMode) yield break;

        GetArrow();
    }

    void Update()
    {
        if (mobileMode) return;

        if (gameManager.gameOver || gameManager.mode == GameManager.Mode.ready || gameManager.mode == GameManager.Mode.deathMatchReady) { return; }
        // MoveとChoiceのアクションを取得
        moveInput = moveAction.ReadValue<Vector2>();  // Move: WASD または Gamepad Left Stick
        choiceInput = choiceAction.triggered;  // Choice: Space または Gamepad A



        if (!CanCheckPeace)
        {
            choiceDown = false;
            choiceUp = false;
            choiseMode = false;
        }

        LineView();
        SelectPeace();
        PointMove();
        CheckPeaceDown();
        CheckPeace();
        CheckPeaceUp();
        DrawCircle2D(pointer.position, pointerSize);
        OpenMonstarsRule();
        CannonShoot();
    }

    Vector2 GetFourDirection(Vector2 input)
    {
        if (input == Vector2.zero) return Vector2.zero;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            return input.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            return input.y > 0 ? Vector2.up : Vector2.down;
        }
    }

    void MovePointer(Vector2 dir)
    {
        Vector3 move = dir * 0.5f; // 1マス移動
        pointer.localPosition += move;

        float X = Mathf.Clamp(pointer.localPosition.x, -2.5f, 2.5f);
        float Y = Mathf.Clamp(pointer.localPosition.y, -2.5f, 2.5f);

        pointer.localPosition = new Vector2(X, Y);
    }


    void OpenMonstarsRule()
    {

        if (openRule == null) return;

        // 押された瞬間かどうか
        if (openRule.WasPressedThisFrame())
        {
            pazzleManager.MonstarsRule.transform.DOLocalMove(pazzleManager.MonstarsRulePos[0], 0.5f);
            Debug.Log("Press");
        }

        // 離された瞬間かどうか
        if (openRule.WasReleasedThisFrame())
        {
            pazzleManager.MonstarsRule.transform.DOLocalMove(pazzleManager.MonstarsRulePos[1], 0.5f);
            Debug.Log("Release");
        }
    }



    // 変数定義（クラス内に入れてください）
    private Vector2 moveDirection = Vector2.zero;
    private float moveDelay = 0.5f;       // 最初の連続移動までの待ち時間
    private float repeatRate = 0.1f;      // 押し続けた場合の連続間隔
    private float moveTimer = 0f;
    private bool isHolding = false;
    Vector2 canDirection;

    // Update() 内で moveInput 読み取り後に呼ばれる
    void PointMove()
    {
        Vector2 input = GetFourDirection(moveInput);

        if (input != moveDirection)
        {
            moveDirection = input;
            moveTimer = moveDelay;
            isHolding = true;

            if (moveDirection != Vector2.zero && CanMoveDirection(moveDirection))
            {
                MovePointer(moveDirection);
            }
        }
        else if (moveDirection != Vector2.zero && isHolding)
        {
            moveTimer -= Time.deltaTime;
            if (moveTimer <= 0f && CanMoveDirection(moveDirection))
            {
                MovePointer(moveDirection);
                moveTimer = repeatRate;
            }
        }

        if (input == Vector2.zero)
        {
            moveDirection = Vector2.zero;
            isHolding = false;
        }
    }

    // 方向に基づいて pointer を動かす（1マス）
    bool CanMoveDirection(Vector2 dir)
    {
        if (checkingPeace.Count == 0) return true;

        // 横方向の制限
        if (dir == Vector2.right)
            return canDirection.x == 1 || canDirection.x == 2;
        if (dir == Vector2.left)
            return canDirection.x == -1 || canDirection.x == 2;

        // 縦方向の制限
        if (dir == Vector2.up)
            return canDirection.y == 1 || canDirection.y == 2;
        if (dir == Vector2.down)
            return canDirection.y == -1 || canDirection.y == 2;

        return false;
    }


    GameObject selectingPeace;
    GameObject selectingPeacePrev;

    void SelectPeace()
    {
        Collider2D col;

        if (Physics2D.OverlapCircle(pointer.position, 0, LayerMask.GetMask("Peace")))
        {
            col = Physics2D.OverlapCircle(pointer.position, 0, LayerMask.GetMask("Peace"));
            selectingPeace = col.gameObject;
        }

        if (selectingPeacePrev == null || selectingPeacePrev != selectingPeace)
        {
            if (selectingPeacePrev != null && selectingPeacePrev.activeSelf)
            {
                selectingPeacePrev.GetComponent<Peace>().selecting = false;
            }

            if (selectingPeace != null && selectingPeace.activeSelf)
            {
                selectingPeacePrev = selectingPeace;
                selectingPeace.GetComponent<Peace>().selecting = true;
            }
        }
    }

    void CheckPeaceDown()
    {
        if (choiceDown && CanCheckPeace && !choiseMode)
        {
            choiseMode = true;
            choiceUp = false;
            choiceDown = false;
            Collider2D col;

            if (selectingPeace != null)
            {
                Peace p = selectingPeace.gameObject.GetComponent<Peace>();

                p.check = true;
                SelectPeaceNumber = p.peaceNumber;
                checkingPeace.Add(selectingPeace);
                canDirection = pazzleManager.HilightPeace(checkingPeace, SelectPeaceNumber);
                ArrowView();
                pazzleManager.BrickCount(checkingPeace.Count);

            }
        }
    }

    [SerializeField] float cannonCT = 1.5f;
    float cannonCTTimer = 0;

    void CannonShoot()
    {
        if (cannonCTTimer > 0)
        {
            cannonCTTimer -= Time.deltaTime;
            return;
        }

        if (choiceUp && CanCheckPeace && !choiseMode && gameManager.mode == GameManager.Mode.play && gameManager.cannonsChages[playerNumber] >= 10)
        {
            cannonCTTimer = cannonCT;
            gameManager.cannonsChages[playerNumber] -= 10;
            cannon.Shoot();
            choiceUp = false;
        }

    }

    public void ButtonCannonShoot()
    {
        if (gameManager.mode == GameManager.Mode.play && gameManager.cannonsChages[playerNumber] >= 10)
        {
            cannonCTTimer = cannonCT;
            gameManager.cannonsChages[playerNumber] -= 10;
            cannon.Shoot();
            choiceUp = false;
        }
    }

    [SerializeField] float pointerSize = 1;
    public List<GameObject> checkingPeace = new List<GameObject>();

    [SerializeField] float chainDis = 1.25f;
    public bool CanCheckPeace = true;
    [SerializeField] int SelectPeaceNumber = -1;

    void CheckPeace()
    {
        if (choiseMode && CanCheckPeace && checkingPeace.Count > 0)
        {
            Collider2D col = Physics2D.OverlapPoint(pointer.position);

            if (selectingPeace != null)
            {
                Peace p = selectingPeace.gameObject.GetComponent<Peace>();
                Peace pPrev = null;
                if (checkingPeace[checkingPeace.Count - 1] != null)
                {
                    pPrev = checkingPeace[checkingPeace.Count - 1].GetComponent<Peace>();
                }

                if (p != null && pPrev != null)
                {
                    float dis = Vector2.Distance(p.transform.position, pPrev.transform.position);
                    bool isPrevPeace = checkingPeace.Count > 1 && selectingPeace == checkingPeace[checkingPeace.Count - 2];

                    if (
                        (p.check == false && chainDis >= dis && (SelectPeaceNumber == p.peaceNumber || 4 == p.peaceNumber || SelectPeaceNumber == 4)) // 新規選択
                        || isPrevPeace // 戻る操作を許可
                    )
                    {
                        if (isPrevPeace)
                        {
                            // 戻る時はチェックを解除して一つ戻る
                            pPrev.check = false;
                            checkingPeace.RemoveAt(checkingPeace.Count - 1);
                            if (checkingPeace.Count == 1)
                            {
                                SelectPeaceNumber = p.peaceNumber;
                            }
                        }
                        else
                        {
                            p.check = true;
                            checkingPeace.Add(selectingPeace);
                            if (p.peaceNumber != 4)
                            {
                                SelectPeaceNumber = p.peaceNumber;
                            }
                        }

                        pazzleManager.BrickCount(checkingPeace.Count);
                        canDirection = pazzleManager.HilightPeace(checkingPeace, SelectPeaceNumber);
                        ArrowView();
                    }
                }

            }
        }
    }

    void CheckPeaceUp()
    {
        if (choiceUp && CanCheckPeace && checkingPeace.Count > 0 && choiseMode)
        {
            choiseMode = false;
            choiceUp = false;
            CanCheckPeace = false;
            StartCoroutine(deletePeace(new List<GameObject>(checkingPeace)));
        }
    }
    public void _checkPeaceUp()
    {
        if (CanCheckPeace && checkingPeace.Count > 0 && choiseMode)
        {
            choiseMode = false;
            choiceUp = false;
            CanCheckPeace = false;
            StartCoroutine(deletePeace(new List<GameObject>(checkingPeace)));
        }

    }

    int[] soloGame_Monsters_LevelGageValue = new int[4];
    int[] soloGame_Monsters_Level = new int[4];
    float[] soloGame_Monsters_Level_Values = { 0.2f, 0.2f, 0.1f, 0.1f };


    public void _DeletePeace(List<GameObject> DeletingPeace)
    {
        StartCoroutine(deletePeace(new List<GameObject>(DeletingPeace)));
    }

    IEnumerator deletePeace(List<GameObject> DeletingPeace)
    {
        pazzleManager.ResetHilightPeace();
        ResetArrow();

        CanCheckPeace = false;
        pazzleManager.blackScreen.SetActive(true);
        pazzleManager.BrickCountOff();

        int goleBodyCount = 0;
        int peaceNumber = -1;
        int soulCount = 0;
        float statusUprate = 0;
        for (int i = 0; i < DeletingPeace.Count; i++)
        {
            Peace PACE = DeletingPeace[i].GetComponent<Peace>();

            if (PACE.peaceNumber == 4)
            {
                if (soulCount == 0)
                {
                    soulCount++;
                }
                else if (soulCount == 1)
                {
                    soulCount++;
                    goleBodyCount++;

                }
                else
                {
                    goleBodyCount++;
                    statusUprate += 0.05f;
                }
            }
            else
            {
                if (peaceNumber == -1)
                {
                    peaceNumber = PACE.peaceNumber;
                }
                goleBodyCount++;
                if (goleBodyCount >= 11) statusUprate += 0.025f;
            }
        }

        if (pazzleManager.soloMode && soulCount == 0)
        {
            Color fillColor;
            switch (peaceNumber)
            {
                case 0:
                    fillColor = Color.green;
                    break;
                case 1:
                    fillColor = Color.red;
                    break;
                case 2:
                    fillColor = Color.cyan;
                    break;
                default:
                    fillColor = Color.yellow;
                    break;
            }
            pazzleManager.levelGage.gameObject.SetActive(true);
            pazzleManager.levelGageFull.color = fillColor;
        }

        for (int i = 0; i < DeletingPeace.Count; i++)
        {
            DeletingPeace[i].GetComponent<SpriteRenderer>().enabled = false;
            Peace PACE = DeletingPeace[i].GetComponent<Peace>();

            DeletingPeace[i].GetComponent<SpriteRenderer>().sortingOrder = 16;


            float pitch = Mathf.Clamp((0.1f * i) + 0.7f, 0.7f, 1.6f);
            AudioManager.PlaySEWithPitch(peaceDeleteSound, pitch);

            ParticleSystem particleSystem = Instantiate(brokenPeaceParticle, DeletingPeace[i].transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            var main = particleSystem.main;
            main.startColor = Color.white;


            if (soulCount > 0)
            {
                gameManager.cannonsChages[playerNumber] += 0.8f;

            }
            else
            {
                gameManager.cannonsChages[playerNumber] += 2.5f;

            }

            if (pazzleManager.soloMode && soulCount == 0)
            {
                soloGame_Monsters_LevelGageValue[peaceNumber]++;
                if (soloGame_Monsters_LevelGageValue[peaceNumber] > 9)
                {
                    soloGame_Monsters_LevelGageValue[peaceNumber] = 0;
                    soloGame_Monsters_Level[peaceNumber]++;
                    StartCoroutine(LevelUpTextAnim(peaceNumber));
                }
                pazzleManager.levelGage.value = (float)soloGame_Monsters_LevelGageValue[peaceNumber] / 10f;
            }


            int j = i + 1;
            float value = j + (j * j / 15f);

            if (gameManager.mode == GameManager.Mode.deathMatch && i == 0)
            {
                gameManager.DMGolemPowers[playerNumber] += value;
            }
            else if (gameManager.mode == GameManager.Mode.deathMatch)
            {
                float prev = i + (i * i / 15f);
                gameManager.DMGolemPowers[playerNumber] += value - prev;
            }

            yield return new WaitForSeconds(0.1f);
        }

        gameManager.resultDatas[playerNumber].deletePeaceCount += DeletingPeace.Count;
        Vector2 offset = new Vector2(0, 1f);
        BrokenText text = Instantiate(brokenText, DeletingPeace[DeletingPeace.Count - 1].transform.position + (Vector3)offset, Quaternion.identity, canvas.transform).GetComponent<BrokenText>();

        text.mode = gameManager.mode;
        text.num = DeletingPeace.Count;
        text.Spwan = soulCount > 0;

        yield return new WaitForSeconds(0.2f);

        GameObject newSoul = null;
        int count = 0;
        while (DeletingPeace.Count > 0)
        {
            count++;
            //  Debug.Log(count);
            Vector2 pos = DeletingPeace[0].transform.localPosition;
            if ((count >= 6 || soulCount > 1) && DeletingPeace.Count == 1)
            {

                newSoul = Instantiate(pazzleManager.peaces[4], pazzleManager.peacePearent.transform);
                newSoul.transform.localPosition = pos;
            }
            Destroy(DeletingPeace[0]);
            DeletingPeace.RemoveAt(0);
        }

        if (soulCount > 0 && gameManager.mode == GameManager.Mode.play)
        {

            Monsters monsters = MonstatSpawn(goleBodyCount, soulCount);

            if (pazzleManager.soloMode)
            {
                monsters.hp += soloGame_Monsters_Level_Values[0] * soloGame_Monsters_Level[0] * monsters.hp;
                monsters.atk += soloGame_Monsters_Level_Values[1] * soloGame_Monsters_Level[1] * monsters.atk;
                monsters.spd += soloGame_Monsters_Level_Values[2] * soloGame_Monsters_Level[2] * monsters.spd;
                monsters.atkSpdRate += soloGame_Monsters_Level_Values[3] * soloGame_Monsters_Level[3];


            }

            switch (peaceNumber)
            {
                case 0:
                    monsters.hp *= 1.3f + statusUprate;
                    break;
                case 1:
                    monsters.atkRate *= 1.3f + statusUprate;
                    break;
                case 2:
                    monsters.spdRate *= 1.15f + statusUprate;
                    break;

                case 3:
                    monsters.atkSpdRate *= 1.25f + statusUprate;
                    break;

            }

            if (SoloManager.instance != null && pazzleManager.soloMode)
            {
                string info = "";
                info += $"{monsters.mName}を召喚\n 攻撃力:{monsters.atk * monsters.atkRate} HP:{(monsters.hp)} \n すばやさ:{monsters.spd * monsters.spdRate} 攻撃速度:{monsters.atkSpdRate}";

                SoloManager.SetHT(info, SoloManager.ToHex(Color.cyan));
            }
        }



        checkingPeace.Clear();
        SelectPeaceNumber = -1;

        yield return new WaitForSeconds(0.5f);

        yield return pazzleManager.PeaceSet();

        if (newSoul != null) Instantiate(soulSpwanPar, newSoul.transform.position, Quaternion.identity);
        if (pazzleManager.soloMode) pazzleManager.levelGage.gameObject.SetActive(false);

        CanCheckPeace = true;
    }


    public Monsters MonstatSpawn(int Count, int soulCount = 1)
    {
        gameManager.resultDatas[playerNumber].spawnCount++;
        pazzleManager.house.DoorAnimTrigger();
        int MonstaersNum = Mathf.Clamp(Count /*- 3*/, 0, 9);//-1.92
        Vector3 pos = Houses[playerNumber].transform.position;
        if (!mobileMode) pos.y = -5f;
        else pos.y -= 1f;
        GameObject monstaer;

        if (Count == 0)
        {
            if (Random.Range(0, 5) == 0)
            {
                monstaer = Instantiate(MonstersPlus[MonstaersNum], pos, Quaternion.identity, MonstersPearent);

            }
            else
            {
                monstaer = Instantiate(Monsters[MonstaersNum], pos, Quaternion.identity, MonstersPearent);

            }
        }
        else if (soulCount == 1)
        {
            monstaer = Instantiate(Monsters[MonstaersNum], pos, Quaternion.identity, MonstersPearent);

        }
        else
        {
            monstaer = Instantiate(MonstersPlus[MonstaersNum], pos, Quaternion.identity, MonstersPearent);

        }

        Monsters monsters = monstaer.GetComponent<Monsters>();
        monsters.player = playerNumber;

        return monsters;
    }

    void LineView()
    {
        if (checkingPeace.Count >= 2)
        {
            List<Vector3> points = new List<Vector3>();

            for (int i = 0; i < checkingPeace.Count; i++)
            {
                if (checkingPeace[i] == null)
                    continue;

                points.Add(checkingPeace[i].gameObject.transform.position);
            }

            if (points.Count >= 2)
            {
                lineRenderer.positionCount = points.Count;
                lineRenderer.SetPositions(points.ToArray());
            }
            else
            {
                lineRenderer.positionCount = 0;
            }
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }

    GameObject[] arrows = new GameObject[4];
    void ArrowView()
    {
        if (pointer == null || !pointer.gameObject.activeSelf) return;
        arrows[0].SetActive(CanMoveDirection(Vector2.right));
        arrows[1].SetActive(CanMoveDirection(Vector2.down));
        arrows[2].SetActive(CanMoveDirection(Vector2.left));
        arrows[3].SetActive(CanMoveDirection(Vector2.up));

    }

    void GetArrow()
    {
        if (pointer == null || !pointer.gameObject.activeSelf) return;

        for (int i = 0; i < pointer.childCount; i++)
        {
            arrows[i] = pointer.GetChild(i).gameObject;
            arrows[i].SetActive(false);

        }
    }

    void ResetArrow()
    {
        if (pointer == null || !pointer.gameObject.activeSelf) return;
        for (int i = 0; i < pointer.childCount; i++)
        {
            arrows[i].SetActive(false);
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

    IEnumerator LevelUpTextAnim(int pn)
    {
        GameObject obj = Instantiate(pazzleManager.levelUpText, pazzleManager.levelGage.transform.position, Quaternion.identity, pazzleManager.levelGage.transform.parent);
        Text txt = obj.GetComponent<Text>();
        if (mobileMode) txt.fontSize *= 2;
        txt.color = pazzleManager.levelGageFull.color;
        txt.text = "Lv " + soloGame_Monsters_Level[pn].ToString() + "\n";
        switch (pn)
        {
            case 0:
                txt.text += "HP+" + soloGame_Monsters_Level_Values[pn].ToString() + "%";
                break;
            case 1:
                txt.text += "ATK+" + soloGame_Monsters_Level_Values[pn].ToString() + "%";
                break;
            case 2:
                txt.text += "SPD+" + soloGame_Monsters_Level_Values[pn].ToString() + "%";
                break;
            case 3:
                txt.text += "ATKSPD+" + soloGame_Monsters_Level_Values[pn].ToString() + "%";
                break;
        }

        txt.text += "\nTOTAL" + ((1 + (soloGame_Monsters_Level_Values[pn] * soloGame_Monsters_Level[pn])) * 100).ToString() + "%";

        obj.transform.DOLocalMoveY(20, 0.5f).SetEase(Ease.OutBack);
        yield return new WaitForSeconds(1f);
        Destroy(obj);
    }
}
