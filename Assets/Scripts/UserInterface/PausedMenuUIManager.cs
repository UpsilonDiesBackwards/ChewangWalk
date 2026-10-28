using System;
using UnityEngine;

public class PausedMenuUIManager : MonoBehaviour {
    public SettingsUIManager settingsUIManager;
    
    private void OnEnable() {
        Show();
    }

    private void OnDisable() {
        Close();
    }

    public void Show() {
        Time.timeScale = 0.0f; // Pause da game
        InputManager.Instance.ToggleCursor(true); // and show da cursor
    }
    
    public void ResumeGame() {
        Close();
    }

    public void OpenSettings() {
        _hide();
        
        settingsUIManager.gameObject.SetActive(true);
    }
    
    public void QuitToMainMenu() {
        // Open the main menu scene when we make one
        Close();
    }
    
    public void Close() {
        Time.timeScale = 1.0f;
        InputManager.Instance.ToggleCursor(false);
        
        gameObject.SetActive(false);
    }
    
    // Hiding the menu is different to closing it as we
    // still want to keep the game paused and the cursor enabled
    private void _hide() {
        gameObject.SetActive(false);
    }
}
