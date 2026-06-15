using UnityEngine;

public class EnemyAlertState : EnemyState
{
    private float movementSpeed = 3.5f;

    public EnemyAlertState(Enemy enemy,EnemyStateMachine enemyStateMachine): base(enemy, enemyStateMachine)
    {

    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (enemy.IsAggroed)
        {
            enemy.StateMachine.ChangeState(
                enemy.ChaseState);

            return;
        }

        if (enemy.CurrentPath == null
            || enemy.CurrentPath.Count == 0)
        {
            enemy.StateMachine.ChangeState(
                enemy.IdleState);

            return;
        }

        PathNode currentNode =
            enemy.CurrentPath[enemy.CurrentPathIndex];

        Vector2 direction =
            ((Vector2)currentNode.transform.position
            - (Vector2)enemy.transform.position)
            .normalized;

        enemy.MoveEnemy(direction * movementSpeed);

        float distance =
            Vector2.Distance(
                enemy.transform.position,
                currentNode.transform.position);

        if (distance < 0.1f)
        {
            enemy.CurrentPathIndex++;

            if (enemy.CurrentPathIndex >=
                enemy.CurrentPath.Count)
            {
                enemy.MoveEnemy(Vector2.zero);

                enemy.StateMachine.ChangeState(
                    enemy.IdleState);
            }
        }
    }
}