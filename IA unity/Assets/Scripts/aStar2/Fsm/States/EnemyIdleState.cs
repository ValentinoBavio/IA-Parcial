using UnityEngine;

public class EnemyIdleState : EnemyState
{

    private Transform currentNode;
    public EnemyIdleState(Enemy enemy, EnemyStateMachine enemeyStateMachine) : base(enemy, enemeyStateMachine)
    {

    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EneterState()
    {
        base.EneterState();

        currentNode = enemy.PatrolNodes[enemy.CurrentPatrolIndex];
    }

    public override void ExistState()
    {
        base.ExistState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (enemy.IsAggroed)
        {
            enemy.StateMachine.ChangeState(enemy.ChaseState);
            return;
        }

        Vector2 direction =(currentNode.position - enemy.transform.position).normalized;

        enemy.MoveEnemy(direction * enemy.movementSpeed);

        float distance =Vector2.Distance(enemy.transform.position, currentNode.position);

        if (distance < 0.1f)
        {
            enemy.CurrentPatrolIndex++;

            if (enemy.CurrentPatrolIndex >= enemy.PatrolNodes.Length)
            {
                enemy.CurrentPatrolIndex = 0;
            }

            currentNode = enemy.PatrolNodes[enemy.CurrentPatrolIndex];
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    
}
