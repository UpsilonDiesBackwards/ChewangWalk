using UnityEngine;

public abstract class DogState
{
    protected DogStateMachine stateMachine;
    protected Dog dog;
    public virtual string Name => GetType().Name;

    public void Init(DogStateMachine owner)
    {
        stateMachine = owner;
        dog = owner.dog;
    }
    
    public virtual void Enter() {}
    public virtual void Tick() {}
    public virtual void Exit() {}
    public virtual void Debug(Transform owner) { }
}
