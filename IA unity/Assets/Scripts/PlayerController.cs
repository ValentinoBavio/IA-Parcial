using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _Speed = 5f;
    [SerializeField] private float _RotationSpeed = 10f;
    [SerializeField] private LayerMask _FloorMask;

    private CharacterController _CharacterController;
    private Animator _Animator;

    private Vector3? _MovePoint = null;
    private Vector3 _Direction = Vector3.zero;
    private Quaternion _TargetRotation;

    private float _MovementSpeedBlend;

    public Vector3 Velocity => new Vector3(_CharacterController.velocity.x, 0, _CharacterController.velocity.z);

    private void Awake()
    {
        _CharacterController = GetComponent<CharacterController>();
        _Animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            StopMove();
            ComputeRotation();
        }
        else if (Input.GetMouseButton(0))
        {
            ComputeTarget();
        }

        if (_MovePoint.HasValue && Vector3.Distance(transform.position.WithNewY(0.0f), _MovePoint.Value) > 0.05f)
        {
            Move();
        }
        else
        {
            StopMove();
        }
    }

    private void Move()
    {
        var stepSpeed = _Speed * Time.deltaTime;
        var movement = _Direction.normalized * stepSpeed;

        _MovementSpeedBlend = Mathf.Lerp(_MovementSpeedBlend, 1, stepSpeed);
        _CharacterController.Move(movement);

        transform.rotation = Quaternion.Slerp(transform.rotation, _TargetRotation, Time.deltaTime * _RotationSpeed);
        _Animator.SetFloat("Speed", _MovementSpeedBlend);
    }

    private void StopMove()
    {
        _MovePoint = null;
        _CharacterController.Move(Vector3.zero);
        _MovementSpeedBlend = Mathf.Lerp(_MovementSpeedBlend, 0, Time.deltaTime * _Speed);
        _Animator.SetFloat("Speed", _MovementSpeedBlend);
    }

    private void ComputeTarget()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100, _FloorMask))
        {
            _MovePoint = new Vector3(hit.point.x, 0.0f, hit.point.z);
            _Direction = _MovePoint.Value - transform.position.WithNewY(0);
            _TargetRotation = Quaternion.LookRotation(_Direction);
        }
    }

    private void ComputeRotation()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100, _FloorMask))
        {
            var direction = new Vector3(hit.point.x, 0.0f, hit.point.z) - transform.position.WithNewY(0);
            _TargetRotation = Quaternion.LookRotation(direction);

            
            transform.rotation = Quaternion.Slerp(transform.rotation, _TargetRotation, Time.deltaTime * _RotationSpeed);
        }
    }
}
