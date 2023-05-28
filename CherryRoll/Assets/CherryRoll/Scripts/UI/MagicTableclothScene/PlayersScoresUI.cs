﻿using System.Collections.Generic;
using UnityEngine;

public class PlayersScoresUI : MonoBehaviour {


    [SerializeField] private Transform container;
    [SerializeField] private Transform playerScoreTemplate;


    private void Awake() {
        playerScoreTemplate.gameObject.SetActive(false);
    }

    private void Start() {
        MagicTableclothGameManager.Instance.OnItemDelivered += MagicTableclothGameManager_OnItemDelivered;

        UpdateVisual();
    }

    private void MagicTableclothGameManager_OnItemDelivered(object sender, System.EventArgs e) {
        UpdateVisual();
    }

    private void UpdateVisual() {
        foreach (Transform child in container) {
            if (child == playerScoreTemplate) continue;
            Destroy(child.gameObject);
        }

        Dictionary<ulong, int> playersScoresDictionary = MagicTableclothGameManager.Instance.playersScoresDictionary.Value;
        
        foreach (KeyValuePair<ulong, int> clientScore in playersScoresDictionary) {
            Transform playerScoreSingleUITransform = Instantiate(playerScoreTemplate, container);
            playerScoreSingleUITransform.gameObject.SetActive(true);
            playerScoreSingleUITransform.GetComponent<PlayersScoresSingleUI>().SetPlayerScore(clientScore);
        }
    }
}
