using DG.Tweening;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    //Stores the bone name associated with whether or not it is still attached
    [SerializeField] private Dictionary<string, bool> playerBones = new Dictionary<string, bool>()
    {
        {"Left Arm", true},
        {"Right Arm", true},
        {"Left Leg", true},
        {"Right Leg", true},
        {"Torso", true},
        {"Head", true }
    };

    //Stores the name associated with the amount to be taken off the default 1f speed multiplier depending on which limbs are missing
    [SerializeField]
    private Dictionary<string, float> boneWeaknessValues = new Dictionary<string, float>()
    {
        {"Left Arm", 0.15f},
        {"Right Arm", 0.15f},
        {"Left Leg", 0.15f},
        {"Right Leg", 0.15f},
        {"Torso", 0.15f},
        {"Head", 0.15f }
    };

    private List<string> boneNames = new List<string>() {"Left Arm", "Right Arm", "Left Leg", "Right Leg", "Torso", "Head"};


    [Header("Adjustable Variables (Testing Only)")]
    [Tooltip("Current characters CharacterController")]
    public PlayerController controller;
    public GameObject thisObject;

    [Header("Bones - Not currently set up for use")]
    //placeholder either to use or to change types later 
    public GameObject rightArm;
    public GameObject leftArm;
    public GameObject rightLeg;
    public GameObject leftLeg;
    public GameObject Torso;
    public GameObject Head;

    //Public
    [Tooltip("Adjust Movement Speed Multiplier")]
    public float MovementSpeed = 10f;

    //Input Actions (To be changed later)
    private InputAction _moveAction;
    private InputAction _throwAction;

    //private
    private Bones LastLostBone = Bones.None;
    private bool _hasThrownArm = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
        _throwAction = InputSystem.actions.FindAction("ThrowBone");

        UnityEngine.Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {

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

        if (_throwAction.WasReleasedThisFrame())
        {
            ThrowBone();
        }

        //Testing if the values change
        //foreach (string i in boneNames)
        //{
        //    ChangeDictionaryValues(i, false);

        //}

    }

    //Basic Movement script
    public void Move(Vector2 moveVector)
    {
        Vector3 move = thisObject.transform.forward * moveVector.y + thisObject.transform.right * moveVector.x;
        move = move * MovementSpeed * Time.deltaTime * ConvertedSpeed();

        thisObject.transform.position = new Vector3(thisObject.transform.position.x + move.x, thisObject.transform.position.y + move.y, thisObject.transform.position.z + move.z);

    }

    public void Jump()
    {
        //add jump when input system issue is fixed
    }

    //Scale number of bones to percentage of speed (For testing purposes this is just set to 0.15 per lost bone
    public float ConvertedSpeed()
    {
        float multiplier = 1.0f;
        foreach (string i in boneNames)
        {
            bool isAttached = CheckDictionaryValue(i);
            if (!isAttached)
            {
                multiplier = multiplier - GetMultiplierValues(i);
            }
        }

        return multiplier;
    }

    //Call to update the most recent bone lost
    public void ChangeDictionaryValues(string lostBone, bool status)
    {

        //Currently just takes the bone and sets the same one to false this is for testing only 
        //searches the dictionary for the key
        bool boneStatus = playerBones.Single(s => s.Key == lostBone).Value;
        Debug.Log(boneStatus);

        //sets [key] to value
        playerBones[lostBone] = status;
        boneStatus = playerBones.Single(s => s.Key == lostBone).Value;
        Debug.Log(lostBone + " : " + boneStatus);
    }

    public float GetMultiplierValues(string lostBone)
    {
        return boneWeaknessValues[lostBone];
    }

    public bool CheckDictionaryValue(string boneName)
    {
        return playerBones.Single(s => s.Key == boneName).Value;
    }

    //If the bone hits a dog, set as true and only temporarily return to 
    public void ThrowBone()
    {
        //throw
        Debug.Log("Bone Thrown");

        if (CheckDictionaryValue("Left Arm"))
        {
            //throw left arm
            //Set this up last
            ChangeDictionaryValues("Left Arm", false);
        }

        else
        {
            //throw right arm
            ChangeDictionaryValues("Right Arm", true);
        }

    }

    public void PickUpBone(string pickedUp)
    {
        //should just be based off of collision, unnecessary to add button
        //should always be true if you're picking this up
        ChangeDictionaryValues(pickedUp, true);

        ShowHideBone(pickedUp);

    }

    public void ShowHideBone(string toChange)
    {
        //Function to show/hide the passed in bone by checking the dictionary value
        bool isShown = CheckDictionaryValue(toChange);
        if (isShown)
        {
            //set bone visibility to the same as isShown
        }

    }

    //use to call any functions necessary to end the game 
    public void EndGame()
    {
        //insert functions here
    }
}
