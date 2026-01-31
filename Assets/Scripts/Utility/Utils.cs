using UnityEngine;

public static class Utils
{
    public static bool CircleVSCircle(Vector2 p0, float r0, Vector2 p1, float r1) 
    {
        float rad = r0 + r1;
        return Vector2.SqrMagnitude(p1 - p0) <= rad * rad;
    }

    public static bool AABBVSAABB(Vector2 min0, Vector2 max0, Vector2 min1, Vector2 max1) 
    {
        return min0.x <= max1.x
            && min0.y <= max1.y
            && max0.x >= min1.x
            && max0.y >= min1.y;
    }
}