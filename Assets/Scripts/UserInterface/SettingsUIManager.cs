using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIManager : MonoBehaviour {
    [Header("References")]
    [SerializeField] private PausedMenuUIManager pausedMenuUIManager;
    [SerializeField] private Slider volumeSlider;

    private SettingsBinder _volumeBinder;
    private const string VolumePrefsKey = "Volume";
    
    private void Awake() {
        if (volumeSlider != null) {
            _volumeBinder = volumeSlider.GetComponent<SettingsBinder>();
            
            float savedVolume = PlayerPrefs.GetFloat(VolumePrefsKey, 1.0f);
            
            if (_volumeBinder != null) {
                _volumeBinder.Initialise(OnVolumeChanged, OnSliderRelease);
                _volumeBinder.SetValueSilently(savedVolume);
            } else {
                volumeSlider.SetValueWithoutNotify(savedVolume);
                volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
            }

            if (AudioManager.Instance != null) {
                AudioManager.Instance.UpdateVcaVolume("Master", savedVolume);
            }
            else {
                Debug.Log("Audio Manager not found");
            }
        }
    }

    private void OnEnable() {
        Time.timeScale = 0.0f; // Pause da game
        InputManager.Instance.ToggleCursor(true); // and show da cursor
    }

    public void OnVolumeChanged(float value) {
        AudioManager.Instance.UpdateVcaVolume("Master", value);
        
        PlayerPrefs.SetFloat(VolumePrefsKey, value);
        PlayerPrefs.Save();
    }
    
    private void OnSliderRelease() {
        AudioManager.Instance.PlayUIClick();
    }
    
    public void Close() {
        if (pausedMenuUIManager != null) {
            pausedMenuUIManager.gameObject.SetActive(true);
        }
        gameObject.SetActive(false);
    }
}