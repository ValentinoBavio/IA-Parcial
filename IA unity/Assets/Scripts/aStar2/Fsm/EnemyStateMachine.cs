using UnityEngine;

public class EnemyStateMachine 
{
    public EnemyState CurrentEnemyState { get; set; }

    public void Initialize(EnemyState startingState)
    {
        CurrentEnemyState = startingState;
        CurrentEnemyState.EneterState();
    }

    public void ChangeState(EnemyState newState)
    {
        CurrentEnemyState.ExistState();
        CurrentEnemyState = newState;
        CurrentEnemyState.EneterState();
    }
}
