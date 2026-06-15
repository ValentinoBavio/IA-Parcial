using UnityEngine;

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
}
