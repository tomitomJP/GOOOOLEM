using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SoloManager : MonoBehaviour
{
    public static SoloManager instance;

    [SerializeField] PazzleManager pazzleManager;
    [SerializeField] TextAsset[] nameFile;  // 0=男性, 1=女性, 2=中性
    public string[][] names = new string[3][];

    [SerializeField] GameObject[] humans;

    private List<int> availableCharacters = new List<int>();
    private List<List<string>> availableNames = new List<List<string>>();
    [SerializeField] int humanLevel = 1;
    [SerializeField] Text LevelViewText;
    [SerializeField] Text waveTimeText;
    [SerializeField] House house;

    bool first = true;
    float waveTimer = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {



        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (true)
        {
            if (GameManager.GetMonsters(GameManager.type.human).Count == 0)
            {
                if (!first) yield return YushaGekiha();
                first = false;
                _SetHT("勇者一行が魔王城前に登場！");
                KnockBack();
                InstantiateMembers();
                //KnockBackColHuman();
            }

            if (Time.timeScale > 0)
            {
                waveTimeText.text = Mathf.Max(Mathf.FloorToInt(waveTimer / 60), 0).ToString("D2") + ":" + Mathf.Max(Mathf.FloorToInt(waveTimer % 60), 0).ToString("D2");

            }
            waveTimer += Time.deltaTime;
            yield return null;
        }
    }

    [SerializeField] Text[] gekiha;
    [SerializeField] RectTransform gekihaText;
    [SerializeField] AudioClip gekihaTextSE;
    [SerializeField] Text clearStatus;

    bool wininnView = false;
    IEnumerator YushaGekiha()
    {
        if (wininnView) yield break;
        wininnView = true;

        float clearTime = waveTimer;
        yield return new WaitForSecondsRealtime(0.6f);
        GameManager.toggleBrackScreen(true);
        Time.timeScale = 0;

        for (int i = 0; i < gekiha.Length; i++)
        {
            Text txt = gekiha[i];
            int startSize = 10;
            txt.fontSize = startSize;
            txt.gameObject.SetActive(true);
            txt.DOFade(1, 0f).SetUpdate(UpdateType.Normal, true);
            clearStatus.DOFade(1, 0f).SetUpdate(UpdateType.Normal, true);
            // フォントサイズを60に0.5秒で変更
            DOTween.To(() => txt.fontSize, x => txt.fontSize = x, 100, 0.5f).SetUpdate(UpdateType.Normal, true).SetEase(Ease.OutBack);
            AudioManager.PlaySE(gekihaTextSE);
            yield return new WaitForSecondsRealtime(0.3f);
        }

        yield return new WaitForSecondsRealtime(0.5f);
        yield return gekihaText.DOAnchorPosY(100, 0.2f).SetUpdate(UpdateType.Normal, true).SetEase(Ease.InQuad).WaitForCompletion();
        yield return new WaitForSecondsRealtime(0.25f);

        int beforeLv = humanLevel;

        clearStatus.text = "勇者どもを返り討ちにする" + "+ Lv01\n";
        humanLevel += 4;

        float houseHeal = 10;

        yield return new WaitForSecondsRealtime(0.25f);

        if (clearTime <= 10)
        {
            clearStatus.text += "10秒以内に勝利" + "  + Lv06\n";
            houseHeal += 10;
        }
        else if (clearTime <= 20)
        {
            clearStatus.text += "20秒以内に勝利" + "  + Lv04\n";
            houseHeal += 10;
        }
        else if (clearTime <= 30)
        {
            clearStatus.text += "30秒以内に勝利" + "  + Lv03\n";
            houseHeal += 5;

        }
        else if (clearTime <= 60)
        {
            clearStatus.text += "60秒以内に勝利" + " + Lv01\n";
            houseHeal += 5;
        }
        else if (clearTime <= 120)
        {
            clearStatus.text += "120秒以内に勝利" + " + Lv01\n";
            houseHeal += 5;
        }
        yield return new WaitForSecondsRealtime(0.25f);

        int savesMon = GameManager.GetMonsters(GameManager.type.mon0).Count;
        int up = Mathf.FloorToInt((savesMon - 3) * 2);
        clearStatus.text += $"モンスター生存({(savesMon - 3).ToString("N1")}体)" + $" + Lv{up.ToString("N1")}\n";

        houseHeal += up;

        yield return new WaitForSecondsRealtime(0.25f);

        clearStatus.text += $"{beforeLv} ====> {humanLevel}\n";
        LevelViewText.text = $"Lv {humanLevel}";

        yield return new WaitForSecondsRealtime(0.25f);
        clearStatus.text += $"ボーナス 城修復 +{houseHeal}\n";

        house.hp += houseHeal;

        if (house.hp > house.hpMax)
        {
            float overflow = house.hp - house.hpMax;

            float upMaxHp = overflow * 0.5f;

            upMaxHp = Mathf.Round(upMaxHp);
            upMaxHp = Mathf.Min(upMaxHp, 10);

            house.hpMax += upMaxHp;

            house.hp = house.hpMax;

            yield return new WaitForSecondsRealtime(0.25f);
            clearStatus.text += $"ボーナス 城上限突破 +{upMaxHp}\n";

        }

        yield return new WaitForSecondsRealtime(0.25f);
        int soultBonus = (int)Mathf.Clamp(houseHeal / 7, 1, 10);

        clearStatus.text += $"ソウルボーナス +{soultBonus}";

        pazzleManager.SoultBonus(soultBonus);



        yield return new WaitForSecondsRealtime(2f);

        for (int i = 0; i < gekiha.Length; i++)
        {
            Text txt = gekiha[i];
            int startSize = txt.fontSize;
            txt.DOFade(0, 0.3f).SetUpdate(UpdateType.Normal, true);
            clearStatus.DOFade(0, 0.3f).SetUpdate(UpdateType.Normal, true);

        }
        yield return new WaitForSecondsRealtime(0.3f);

        for (int i = 0; i < gekiha.Length; i++)
        {
            Text txt = gekiha[i];
            txt.gameObject.SetActive(false);
            clearStatus.text = "";
        }
        waveTimer = 0;
        Time.timeScale = 1;
        GameManager.toggleBrackScreen(false);

        wininnView = false;
        yield break;
    }

    string menberInfo = "";
    void InstantiateMembers()
    {
        menberInfo = "";
        availableCharacters = new List<int>();
        availableNames = new List<List<string>>();
        // 名前ファイルを読み込む
        for (int i = 0; i < names.Length; i++)
        {
            names[i] = nameFile[i].text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> nameList = new List<string>(names[i]);
            Shuffle(nameList);
            availableNames.Add(nameList);
        }

        // キャラクター番号リストを作成してシャッフル
        for (int i = 0; i < humans.Length; i++)
            availableCharacters.Add(i);
        Shuffle(availableCharacters);

        // 1人目は humans[0] 固定
        InstantiateMember(0, 0);

        // 残り3人はランダム
        for (int i = 0; i < 3; i++)
        {
            InstantiateMember(-1, i + 1); // -1はランダム
        }

        _SetHT(menberInfo);
    }

    void InstantiateMember(int fixedIndex, int count)
    {
        int charIndex;
        if (fixedIndex >= 0)
        {
            charIndex = fixedIndex;
            availableCharacters.Remove(charIndex);
        }
        else
        {
            // ランダムで被らないキャラクターを選ぶ
            charIndex = availableCharacters[0];
            availableCharacters.RemoveAt(0);
        }

        Vector3 offset = Vector3.zero;
        offset.x = count + 0.2f;

        Human human = Instantiate(humans[charIndex], transform.position + offset, Quaternion.identity).GetComponent<Human>();

        human.gameObject.GetComponent<SpriteRenderer>().sortingOrder = -count;
        // 名前を取得（性別に応じたリストから被りなしで）
        int sexIndex = (int)human.sex;
        string name = availableNames[sexIndex][0];
        availableNames[sexIndex].RemoveAt(0);

        human.name = name;
        human.level = humanLevel;
        GetHP(human.level, human);
        GetATK(human.level, human);
        StartCoroutine(human.First());

        menberInfo += $"{human.job} 攻撃力:{human.atk} HP:{human.hp}\n";

    }

    public void GetHP(int level, Human human)
    {
        level = Mathf.Max(level, 1);

        float hpValue = human.hp + (400f * ((level + 10) - 1f) / 90f);
        human.hp = Mathf.Floor(hpValue);
    }

    public void GetATK(int level, Human human)
    {
        level = Mathf.Max(level, 1);

        float atkValue = human.atk * (1f + ((level - 1f) / 30f));
        human.atk = Mathf.Floor(atkValue);
    }

    // シャッフル用ユーティリティ
    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[r];
            list[r] = temp;
        }
    }

    [SerializeField] GameObject humanTextWindow;
    [SerializeField] Transform WindowParent;
    [SerializeField] Transform startPos;
    [SerializeField] Transform endPos;

    [SerializeField] List<GameObject> HTWindows = new List<GameObject>();
    List<GameObject> HTAnimOrder = new List<GameObject>();


    public static void SetHT(string message, string color = "#FFFFFF", Human sayer = null, string p1 = "A")
    {
        instance._SetHT(message, color, sayer, p1);
    }

    void _SetHT(string message, string color = "#FFFFFF", Human sayer = null, string p1 = "A")
    {
        GameObject ht = Instantiate(humanTextWindow, startPos.position, Quaternion.identity, WindowParent);
        Text tessageText = ht.GetComponentInChildren<Text>();
        tessageText.text = "";
        if (sayer != null)
        {

            tessageText.text = $"<color=#FFA500>Lv{sayer.level} {sayer.name} {sayer.job}</color> \n";
        }
        tessageText.text += $"<color={color}>{message}</color>";


        StartCoroutine(HTAnim(ht));
    }

    IEnumerator HTAnim(GameObject obj)
    {

        HTAnimOrder.Add(obj);
        while (obj != HTAnimOrder[0])
        {
            yield return null;
        }

        for (int i = 0; i < HTWindows.Count; i++)
        {
            Debug.Log(i);
            float newY = HTWindows[i].transform.localPosition.y + 80;
            HTWindows[i].transform.DOLocalMoveY(newY, 0.15f);
        }
        HTWindows.Add(obj);
        obj.transform.DOMove(endPos.position, 0.2f);
        yield return new WaitForSeconds(0.4f);
        HTAnimOrder.Remove(obj);

        yield return new WaitForSeconds(10f);

        HTWindows.Remove(obj);
        Destroy(obj);

    }

    /// <summary>
    /// Colorを#RRGGBB形式の文字列に変換
    /// </summary>
    public static string ToHex(Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255f);
        int g = Mathf.RoundToInt(color.g * 255f);
        int b = Mathf.RoundToInt(color.b * 255f);
        return $"#{r:X2}{g:X2}{b:X2}";
    }

    /// <summary>
    /// Colorを#RRGGBBAA形式の文字列に変換
    /// </summary>
    public static string ToHexWithAlpha(Color color)
    {
        int r = Mathf.RoundToInt(color.r * 255f);
        int g = Mathf.RoundToInt(color.g * 255f);
        int b = Mathf.RoundToInt(color.b * 255f);
        int a = Mathf.RoundToInt(color.a * 255f);
        return $"#{r:X2}{g:X2}{b:X2}{a:X2}";
    }

    float knockBackPower = 5.5f;
    void KnockBack()
    {
        StartCoroutine(KnockBackCol());

    }

    IEnumerator KnockBackCol()
    {
        var targets = GameManager.GetMonsters(GameManager.type.mon0);
        foreach (var target in targets)
        {

            Rigidbody2D rb = target.gameObject.GetComponent<Rigidbody2D>();
            GameObject mon = target.gameObject;

            mon.GetComponent<SpriteRenderer>().flipX = true;

            Vector2 monUp = mon.transform.up;
            Vector2 myLeft = -transform.right;
            Vector2 dire;
            dire = new Vector2(-1.5f, 0.3f).normalized;


            rb.AddForce(dire * knockBackPower, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(1);
        foreach (var target in targets)
        {
            if (target == null || !target.gameObject.activeSelf) continue;
            GameObject mon = target.gameObject;

            mon.GetComponent<SpriteRenderer>().flipX = false;

        }

        yield break;
    }

    void KnockBackColHuman()
    {
        var targets = GameManager.GetMonsters(GameManager.type.human);
        foreach (var target in targets)
        {

            Rigidbody2D rb = target.gameObject.GetComponent<Rigidbody2D>();
            GameObject mon = target.gameObject;


            Vector2 monUp = mon.transform.up;
            Vector2 myLeft = -transform.right;
            Vector2 dire;
            dire = new Vector2(-1.5f, 0.3f).normalized;


            rb.AddForce(dire * knockBackPower * Random.Range(0.8f, 1.3f), ForceMode2D.Impulse);
        }
    }
}
