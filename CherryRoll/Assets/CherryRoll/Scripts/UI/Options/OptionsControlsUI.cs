using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsControlsUI : MonoBehaviour {


    public static OptionsControlsUI Instance { get; private set; }


    [SerializeField] private Transform container;
    [SerializeField] private Transform controlButtonTemplate;

    [SerializeField] private Button closeButton;
    [SerializeField] private Transform pressToRebindTransform;

    public Dictionary<GameInput.Binding, string> bindingsNamesDictionary = new Dictionary<GameInput.Binding, string>() {
        { GameInput.Binding.Move_Up, "Move Up" },
        { GameInput.Binding.Move_Down, "Move Down" },
        { GameInput.Binding.Move_Left, "Move Left" },
        { GameInput.Binding.Move_Right, "Move Right" },
        { GameInput.Binding.Jump, "Jump" },
        { GameInput.Binding.Interact, "Interact" },
        { GameInput.Binding.InteractAlternate, "Interact Alternate" },
        { GameInput.Binding.MenuOpenClose, "Menu Open Close" },

        { GameInput.Binding.Move_Gamepad, "Move Gamepad" },
        { GameInput.Binding.Jump_Gamepad, "Jump Gamepad" },
        { GameInput.Binding.Interact_Gamepad, "Interact Gamepad" },
        { GameInput.Binding.InteractAlternate_Gamepad, "Interact Alternate Gamepad" },
        { GameInput.Binding.MenuOpenClose_Gamepad, "Menu Open Close Gamepad" },
    };

    private void Awake() {
        Instance = this;

        controlButtonTemplate.gameObject.SetActive(false);

        closeButton.onClick.AddListener(() => {
            Hide();
        });
    }

    private void Start() {
        UpdateVisual();

        HidePressToRebindKey();
        Hide();
    }

    public void UpdateVisual() {
        foreach (Transform child in container) {
            if (child == controlButtonTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (GameInput.Binding binding in (GameInput.Binding[])Enum.GetValues(typeof(GameInput.Binding))) {
            Transform playerScoreSingleUITransform = Instantiate(controlButtonTemplate, container);
            playerScoreSingleUITransform.gameObject.SetActive(true);
            playerScoreSingleUITransform.GetComponent<OptionsControlsSingleUI>().SetBindingToButton(binding);
        }
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    public void ShowPressToRebindKey() {
        pressToRebindTransform.gameObject.SetActive(true);
    }

    public void HidePressToRebindKey() {
        pressToRebindTransform.gameObject.SetActive(false);
    }


}
