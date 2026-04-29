using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float _Speed = 3f;
    [SerializeField] private float _RotationSpeed = 8f;

    [Header("Food")]
    [SerializeField] private LayerMask _FoodMask;
    [SerializeField] private float _FoodDetectionDistance = 15f;
    [SerializeField] private float _EatDistance = 1.5f;
    [SerializeField] private float _ArriveSlowRadius = 3f;

    [Header("Hunter")]
    [SerializeField] private Transform _Hunter;
    [SerializeField] private float _HunterDetectionDistance = 5f;
    [SerializeField] private float _EvadePredictionTime = 0.2f;
    [SerializeField] private float _EvadeSpeedMultiplier = 1.4f;

    [Header("Boids")]
    [SerializeField] private LayerMask _BoidMask;
    [SerializeField] private float _DetectionDistance = 4f;
    [SerializeField] private float _SeparationWeight = 3f;
    [SerializeField] private float _AlignmentWeight = 4f;
    [SerializeField] private float _CohesionWeight = 5f;
    [SerializeField] private int _MinBoidsToFlock = 3;
    [SerializeField] private float _JoinGroupStopDistance = 1.5f;

    [Header("Random Move")]
    [SerializeField] private float _RandomDirectionTime = 2f;

    [Header("Circular Limit")]
    [SerializeField] private bool _UseCircularLimit = true;
    [SerializeField] private Transform _LimitCenter;
    [SerializeField] private float _LimitRadius = 25f;
    [SerializeField] private float _LimitMargin = 4f;
    [SerializeField] private float _LimitRandomDirectionTime = 1.2f;

    private Animator _Animator;

    private INode _Init;

    private Vector3 _Direction = Vector3.zero;
    private Quaternion _TargetRotation;

    private float _MovementSpeedBlend;

    private Vector3 _FlockingForce;

    private Food _FoodTarget;
    private Vector3 _FoodTargetPosition;

    private List<EnemyController> _Neighbours = new List<EnemyController>();

    private Vector3 _RandomDirection;
    private float _RandomTimer;

    private Vector3 _LastHunterPosition;
    private Vector3 _HunterVelocity;

    private float _LimitRandomTimer;
    private float _CurrentSpeedMultiplier = 1f;

    private void Awake()
    {
        _Animator = GetComponent<Animator>();

        CreateDecisionTree();

        _RandomDirection = GetRandomDirection();

        if (_Hunter != null)
        {
            _LastHunterPosition = _Hunter.position;
        }
    }

    private void Update()
    {
        UpdateHunterVelocity();

        _Direction = Vector3.zero;
        _FlockingForce = Vector3.zero;
        _CurrentSpeedMultiplier = 1f;

        _Init.Execute();

        AvoidLeavingCircle();

        Move();

        ClampInsideCircle();

        RotateTowardsDirection();

        CheckEatFood();
    }

    private void CreateDecisionTree()
    {
        ActionNode goToFood = new ActionNode(GoToFood);
        ActionNode evadeHunter = new ActionNode(EvadeHunter);
        ActionNode flocking = new ActionNode(DoFlocking);
        ActionNode randomMove = new ActionNode(DoRandomMove);

        QuestionNode questionBoids = new QuestionNode(HasBoidsNear, flocking, randomMove);
        QuestionNode questionHunter = new QuestionNode(HasHunterNear, evadeHunter, questionBoids);
        QuestionNode questionFood = new QuestionNode(HasFoodNear, goToFood, questionHunter);

        _Init = questionFood;
    }

    private bool HasFoodNear()
    {
        Collider[] colliders;

        if (_FoodMask.value == 0)
        {
            colliders = Physics.OverlapSphere(
                transform.position,
                _FoodDetectionDistance,
                ~0,
                QueryTriggerInteraction.Collide
            );
        }
        else
        {
            colliders = Physics.OverlapSphere(
                transform.position,
                _FoodDetectionDistance,
                _FoodMask,
                QueryTriggerInteraction.Collide
            );
        }

        if (colliders.Length <= 0)
        {
            _FoodTarget = null;
            return false;
        }

        float minDistance = Mathf.Infinity;
        Food closestFood = null;
        Vector3 closestFoodPosition = Vector3.zero;

        for (int i = 0; i < colliders.Length; i++)
        {
            Food food = colliders[i].GetComponentInParent<Food>();

            if (food == null)
            {
                continue;
            }

            Vector3 boidPos = transform.position.WithNewY(0);
            Vector3 foodPos = colliders[i].bounds.center.WithNewY(0);

            float distance = Vector3.Distance(boidPos, foodPos);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestFood = food;
                closestFoodPosition = foodPos;
            }
        }

        _FoodTarget = closestFood;
        _FoodTargetPosition = closestFoodPosition;

        return _FoodTarget != null;
    }

    private bool HasHunterNear()
    {
        if (_Hunter == null)
        {
            return false;
        }

        Vector3 boidPos = transform.position.WithNewY(0);
        Vector3 hunterPos = _Hunter.position.WithNewY(0);

        float distance = Vector3.Distance(boidPos, hunterPos);

        return distance <= _HunterDetectionDistance;
    }

    private bool HasBoidsNear()
    {
        _Neighbours = GetNeighbours();

        return _Neighbours.Count > 0;
    }

    private void GoToFood()
    {
        if (_FoodTarget == null)
        {
            return;
        }

        _Direction = GetArriveDirection(_FoodTargetPosition);

        if (_Direction == Vector3.zero)
        {
            EatCurrentFood();
        }
    }

    private Vector3 GetArriveDirection(Vector3 targetPosition)
    {
        Vector3 dir = targetPosition - transform.position;
        dir = dir.WithNewY(0);

        float distance = dir.magnitude;

        if (distance <= _EatDistance)
        {
            return Vector3.zero;
        }

        dir.Normalize();

        if (distance < _ArriveSlowRadius)
        {
            float speedPercent = distance / _ArriveSlowRadius;
            return dir * speedPercent;
        }

        return dir;
    }

    private void EvadeHunter()
    {
        if (_Hunter == null)
        {
            return;
        }

        Vector3 hunterPos = _Hunter.position.WithNewY(0);
        Vector3 boidPos = transform.position.WithNewY(0);

        Vector3 dirAwayFromHunter = boidPos - hunterPos;

        if (dirAwayFromHunter == Vector3.zero)
        {
            dirAwayFromHunter = -_Hunter.forward.WithNewY(0);
        }

        Vector3 futurePosition = hunterPos + _HunterVelocity.WithNewY(0) * _EvadePredictionTime;
        Vector3 dirAwayFromPrediction = boidPos - futurePosition;

        if (dirAwayFromPrediction == Vector3.zero)
        {
            dirAwayFromPrediction = dirAwayFromHunter;
        }

        float dot = Vector3.Dot(dirAwayFromPrediction.normalized, dirAwayFromHunter.normalized);

        if (dot < 0.2f)
        {
            _Direction = dirAwayFromHunter.normalized;
        }
        else
        {
            _Direction = dirAwayFromPrediction.normalized;
        }

        _CurrentSpeedMultiplier = _EvadeSpeedMultiplier;
    }

    private void DoFlocking()
    {
        _FlockingForce = Vector3.zero;

        if (_Neighbours == null || _Neighbours.Count <= 0)
        {
            DoRandomMove();
            return;
        }

        int boidsInGroup = _Neighbours.Count + 1;

        if (boidsInGroup < _MinBoidsToFlock)
        {
            JoinSmallGroup();
            return;
        }

        CalculateSeparationForce(_Neighbours);
        ApplyAlligment(_Neighbours);
        ApplyeCohesion(_Neighbours);

        _Direction = _FlockingForce;

        if (_Direction != Vector3.zero)
        {
            _Direction.Normalize();
        }
        else
        {
            DoRandomMove();
        }
    }

    private void JoinSmallGroup()
    {
        Vector3 center = Vector3.zero;

        for (int i = 0; i < _Neighbours.Count; i++)
        {
            center += _Neighbours[i].transform.position;
        }

        center /= _Neighbours.Count;

        Vector3 dir = center - transform.position;
        dir = dir.WithNewY(0);

        if (dir.magnitude <= _JoinGroupStopDistance)
        {
            DoRandomMove();
            return;
        }

        _Direction = dir.normalized;
    }

    private void DoRandomMove()
    {
        _RandomTimer += Time.deltaTime;

        if (_RandomTimer >= _RandomDirectionTime)
        {
            _RandomTimer = 0f;
            _RandomDirection = GetRandomDirection();
        }

        _Direction = _RandomDirection;
    }

    private Vector3 GetRandomDirection()
    {
        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

        if (randomDir == Vector3.zero)
        {
            randomDir = transform.forward.WithNewY(0);
        }

        if (randomDir == Vector3.zero)
        {
            randomDir = Vector3.forward;
        }

        return randomDir.normalized;
    }

    private void CalculateSeparationForce(List<EnemyController> neighbours)
    {
        for (int i = 0; i < neighbours.Count; i++)
        {
            Vector3 dir = transform.position - neighbours[i].transform.position;
            dir = dir.WithNewY(0);

            float distance = dir.magnitude;

            if (distance > 0)
            {
                Vector3 away = dir.normalized;
                _FlockingForce += (away / distance) * _SeparationWeight;
            }
        }
    }

    private void ApplyAlligment(List<EnemyController> neighbours)
    {
        Vector3 neighboursForward = Vector3.zero;

        for (int i = 0; i < neighbours.Count; i++)
        {
            neighboursForward += neighbours[i].transform.forward.WithNewY(0);
        }

        neighboursForward /= neighbours.Count;

        if (neighboursForward != Vector3.zero)
        {
            neighboursForward.Normalize();
        }

        _FlockingForce += neighboursForward * _AlignmentWeight;
    }

    private void ApplyeCohesion(List<EnemyController> neighbours)
    {
        Vector3 averagePosition = Vector3.zero;

        for (int i = 0; i < neighbours.Count; i++)
        {
            averagePosition += neighbours[i].transform.position;
        }

        averagePosition /= neighbours.Count;

        Vector3 cohesionDir = averagePosition - transform.position;
        cohesionDir = cohesionDir.WithNewY(0);

        if (cohesionDir != Vector3.zero)
        {
            cohesionDir.Normalize();
        }

        _FlockingForce += cohesionDir * _CohesionWeight;
    }

    private List<EnemyController> GetNeighbours()
    {
        List<EnemyController> neighbours = new List<EnemyController>();

        Collider[] colliders;

        if (_BoidMask.value == 0)
        {
            int enemyMask = LayerMask.GetMask("Enemy");
            colliders = Physics.OverlapSphere(transform.position, _DetectionDistance, enemyMask);
        }
        else
        {
            colliders = Physics.OverlapSphere(transform.position, _DetectionDistance, _BoidMask);
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            EnemyController otherBoid = colliders[i].GetComponentInParent<EnemyController>();

            if (otherBoid == null)
            {
                continue;
            }

            if (otherBoid == this)
            {
                continue;
            }

            if (!neighbours.Contains(otherBoid))
            {
                neighbours.Add(otherBoid);
            }
        }

        return neighbours;
    }

    private void AvoidLeavingCircle()
    {
        if (!_UseCircularLimit)
        {
            return;
        }

        if (_Direction == Vector3.zero)
        {
            return;
        }

        Vector3 center = GetLimitCenter();

        Vector3 currentPos = transform.position.WithNewY(0);
        Vector3 nextPos = currentPos + _Direction.normalized * _Speed * Time.deltaTime;

        float currentDistance = Vector3.Distance(center, currentPos);
        float nextDistance = Vector3.Distance(center, nextPos);

        bool nearLimit = currentDistance >= _LimitRadius - _LimitMargin;
        bool leavingLimit = nextDistance > _LimitRadius;

        if (!nearLimit && !leavingLimit)
        {
            _LimitRandomTimer = 0f;
            return;
        }

        _LimitRandomTimer += Time.deltaTime;

        if (!leavingLimit && _LimitRandomTimer < _LimitRandomDirectionTime)
        {
            return;
        }

        _LimitRandomTimer = 0f;

        Vector3 dirToCenter = center - currentPos;
        dirToCenter = dirToCenter.WithNewY(0);

        if (dirToCenter == Vector3.zero)
        {
            return;
        }

        dirToCenter.Normalize();

        Vector3 randomDir = GetRandomDirection();

        if (Vector3.Dot(randomDir, dirToCenter) < 0)
        {
            randomDir = -randomDir;
        }

        _Direction = (dirToCenter + randomDir).normalized;

        _RandomDirection = _Direction;
        _RandomTimer = 0f;
    }

    private void ClampInsideCircle()
    {
        if (!_UseCircularLimit)
        {
            return;
        }

        Vector3 center = GetLimitCenter();

        Vector3 currentPos = transform.position.WithNewY(0);
        Vector3 fromCenter = currentPos - center;

        float distance = fromCenter.magnitude;

        if (distance <= _LimitRadius)
        {
            return;
        }

        Vector3 fixedPos = center + fromCenter.normalized * _LimitRadius;

        transform.position = new Vector3(
            fixedPos.x,
            transform.position.y,
            fixedPos.z
        );
    }

    private Vector3 GetLimitCenter()
    {
        if (_LimitCenter != null)
        {
            return _LimitCenter.position.WithNewY(0);
        }

        return Vector3.zero;
    }

    private void Move()
    {
        if (_Direction == Vector3.zero)
        {
            StopMove();
            return;
        }

        Vector3 finalDirection = _Direction;

        if (finalDirection.magnitude > 1)
        {
            finalDirection.Normalize();
        }

        Vector3 movement = finalDirection * _Speed * _CurrentSpeedMultiplier * Time.deltaTime;

        transform.position += movement;

        float animationSpeed = Mathf.Clamp01(finalDirection.magnitude);
        _MovementSpeedBlend = Mathf.Lerp(_MovementSpeedBlend, animationSpeed, Time.deltaTime * _Speed);

        if (_Animator != null)
        {
            _Animator.SetFloat("Speed", _MovementSpeedBlend);
        }
    }

    private void StopMove()
    {
        _MovementSpeedBlend = Mathf.Lerp(_MovementSpeedBlend, 0, Time.deltaTime * _Speed);

        if (_Animator != null)
        {
            _Animator.SetFloat("Speed", _MovementSpeedBlend);
        }
    }

    private void RotateTowardsDirection()
    {
        if (_Direction == Vector3.zero)
        {
            return;
        }

        Vector3 lookDir = _Direction.WithNewY(0);

        if (lookDir == Vector3.zero)
        {
            return;
        }

        _TargetRotation = Quaternion.LookRotation(lookDir.normalized);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            _TargetRotation,
            Time.deltaTime * _RotationSpeed
        );
    }

    private void CheckEatFood()
    {
        if (_FoodTarget != null)
        {
            Vector3 boidPos = transform.position.WithNewY(0);
            Vector3 foodPos = _FoodTargetPosition.WithNewY(0);

            float distance = Vector3.Distance(boidPos, foodPos);

            if (distance <= _EatDistance)
            {
                EatCurrentFood();
                return;
            }
        }

        Collider[] colliders;

        if (_FoodMask.value == 0)
        {
            colliders = Physics.OverlapSphere(
                transform.position,
                _EatDistance,
                ~0,
                QueryTriggerInteraction.Collide
            );
        }
        else
        {
            colliders = Physics.OverlapSphere(
                transform.position,
                _EatDistance,
                _FoodMask,
                QueryTriggerInteraction.Collide
            );
        }

        for (int i = 0; i < colliders.Length; i++)
        {
            Food food = colliders[i].GetComponentInParent<Food>();

            if (food == null)
            {
                continue;
            }

            Destroy(food.gameObject);

            if (_FoodTarget == food)
            {
                _FoodTarget = null;
            }

            return;
        }
    }

    private void EatCurrentFood()
    {
        if (_FoodTarget == null)
        {
            return;
        }

        Destroy(_FoodTarget.gameObject);
        _FoodTarget = null;
    }

    private void UpdateHunterVelocity()
    {
        if (_Hunter == null)
        {
            return;
        }

        if (Time.deltaTime > 0)
        {
            _HunterVelocity = (_Hunter.position - _LastHunterPosition) / Time.deltaTime;
        }

        _LastHunterPosition = _Hunter.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _FoodDetectionDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _HunterDetectionDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _DetectionDistance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _EatDistance);

        if (_UseCircularLimit)
        {
            Gizmos.color = Color.white;

            Vector3 center = Vector3.zero;

            if (_LimitCenter != null)
            {
                center = _LimitCenter.position;
            }

            Gizmos.DrawWireSphere(center, _LimitRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(center, _LimitRadius - _LimitMargin);
        }
    }
}