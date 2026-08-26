using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputManagerLobby : MonoBehaviour
{
    // プレイヤーがゲームにJoinするためのInputAction
    [SerializeField] private InputAction playerJoinInputAction = default;
    [SerializeField] private PlayerInput playerPrefab = default;
    // 最大参加人数
    public int maxPlayerCount = default;

    // Join済みのデバイス情報
    public InputDevice[] joinedDevices { get; set; } = default;
    // 現在のプレイヤー数
    public int currentPlayerCount { get; set; } = 0;
    ControllerManager controllerManager;
    [SerializeField] LobbyManager lobbyManager;


    private void Awake()
    {
        playerJoinInputAction = lobbyManager.playerJoinInputAction;
        // 最大参加可能数で配列を初期化
        joinedDevices = new InputDevice[maxPlayerCount];

        // InputActionを有効化し、コールバックを設定
        playerJoinInputAction.Enable();
        playerJoinInputAction.performed += OnJoin;
    }

    void Start()
    {
        controllerManager = GameObject.FindWithTag("ControllerManager").GetComponent<ControllerManager>();
        LoadingPairDevice();
    }

    void Update()
    {
        HilghtOninePlayerNum();
    }

    private void OnDestroy()
    {
        playerJoinInputAction.Dispose();
    }

    /// <summary>
    /// デバイスによってJoin要求が発火したときに呼ばれる処理
    /// </summary>
    private void OnJoin(InputAction.CallbackContext context)
    {
        // プレイヤー数が最大数に達していたら、処理を終了
        if (currentPlayerCount >= maxPlayerCount)
        {
            return;
        }

        // Join要求元のデバイスが既に参加済みのとき、処理を終了
        foreach (var device in joinedDevices)
        {
            if (context.control.device == device)
            {
                return;
            }
        }

        // PlayerInputを所持した仮想のプレイヤーをインスタンス化
        // ※Join要求元のデバイス情報を紐づけてインスタンスを生成する
        PlayerInput.Instantiate(
            prefab: playerPrefab.gameObject,
            playerIndex: currentPlayerCount,
            pairWithDevice: context.control.device
            );

        // Joinしたデバイス情報を保存
        joinedDevices[currentPlayerCount] = context.control.device;
        controllerManager.PairWithDevice[currentPlayerCount] = context.control.device;

        currentPlayerCount++;
    }

    void LoadingPairDevice()
    {
        for (int i = 0; i < 2; i++)
        {
            if (controllerManager.PairWithDevice[i] != null)
            {
                Debug.Log(controllerManager.PairWithDevice[i].device.name);
                PlayerInput.Instantiate(
                           prefab: playerPrefab.gameObject,
                           playerIndex: currentPlayerCount,
                           pairWithDevice: controllerManager.PairWithDevice[i]
                           );

                // Joinしたデバイス情報を保存
                joinedDevices[currentPlayerCount] = controllerManager.PairWithDevice[i];

                currentPlayerCount++;
            }
        }
    }

    [SerializeField] Text[] onlinePlayerViews; private float disconnectTimer = 0f;
    private bool is1PDisconnected = false;
    [SerializeField] private float disconnectThreshold = 5f;

    void HilghtOninePlayerNum()
    {
        bool player1Disconnected = false;

        for (int i = 0; i < 2; i++)
        {
            if (currentPlayerCount > i)
            {
                var device = joinedDevices[i];

                if (device != null && device.added)
                {
                    onlinePlayerViews[i].color = Color.white;
                }
                else
                {
                    onlinePlayerViews[i].color = new Color(1, 1, 1, 0.2f);

                    if (i == 0) // 1P の場合
                    {
                        player1Disconnected = true;
                    }
                }
            }
            else
            {
                onlinePlayerViews[i].color = new Color(1, 1, 1, 0.2f);
            }



        }

        // 1P が切断状態か監視
        if (player1Disconnected)
        {
            if (!is1PDisconnected)
            {
                // 切断を検知した瞬間
                is1PDisconnected = true;
                disconnectTimer = 0f;
            }
            else
            {
                // 切断が継続している
                disconnectTimer += Time.deltaTime;

                if (disconnectTimer >= disconnectThreshold)
                {
                    Debug.Log("1P が 5 秒間切断状態。全デバイスリセットを実行。");
                    lobbyManager.ResetControllerButton();

                    is1PDisconnected = false;
                }
            }
        }
        else
        {
            // 1P が再接続されたら監視をリセット
            is1PDisconnected = false;
            disconnectTimer = 0f;
        }
    }


}
