using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class TestPlayerPlaceholder : MonoBehaviour
{
    public float speed = 7f;
    CharacterController controller;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;
        
        Vector3 movement = Vector3.zero;
        if (keyboard.wKey.isPressed) movement.z -= 1f;
        if (keyboard.sKey.isPressed) movement.z += 1f;
        if (keyboard.dKey.isPressed) movement.x += 1f;
        if (keyboard.aKey.isPressed) movement.x -= 1f;
        
        controller.SimpleMove(movement.normalized * speed);
    }
}
