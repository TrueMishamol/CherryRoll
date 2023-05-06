using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinCodeOutput : MonoBehaviour {


    [SerializeField] private TextMeshProUGUI joinCodeOutputText;
    [SerializeField] private Button joinCodeCopyButton;


    private void Awake() {
        // Originally events are listening on start, but also on the start we ran IngameMenuUI.Hide()
        NetworkHandleConnection.OnJoinCodeUpdated += NetworkHandleConnection_OnJoinCodeUpdated;

        joinCodeCopyButton.onClick.AddListener(() => {
            CopyToClipboard.Copy(NetworkHandleConnection.JoinCode);
        });
    }

    private void NetworkHandleConnection_OnJoinCodeUpdated(object sender, EventArgs e) {
        joinCodeOutputText.text = NetworkHandleConnection.JoinCode;
    }
}
