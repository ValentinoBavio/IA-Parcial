using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _Speed = 5f;
    [SerializeField] private float _RotationSpeed = 10f;
    [SerializeField] private LayerMask _FloorMask;

    private UnityEngine.CharacterController _CharacterController;
    private Animator _Animator;

    private Vector3? _MovePoint = null;
    private Vector3 _Direction = Vector3.zero;
    private Quaternion _TargetRotation;

    private float _MovementSpeedBlend;

    public Vector3 Velocity => _Direction.normalized * _Speed;

    private void Awake()
    {
        _CharacterController = GetComponent<UnityEngine.CharacterController>();
        _Animator = GetComponent<Animator>();
    }

    private void Update()
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

        Vector3 flatPosition = new Vector3(transform.position.x, 0f, transform.position.z);

        if (_MovePoint.HasValue &&
            Vector3.Distance(flatPosition, _MovePoint.Value) > 0.05f)
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
        float stepSpeed = _Speed * Time.deltaTime;

        Vector3 movement = _Direction.normalized * stepSpeed;

        _MovementSpeedBlend = Mathf.Lerp(_MovementSpeedBlend, 1f, stepSpeed);

        _CharacterController.Move(movement);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            _TargetRotation,
            Time.deltaTime * _RotationSpeed
        );

        _Animator.SetFloat("Speed", _MovementSpeedBlend);
    }

    private void StopMove()
    {
        _MovePoint = null;

        _MovementSpeedBlend = Mathf.Lerp(
            _MovementSpeedBlend,
            0f,
            Time.deltaTime * _Speed
        );

        _Animator.SetFloat("Speed", _MovementSpeedBlend);
    }

    private void ComputeTarget()
    {
        if (Physics.Raycast(
            Camera.main.ScreenPointToRay(Input.mousePosition),
            out RaycastHit hit,
            100f,
            _FloorMask))
        {
            _MovePoint = new Vector3(hit.point.x, 0f, hit.point.z);

            Vector3 flatPosition = new Vector3(
                transform.position.x,
                0f,
                transform.position.z
            );

            _Direction = _MovePoint.Value - flatPosition;

            _TargetRotation = Quaternion.LookRotation(_Direction);
        }
    }

    private void ComputeRotation()
    {
        if (Physics.Raycast(
            Camera.main.ScreenPointToRay(Input.mousePosition),
            out RaycastHit hit,
            100f,
            _FloorMask))
        {
            Vector3 flatPosition = new Vector3(
                transform.position.x,
                0f,
                transform.position.z
            );

            Vector3 direction =
                new Vector3(hit.point.x, 0f, hit.point.z) - flatPosition;

            _TargetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                _TargetRotation,
                Time.deltaTime * _RotationSpeed
            );
        }
    }
}