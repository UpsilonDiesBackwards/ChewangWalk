using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Dog : MonoBehaviour
{
    public Transform target;
    public float navMeshRadius = 2f;
    NavMeshAgent agent;
    public NavMeshAgent dogAgent => agent;
    public NavMeshQueryFilter queryFilter => new NavMeshQueryFilter
    {
        agentTypeID = agent.agentTypeID,
        areaMask = agent.areaMask
    };

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (target != null)
            Move(target.position);
    }

    public bool Move(Vector3 worldPoint)
    {
        // Snaps the target location to the navmesh
        if (NavMesh.SamplePosition(worldPoint, out NavMeshHit hit, navMeshRadius, queryFilter))
        {
            agent.isStopped = false;
            return agent.SetDestination(hit.position);
        }
        
        return false;
    }

    public void Stop()
    {
        agent.isStopped = true;
        agent.ResetPath();
    }
    
    public bool Arrived => !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f);
    public float speed => agent.velocity.magnitude;
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
