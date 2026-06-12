using UnityEngine;

public class EnemyStateController : MonoBehaviour
{
    public FSM<string> _fsm;

    [Header("Energy")]
    [Tooltip("ENERGIA = SEGUNDOS QUE AGUANTA")]
    public float maxEnergy = 60f;
    public float currentEnergy;

    [Tooltip("CUANTA ENERGIA CONSUME POR SEGUNDO")]
    public float energyDrain = 1f;

    [Tooltip("Cuantos segundos tarda en recomponerse")]
    public float restDuration = 10f;

    [Header("Movement")]
    public float speed = 3f;
    public float rotationSpeed = 10f;
    public float chaseStopDistance = 1.5f;
    public float chaseResumeDistance = 2.2f;
    public Transform[] waypoints;
    public int currentWaypointIndex;

    [Header("Detection")]
    public float visionRange = 5f;
    public LayerMask boidMask;
    public Transform target;

    [Header("Pursuit")]
    public float predictionTime = 0.5f;
    public float maxPredictionTime = 1f;

    [Header("Animation")]
    public float animationSmoothTime = 0.2f;

    public Animator _Animator;

    [HideInInspector]
    public float speedAnimacion;

    private void Awake()
    {
        _Animator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        //Entendemos que no habia que usar rigidbody, 
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("PLUMBA");
            Destroy(other.gameObject);
        }
    }

    private void Start()
    {
        currentEnergy = maxEnergy;

        var idle = new IdleState<string>();
        var patrol = new PatrolState<string>();
        var chase = new ChaseState<string>();

        idle.SetController(this);
        patrol.SetController(this);
        chase.SetController(this);

        idle.AddTransition("patrol", patrol);

        patrol.AddTransition("chase", chase);
        patrol.AddTransition("idle", idle);

        chase.AddTransition("patrol", patrol);
        chase.AddTransition("idle", idle);

        _fsm = new FSM<string>(patrol);
    }

    private void Update()
    {
        _fsm.OnUpdate();

        if (_Animator != null)
        {
            _Animator.SetFloat("Speed", speedAnimacion, animationSmoothTime, Time.deltaTime);
        }
    }

    public void SearchTarget()
    {
        Collider[] boids = Physics.OverlapSphere(transform.position, visionRange, boidMask, QueryTriggerInteraction.Collide);

        if (boids.Length <= 0)
        {
            target = null;
            return;
        }

        float minDistance = Mathf.Infinity;
        Transform closestBoid = null;

        for (int i = 0; i < boids.Length; i++)
        {
            EnemyController boid = boids[i].GetComponentInParent<EnemyController>();

            if (boid == null)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, boid.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestBoid = boid.transform;
            }
        }

        target = closestBoid;
    }

    [HideInInspector] public Vector3 predictedPosition;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, target.position);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, chaseStopDistance);

        if (waypoints == null || waypoints.Length == 0)
        {
            return;
        }

        Gizmos.color = Color.cyan;

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null)
            {
                continue;
            }

            Gizmos.DrawSphere(waypoints[i].position, 0.2f);

            int next = (i + 1) % waypoints.Length;

            if (waypoints[next] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[next].position);
            }
        }

        if (waypoints.Length > currentWaypointIndex)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(waypoints[currentWaypointIndex].position, 0.3f);
        }

        //Pursuit
        if (target != null && predictedPosition != Vector3.zero)
        {

            Gizmos.color = new Color(1f, 0f, 1f);
            Gizmos.DrawLine(transform.position, predictedPosition);


            Gizmos.DrawSphere(predictedPosition, 0.25f);


            Gizmos.color = Color.white;
            Gizmos.DrawLine(target.position, predictedPosition);
        }



    }
}