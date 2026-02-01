
using UnityEngine;
using UnityEngine.UIElements;

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

    public static Vector2 Rotate(Vector2 vec, float rads)
    {
        float cos = Mathf.Cos(rads);
        float sin = Mathf.Sin(rads);
        return new Vector2(vec.x * cos + vec.y * -sin, vec.x * sin + vec.y * cos);
    }


    public static Vector2 Perpendicular(Vector2 vec)
    {
        return new Vector2(vec.y, -vec.x);
    }

    public struct CollisionContact
    {
        public Vector2 point;
        public Vector2 normal;
        public float depth;
        public bool wasHit;
    }

    public static void CircleVSCircle(Vector2 p0, float r0, Vector2 p1, float r1, ref CollisionContact contact)
    {
        Vector2 dir = p1 - p0;
        float sqrDst = Vector2.SqrMagnitude(dir);
        float rad = r0 + r1;

        contact = default;
        if (sqrDst > rad * rad)
        {
            return;
        }

        contact.wasHit = true;
        float dist = Mathf.Sqrt(sqrDst);
        if (dist == 0.0f)
        {
            contact.normal = new Vector2(1, 0);
            contact.depth = rad;
            contact.point = p0 + contact.normal * r0;
        }
        else
        {

            contact.normal = dir / dist;
            contact.depth = rad - dist;
            contact.point = p0 + contact.normal * r0;
        }
    }
}