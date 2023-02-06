﻿using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;

public class NetworkManagerUI : NetworkBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private TextMeshProUGUI joinCodeInputField;
    //[SerializeField] private TextMeshProUGUI nameInputField;

    [SerializeField] private TextMeshProUGUI joinCodeOutputText;
    //[SerializeField] private TextMeshProUGUI nameChangeInputField;
    //[SerializeField] private Button nameChangeButton;
    [SerializeField] private TextMeshProUGUI playersCountText;

    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject menuConnect;
    [SerializeField] private GameObject menuDisconnect;

    public string joinCode;
    public string playerName;

    private NetworkVariable<int> playersNum = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private async void Start()
    {
        //Отправляет запрос в unity services чтобы инициализировать API. При помощи Async игра не зависает до тех пор пока не получит ответа
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Awake()
    {
        hostButton.onClick.AddListener(() => {
            CreateRelay();
            //menu.gameObject.SetActive(false);
            menuDisconnect.gameObject.SetActive(true);
            menuConnect.gameObject.SetActive(false);
        });

        clientButton.onClick.AddListener(() => {
            //Конвертация из TMPro. Он добавляет невидимый символ в конце
            joinCode = joinCodeInputField.text.ToUpper();
            joinCode = joinCode.Remove(joinCode.Length - 1);
            JoinRelay();
            menu.gameObject.SetActive(false);
            menuDisconnect.gameObject.SetActive(true);
            menuConnect.gameObject.SetActive(false);
        });

        //nameChangeButton.onClick.AddListener(() => {
        //    //if (!IsOwner) return;
        //    //Конвертация из TMPro. Он добавляет невидимый символ в конце
        //    playerName = nameChangeInputField.text;
        //    playerName = playerName.Remove(playerName.Length - 1);

            
        //});
    }

    public async void CreateRelay()
    {
        try
        {
            //Creating Allocation on Relay
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(49); //50 игроков

            //Getting the joinCode to join Allocation. joinCode надо отослать друзьям
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            joinCodeOutputText.text = joinCode;
            Debug.Log("Join Code " + joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void JoinRelay()
    {
        try
        {
            joinCodeOutputText.text = joinCode;
            Debug.Log("Joining relay with " + joinCode);

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void Update()
    {
        //Подсчёт числа игроков для UI
        playersCountText.text = "Players: " + playersNum.Value.ToString();

        if (!IsServer) return;
        try
        {
            playersNum.Value = NetworkManager.Singleton.ConnectedClients.Count;
        } catch (Unity.Netcode.NotServerException)
        {
            //Если остановить хост, то постоянно возникает ошибка Unity.Netcode.NotServerException: ConnectedClients should only be accessed on server
            playersCountText.text = "Server stopped";
            return;
        }  
    }
}
