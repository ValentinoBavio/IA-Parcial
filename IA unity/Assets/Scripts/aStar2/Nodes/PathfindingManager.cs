using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager Instance;

    private void Awake()
    {
        Instance = this;
    }




    public PathNode GetClosestNode(Vector2 position)
    {
        PathNode[] allNodes =
            FindObjectsOfType<PathNode>();

        PathNode closest = null;

        float minDistance = Mathf.Infinity;

        foreach (PathNode node in allNodes)
        {
            float distance =
                Vector2.Distance(
                    position,
                    node.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closest = node;
            }
        }

        return closest;
    }


    public List<PathNode> FindPath(PathNode startNode,PathNode targetNode)
    {
        List<PathNode> openSet = new();
        HashSet<PathNode> closedSet = new();

        Dictionary<PathNode, PathNode> cameFrom = new();

        Dictionary<PathNode, float> gScore = new();
        Dictionary<PathNode, float> fScore = new();

        openSet.Add(startNode);

        gScore[startNode] = 0;

        fScore[startNode] =
            Heuristic(startNode, targetNode);

        while (openSet.Count > 0)
        {
            PathNode current =
                openSet.OrderBy(n => fScore.GetValueOrDefault(n, Mathf.Infinity))
                .First();

            if (current == targetNode)
            {
                return ReconstructPath(
                    cameFrom,
                    current);
            }

            openSet.Remove(current);

            closedSet.Add(current);

            foreach (PathNode neighbor in current.connectedNodes)
            {
                if (closedSet.Contains(neighbor))
                    continue;

                float tentativeG =
                    gScore[current]
                    + Vector2.Distance(
                        current.transform.position,
                        neighbor.transform.position);

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentativeG >=
                    gScore.GetValueOrDefault(neighbor, Mathf.Infinity))
                {
                    continue;
                }

                cameFrom[neighbor] = current;

                gScore[neighbor] = tentativeG;

                fScore[neighbor] =
                    tentativeG
                    + Heuristic(neighbor, targetNode);
            }
        }

        return null;
    }

    private float Heuristic(
        PathNode a,
        PathNode b)
    {
        return Vector2.Distance(
            a.transform.position,
            b.transform.position);
    }

    private List<PathNode> ReconstructPath(
        Dictionary<PathNode, PathNode> cameFrom,
        PathNode current)
    {
        List<PathNode> totalPath = new();

        totalPath.Add(current);

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];

            totalPath.Insert(0, current);
        }

        return totalPath;
    }
}
