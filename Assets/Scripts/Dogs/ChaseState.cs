using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class ChaseState : DogState
{
    public float dogHeight = 0.5f;
    public LayerMask visionBlockers = ~0;
    public float chaseSpeed = 10f;
    public float repathInterval = 0.25f;
    public float timeout = 0.1f;
    float repathTimer;
    float sightTimer;
    public float chaseProximityRange = 10f;
    public float chaseSightRange = 40f;
    public float chaseAngleRange = 360f;
    
    enum Phase { sighted, losing }
    Phase phase;
    
    public override void Enter()
    {
        NavMeshAgent dogAgent = dog.dogAgent;
        dogAgent.speed = chaseSpeed;
        dogAgent.autoBraking = false;
        repathTimer = 0f;
        sightTimer = timeout;
    }

    public override void Tick()
    {
        // Handles state switching
        if (stateMachine.attack.PlayerCaught())
        {
            stateMachine.SetState(stateMachine.attack);
            return;
        }

        Transform player = stateMachine.player;
        if (player == null)
        {
            stateMachine.SetState(stateMachine.patrol);
            return;
        }
        
        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            repathTimer = repathInterval;
            dog.Move(player.position);
        }

        switch (phase)
        {
            case Phase.sighted:
                if (PlayerSpotted(chaseProximityRange, chaseSightRange, chaseAngleRange)) break;

                sightTimer = timeout;
                phase = Phase.losing;
                break;
            case Phase.losing:
                if (PlayerSpotted(chaseProximityRange, chaseSightRange, chaseAngleRange))
                {
                    phase = Phase.sighted;
                }
                else
                {
                    sightTimer -= Time.deltaTime;
                    if (sightTimer <= 0f) stateMachine.SetState(stateMachine.search);
                }
                break;
        }
    }

    public override void Exit()
    {
        dog.Stop();
    }

    public bool PlayerSpotted(float proximityRange, float sightRange, float angleRange)
    {
        Transform player = stateMachine.player;
        if (player == null) return false;
        
        // Calculates player position respective to the dog
        Vector3 eyes = dog.transform.position + Vector3.up * dogHeight;
        Vector3 playerPosition = player.position - eyes;
        float distance = playerPosition.magnitude;

        // The dog spots the player regardless if they're too close
        if (distance < proximityRange) return true;
        // The dog cannot see the player regardless if they're too far
        if (distance > sightRange) return false;

        // Checks if the player is within the angle range
        Vector3 flatPlayerPosition = new Vector3(playerPosition.x, 0f, playerPosition.z);
        if (Vector3.Angle(dog.transform.forward, flatPlayerPosition) > angleRange * 0.5f)
            return false;
        
        // Checks if anything is blocking the dogs vision of the player
        if (Physics.Raycast(eyes, playerPosition / distance, out RaycastHit hit, distance + 0.5f, visionBlockers,
                QueryTriggerInteraction.Ignore))
        {
            bool spotted = hit.transform == player || hit.transform.IsChildOf(player);
            return spotted;
        }
         
        return false;
    }
}