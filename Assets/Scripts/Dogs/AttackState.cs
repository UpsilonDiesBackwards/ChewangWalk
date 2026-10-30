using FMOD;
using FMODUnity;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class AttackState : DogState
{
    public float dogHeight = 0.5f;
    public LayerMask blockers = ~0;
    public float attackRange = 2.5f;
    public float attackAngle = 90f;
    private string stolenBone = " ";
    
    public EventReference BarkSound;
    
    public override void Enter()
    {
        PlayerController playerController = stateMachine.player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            Vector3 dogDirection = dog.transform.position - stateMachine.player.position;
            float right = Vector3.Dot(stateMachine.player.right, dogDirection);
            if (right > 0f)
            {
                if (playerController.CheckDictionaryValue("Right Leg"))
                {
                   stolenBone = "Right Leg";
                }
                else if (playerController.CheckDictionaryValue("Left Leg"))
                {
                    stolenBone = "Left Leg";
                }
                else if (playerController.CheckDictionaryValue("Right Arm"))
                {
                    stolenBone = "Right Arm";
                }
                else if (playerController.CheckDictionaryValue("Left Arm"))
                {
                    stolenBone = "Left Arm";
                }
                else if (playerController.CheckDictionaryValue("Torso"))
                {
                    stolenBone = "Torso";
                }
                else if (playerController.CheckDictionaryValue("Head"))
                {
                    stolenBone = "Head";
                }
                else
                {
                    stolenBone = null;
                }
            }
            else
            {
                if (playerController.CheckDictionaryValue("Left Leg"))
                {
                    stolenBone = "Left Leg";
                }
                else if (playerController.CheckDictionaryValue("Right Leg"))
                {
                    stolenBone = "Right Leg";
                }
                else if (playerController.CheckDictionaryValue("Left Arm"))
                {
                    stolenBone = "Left Arm";
                }
                else if (playerController.CheckDictionaryValue("Right Arm"))
                {
                    stolenBone = "Right Arm";
                }
                else if (playerController.CheckDictionaryValue("Torso"))
                {
                    stolenBone = "Torso";
                }
                else if (playerController.CheckDictionaryValue("Head"))
                {
                    stolenBone = "Head";
                }
                else
                {
                    stolenBone = null;
                }
            }
            
            playerController.ShowHideBone(stolenBone);
            playerController.ChangeDictionaryValues(stolenBone, false);
        }
        
        Transform dogBed = stateMachine.dogBed;
        if (dogBed == null) stateMachine.SetState(stateMachine.search);
        else
        {
            dog.Move(dogBed.position);
        }
        
        AudioManager.Instance.PlayOneShot(BarkSound);
    }

    public override void Tick()
    {
        if (dog.arrived) stateMachine.SetState(stateMachine.search);
    }

    public override void Exit()
    {
        BonePile bonePile = stateMachine.dogBed.GetComponent<BonePile>();
        bonePile.AddBone(stolenBone);
        dog.Stop();
    }

    public bool PlayerCaught()
    {
        Transform player = stateMachine.player;
        if (player == null) return false;
        
        // Calculates player position respective to the dog
        Vector3 mouth = dog.transform.position + Vector3.up * dogHeight;
        Vector3 playerPosition = player.position - mouth;
        float distance = playerPosition.magnitude;
        
        // The dog cannot attack the player if they're too far away
        if (distance > attackRange) return false;

        // Checks if the player is within the angle range
        Vector3 flatPlayerPosition = new Vector3(playerPosition.x, 0f, playerPosition.z);
        if (Vector3.Angle(dog.transform.forward, flatPlayerPosition) > attackAngle * 0.5f)
            return false;
        
        // Checks if anything is blocking the dog from of the player
        if (Physics.Raycast(mouth, playerPosition / distance, out RaycastHit hit, distance + 0.5f, blockers,
                QueryTriggerInteraction.Ignore))
        {
            bool caught = hit.transform == player || hit.transform.IsChildOf(player);
            return caught;
        }
        
        return false;
    }
}