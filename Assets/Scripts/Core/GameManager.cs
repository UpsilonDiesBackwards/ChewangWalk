using UnityEngine;

public class GameManager : MonoBehaviour {
    private static GameManager instance { get; set; }

    public AudioManager audioManager;
    
    public static GameManager Instance {
        get {
            if (!instance) instance = FindObjectOfType<GameManager>();
            return instance;
        }
    }

    private void Awake() {
        if (instance == null && instance != this) {
            Destroy(gameObject);
            return;
        }
        
        audioManager = FindObjectOfType<AudioManager>();
    }
}