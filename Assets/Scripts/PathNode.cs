using UnityEngine;
using System.Collections.Generic;

public class PathNode : MonoBehaviour
{
    [Tooltip("Bu node'dan sonra gidilebilecek yollar")]
    public List<PathNode> nextNodes = new List<PathNode>();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.2f);

        if (nextNodes != null)
        {
            Gizmos.color = Color.green;
            foreach (var node in nextNodes)
            {
                if (node != null)
                    Gizmos.DrawLine(transform.position, node.transform.position);
            }
        }
    }
}
