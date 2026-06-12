using UnityEngine;
using static Enemy;

public class EnemyState
{
    protected Enemy enemy;
    protected EnemyStateMachine enemeyStateMachine;

    public EnemyState(Enemy enemy, EnemyStateMachine enemeyStateMachine)
    {
        this.enemy = enemy;
        this.enemeyStateMachine = enemeyStateMachine;
    }

    public virtual void EneterState() { }
    public virtual void ExistState() { }
    public virtual void FrameUpdate() { }
    public virtual void PhysicsUpdate() { }
    public virtual void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType) { }
}
