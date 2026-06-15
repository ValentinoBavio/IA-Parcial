using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class EnemyChaseState : EnemyState
{
    private Transform playerTransform;

    private float movementSpeed = 4.5f;

    private float alertCooldown = 1f;
    private float alertTimer;

    public EnemyChaseState(Enemy enemy,EnemyStateMachine enemeyStateMachine): base(enemy, enemeyStateMachine)
    {
        playerTransform =GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void AnimationTriggerEvent(Enemy.AnimationTriggerType triggerType)
    {
        base.AnimationTriggerEvent(triggerType);
    }

    public override void EneterState()
    {
        base.EneterState();
    }

    public override void ExistState()
    {
        base.ExistState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        alertTimer -= Time.deltaTime;

        if (!enemy.IsAggroed)
        {
            enemy.StateMachine.ChangeState(enemy.IdleState);

            return;
        }

        
        enemy.LastKnownPlayerPosition =playerTransform.position;

       
        if (alertTimer <= 0f)
        {
            enemy.AlertNearbyEnemies(playerTransform.position);

            alertTimer = alertCooldown;
        }

        Vector2 moveDirection =(playerTransform.position- enemy.transform.position).normalized;

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
