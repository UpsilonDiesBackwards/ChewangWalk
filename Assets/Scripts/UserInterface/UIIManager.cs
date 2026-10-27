using System;
using UnityEngine;

public class UIIManager : MonoBehaviour {
    [Header("UI Refrences")]
    public PausedMenuUIManager _pausedMenuUIManager;

    private void Update() {
        if (InputManager.Instance.PauseGamePressed) {
            _pausedMenuUIManager.gameObject.SetActive(!_pausedMenuUIManager.gameObject.activeSelf);
        }
    }
}