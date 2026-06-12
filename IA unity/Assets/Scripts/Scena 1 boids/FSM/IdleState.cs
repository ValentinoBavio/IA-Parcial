using UnityEngine;

public class IdleState<T> : States<T>
{
    private float _timer;

    public override void Awake()
    {
        _timer = 0f;
        Debug.Log("Rest");
    }

    public override void Exceute()
    {
        _timer += Time.deltaTime;

        if (_timer >= _controller.restDuration)
        {
            _controller.currentEnergy = _controller.maxEnergy;
            _controller._fsm.Transition("patrol");
        }

        _controller.speedAnimacion = 0f;
    }

    public override void Sleep()
    {
        Debug.Log("idle Sleep");
    }





}
