using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MovementPoint : MonoBehaviour
{
    public Vector3 Position => _transform != null ? _transform.position : Vector3.zero;

    public int Index
    {
        get => _index;
        set => _index = value;
    }

    public MovementPointType movementPointType;
    public MovementPoint[] NeighbourPoints = new MovementPoint[4];
    private Transform _transform;
    private int _index;

    private void Awake()
    {
        _transform = transform;
    }

    private void OnDrawGizmos()
    {
        if (NeighbourPoints == null) return;
        if (!GameManager.DebugFlags.HasFlag(GameManager.DebugView.Nodes)) { return; }

        Gizmos.color = GetTypeColor(movementPointType);
        
        Gizmos.DrawSphere(transform.position, 0.1f);
        
        foreach (var point in NeighbourPoints)
        {
            if (point == null) continue;
            Gizmos.DrawLine(transform.position, point.transform.position);
        }
    }

    public static Color GetTypeColor(MovementPointType type)
    {
        switch (type)
        {
            default:
                return Color.gray;
            case MovementPointType.Movement:
                return Color.green;
            case MovementPointType.Socializing:
                return Color.cyan;
            case MovementPointType.PunchBowl:
                return Color.red;
            case MovementPointType.Bathroom:
                return Color.yellow;
        }
    }
}

public enum MovementPointType
{
    Unset = -1,
    Movement = 0,
    Socializing = 1,
    PunchBowl = 2,
    Bathroom = 3,
}

