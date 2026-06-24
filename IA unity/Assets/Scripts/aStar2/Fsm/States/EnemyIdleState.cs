/*using UnityEngine;

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

    
}*/

using UnityEngine;

public class EnemyIdleState : EnemyState
{
    private Transform currentNode;

    public EnemyIdleState(
        Enemy enemy,
        EnemyStateMachine enemeyStateMachine
    ) : base(enemy, enemeyStateMachine)
    {

    }

    public override void AnimationTriggerEvent(
        Enemy.AnimationTriggerType triggerType
    )
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EneterState()
    {
        base.EneterState();

        SetCurrentPatrolNode();
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

        if (currentNode == null)
        {
            enemy.MoveEnemy(Vector2.zero);
            return;
        }

        Vector2 targetPosition = currentNode.position;

        float distanceToWaypoint = Vector2.Distance(
            enemy.transform.position,
            targetPosition
        );

        if (distanceToWaypoint < 0.1f)
        {
            AdvancePatrolNode();
            return;
        }

        if (enemy.HasLineOfSight(targetPosition))
        {
            Vector2 direction =
                (targetPosition -
                 (Vector2)enemy.transform.position).normalized;

            enemy.MoveEnemy(
                direction * enemy.movementSpeed
            );

            return;
        }

        if (enemy.CurrentPath == null ||
            enemy.CurrentPath.Count == 0 ||
            enemy.CurrentPathIndex >= enemy.CurrentPath.Count)
        {
            CalculatePath();
        }

        if (enemy.CurrentPath == null ||
            enemy.CurrentPath.Count == 0 ||
            enemy.CurrentPathIndex >= enemy.CurrentPath.Count)
        {
            enemy.MoveEnemy(Vector2.zero);
            return;
        }

        PathNode pathNode =
            enemy.CurrentPath[enemy.CurrentPathIndex];

        Vector2 pathDirection =
            ((Vector2)pathNode.transform.position -
             (Vector2)enemy.transform.position).normalized;

        enemy.MoveEnemy(
            pathDirection * enemy.movementSpeed
        );

        float distanceToPathNode = Vector2.Distance(
            enemy.transform.position,
            pathNode.transform.position
        );

        if (distanceToPathNode < 0.1f)
        {
            enemy.CurrentPathIndex++;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    private void SetCurrentPatrolNode()
    {
        if (enemy.PatrolNodes == null ||
            enemy.PatrolNodes.Length == 0)
        {
            currentNode = null;
            return;
        }

        if (enemy.CurrentPatrolIndex >=
            enemy.PatrolNodes.Length)
        {
            enemy.CurrentPatrolIndex = 0;
        }

        currentNode =
            enemy.PatrolNodes[enemy.CurrentPatrolIndex];

        enemy.CurrentPath = null;
        enemy.CurrentPathIndex = 0;
    }

    private void AdvancePatrolNode()
    {
        enemy.CurrentPatrolIndex++;

        if (enemy.CurrentPatrolIndex >=
            enemy.PatrolNodes.Length)
        {
            enemy.CurrentPatrolIndex = 0;
        }

        SetCurrentPatrolNode();
    }

    private void CalculatePath()
    {
        PathNode startNode =
            PathfindingManager.Instance.GetClosestNode(
                enemy.transform.position
            );

        PathNode targetNode =
            PathfindingManager.Instance.GetClosestNode(
                currentNode.position
            );

        if (startNode == null || targetNode == null)
        {
            enemy.CurrentPath = null;
            return;
        }

        enemy.CurrentPath =
            PathfindingManager.Instance.FindPath(
                startNode,
                targetNode
            );

        enemy.CurrentPathIndex = 0;
    }
}
