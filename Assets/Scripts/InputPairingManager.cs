using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの入退室の管理クラス（アウトゲーム）
/// </summary>
public class PlayerJoinManager : MonoBehaviour
{
    // プレイヤーがゲームにJoinするためのInputAction
    private InputAction playerJoinInputAction = default;
    [SerializeField] private InputActionAsset inputActionAsset;
    // PlayerInputがアタッチされているプレイヤーオブジェクト
    [SerializeField] private PlayerInput playerPrefab = default;
    // 最大参加人数
    [SerializeField] private int maxPlayerCount = default;
    [SerializeField] GameManager gameManager;

    // Join済みのデバイス情報
    private InputDevice[] joinedDevices = default;
    // 現在のプレイヤー数
    public int currentPlayerCount = 0;
    ControllerManager controllerManager;


    private void Awake()
    {

        // 最大参加可能数で配列を初期化
        joinedDevices = new InputDevice[maxPlayerCount];



        var obj = GameObject.FindWithTag("ControllerManager");

        if (obj != null && obj.TryGetComponent<ControllerManager>(out controllerManager))
        {
            LoadingPairDevice();

        }
        else
        {
            //playerJoinInputAction.Enable();
            //playerJoinInputAction.performed += OnJoin;
        }

    }

    private void OnDestroy()
    {
        //playerJoinInputAction.Dispose();
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
        GameObject C = PlayerInput.Instantiate(
            prefab: playerPrefab.gameObject,
            playerIndex: currentPlayerCount,
            pairWithDevice: context.control.device
            ).gameObject;
        C.GetComponent<ControlManager>().playerNumber = currentPlayerCount;
        // Joinしたデバイス情報を保存
        joinedDevices[currentPlayerCount] = context.control.device;

        gameManager.playerInputs[currentPlayerCount] = C.GetComponent<PlayerInput>();

        currentPlayerCount++;
    }

    void LoadingPairDevice()
    {
        for (int i = 0; i < 2; i++)
        {
            if (controllerManager.PairWithDevice[i] != null)
            {
                GameObject C = PlayerInput.Instantiate(
            prefab: playerPrefab.gameObject,
            playerIndex: currentPlayerCount,
            pairWithDevice: controllerManager.PairWithDevice[i]
            ).gameObject;
                C.GetComponent<ControlManager>().playerNumber = currentPlayerCount;
                // Joinしたデバイス情報を保存
                joinedDevices[currentPlayerCount] = controllerManager.PairWithDevice[i];

                currentPlayerCount++;
            }
        }
    }
}
