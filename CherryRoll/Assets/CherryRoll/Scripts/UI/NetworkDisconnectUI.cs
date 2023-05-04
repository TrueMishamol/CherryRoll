using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkDisconnectUI : MonoBehaviour {

    public static NetworkDisconnectUI Instance { get; private set; }

    [Header("Second Screen Elements (menuDisconnect)")]
    [SerializeField] private Button disconnectButton;
    [SerializeField] private TextMeshProUGUI nameChangeInputField;
    [SerializeField] private Button nameChangeButton;
    [SerializeField] private TextMeshProUGUI playersCountText;

    private void Awake() {
        Instance = this;

        //nameChangeButton.onClick.AddListener(() => {
        //    UpdateTheNameFromField();
        //});

        // Originally events are listening on start, but also on the start we ran IngameMenuUI.Hide()
        NetworkHandlePlayersCount.OnPlayersCountUpdated += NetworkHandleConnection_OnPlayersCountUpdated;
    }

    private void Start() {
        Hide();
    }

    private void NetworkHandleConnection_OnPlayersCountUpdated(object sender, System.EventArgs e) {
        if (NetworkHandlePlayersCount.Instance.GetPlayersCount() == 0) {
            playersCountText.text = "Server stopped";
        } else {
            playersCountText.text = "Players: " + NetworkHandlePlayersCount.Instance.GetPlayersCount().ToString();
        }
    }

    //private void Update() {
    //    // Player Counting for UI
    //    playersCountText.text = "Players: " + playersNum.Value.ToString();

    //    if (!IsServer) return;
    //    try {
    //        playersNum.Value = NetworkManager.Singleton.ConnectedClients.Count;
    //    } catch (Unity.Netcode.NotServerException) {
    //        // If the host stops, then constantly occurs thiss exception Unity.Netcode.NotServerException: ConnectedClients should only be accessed on server
    //        playersCountText.text = "Server stopped";
    //        return;
    //    }
    //}

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }
}
