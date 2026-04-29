using UnityEngine;

public class PatrolState<T> : States<T>
{
    public override void Awake()
    {
        Debug.Log("Patrol");
    }

    public override void Exceute()
    {
        var controller = _controller;

        controller.currentEnergy -= controller.energyDrain * Time.deltaTime;

        if (controller.currentEnergy <= 0)
        {
            controller._fsm.Transition("idle");
            return;
        }

        controller.SearchTarget();

        if (controller.target != null)
        {
            controller._fsm.Transition("chase");
            return;
        }

        if (controller.waypoints == null || controller.waypoints.Length == 0)
        {
            controller.speedAnimacion = 0f;
            return;
        }

        Transform targetWp = controller.waypoints[controller.currentWaypointIndex];

        if (targetWp == null)
        {
            controller.currentWaypointIndex++;

            if (controller.currentWaypointIndex >= controller.waypoints.Length)
            {
                controller.currentWaypointIndex = 0;
            }

            return;
        }

        Vector3 dir = targetWp.position - controller.transform.position;
        dir.y = 0;

        if (dir.magnitude < 0.2f)
        {
            controller.currentWaypointIndex++;

            if (controller.currentWaypointIndex >= controller.waypoints.Length)
            {
                controller.currentWaypointIndex = 0;
            }

            return;
        }

        dir = dir.normalized;

        controller.transform.position += dir * controller.speed * Time.deltaTime;

        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);

            controller.transform.rotation = Quaternion.Slerp(
                controller.transform.rotation,
                targetRotation,
                controller.rotationSpeed * Time.deltaTime
            );
        }

        controller.speedAnimacion = 1f;
    }

    public override void Sleep()
    {
        Debug.Log("Patrol Sleep");
    }
}