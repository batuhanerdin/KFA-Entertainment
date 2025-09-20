using UnityEngine;

public class PathSystem : MonoBehaviour
{
    [Header("Baþlangýç Noktalarý (Her yolun ilk node'u)")]
    public PathNode[] startNodes;

    public PathNode GetPath(int index)
    {
        if (index < 0 || index >= startNodes.Length) return null;
        return startNodes[index];
    }
}
