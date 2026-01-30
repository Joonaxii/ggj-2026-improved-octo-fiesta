using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyPoint : MonoBehaviour
{
    public PartyPointType PartyPointType;
    public PartyPoint[] NeighbourPoints = new PartyPoint[4];

    private void OnDrawGizmos()
    {
        if (NeighbourPoints == null) return;
        Gizmos.color = GetTypeColor(PartyPointType);
        
        Gizmos.DrawSphere(transform.position, 0.1f);
        
        foreach (var point in NeighbourPoints)
        {
            if (point == null) continue;
            Gizmos.DrawLine(transform.position, point.transform.position);
        }
    }

    public static Color GetTypeColor(PartyPointType type)
    {
        switch (type)
        {
            default:
                return Color.gray;
            case PartyPointType.Movement:
                return Color.green;
            case PartyPointType.Socializing:
                return Color.cyan;
            case PartyPointType.PunchBowl:
                return Color.red;
            case PartyPointType.Bathroom:
                return Color.yellow;
        }
    }
}

public enum PartyPointType
{
    Unset = -1,
    Movement = 0,
    Socializing = 1,
    PunchBowl = 2,
    Bathroom = 3,
}

