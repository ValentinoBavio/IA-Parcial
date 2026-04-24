using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float _Speed = 4f;
    [SerializeField] private float _RotationSpeed = 8f;
    [SerializeField] private float _StopDistance = 1.5f;


    [Header("Boids")]
    [SerializeField] private float _DetectionDistance = 1f;

    [SerializeField] private float _SeparationWeight = 1f;
    [SerializeField] private float _AlignmentWeight = 1f;
    [SerializeField] private float _CohesionWeight = 1f;


    private Animator _Animator;
    public Transform _Target;

    private Vector3 _Direction = Vector3.zero;
    private Quaternion _TargetRotation;

    private float _MovementSpeedBlend;

    private Vector3 _SeparationForce;

    private void Awake()
    {
        _Animator = GetComponent<Animator>();
        //_Target = FindObjectOfType<PlayerController>().transform;
    }

    void Update()
    {
        if (_Target != null)
        {
            FollowTarget();
        }
    }

    

    private void FollowTarget()
    {
        _SeparationForce = Vector3.zero;
        _Direction = (_Target.position - transform.position).WithNewY(0);
        float distance = _Direction.magnitude;

        var neighbours = GetNeighbours();

        if( neighbours.Length > 0 )
        {
            CalculateSeparationForce(neighbours);
            ApplyAlligment(neighbours);
            ApplyeCohesion(neighbours);
        }
        

        if (distance > _StopDistance)
        {
            MoveTowardsTarget();
        }
        else
        {
            StopMove();
        }

        RotateTowardsTarget();
    }

    private void ApplyeCohesion(Collider[] neighbours)
    {
        Vector3 averagePosition = Vector3.zero;

        foreach (var neighbour in neighbours)
        {
           
            averagePosition += neighbour.transform.position;
        }

        averagePosition /= neighbours.Length;
        Vector3 cohesionDir = (averagePosition - transform.position).normalized;
        _SeparationForce += cohesionDir * _CohesionWeight;
    }

    private void ApplyAlligment(Collider[] neighbours)
    {
        Vector3 neighboursForward = Vector3.zero;

        foreach (var neighbour in neighbours)
        {
            neighboursForward += neighbour.transform.forward;
        }

        if(neighboursForward != Vector3.zero)
        {
            neighboursForward.Normalize();
        }

        _SeparationForce += neighboursForward * _AlignmentWeight;

    }

    private void CalculateSeparationForce(Collider[] neighbours)
    {
        foreach(var neighbour in neighbours)
        {
            var dir = neighbour.transform.position - transform.position;
            var distance = dir.magnitude;
            var away = -dir.normalized;

            if ( distance > 0 )
            {
                _SeparationForce += (away / distance) * _SeparationWeight;
            }
        }
    }

    private Collider [] GetNeighbours()
    {
        var enemyMask = LayerMask.GetMask("Enemy");
        return Physics.OverlapSphere(transform.position, _DetectionDistance, enemyMask);
    }

    private void MoveTowardsTarget()
    {
        _Direction = _Direction.normalized;
        var combinedDirection = (_Direction + _SeparationForce).normalized;
        Vector3 movement = combinedDirection * _Speed * Time.deltaTime;
        transform.position += movement;
        _MovementSpeedBlend = Mathf.Lerp(_MovementSpeedBlend, 1, Time.deltaTime * _Speed);
        _Animator.SetFloat("Speed", _MovementSpeedBlend);
    }

    private void StopMove()
    {
        _MovementSpeedBlend = Mathf.Lerp(_MovementSpeedBlend, 0, Time.deltaTime * _Speed);
        _Animator.SetFloat("Speed", _MovementSpeedBlend);
    }

    private void RotateTowardsTarget()
    {
        _TargetRotation = Quaternion.LookRotation(_Direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, _TargetRotation, Time.deltaTime * _RotationSpeed);
    }
}
