using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class PatrolState : DogState
{
    public float maxDistance = 10f;
    public float minDistance = 2f;
    public Transform dogBed;
    public float minIdleTime = 1f;
    public float maxIdleTime = 5f;
    public float timeout = 20f;
    public float navMeshRadius = 1f;
    public int maxAttempts = 10;
    public float patrolSpeed = 5f;
    
    enum Phase { Idle, Moving }

    Phase phase;
    float timer;
    NavMeshPath path;
    
    public override void Enter()
    {
        if (path == null) path = new NavMeshPath();
        phase = Phase.Idle;
        timer = Random.Range(minIdleTime, maxIdleTime);
        dog.dogAgent.speed = patrolSpeed;
    }

    public override void Tick()
    {
        // Handles state switching
        if (stateMachine.chase.PlayerSpotted())
        {
            stateMachine.SetState(stateMachine.chase);
            return;
        }

        if (stateMachine.track.PlayerSmelt())
        {
            stateMachine.SetState(stateMachine.track);
            return;
        }
        
        timer -= Time.deltaTime;

        switch (phase)
        {
            case Phase.Idle:
                if (timer > 0f) break; // Stays idle until timer runs out
                
                // Moves to a random point within the area
                if (FindRandomPoint(out Vector3 target) && dog.Move(target))
                {
                    phase = Phase.Moving; 
                    timer = timeout;
                }
                else
                {
                    timer = 0.5f;
                }

                break;
            
            case Phase.Moving:
                if (dog.arrived || timer <= 0f)
                {
                    phase = Phase.Idle;
                    timer = Random.Range(minIdleTime, maxIdleTime);
                }

                break;
        }
    }

    public override void Exit()
    {
        dog.Stop();
    }

    public bool FindRandomPoint(out Vector3 target)
    {
        NavMeshQueryFilter queryFilter = dog.queryFilter;
        Vector3 start = dog.dogAgent.nextPosition;
        Vector3 center = dogBed.position;
        
        for (int i = 0; i < maxAttempts; i++)
        {
            // Finds a random point around the dog bed
            Vector2 random = Random.insideUnitCircle * maxDistance;
            Vector3 randomPoint = center + new Vector3(random.x, 0f, random.y);
            
            // Checks if this location is walkable
            if (!NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, navMeshRadius, queryFilter))
                continue;
            
            // Checks that the location isn't too close
            if ((hit.position - start).sqrMagnitude < minDistance * minDistance)
                continue;

            // Checks that the location is reachable
            if (!dog.dogAgent.CalculatePath(hit.position, path))
                continue;

            if (path.status != NavMeshPathStatus.PathComplete)
                continue;
            
            target = hit.position;
            return true;
        }
        
        target = start;
        return false;
    }
}
