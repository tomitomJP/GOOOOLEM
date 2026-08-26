using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [SerializeField] bool debugMode = false;
    [SerializeField] int playerCount = 2;
    [SerializeField] Text[] Ready;
    [SerializeField] PlayerJoinManager playerJoinManager;
    [SerializeField] Text CountDownText;
    [SerializeField] PazzleManager[] pazzleManager;
    [SerializeField] GameObject ReadyCanvas;
    [SerializeField] GameObject BlackScreenObj;
    [SerializeField] GameObject RuleCanvas;
    public PlayerInput[] playerInputs = new PlayerInput[2];
    bool[] choiceDown = new bool[2];
    bool[] choiceUp = new bool[2];
    bool[] ReadyOk = new bool[2];

    [SerializeField] GameObject GameCanvas;
    [SerializeField] ResultVeiwer resultVeiwer;

    [SerializeField] AudioClip PlayBgm;
    [SerializeField] AudioClip deathMatchStartSe;
    [SerializeField] AudioClip ReadyBgm;
    [SerializeField] AudioClip ResultBgm;
    [SerializeField] AudioClip scoreSE;
    [SerializeField] AudioClip FinishSE;
    [SerializeField] AudioClip GameStartCount;
    [SerializeField] AudioClip GameStart;
    [SerializeField] AudioClip TimerRemind;
    [SerializeField] Text timerText;
    [SerializeField] float timer = 100;
    public ResultVeiwer.resultData[] resultDatas = new ResultVeiwer.resultData[2];

    [Header("大砲")]
    public CannonController[] cannons = new CannonController[2];
    public Slider[] cannonGaged;
    public float[] cannonsChages = { 0, 0 };
    public float maxCannonsChages = 80;

    [Header("ゴブリンの壁")]
    public GoblinBuild[] GoblinBuilds = new GoblinBuild[2];

    public enum Mode
    {
        ready,
        play,
        deathMatchReady,
        deathMatch,
    }

    public Mode mode = Mode.ready;

    [Header("ソロモード")]
    [SerializeField] bool soloMode = false;
    [SerializeField] SoloManager soloManager;

    [Header("モバイルモード")]
    [SerializeField] bool mobileMode = false;

    void Awake()
    {
        instance = this;
    }
    IEnumerator Start()
    {
        if (debugMode) { yield break; }
        if (!mobileMode)
        {
            yield return new WaitForSeconds(0.5f);
            AudioManager.PlayBGM(ReadyBgm);

            BlackScreenObj.SetActive(true);




            for (int i = 0; i < playerCount; i++)
            {
                int index = i; // i をキャプチャして固定
                playerInputs[index].actions["Choice"].performed += ctx => choiceDown[index] = true;
                playerInputs[index].actions["Choice"].canceled += ctx => choiceUp[index] = true;
            }

            int okCount = 0;

            while (okCount < playerCount)
            {
                for (int i = 0; i < playerCount; i++)
                {
                    if (choiceDown[i] && !ReadyOk[i])
                    {
                        ReadyOk[i] = true;
                        Ready[i].text = "OK";
                        okCount++;
                    }
                }

                yield return null;
            }

            RuleCanvas.SetActive(false);
            AudioManager.StopBGM();
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            AudioManager.PlayBGM(ReadyBgm);

            yield return new WaitUntil(() => Input.touchCount > 0 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Began);

            BlackScreenObj.SetActive(true);
            RuleCanvas.SetActive(false);
            AudioManager.StopBGM();
            yield return new WaitForSeconds(0.5f);
        }

        float time;
        float timer;

        timer = 0;
        time = 0.5f;
        string number = "3";

        CountDownText.text = number;
        AudioManager.PlaySE(GameStartCount);
        while (timer <= time)
        {
            CountDownText.fontSize = (int)Mathf.Lerp(30, 120, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        timer = 0;
        time = 0.5f;
        number = "2";

        CountDownText.text = number;
        AudioManager.PlaySE(GameStartCount);
        while (timer <= time)
        {
            CountDownText.fontSize = (int)Mathf.Lerp(30, 120, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        timer = 0;
        time = 0.5f;
        number = "1";

        CountDownText.text = number;
        AudioManager.PlaySE(GameStartCount);
        while (timer <= time)
        {
            CountDownText.fontSize = (int)Mathf.Lerp(30, 120, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        timer = 0;
        time = 0.5f;
        number = "START";

        CountDownText.text = number;
        AudioManager.PlaySE(GameStart);
        AudioManager.PlayBGM(PlayBgm);

        while (timer <= time)
        {
            CountDownText.fontSize = (int)Mathf.Lerp(30, 60, timer / time);
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        mode = Mode.play;


        for (int i = 0; i < playerCount; i++)
        {
            pazzleManager[i].enabled = true;

        }

        ReadyCanvas.SetActive(false);
        BlackScreenObj.SetActive(false);

        if (soloMode)
        {
            soloManager.gameObject.SetActive(true);
        }

    }

    [SerializeField] Text RedGameOverText;
    [SerializeField] Text BlueGameOverText;
    [SerializeField] House houseRed;
    [SerializeField] House houseBlue;
    [SerializeField] GameObject blackScreen;
    [SerializeField] Text GameOverMessage;
    public bool gameOver = false;
    public bool CanRemake = false;
    [SerializeField] private InputAction playerJoinInputAction = default;
    bool timer10 = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            LoadSceneManager.FadeLoadScene("Lobby");
        }

        if (!gameOver)
        {
            if (houseRed.hp <= 0)
            {
                StartCoroutine(GameOver(false));
            }
            if (houseBlue.hp <= 0)
            {
                StartCoroutine(GameOver(true));
            }

            if (mode == Mode.play)
            {
                if (timer <= 0)
                {
                    timer = 0;
                    StartCoroutine(DeathMatchStart());
                    timerText.enabled = false;
                    mode = Mode.deathMatchReady;
                }
                if (timer < 60 && timer10 == false)
                {
                    timer10 = true;
                    AudioManager.BgmOption(1.05f);
                    StartCoroutine(Timer10());
                }
                if (timer > 0 && !soloMode)
                {
                    timer -= Time.deltaTime;

                }
            }

            CannonSliders();
            //SoulGageUpdata();
            if (timer > 60)
            {
                timerText.text = Mathf.Max(Mathf.FloorToInt(timer / 60), 0).ToString("D2") + ":" + Mathf.Max(Mathf.FloorToInt(timer % 60), 0).ToString("D2");
            }
        }
    }

    public float[] soulPt = { 0, 0 };
    [SerializeField] float soulPtMax = 8;
    [SerializeField] Slider[] soulGage;
    void SoulGageUpdata()
    {
        for (int i = 0; i < playerCount; i++)
        {
            soulGage[i].value = soulPt[i] / soulPtMax;
        }
    }



    IEnumerator Timer10()
    {
        int originalSize = timerText.fontSize;
        DOTween.To(() => timerText.fontSize,
                   x => timerText.fontSize = x,
                   Mathf.RoundToInt(originalSize * 1.5f),
                   0.1f);
        timerText.DOColor(Color.red, 0.1f);

        AudioManager.PlaySEWithPitch(TimerRemind, 1);
        yield return new WaitForSeconds(0.5f);

        AudioManager.PlaySEWithPitch(TimerRemind, 1.05f);
        yield return new WaitForSeconds(0.25f);
        AudioManager.PlaySEWithPitch(TimerRemind, 1.1f);

        DOTween.To(() => timerText.fontSize,
                   x => timerText.fontSize = x,
                   Mathf.RoundToInt(originalSize * 1.5f),
                   0.5f)
               .OnComplete(() =>
               {
                   DOTween.To(() => timerText.fontSize,
                              x => timerText.fontSize = x,
                              originalSize,
                              0.1f);
               });
        yield return new WaitForSeconds(0.5f);
        timerText.text = "??:??";

    }

    [SerializeField] Text DeathMatchTitleText;
    [SerializeField] Text DeathMatchSubTitleText;
    [SerializeField] GameObject[] DMchargePar = new GameObject[2];
    [SerializeField] GameObject[] DMbeamPar = new GameObject[2];
    [SerializeField] GameObject hibana;
    [SerializeField] Slider DMslider;
    [SerializeField] GameObject[] Lights;
    Light2D beamLight;
    [SerializeField] float maxGlowValue = 15;

    public Monsters[] DMGolem = new Monsters[2];
    public float[] DMGolemPowers = new float[2];
    [SerializeField] AudioClip beamCharge;
    [SerializeField] AudioClip beamShoot;


    IEnumerator DeathMatchStart()
    {

        for (int i = 0; i < 2; i++)
        {
            pazzleManager[i].controllManager._checkPeaceUp();
        }

        yield return new WaitForSeconds(1.5f);

        GameObject[] monstras = GameObject.FindGameObjectsWithTag("Monster");

        foreach (GameObject monstar in monstras)
        {
            monstar.GetComponent<Monsters>().spdRate = 0;
            monstar.GetComponent<Monsters>().atkSpdRate = 0;

        }

        BlackScreenObj.SetActive(true);
        AudioManager.StopBGM();
        AudioManager.PlaySE(deathMatchStartSe, 0.4f);
        DeathMatchTitleText.enabled = true;
        DeathMatchSubTitleText.enabled = true;

        DeathMatchTitleText.transform.DOScale(Vector3.one * 0.8f, 0.3f);
        DeathMatchTitleText.transform.DORotate(new Vector3(0, 0, 1820), 0.3f, RotateMode.FastBeyond360);
        yield return new WaitForSeconds(0.3f);
        DeathMatchTitleText.transform.DOScale(Vector3.one, 0.3f);
        yield return new WaitForSeconds(3f);

        monstras = GameObject.FindGameObjectsWithTag("Monster");

        foreach (GameObject monstar in monstras)
        {
            monstar.GetComponent<Monsters>().Damaged(1000, null, false);
        }

        for (int i = 0; i < playerCount; i++)
        {
            DMGolem[i] = pazzleManager[i].controllManager.MonstatSpawn(10);
        }

        yield return new WaitForSeconds(3f);

        for (int i = 0; i < playerCount; i++)
        {
            DMGolem[i].spd = 0;
        }

        DeathMatchTitleText.DOFade(0, 0.5f);
        yield return new WaitForSeconds(0.6f);

        BlackScreenObj.SetActive(false);

        AudioManager.PlayBGM(PlayBgm);
        AudioManager.BgmOption(1.05f);

        mode = Mode.deathMatch;
        DMslider.gameObject.SetActive(true);


        GameObject[] beamPars = new GameObject[2];

        float DMtime = 15;
        float DMtimer = 0;

        for (int i = 0; i < playerCount; i++)
        {
            beamPars[i] = Instantiate(DMchargePar[i], DMGolem[i].transform.position, Quaternion.identity);
        }
        AudioManager.PlaySE(beamCharge, 0.7f);
        while (DMtime >= DMtimer)
        {
            for (int i = 0; i < playerCount; i++)
            {
                DMGolem[i].GetComponent<Goolem>().GolemBeamCharge();

                var main = beamPars[i].GetComponent<ParticleSystem>().main;
                main.startSize = 1f + (DMGolemPowers[i] / 10);

            }
            DMslider.value = 1 - (DMtimer / DMtime);
            DMtimer += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < playerCount; i++)
        {
            pazzleManager[i].controllManager._checkPeaceUp();
        }

        float wait = 2;
        while (wait > 0)
        {
            for (int i = 0; i < playerCount; i++)
            {
                DMGolem[i].GetComponent<Goolem>().GolemBeamCharge();
            }
            wait -= Time.deltaTime;
            yield return null;
        }


        DMslider.gameObject.SetActive(false);
        DeathMatchSubTitleText.enabled = false;


        mode = Mode.deathMatchReady;

        for (int i = 0; i < Lights.Length; i++)
        {
            //Lights[i].SetActive(false);
        }




        GameObject _hibana = Instantiate(hibana, new Vector2(0, DMGolem[0].transform.position.y), Quaternion.identity);
        beamLight = _hibana.GetComponent<Light2D>();

        float lightTggleTime = 0.08f;
        float lightTggleTimer = 0;
        bool lightFragg = true;

        for (int i = 0; i < playerCount; i++)
        {
            Destroy(beamPars[i]);
            beamPars[i] = Instantiate(DMbeamPar[i], DMGolem[i].transform.position, Quaternion.identity);

            var main = beamPars[i].GetComponent<ParticleSystem>().main;
            main.startSize = 1f + (DMGolemPowers[i] / 10);

        }


        for (int m = 0; m < 4; m++)
        {
            DMtime = 2;
            DMtimer = 0;
            AudioManager.PlaySE(beamShoot);
            while (DMtime >= DMtimer)
            {
                for (int i = 0; i < playerCount; i++)
                {
                    DMGolem[i].GetComponent<Goolem>().GolemBeamCharge();

                    ParticleSystem p = beamPars[i].GetComponent<ParticleSystem>();
                    Vector2 pos = p.gameObject.transform.InverseTransformPoint(_hibana.transform.position);

                    var vel = p.velocityOverLifetime;
                    vel.orbitalOffsetX = new ParticleSystem.MinMaxCurve(pos.x);
                    vel.orbitalOffsetY = new ParticleSystem.MinMaxCurve(pos.y);

                }

                if (lightTggleTimer >= lightTggleTime)
                {
                    if (lightFragg) beamLight.intensity = maxGlowValue;
                    if (!lightFragg) beamLight.intensity = 0;

                    lightFragg = !lightFragg;
                    lightTggleTimer = 0;
                }
                lightTggleTimer += Time.deltaTime;
                DMtimer += Time.deltaTime;
                yield return null;
            }

            _hibana.transform.DOMove(new Vector2(Random.Range(DMGolem[0].transform.position.x + 1, DMGolem[1].transform.position.x - 1f), _hibana.transform.position.y), 0.3f);
        }

        if (DMGolemPowers[0] > DMGolemPowers[1])
        {
            _hibana.transform.DOMove(new Vector2(15, _hibana.transform.position.y), 0.3f);
            beamPars[1].SetActive(false);
        }
        else
        {
            _hibana.transform.DOMove(new Vector2(-15, _hibana.transform.position.y), 0.3f);
            beamPars[0].SetActive(false);

        }

        DMtime = 1;
        DMtimer = 0;
        AudioManager.PlaySE(beamShoot);
        while (DMtime >= DMtimer)
        {
            for (int i = 0; i < playerCount; i++)
            {
                DMGolem[i].GetComponent<Goolem>().GolemBeamCharge();

                ParticleSystem p = beamPars[i].GetComponent<ParticleSystem>();
                Vector2 pos = p.gameObject.transform.InverseTransformPoint(_hibana.transform.position);

                var vel = p.velocityOverLifetime;
                vel.orbitalOffsetX = new ParticleSystem.MinMaxCurve(pos.x);
                vel.orbitalOffsetY = new ParticleSystem.MinMaxCurve(pos.y);

                var main = p.main;
                main.startSize = 3f;

            }

            if (lightTggleTimer >= lightTggleTime)
            {
                if (lightFragg) beamLight.intensity = maxGlowValue * 1.3f;
                if (!lightFragg) beamLight.intensity = 0;

                lightFragg = !lightFragg;
                lightTggleTimer = 0;
            }
            lightTggleTimer += Time.deltaTime;
            DMtimer += Time.deltaTime;
            yield return null;
        }

        if (DMGolemPowers[0] > DMGolemPowers[1])
        {
            DMGolem[1].Damaged(10000, null, false);
            houseBlue.Damaged(10000, null, false);
        }
        else
        {
            DMGolem[0].Damaged(10000, null, false);
            houseRed.Damaged(10000, null, false);

        }

        yield return new WaitForSeconds(0.3f);
        beamPars[0].SetActive(false);
        beamPars[1].SetActive(false);

    }

    IEnumerator DeathMatchFinish()
    {
        Debug.Log("デスマッチ終わる");
        yield return null;
    }



    IEnumerator GameOver(bool RedWin)
    {
        AudioManager.PlaySE(FinishSE, 0.8f);
        AudioManager.StopBGM();
        //BlackScreenObj.SetActive(true);
        mode = Mode.ready;
        timerText.enabled = false;

        gameOver = true;
        blackScreen.SetActive(true);
        GameCanvas.SetActive(false);
        if (RedWin)
        {
            StartCoroutine(GameOverWin(RedGameOverText));
            StartCoroutine(GameOverLose(BlueGameOverText));
            pazzleManager[1].gameObject.AddComponent<Rigidbody2D>();

            resultDatas[0].won = true;

        }
        else
        {
            StartCoroutine(GameOverWin(BlueGameOverText));
            StartCoroutine(GameOverLose(RedGameOverText));
            pazzleManager[0].gameObject.AddComponent<Rigidbody2D>();
            resultDatas[1].won = true;

        }
        AudioManager.FadeInBGM(ResultBgm, 5f);

        yield return new WaitForSeconds(3f);
        resultVeiwer.gameObject.SetActive(true);
        resultVeiwer.resultDatas = resultDatas;
        blackScreen.SetActive(false);



    }
    void OnEnable()
    {
        // Aボタン = buttonSouth（Xbox：A / PS：× / Switch：B）
        playerJoinInputAction.Enable();

        playerJoinInputAction.performed += OnAPressed;
    }

    void OnDisable()
    {
        playerJoinInputAction.performed -= OnAPressed;
        playerJoinInputAction.Disable();
    }

    void OnAPressed(InputAction.CallbackContext context)
    {
        // ゲームオーバー状態でのみ再マッチ受け付ける
        if (gameOver & CanRemake)
        {
            GameOverMessage.text = "再マッチを確認";
            Invoke("Remake", 1f);
        }
    }

    void Remake()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator GameOverWin(Text winText)
    {
        winText.text = "WIN";
        winText.color = Color.green;
        Transform TectPos = winText.transform;

        float time;
        float timer;

        timer = 0;
        time = 0.5f;
        while (time >= timer)
        {
            float t = timer / time;
            winText.fontSize = (int)math.lerp(0, 100, t);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        time = 0.2f;
        while (time >= timer)
        {
            float t = timer / time;
            winText.fontSize = (int)math.lerp(100, 60, t);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        time = 0.2f;
        while (time >= timer)
        {
            float t = timer / time;
            winText.fontSize = (int)math.lerp(60, 80, t);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator GameOverLose(Text winText)
    {
        winText.text = "LOSE";

        Transform TectPos = winText.transform;

        float time;
        float timer;

        TectPos.eulerAngles = new Vector3(0, 0, -25);
        TectPos.position = new Vector3(TectPos.position.x, 3);

        timer = 0;
        time = 0.3f;
        while (time >= timer)
        {
            float t = timer / time;
            winText.fontSize = (int)math.lerp(100, 70, t);
            TectPos.position = Vector2.Lerp(new Vector3(TectPos.position.x, 3), new Vector3(TectPos.position.x, -0.5f), t);
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        time = 0.2f;
        while (time >= timer)
        {
            float t = timer / time;
            winText.fontSize = (int)math.lerp(70, 80, t);
            TectPos.position = Vector2.Lerp(new Vector3(TectPos.position.x, -0.5f), new Vector3(TectPos.position.x, 0.5f), t);
            timer += Time.deltaTime;
            yield return null;
        }

    }


    void CannonSliders()
    {
        for (int i = 0; i < playerCount; i++)
        {
            cannonGaged[i].value = cannonsChages[i] / maxCannonsChages;

            if (cannonsChages[i] > maxCannonsChages)
            {
                cannonsChages[i] = maxCannonsChages;
            }
        }
    }

    [SerializeField] List<Monsters> monsters0 = new List<Monsters>();
    [SerializeField] List<Monsters> monsters1 = new List<Monsters>();
    [SerializeField] List<Monsters> humans = new List<Monsters>();

    public static GameManager instance;

    public static void AddCharacter(Monsters target)
    {
        instance._AddCharacter(target);
    }
    void _AddCharacter(Monsters target)
    {
        Human human;
        if (target.TryGetComponent<Human>(out human))
        {
            humans.Add(target);
        }
        else
        {
            if (target.player == 0)
            {
                monsters0.Add(target);
            }
            else
            {
                monsters1.Add(target);

            }
        }
    }

    public static void RemoveCharacter(Monsters target)
    {
        instance._RemoveCharacter(target);
    }

    void _RemoveCharacter(Monsters target)
    {
        if (monsters0.Contains(target)) monsters0.Remove(target);
        if (monsters1.Contains(target)) monsters1.Remove(target);
        if (humans.Contains(target)) humans.Remove(target);

        monsters0.RemoveAll(m => m == null);
        monsters1.RemoveAll(m => m == null);
        humans.RemoveAll(m => m == null);

    }

    public enum type
    {
        All,
        mon,
        mon0,
        mon1,
        human,

    }

    public static List<Monsters> GetMonsters(type _type)
    {
        return instance._GetMonsters(_type);
    }

    List<Monsters> _GetMonsters(type _type)
    {
        List<Monsters> targets = new List<Monsters>();
        switch (_type)
        {
            case type.All:
                targets.AddRange(monsters0);
                targets.AddRange(monsters1);
                targets.AddRange(humans);
                break;
            case type.mon:
                targets.AddRange(monsters0);
                targets.AddRange(monsters1);
                break;
            case type.mon0:
                targets.AddRange(monsters0);
                break;
            case type.mon1:
                targets.AddRange(monsters1);
                break;
            case type.human:
                targets.AddRange(humans);
                break;
        }
        return targets;
    }


    public static void toggleBrackScreen(bool boo)
    {
        instance.BlackScreenObj.SetActive(boo);
    }
}


