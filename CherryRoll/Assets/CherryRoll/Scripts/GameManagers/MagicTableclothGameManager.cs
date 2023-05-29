﻿using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MagicTableclothGameManager : NetworkBehaviour {


    public static MagicTableclothGameManager Instance { get; private set; }

    public event EventHandler OnItemDelivered;
    public event EventHandler OnPlayersScoresDictionaryUpdated;

    private Dictionary<ulong, int> allPlayersScoresDictionary = new Dictionary<ulong, int>(); // Server side only
    public Dictionary<ulong, int> connectedPlayersScoresDictionary = new Dictionary<ulong, int>();


    private void Awake() {
        Instance = this;

        if (!IsServer) return;

        allPlayersScoresDictionary = new Dictionary<ulong, int>();
    }

    private void Start() {
        if (!IsServer) return;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            allPlayersScoresDictionary[clientId] = 0;
        }

        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;

        // Присваивание значения 0 для всех клиентов, уже подключенных на сервере
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            allPlayersScoresDictionary[clientId] = 0;
        }

        UpdatePlayersScoresDictionaryServerRpc();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong obj) {
        //! Срабатывает, но UI не обновляется
        UpdatePlayersScoresDictionaryServerRpc();
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId) {
        // Добавление новой строки в словарь с новым подключенным клиентом
        allPlayersScoresDictionary[clientId] = 0;

        UpdatePlayersScoresDictionaryServerRpc();
    }

    private void Player_OnAnyPlayerSpawned(object sender, EventArgs e) {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (!allPlayersScoresDictionary.ContainsKey(clientId)) {
                allPlayersScoresDictionary[clientId] = 0;
            }
        }

        UpdatePlayersScoresDictionaryServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeliverItemScoreServerRpc(int itemCost = 1, ServerRpcParams serverRpcParams = default) {
        allPlayersScoresDictionary[serverRpcParams.Receive.SenderClientId] += itemCost;
        UpdatePlayersScoresDictionaryServerRpc(); //! Лучше локально увеличить значение у каждого клиента, чем пересоздавать словарь.

        OnItemDeliveredClientRpc();
    }

    [ClientRpc]
    public void OnItemDeliveredClientRpc() {
        OnItemDelivered?.Invoke(this, EventArgs.Empty); //! Зачем для этого создавать отдельную функцию
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayersScoresDictionaryServerRpc() {
        CreatePlayersScoresDictionaryClientRpc();

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            UpdatePlayersScoresDictionaryClientRpc(clientId, allPlayersScoresDictionary[clientId]);
        }

        OnPlayersScoresDictionaryUpdatedClientRpc();
    }

    [ClientRpc]
    public void CreatePlayersScoresDictionaryClientRpc() {
        connectedPlayersScoresDictionary = new Dictionary<ulong, int>();
    }

    [ClientRpc]
    public void UpdatePlayersScoresDictionaryClientRpc(ulong clientId, int score) {
        connectedPlayersScoresDictionary[clientId] = score;
    }

    [ClientRpc]
    public void OnPlayersScoresDictionaryUpdatedClientRpc() {
        OnPlayersScoresDictionaryUpdated?.Invoke(this, EventArgs.Empty); //! Зачем для этого создавать отдельную функцию
    }

    public override void OnDestroy() {
        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
        Player.OnAnyPlayerSpawned -= Player_OnAnyPlayerSpawned;
    }
}
