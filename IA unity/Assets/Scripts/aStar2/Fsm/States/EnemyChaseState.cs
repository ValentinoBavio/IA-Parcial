using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class EnemyChaseState : EnemyState
{
    private Transform playerTransform;
    private float movementSpeed =4.5f;
    public EnemyChaseState(Enemy enemy, EnemyStateMachine enemeyStateMachine) : base(enemy, enemeyStateMachine)
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EneterState()
    {
        base.EneterState();

        //Debug.Log("ChaseState activado");
    }

    public override void ExistState()
    {
        base.ExistState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        if (!enemy.IsAggroed)
        {
            enemy.StateMachine.ChangeState(enemy.IdleState);
            return;
        }


        Vector2 moveDirection = (playerTransform.position - enemy.transform.position).normalized;
        
        enemy.MoveEnemy(moveDirection * movementSpeed);

        if (enemy.IsWithinStrikingDistance)
        {
            enemy.StateMachine.ChangeState(enemy.AttackState);
        }


    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
