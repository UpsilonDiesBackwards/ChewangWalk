using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(Dog))]
public class DogStateMachine : MonoBehaviour
{
    public Transform player;
    public Transform dogBed;

    public PatrolState patrol = new PatrolState();
    public ChaseState chase = new ChaseState();
    public SearchState search = new SearchState();
    public TrackState track = new TrackState();
    public AttackState attack = new AttackState();

    [SerializeField] private string currentStateName;
    public Dog dog { get; private set; }
    public DogState currentState { get; private set; }

    void Awake()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null) player = playerObject.transform;
        }
        
        dog = GetComponent<Dog>();

        patrol.Init(this);
        chase.Init(this);
        search.Init(this);
        track.Init(this);
        attack.Init(this);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetState(patrol);
    }

    // Update is called once per frame
    void Update()
    {
        if (!dog.dogAgent.isOnNavMesh) return;
        currentState?.Tick();
    }

    public void SetState(DogState newState)
    {
        if (newState == null || newState == currentState) return;

        currentState?.Exit();
        currentState = newState;
        currentStateName = newState.Name;
        currentState.Enter();
    }
}
