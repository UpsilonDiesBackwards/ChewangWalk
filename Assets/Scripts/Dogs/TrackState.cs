using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class TrackState : DogState
{
    public float trackSpeed = 2f;
    public float trackProximityRange = 10f;
    public float trackSightRange = 40f;
    public float trackAngleRange = 180f;
    public float trackSmellRange = 10f;
    public float sniffInterval = 0.2f;
    public float maxScentChecks = 10f;
    public float dogHeight = 0.5f;

    PlayerScent trail;
    NavMeshPath path;
    float sniffTimer;
    float targetScent;
    bool smellsPlayer;
    
    public override void Enter()
    {
        dog.dogAgent.speed = trackSpeed;
        smellsPlayer = false;
        targetScent = float.MinValue;
        sniffTimer = 0f;
    }

    public override void Tick()
    {
        // Handles state switching
        if (stateMachine.chase.PlayerSpotted(trackProximityRange, trackSightRange, trackAngleRange))
        {
            stateMachine.SetState(stateMachine.chase);
            return;
        }

        sniffTimer -= Time.deltaTime;
        if (sniffTimer > 0f) return;
        sniffTimer = sniffInterval;
        
        // Tracks the most recent scent point within range
        if (TrackPlayer(targetScent, out ScentTrail scent) && dog.Move(scent.position))
        {
            targetScent = scent.time;
            smellsPlayer = true;
        }
        // Switches to search state if trail is lost
        else if (!smellsPlayer || dog.arrived)
        {
            stateMachine.SetState(stateMachine.search);
        }
    }

    public override void Exit()
    {
        dog.Stop();
    }

    public bool PlayerSmelt(float smellRange)
    {
        sniffTimer -= Time.deltaTime;
        if (sniffTimer > 0f) return false;
        sniffTimer = sniffInterval;

        return TrackPlayer(float.MinValue, out _);
    }

    public bool TrackPlayer(float currentScent, out ScentTrail newScent)
    {
        newScent = default;
        PlayerScent scentTrail = GetScentTrail();
        if (scentTrail == null) return false;
        if (path == null) path = new NavMeshPath();
        Vector3 nose = dog.transform.position + Vector3.up * dogHeight;
        float rangeSquared = trackSmellRange * trackSmellRange;
        int scentChecks = 0;

        for (int i = scentTrail.scentTrail.Count - 1; i >= 0; i--)
        {
            ScentTrail scentPoint = scentTrail.scentTrail[i];

            // Ends search when it reaches a scent before the currently tracked one
            if (scentPoint.time <= currentScent) break;
            // Skips scent points that aren't within range
            if ((scentPoint.position - nose).sqrMagnitude > rangeSquared) continue;
            // Times out if the dog is failing to path to the scent points
            if (scentChecks >= maxScentChecks) break;
            scentChecks++;
            if (!NavMesh.SamplePosition(scentPoint.position, out NavMeshHit hit, dog.navMeshRadius,
                    dog.queryFilter)) continue;
            if (!dog.dogAgent.CalculatePath(hit.position, path)) continue;
            
            newScent = scentPoint;
            return true;
        }
        return false;
    }

    PlayerScent GetScentTrail()
    {
        Transform player = stateMachine.player;
        if (player == null) return null;

        if (trail == null || trail.transform != player)
            trail = player.GetComponent<PlayerScent>();
        
        return trail;
    }
}