using System.Collections.Generic;
using UnityEngine;

public class States<T>
{
    Dictionary<T, States<T>> _dic = new Dictionary<T, States<T>>();

    public virtual void Awake() { }

    public virtual void Exceute() { }

    public virtual void Sleep() { }



    public void AddTransition(T input, States<T> state)
    {
        _dic.TryAdd(input, state);        
    }


    public void RemoveTransition(T input)
    {
        if (_dic.ContainsKey(input))
        {
            _dic.Remove(input);
        }
    }


    public States<T> GetState(T input)
    {
        if (_dic.ContainsKey(input))
        {
            return _dic[input];
        }
        return null;
    }



    protected EnemyStateController _controller;

    public void SetController(EnemyStateController controller)
    {
        _controller = controller;
    }


}
