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
        /*
         * triggers attack animation and player losing bone
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