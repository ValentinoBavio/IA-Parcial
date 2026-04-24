using UnityEngine;

public class EnemyStateController : MonoBehaviour
{

    public FSM<string> _fsm;

    [Header("Energy")]
    [Tooltip("ENERGIA = SEGUNDOS QUE AGUANTA")]
    public float maxEnergy = 60f;
    public float currentEnergy;
    [Tooltip("CUANTA ENERGIA CONSUME POR SEGUNDO (dejemoslo en 1 es mas facil para calcular otras cosas)")]
    public float energyDrain = 1f;
    [Tooltip("Cuantos segundos tarda en recomponerse")]
    public float restDuration = 10f;

    [Header("Movement")]
    public float speed = 3f;
    public float rotationSpeed = 10f;
    public Transform[] waypoints;
    public int currentWaypointIndex;


    [Header("Detection")]
    public float visionRange = 5f;
    public Transform target;


    public Animator _Animator;

    [HideInInspector]
    public float speedAnimacion;



    private void Awake()
    {
        _Animator = GetComponent<Animator>();
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

        _fsm = new FSM<string>(idle);
    }

    private void Update()
    {
        _fsm.OnUpdate();

        _Animator.SetFloat("Speed", speedAnimacion);
    }

    private void OnDrawGizmosSelected()
    {
        //if (!Application.isPlaying) return;

        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, target.position);
        }

        
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.cyan;

        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;

            
            Gizmos.DrawSphere(waypoints[i].position, 0.2f);

            
            int next = (i + 1) % waypoints.Length;
            if (waypoints[next] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[next].position);
            }
        }

        
        if (/*Application.isPlaying && */waypoints.Length > currentWaypointIndex)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(waypoints[currentWaypointIndex].position, 0.3f);
        }
    }
}