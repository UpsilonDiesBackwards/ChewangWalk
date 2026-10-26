using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    public static InputManager instance { get; private set; }
    
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private InputSettings inputSettings;

    public enum InputControllerType {
        InputControllerType_KEYBOARD,
        InputControllerType_CONTROLLER
    }
    
    public InputControllerType CurrentControllerType { get; private set; }
    public Gamepad ActiveGamepad { get; private set; }
    
    public Action<InputControllerType> OnControllerTypeChanged;
}
