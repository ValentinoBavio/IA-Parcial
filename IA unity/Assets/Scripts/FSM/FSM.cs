
using System;
using UnityEngine;

public class FSM<T>
{
    private States<T> _currentState;

    public FSM(States<T> init)
    {
        if(init != null)
        {
            SetInit(init);
        }
    }

    public void SetInit(States<T> init)
    {
        _currentState = init;
        _currentState.Awake();
    }

    public void OnUpdate()
    {
        _currentState.Exceute();
    }

    public void Transition(T input)
    {
        States<T> newState = _currentState.GetState(input);

        if(newState == null)
        {
            return;
        }

        _currentState.Sleep();
        newState.Awake();
        _currentState = newState;

       
    }

   
}
