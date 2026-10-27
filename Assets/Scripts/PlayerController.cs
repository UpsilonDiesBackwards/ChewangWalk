using DG.Tweening;
using System.Globalization;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public enum Bones
{
    None,
    Leg1, 
    Leg2,
    Arm1,
    Arm2,
    Torso,
    Head

}

//I am well aware this could be named better but i am rusty and forgot this was already a component name
public class PlayerController : MonoBehaviour
{
    [Header("Adjustable Variables (Testing Only)")]
    [Tooltip("Current characters CharacterController")]
    public PlayerController controller;
    public GameObject thisObject;

    //Public
    [Tooltip("Adjust Movement Speed Multiplier")]
    public float MovementSpeed = 10f;

    //Input Actions (To be changed later)
    private InputAction _moveAction;

    //private
    private Bones LastLostBone = Bones.None; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");

        UnityEngine.Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {

        //Checking the value every frame for now, might update to only check on input
        //Vector2 moveVector = _moveAction.ReadValue<Vector2>();

        Vector2 moveV = _moveAction.ReadValue<Vector2>();
        
        if (moveV.x > 0 || moveV.y > 0 || moveV.x <0 || moveV.y <0  )
        {
            Debug.Log(moveV);
            controller.Move(moveV);
        }

        if (LastLostBone == Bones.Head)
        {
            Debug.Log("Dead");
        }
    }

    //Basic Movement script
    public void Move(Vector2 moveVector)
    {
        Vector3 move = thisObject.transform.forward * moveVector.y + thisObject.transform.right * moveVector.x;
        move = move * MovementSpeed * Time.deltaTime * ConvertedSpeed();

        thisObject.transform.position = new Vector3(thisObject.transform.position.x + move.x, thisObject.transform.position.y + move.y, thisObject.transform.position.z + move.z);

        
        //thisObject.transform.position = move;


    }

    //Scale number of bones to percentage of speed
    //Later change to reflect weight of losing one limb over using the other
    //e.g. losing a leg would be far more detrimental to movement over losing an arm, and losing the second arm would have a worse effect than losing the first 
    //What if you lose a head (Death 2)
    //To be called when you lose another bone
    public float ConvertedSpeed()
    {
        //Leave commented for now
        //switch (LastLostBone) 
        //{
        //    //Can be adjusted to remove bones as necessary
        //    case Bones.None:
        //        return 1f;
        //    case Bones.Leg1:
        //        return 0.8f;
        //    case Bones.Leg2:
        //        return 0.6f;
        //    case Bones.Arm1:
        //        return 0.4f;
        //    case Bones.Arm2:
        //        return 0.2f;
        //    case Bones.Torso:
        //        //You're now just a head
        //        break;
        //    case Bones.Head:
        //        //You're dead now
        //        //It shouldn't reach here
        //        break;
        //}

        return 1f;
    }

    public void ChangeNumberOfBones()
    {
        switch (LastLostBone)
        {
            //Can be adjusted to remove bones as necessary
            case Bones.None:
                LastLostBone = Bones.Leg1;
                Debug.Log("Lost one bone");
                break;
            case Bones.Leg1:
                LastLostBone = Bones.Leg2;
                Debug.Log("Lost second bone");
                break;
            case Bones.Leg2:
                LastLostBone = Bones.Arm1;
                Debug.Log("Lost third bone");
                break;
            case Bones.Arm1:
                LastLostBone = Bones.Arm2;
                Debug.Log("Lost fourth bone");
                break;
            case Bones.Arm2:
                LastLostBone = Bones.Torso;
                Debug.Log("Lost fifth bone");
                break;
            case Bones.Torso:
                LastLostBone = Bones.Head;
                Debug.Log("Lost final bone");
                EndGame();
                break;
            case Bones.Head:
                //You're dead now
                //It shouldn't reach here
                break;
        }
    }

    //If the bone hits a dog, set as true and only temporarily return to 
    public void ThrowBone()
    {
        //throw
    }

    //use to call any functions necessary to end the game 
    public void EndGame()
    {
        //insert functions here
    }
}
