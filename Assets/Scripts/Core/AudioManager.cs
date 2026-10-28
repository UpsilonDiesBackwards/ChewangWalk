using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    private static AudioManager _instance { get; set; }

    [SerializeField] private EventReference UIClick;
    
    private Coroutine _fadeCoroutine;

    private bool _isInitialised = false;

    private List<EventInstance> _eventInstances;
    private List<StudioEventEmitter> _eventEmitters;

    private VCA _vcaMaster;

    public static AudioManager Instance {
        get {
            if (!_instance) _instance = FindObjectOfType<AudioManager>();
            return _instance;
        }
    }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
            return;
        }
    
        _instance = this;

        _eventInstances = new List<EventInstance>();
        _eventEmitters = new List<StudioEventEmitter>();

        _isInitialised = _initalise();

        if (!_isInitialised) {
            Debug.Log("Audio Manager failed to initialise");
        }
    }

    private bool _initalise() {
        if (!RuntimeManager.HaveMasterBanksLoaded) {
            return false;
        }

        try {
            _vcaMaster = RuntimeManager.GetVCA("vca:/Master");
            return true;
        }
        catch (VCANotFoundException e) {
            Debug.LogError($"Failed to find VCA path: {e.Message}");
            return false;
        }
        catch (System.Exception e) {
            Debug.LogError($"Unexpected error whilst finding VCA: {e.Message}"); 
            return false;
        }
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos) {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }
    
    public void FadeOutAllAudio(float duration) {
        if (_fadeCoroutine != null) {
            StopCoroutine(_fadeCoroutine);
        }

        _fadeCoroutine = StartCoroutine(_fadeAudioRoutine(0.0f, duration));
    }
    
    private void FadeInAllAudio(float duration) {
        if (_fadeCoroutine != null) {
            StopCoroutine(_fadeCoroutine);
        }
        
        // TODO: FIX
        // float targetVolume = Settings.targetvolume
        // _fadeCoroutine = StartCoroutine(_fadeAudioRoutine(targetVolume, duration));
        _fadeCoroutine = StartCoroutine(_fadeAudioRoutine(1.0f, duration));
    }

    private void SetInstanceParameter(EventInstance instance, string paramName, float val) {
        if (instance.isValid()) {
            instance.setParameterByName(paramName, val);
        }
    }
    
    private void SetGlobalParameter(EventInstance instance, string paramName, float val) {
        RuntimeManager.StudioSystem.setParameterByName(paramName, val);
    }

    private IEnumerator _fadeAudioRoutine(float targetVol, float duration) {
        if (!_isInitialised) { Debug.Log("AudioManager is not initialised, can not continue _fadeOutRoutine");
            yield return null;
        }

        _vcaMaster.getVolume(out float startVol);
        float timeElapsed = 0.0f;

        while (timeElapsed < duration) {
            timeElapsed += Time.deltaTime;
            float currentVol = Mathf.Lerp(startVol, targetVol, timeElapsed / duration);
            _vcaMaster.setVolume(currentVol);
            yield return null;
        }
        
        _vcaMaster.setVolume(targetVol);
        _fadeCoroutine = null;
    }
    
    public EventInstance CreateInstance(EventReference eventRef) {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventRef);
        _eventInstances.Add(eventInstance);
        return eventInstance;
    }
    
    public StudioEventEmitter CreateEmitter(EventReference eventRef, GameObject emitter) {
        StudioEventEmitter emit = emitter.GetComponent<StudioEventEmitter>();
        emit.EventReference = eventRef;
        _eventEmitters.Add(emit);

        return emit;
    }
    
    private void _setVcaVolume(VCA vca, float volume) {
        if (vca.isValid()) {
            vca.setVolume(volume);
        }
    }
    
    public void UpdateVcaVolume(string vcaName, float value) {
        VCA vca = RuntimeManager.GetVCA($"vca:/{vcaName}");
        if (vca.isValid()) {
            vca.setVolume(value);
        } else {
            Debug.LogWarning($"VCA 'vca:/{vcaName}' not found or banks not loaded");
        }
    }
    
    public void PlayUIClick() {
        if (!UIClick.IsNull) {
            RuntimeManager.PlayOneShot(UIClick);
        }
    }
    
    // private void _cleanUp() {
    //     if (_eventInstances.Count < 0) {
    //         foreach (var eventInstance in _eventInstances) {
    //             eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    //             eventInstance.release();
    //         }
    //     }
    //     else {
    //         return;
    //     }
    //
    //     if (_eventEmitters.Count < 0) {
    //         foreach (var eventEmitter in _eventEmitters) {
    //             eventEmitter.Stop();
    //         }
    //     }
    // }
    //
    // private void OnDestroy() {
    //     _cleanUp();
    // }
}
