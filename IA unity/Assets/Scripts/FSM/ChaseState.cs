using UnityEngine;

public class ChaseState<T> : States<T>
{
    private Vector3 _lastTargetPosition;
    private Vector3 _targetVelocity;

    public override void Awake()
    {
        Debug.Log("Chase");

        if (_controller.target != null)
        {
            _lastTargetPosition = _controller.target.position;
        }
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
            controller.target = null;
            controller._fsm.Transition("patrol");
            return;
        }

        if (dist <= controller.chaseStopDistance)
        {
            controller.speedAnimacion = 0f;

            Vector3 lookDir = controller.target.position - controller.transform.position;
            lookDir.y = 0;

            if (lookDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDir);

                controller.transform.rotation = Quaternion.Slerp(
                    controller.transform.rotation,
                    targetRotation,
                    controller.rotationSpeed * Time.deltaTime
                );
            }

            return;
        }

        if (Time.deltaTime > 0)
        {
            _targetVelocity = (controller.target.position - _lastTargetPosition) / Time.deltaTime;
        }

        _lastTargetPosition = controller.target.position;

        Vector3 futurePosition = controller.target.position + _targetVelocity * controller.predictionTime;

        Vector3 dir = futurePosition - controller.transform.position;
        dir.y = 0;

        if (dir == Vector3.zero)
        {
            controller.speedAnimacion = 0f;
            return;
        }

        dir = dir.normalized;

        controller.transform.position += dir * controller.speed * Time.deltaTime;

        Quaternion rotation = Quaternion.LookRotation(dir);

        controller.transform.rotation = Quaternion.Slerp(
            controller.transform.rotation,
            rotation,
            controller.rotationSpeed * Time.deltaTime
        );

        controller.speedAnimacion = 1f;
    }

    public override void Sleep()
    {
        Debug.Log("Chase Sleep");
    }
}