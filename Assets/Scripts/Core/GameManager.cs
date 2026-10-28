using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    public AudioManager audioManager;

    private void Awake() {
        Instance = this;
        audioManager = FindObjectOfType<AudioManager>();
    }
}