using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Path
{
    public const float MIN_DIST = 0.333f;
    public const float MIN_SQR_DIST = MIN_DIST * MIN_DIST;

    public MovementPoint Current => current >= nodes.Count ? null : nodes[current];

    public List<MovementPoint> nodes = new List<MovementPoint>();
    public int from;
    public int to;
    public int current;

    public void Clear()
    {
        nodes.Clear();
        from = -1;
        to = -1;
        current = 0;
    }

    public bool IsAtTarget(Vector3 position, out MovementPoint next)
    {
        next = null;
        if(current >= nodes.Count) { return false; }

        var cPoint = nodes[current];

        float dst = Vector3.SqrMagnitude(cPoint.Position - position);
        if (dst <= MIN_SQR_DIST)
        {
            ++current;
            next = current >= nodes.Count ? null : nodes[current];
            return true;
        }
        return false;
    }
}