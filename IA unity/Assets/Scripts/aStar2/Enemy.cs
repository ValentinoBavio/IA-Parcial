using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, IEnemyMoveable, ITriggerCheckable
{
    [field: SerializeField] public float maxHealth { get; set; } = 100f;

    public float currentHealth { get; set; }
    public Rigidbody2D rb { get; set; }
    public bool isFacingRight { get; set; } = true;

    public bool IsAggroed { get; set; }
    public bool IsWithinStrikingDistance { get; set; }


    [field: SerializeField] public Transform[] PatrolNodes { get; private set; }
    public int CurrentPatrolIndex { get; set; }

    public List<PathNode> CurrentPath { get; set; }

    public int CurrentPathIndex { get; set; }



    public EnemyAlertState AlertState { get; set; }

    public Vector2 LastKnownPlayerPosition { get; set; }

    [SerializeField] private Enemy[] linkedEnemies;




    #region StateVariables

    public EnemyStateMachine StateMachine { get; set; }
    public EnemyIdleState IdleState { get; set; }
    public EnemyChaseState ChaseState { get; set; }
    public EnemyAttackState AttackState { get; set; }




    #endregion


    #region Idle

    public Rigidbody2D BulletPrefab;
    public float movementSpeed = 1f;

    #endregion




    private void Awake()
    {
        StateMachine = new EnemyStateMachine();

        IdleState = new EnemyIdleState(this, StateMachine);
        ChaseState = new EnemyChaseState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine);
        AlertState = new EnemyAlertState(this, StateMachine);
    }
    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        StateMachine.CurrentEnemyState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentEnemyState.PhysicsUpdate();
    }

    #region AlertState


    public void AlertNearbyEnemies(Vector2 playerPosition)
    {
        foreach (Enemy otherEnemy in linkedEnemies)
        {
            if (otherEnemy == null)
                continue;

            if (otherEnemy == this)
                continue;

            
            if (otherEnemy.StateMachine.CurrentEnemyState == otherEnemy.ChaseState)
                continue;

            otherEnemy.ReceiveAlert(playerPosition);
        }
    }

    public void ReceiveAlert(Vector2 playerPosition)
    {
        PathNode startNode =PathfindingManager.Instance.GetClosestNode(transform.position);

        PathNode targetNode =PathfindingManager.Instance.GetClosestNode(playerPosition);

        CurrentPath =PathfindingManager.Instance.FindPath(startNode,targetNode);

        CurrentPathIndex = 0;

        LastKnownPlayerPosition = playerPosition;

        StateMachine.ChangeState(AlertState);
    }

    #endregion

    #region IDamageable
    public void Damageable(float damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {

    }

    #endregion

    #region IEnemyMoveable
    public void MoveEnemy(Vector2 velocity)
    {
        rb.linearVelocity = velocity;
        CheckForLeftOrRightFacing(velocity);
    }

    public void CheckForLeftOrRightFacing(Vector2 velocity)
    {
        if (isFacingRight && velocity.x < 0f)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            isFacingRight = !isFacingRight;
        }
        else if (!isFacingRight && velocity.x > 0f)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);
            isFacingRight = !isFacingRight;
        }
    }

    #endregion

    #region ITriggerCheckable

    public void SetAggroStatus(bool isAggroed)
    {
        IsAggroed = isAggroed;
    }

    public void SetStrikingDistanceBool(bool isWithinStrikingDistance)
    {
        IsWithinStrikingDistance = isWithinStrikingDistance;
    }

    #endregion

    #region Animation

    private void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        StateMachine.CurrentEnemyState.AnimationTriggerEvent(triggerType);
    }


    public enum AnimationTriggerType
    {
        EnemyDamaged,
        PlayFootstepSound
    }
    #endregion
}
