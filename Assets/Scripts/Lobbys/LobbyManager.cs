using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public enum Mode
    {
        title,
        menu,
    }
    public Mode mode = Mode.title;

    [SerializeField] GameObject anyKeyPress;
    [SerializeField] GameObject MenuWindw;
    ControllerManager controllerManager;

    [SerializeField] InputManagerLobby inputManagerLobby;
    [SerializeField] Transform titleLogo;

    public bool canOnButton = true;
    public float canOnButtonCT = 1;
    [SerializeField] AudioClip buttonSE;
    [SerializeField] AudioClip titleBgm;
    public InputAction playerJoinInputAction = default;

    [SerializeField] float idleSeconds = 30f;
    [SerializeField] string demoSceneName = "Demo";
    float idleTimer = 0f;
    bool idleTriggered = false;
    [SerializeField] bool mobileMode = false;


    void Awake()
    {
        Application.targetFrameRate = 60;

        if (mobileMode)
        {
            Debug.Log("MoblieLobby");
            SceneManager.LoadScene("MoblieLobby");

        }
        else
        {
            playerJoinInputAction.started += ctx =>
                   {
                       if (mode == Mode.title) SwitchMode();
                   };


            playerJoinInputAction.Enable();
            StartCoroutine(_Start());
        }
    }

    IEnumerator _StartMobile()
    {
        yield return null;
        LoadSceneManager.FadeLoadScene("MoblieLobby");

    }


    public static bool MobileCheck()
    {
#if UNITY_EDITOR
        return true; // Editorでもスマホ扱い
#else
    return Application.platform == RuntimePlatform.Android ||
           Application.platform == RuntimePlatform.IPhonePlayer;
#endif
    }

    IEnumerator _Start()
    {
        yield return null;
        controllerManager = GameObject.FindWithTag("ControllerManager").GetComponent<ControllerManager>();

        StartCoroutine(TitleAnim());

        AudioManager.FadeOutBGM(0.5f);
        yield return new WaitForSeconds(0.5f);
        AudioManager.PlayBGM(titleBgm);
    }


    // Update is called once per frame
    void Update()
    {

        if (mode == Mode.title)
        {
            if (HasAnyInputThisFrame())
            {
                idleTimer = 0f;
                idleTriggered = false;
            }
            else
            {
                idleTimer += Time.deltaTime;
                if (!idleTriggered && idleTimer >= idleSeconds)
                {
                    idleTriggered = true;
                    LoadSceneManager.FadeLoadScene(demoSceneName);
                }
            }
        }
        else
        {
            idleTimer = 0f;
            idleTriggered = false;
        }

    }

    void SwitchMode(bool boo = true)
    {
        if (boo == true)
        {
            ButtonCT(0.3f);
            mode = Mode.menu;
            anyKeyPress.SetActive(false);
            MenuWindw.SetActive(true);
        }
        else
        {
            ButtonCT(0.3f);
            mode = Mode.title;
            anyKeyPress.SetActive(true);
            MenuWindw.SetActive(false);
        }
    }

    void ReconnectController()
    {
        controllerManager.PairWithDevice[0] = null;
        controllerManager.PairWithDevice[1] = null;

        var playerInputs = FindObjectsOfType<PlayerInput>();

        inputManagerLobby.currentPlayerCount = 0;
        inputManagerLobby.joinedDevices = default;
        inputManagerLobby.joinedDevices = new InputDevice[inputManagerLobby.maxPlayerCount];

        foreach (var playerInput in playerInputs)
        {
            Destroy(playerInput.gameObject);
        }
    }


    public void StartVSMButton()
    {

        if (!canOnButton) { return; }
        canOnButton = false;
        AudioManager.PlaySE(buttonSE, 0.6f);
        Debug.Log("Monstar");
        StartCoroutine(StartGame());

    }

    IEnumerator StartGame()
    {
        if (inputManagerLobby.currentPlayerCount == 2)
        {
            Messager.ViewText("ゲームを開始します", 1);
            yield return new WaitForSeconds(1f);
            AudioManager.FadeOutBGM(0.5f);
            yield return new WaitForSeconds(0.2f);
            LoadSceneManager.FadeLoadScene("Game");


        }
        else
        {
            yield return Messager.ViewText("コントローラーを二つ接続してください");
            canOnButton = true;

        }

    }

    public void StartVSHButton()
    {
        if (!canOnButton) { return; }
        AudioManager.PlaySE(buttonSE, 0.2f);
        canOnButton = false;
        StartCoroutine(StartGameSolo());
    }

    IEnumerator StartGameSolo()
    {
        if (inputManagerLobby.currentPlayerCount >= 1)
        {
            Messager.ViewText("ゲームを開始します", 1);
            yield return new WaitForSeconds(1f);
            AudioManager.FadeOutBGM(0.5f);
            yield return new WaitForSeconds(0.2f);
            LoadSceneManager.FadeLoadScene("SoloGame");


        }
    }

    public void ResetControllerButton()
    {
        if (!canOnButton) { return; }
        AudioManager.PlaySE(buttonSE, 0.2f);
        Debug.Log("再接続");
        ReconnectController();

    }

    public void CloseButton()
    {
        if (!canOnButton) { return; }
        SwitchMode(false);
        AudioManager.PlaySE(buttonSE, 0.2f);
    }

    public void MenberButton()
    {

        if (!canOnButton) { return; }
        canOnButton = false;
        AudioManager.PlaySE(buttonSE, 0.2f);
        StartCoroutine(MenberView());
    }


    IEnumerator MenberView()
    {
        yield return Messager.ViewText("Accompany/アカンパニー \n冨田陽士 工藤優馬 木村涼介");
        canOnButton = true;
    }

    IEnumerator TitleAnim()
    {
        Vector2 ooo = Vector2.up;
        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                titleLogo.Translate(0.01f * ooo);
                yield return new WaitForSeconds(0.5f);
            }
            ooo *= -1;
            yield return null;
        }
    }


    Coroutine ButtonCTCoru = null;
    void ButtonCT(float duration)
    {
        if (ButtonCTCoru != null)
        {
            StopCoroutine(ButtonCTCoru);
            canOnButton = true;
        }
        ButtonCTCoru = StartCoroutine(_ButtonCT(duration));
    }

    IEnumerator _ButtonCT(float duration)
    {
        canOnButton = false;
        yield return new WaitForSeconds(duration);
        canOnButton = true;
    }

    bool HasAnyInputThisFrame()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) return true;

        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame) return true;
            if (Mouse.current.rightButton.wasPressedThisFrame) return true;
            if (Mouse.current.middleButton.wasPressedThisFrame) return true;
            if (Mouse.current.scroll.ReadValue() != Vector2.zero) return true;
        }

        if (Gamepad.current != null)
        {
            var gp = Gamepad.current;
            if (gp.leftStick.ReadValue() != Vector2.zero) return true;
            if (gp.rightStick.ReadValue() != Vector2.zero) return true;
            if (gp.leftTrigger.ReadValue() > 0f) return true;
            if (gp.rightTrigger.ReadValue() > 0f) return true;
            if (gp.buttonSouth.wasPressedThisFrame) return true;
            if (gp.startButton.wasPressedThisFrame) return true;
            if (gp.dpad.up.wasPressedThisFrame) return true;
            if (gp.dpad.down.wasPressedThisFrame) return true;
            if (gp.dpad.left.wasPressedThisFrame) return true;
            if (gp.dpad.right.wasPressedThisFrame) return true;
        }

        if (Input.anyKeyDown) return true;

        return false;
    }




}
