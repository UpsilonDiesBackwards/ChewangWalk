using System;
using System.Collections;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class UIIManager : MonoBehaviour {
    [Header("UI Refrences")]
    public PausedMenuUIManager _pausedMenuUIManager;

    [Header("Rupert")] 
    [SerializeField] private Image rupert;
    [SerializeField] private Sprite[] ruperts;

    [Header("Sound")]
    [SerializeField] private EventReference rupertAudio;
    [SerializeField] private EventReference rupertAudio_alt;
    
    private void Update() {
        if (InputManager.Instance.PauseGamePressed && 
            !_pausedMenuUIManager.settingsUIManager.gameObject.activeSelf) {
            
            _pausedMenuUIManager.gameObject.SetActive(!_pausedMenuUIManager.gameObject.activeSelf);
        }
    }

    public void RupertThemJohn() {
        StartCoroutine(Rupert());
    }

    public IEnumerator Rupert() {
        int specificRupert = UnityEngine.Random.Range(0, ruperts.Length);
        
        rupert.gameObject.SetActive(true);

        rupert.sprite = ruperts[specificRupert];
        
        if (specificRupert == 0) {
            AudioManager.Instance.PlayOneShot(rupertAudio_alt);
            yield return new WaitForSeconds(3f);
        
            rupert.gameObject.SetActive(false);
        }
        else {
            AudioManager.Instance.PlayOneShot(rupertAudio);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        rupert.gameObject.SetActive(false);
    }
}