using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class AttackState : DogState
{
    public float dogHeight = 0.5f;
    public LayerMask blockers = ~0;
    public float attackRange = 2.5f;
    public float attackAngle = 90f;
    
    public override void Enter()
    {
        /* unused at the moment - ignore
        PlayerController playerController = stateMachine.player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            Vector3 dogDirection = dog.transform.position - stateMachine.player.position;
            float right = Vector3.Dot(stateMachine.player.right, dogDirection);
            if (right > 0f)
            {
                if (playerController.CheckDictionaryValue("Right Leg"))
                {
                    playerController.ShowHideBone("Right Leg");
                }
                else if (playerController.CheckDictionaryValue("Left Leg"))
                {
                    playerController.ShowHideBone("Left Leg");
                }
                else if (playerController.CheckDictionaryValue("Right Arm"))
                {
                    playerController.ShowHideBone("Right Arm");
                }
                else if (playerController.CheckDictionaryValue("Left Arm"))
                {
                    playerController.ShowHideBone("Left Arm");
                }
                else if (playerController.CheckDictionaryValue("Torso"))
                {
                    playerController.ShowHideBone("Torso");
                }
                else
                {
                    playerController.ShowHideBone("Head");
                }
            }
            else
            {
                if (playerController.CheckDictionaryValue("Left Leg"))
                {
                    playerController.ShowHideBone("Left Leg");
                }
                else if (playerController.CheckDictionaryValue("Right Leg"))
                {
                    playerController.ShowHideBone("Right Leg");
                }
                else if (playerController.CheckDictionaryValue("Left Arm"))
                {
                    playerController.ShowHideBone("Left Arm");
                }
                else if (playerController.CheckDictionaryValue("Right Arm"))
                {
                    playerController.ShowHideBone("Right Arm");
                }
                else if (playerController.CheckDictionaryValue("Torso"))
                {
                    playerController.ShowHideBone("Torso");
                }
                else
                {
                    playerController.ShowHideBone("Head");
                }
            }
        }
        */
        
        Transform dogBed = stateMachine.dogBed;
        if (dogBed == null) stateMachine.SetState(stateMachine.search);
        else
        {
            dog.Move(dogBed.position);
        }
    }

    public override void Tick()
    {
        if (dog.arrived) stateMachine.SetState(stateMachine.search);
    }

    public override void Exit()
    {
        /*
         * triggers logic for dropping bone
         */
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