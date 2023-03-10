using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenCloseUI : MonoBehaviour
{
    [SerializeField] GameObject debugConsole;
    [SerializeField] GameObject menu;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3)) debugConsole.SetActive(!debugConsole.activeInHierarchy);
        if (Input.GetKeyDown(KeyCode.Escape)) menu.SetActive(!menu.activeInHierarchy);
    }
}
