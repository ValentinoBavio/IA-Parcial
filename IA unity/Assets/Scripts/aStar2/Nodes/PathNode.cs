/*using UnityEngine;

public class PathNode : MonoBehaviour
{
    public PathNode[] connectedNodes;

    private void OnDrawGizmos()
    {
        if (connectedNodes == null) return;

        Gizmos.color = Color.green;

        foreach (PathNode node in connectedNodes)
        {
            if (node == null) continue;

            Gizmos.DrawLine(transform.position, node.transform.position);
        }
    }
}*/

using UnityEngine;

public class PathNode : MonoBehaviour
{
    [Header("Conexiones disponibles")]
    public PathNode[] connectedNodes;

    [Header("Visualización")]
    [SerializeField] private Color connectionColor = Color.gray;
    [SerializeField] private Color nodeColor = Color.magenta;
    [SerializeField] private float nodeRadius = 0.12f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = nodeColor;
        Gizmos.DrawSphere(transform.position, nodeRadius);

        if (connectedNodes == null)
        {
            return;
        }

        Gizmos.color = connectionColor;

        foreach (PathNode node in connectedNodes)
        {
            if (node == null)
            {
                continue;
            }

            Gizmos.DrawLine(
                transform.position,
                node.transform.position
            );
        }
    }
}
