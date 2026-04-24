using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState<T> : States<T>
{
    public override void Awake()
    {
        Debug.Log("Patrol");
    }

    public override void Exceute()
    {
        Debug.Log("estoy re de la patrulla");
        var controller = _controller;

        
        controller.currentEnergy -= controller.energyDrain * Time.deltaTime;

        if (controller.currentEnergy <= 0)
        {
            controller._fsm.Transition("idle");
            return;
        }

        
        Transform targetWp = controller.waypoints[controller.currentWaypointIndex];

        Vector3 dir = (targetWp.position - controller.transform.position).normalized;
        controller.transform.position += dir * controller.speed * Time.deltaTime;

        
        if (Vector3.Distance(controller.transform.position, targetWp.position) < 0.2f)
        {
            controller.currentWaypointIndex++;

            if (controller.currentWaypointIndex >= controller.waypoints.Length)
                controller.currentWaypointIndex = 0;
        }

        
        if (controller.target != null)
        {
            float dist = Vector3.Distance(controller.transform.position, controller.target.position);

            if (dist <= controller.visionRange)
            {
                controller._fsm.Transition("chase");
            }
        }

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
}
