using UnityEngine;

public class EnemyAggroCheck : MonoBehaviour
{
   public GameObject PlayerTarget { get; set; }

    private Enemy enemy;

    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float viewAngle = 90f;

    private void Awake()
    {
        PlayerTarget = GameObject.FindGameObjectWithTag("Player");
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject != PlayerTarget)
            return;

        Vector2 origin = transform.position;
        Vector2 target = PlayerTarget.transform.position;

        Vector2 direction = (target - origin).normalized;
        float distance = Vector2.Distance(origin, target);

        float angle = Vector2.Angle(enemy.LastMoveDirection, direction);

        if (angle > viewAngle / 2f)
        {
            enemy.SetAggroStatus(false);
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, obstacleLayer);

        Debug.DrawRay(origin, direction * distance, hit.collider == null ? Color.green : Color.red);
        
        enemy.SetAggroStatus(hit.collider == null);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerTarget)
            enemy.SetAggroStatus(false);
    }
}
