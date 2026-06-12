using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _FoodPrefab;
    [SerializeField] private int _FoodAmount = 10;

    [Header("Circular Spawn Area")]
    [SerializeField] private float _SpawnRadius = 10f;
    [SerializeField] private float _FoodHeight = 0f;

    private void Start()
    {
        SpawnFood();
    }

    private void SpawnFood()
    {
        for (int i = 0; i < _FoodAmount; i++)
        {
            Vector3 randomPosition = GetRandomPosition();

            GameObject newFood = Instantiate(_FoodPrefab, randomPosition, Quaternion.identity);

            FixFoodColliderPosition(newFood, randomPosition);
        }
    }

    private Vector3 GetRandomPosition()
    {
        Vector2 randomCircle = Random.insideUnitCircle * _SpawnRadius;

        Vector3 position = transform.position;
        position.x += randomCircle.x;
        position.y = _FoodHeight;
        position.z += randomCircle.y;

        return position;
    }

    private void FixFoodColliderPosition(GameObject foodObject, Vector3 targetPosition)
    {
        Collider[] colliders = foodObject.GetComponentsInChildren<Collider>();

        if (colliders.Length <= 0)
        {
            return;
        }

        Bounds bounds = colliders[0].bounds;

        for (int i = 1; i < colliders.Length; i++)
        {
            bounds.Encapsulate(colliders[i].bounds);
        }

        Vector3 colliderCenter = bounds.center;

        Vector3 difference = targetPosition - colliderCenter;
        difference.y = 0f;

        foodObject.transform.position += difference;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Vector3 center = transform.position;
        center.y = _FoodHeight;

        int segments = 64;
        Vector3 previousPoint = center + new Vector3(_SpawnRadius, 0f, 0f);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;

            Vector3 nextPoint = center + new Vector3(
                Mathf.Cos(angle) * _SpawnRadius,
                0f,
                Mathf.Sin(angle) * _SpawnRadius
            );

            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }
}