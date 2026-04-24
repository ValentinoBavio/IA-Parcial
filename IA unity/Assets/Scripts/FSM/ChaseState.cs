using UnityEngine;

public class ChaseState<T> : States<T>
{


    public override void Awake()
    {
        Debug.Log("Chase");
    }

    public override void Exceute()
    {

        var controller = _controller;

        if (controller.target == null)
        {
            controller._fsm.Transition("patrol");
            return;
        }

        
        controller.currentEnergy -= controller.energyDrain * Time.deltaTime;

        

        if (controller.currentEnergy <= 0)
        {
            controller._fsm.Transition("idle");
            return;
        }

        float dist = Vector3.Distance(controller.transform.position, controller.target.position);

        
        if (dist > controller.visionRange)
        {
            controller._fsm.Transition("patrol");
            return;
        }



        
        Vector3 dir = (controller.target.position - controller.transform.position).normalized;
        controller.transform.position += dir * controller.speed * Time.deltaTime;

        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            _controller.transform.rotation = Quaternion.Slerp(
                _controller.transform.rotation,
                targetRotation,
                _controller.rotationSpeed * Time.deltaTime
            );
        }

        _controller.speedAnimacion = 1f;
    }

    public override void Sleep()
    {
        Debug.Log("Chase Sleep");
    }




}
