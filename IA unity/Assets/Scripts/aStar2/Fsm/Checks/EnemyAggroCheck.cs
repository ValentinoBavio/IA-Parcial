using UnityEngine;

public class EnemyAggroCheck : MonoBehaviour
{
   public GameObject PlayerTarget { get; set; }

    private Enemy enemy;

    [SerializeField]
    private LayerMask obstacleLayer;

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

        // Raycast hacia el jugador
        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            direction,
            distance,
            obstacleLayer
        );

        // Debug visual
        Debug.DrawRay(origin, direction * distance, Color.red);

        // Si NO chocó contra una pared
        if (hit.collider == null)
        {
            enemy.SetAggroStatus(true);
        }
        else
        {
            enemy.SetAggroStatus(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerTarget)
        {
            enemy.SetAggroStatus(false);
        }
    }
}
