using UnityEngine;

public class EnemyAttackState : EnemyState
{
    private Transform playerTransform;

    private float timer;
    private float timeBetweenShots = 2f;

    private float exitTimer;
    private float timeTillExit = 3f;
    private float distanceToCountExit = 3f;

    private float bulletSped = 10f;
    public EnemyAttackState(Enemy enemy, EnemyStateMachine enemeyStateMachine) : base(enemy, enemeyStateMachine)
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
    }

    public override void ExistState()
    {
        base.ExistState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        enemy.MoveEnemy(Vector2.zero);

        if (timer > timeBetweenShots)
        {
            timer = 0f;

            Vector2 dir = (playerTransform.position - enemy.transform.position).normalized;

            Rigidbody2D bullet = GameObject.Instantiate(enemy.BulletPrefab, enemy.transform.position, Quaternion.identity);
            bullet.linearVelocity = dir * bulletSped;
        }

        if (Vector2.Distance(playerTransform.position, enemy.transform.position) > distanceToCountExit)
        {
            exitTimer += Time.deltaTime;

            if (exitTimer > timeTillExit)
            {
                enemy.StateMachine.ChangeState(enemy.ChaseState);            
            }


        }
        else
        {
            exitTimer = 0f;
        }



            timer += Time.deltaTime;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
