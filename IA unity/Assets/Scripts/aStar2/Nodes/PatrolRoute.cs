using UnityEngine;

public class PatrolRoute : MonoBehaviour
{
    [Header("Ruta de patrullaje")]
    [Tooltip("Colocá los nodos en el orden en que serán recorridos.")]
    public PathNode[] patrolNodes;

    [Header("Visualización")]
    [SerializeField] private Color routeColor = Color.red;

    private const float LineHeight = 0.15f;
    private const float RouteNodeRadius = 0.16f;

    private void OnDrawGizmos()
    {
        if (patrolNodes == null || patrolNodes.Length == 0)
        {
            return;
        }

        Gizmos.color = routeColor;

        // Dibuja los nodos de la ruta.
        foreach (PathNode node in patrolNodes)
        {
            if (node == null)
            {
                continue;
            }

            Gizmos.DrawSphere(
                GetRaisedPosition(node),
                RouteNodeRadius
            );
        }

        // Dibuja las líneas entre los nodos.
        for (int i = 0; i < patrolNodes.Length - 1; i++)
        {
            PathNode currentNode = patrolNodes[i];
            PathNode nextNode = patrolNodes[i + 1];

            if (currentNode == null || nextNode == null)
            {
                continue;
            }

            Gizmos.DrawLine(
                GetRaisedPosition(currentNode),
                GetRaisedPosition(nextNode)
            );
        }

        // Todas las rutas son cerradas:
        // conecta automáticamente el último nodo con el primero.
        if (patrolNodes.Length > 1)
        {
            PathNode lastNode = patrolNodes[patrolNodes.Length - 1];
            PathNode firstNode = patrolNodes[0];

            if (lastNode != null && firstNode != null)
            {
                Gizmos.DrawLine(
                    GetRaisedPosition(lastNode),
                    GetRaisedPosition(firstNode)
                );
            }
        }
    }

    private Vector3 GetRaisedPosition(PathNode node)
    {
        return node.transform.position + Vector3.up * LineHeight;
    }
}