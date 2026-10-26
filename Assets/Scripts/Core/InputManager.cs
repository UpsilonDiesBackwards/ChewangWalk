using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    public static InputManager _instance { get; private set; }

    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private InputSettings inputSettings;

    public enum InputControllerType {
        InputControllerType_KEYBOARD,
        InputControllerType_CONTROLLER
    }

    public InputControllerType CurrentControllerType { get; private set; }
    public Gamepad ActiveGamepad { get; private set; }

    public Action<InputControllerType> OnControllerTypeChanged;

    // Continuous
    public Vector2 Move { get; private set; }

    // Buttons
    public bool ThrowBonePressed { get; private set; }

    private void OnEnable() {
        InputSystem.onActionChange += _handleInputDeviceChange;
    }

    private void OnDisable() {
        InputSystem.onActionChange -= _handleInputDeviceChange;
    }

    public static InputManager Instance {
        get {
            if (!_instance) _instance = FindObjectOfType<InputManager>();
            return _instance;
        }
    }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(gameObject);
        _loadInputActions();

        if (Gamepad.current != null) {
            CurrentControllerType = _getControlTypeFromDevice(Gamepad.current);
            ToggleCursor(CurrentControllerType == InputControllerType.InputControllerType_KEYBOARD);
        }
    }

    private InputControllerType _getControlTypeFromDevice(Gamepad current) {
        throw new NotImplementedException();
    }

    private void _loadInputActions() {
        foreach (var map in inputActions.actionMaps) {
            map.Enable();
        }
    }

    private void LateUpdate() {
        ThrowBonePressed = false;
    }

    /*
     * Input Callbacks
     */

    private void OnMove(InputValue value) => Move = value.Get<Vector2>();

    private void OnThrowBonePressed(InputValue value) {
        if (value.isPressed) ThrowBonePressed = true;
    }

    private bool _cursorDisabled;
    private bool _cursorState = true;

    public void ForceDisableCursor(bool value) {
        _cursorDisabled = value;
        if (value) {
            _forceSetCursor(false);
        }
        else {
            _setCursor();
        }
    }

    public void ToggleCursor(bool value) {
        _cursorState = value;

        if (!_cursorDisabled) _setCursor();
    }

    private void _setCursor() {
        Cursor.visible = _cursorState;
        Cursor.lockState = _cursorState ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void _forceSetCursor(bool value) {
        Cursor.visible = value;
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
    }

    /*
     * Input Device Change logic
     */

    private void _handleInputDeviceChange(object obj, InputActionChange change) {
        if (change == InputActionChange.ActionStarted) {
            var action = (InputAction)obj;
            if (action?.activeControl?.device == null) return;

            var device = action.activeControl.device;

            InputControllerType detectedInputType = _getControlTypeFromDevice(device);

            if (detectedInputType != CurrentControllerType) {
                CurrentControllerType = detectedInputType;
                OnControllerTypeChanged?.Invoke(CurrentControllerType);

                if (device is Gamepad gamepad) {
                    ActiveGamepad = gamepad;
                }
                else {
                    ActiveGamepad = null;
                }

                // Hide da cursor if using a controller
                ToggleCursor(CurrentControllerType == InputControllerType.InputControllerType_KEYBOARD);
            }
        }
    }

    private InputControllerType _getControlTypeFromDevice(InputDevice device) {
        if (device is Keyboard || device is Mouse) return InputControllerType.InputControllerType_KEYBOARD;

        return InputControllerType.InputControllerType_CONTROLLER;
    }
}