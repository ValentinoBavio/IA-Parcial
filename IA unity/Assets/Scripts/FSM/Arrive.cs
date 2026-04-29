using UnityEngine;

public class Arrive
{
    private Transform _from;
    private Transform _target;
    private float _slowRadius;
    private float _stopDistance;

    public Arrive(Transform from, Transform target, float slowRadius, float stopDistance)
    {
        _from = from;
        _target = target;
        _slowRadius = slowRadius;
        _stopDistance = stopDistance;
    }

    public Vector3 GetDir()
    {
        if (_target == null)
        {
            return Vector3.zero;
        }

        Vector3 dir = _target.position - _from.position;
        dir.y = 0;

        float distance = dir.magnitude;

        if (distance <= _stopDistance)
        {
            return Vector3.zero;
        }

        dir.Normalize();

        if (distance < _slowRadius)
        {
            float speedPercent = distance / _slowRadius;
            return dir * speedPercent;
        }

        return dir;
    }
}