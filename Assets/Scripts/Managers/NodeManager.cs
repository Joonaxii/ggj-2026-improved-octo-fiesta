using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NodeManager : Singleton<NodeManager>
{
    [SerializeField] private LayerMask _levelMask;
    private List<MovementPoint> _allPoints = new List<MovementPoint>();
    private List<MovementPoint> _punchTables = new List<MovementPoint>();
    private List<MovementPoint> _toilets = new List<MovementPoint>();
    private List<MovementPoint> _socializing = new List<MovementPoint>();

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if(child.TryGetComponent<MovementPoint>(out var point))
            {
                point.Index = _allPoints.Count;

                _allPoints.Add(point);

                switch (point.movementPointType)
                {
                    case MovementPointType.Socializing:
                        _socializing.Add(point);
                        break;
                    case MovementPointType.Bathroom:
                        _toilets.Add(point);
                        break;
                    case MovementPointType.PunchBowl:
                        _punchTables.Add(point);
                        break;
                }
            }
        }

    }

    public Vector3 GetRandomOffsetTarget(int target)
    {
        if (target <= 0) { return Vector3.zero; }

        var node = _allPoints[target];

        var offset = Random.insideUnitCircle * 2.5f;
        return node.Position + (Vector3)offset;
    }

    private static Queue<int> EVAL_QUEUE = new Queue<int>();
    private static int[] CAME_FROM = new int[32];

    public void BuildPath(Path path, int from, int to)
    {
        path.Clear();
        path.from = from;
        path.to = to;

        if(from < 0 || to < 0)
        {
            return;
        }

        EVAL_QUEUE.Clear();
        if(CAME_FROM.Length < _allPoints.Count)
        {
            Array.Resize(ref CAME_FROM, _allPoints.Count);
        }
        Array.Fill(CAME_FROM, -1);
        EVAL_QUEUE.Enqueue(from);
        CAME_FROM[from] = from;

        while(EVAL_QUEUE.Count > 0)
        {
            int cur = EVAL_QUEUE.Dequeue();
            if(cur == to) { break; }

            var node = _allPoints[cur];
            foreach (var item in node.NeighbourPoints)
            {
                if(CAME_FROM[item.Index] > -1)
                {
                    continue;
                }

                CAME_FROM[item.Index] = cur;
                EVAL_QUEUE.Enqueue(item.Index);
            }
        }

        int current = to;
        while(current != from)
        {
            path.nodes.Add(_allPoints[current]);
            current = CAME_FROM[current];
        }

        path.nodes.Add(_allPoints[from]);
        path.nodes.Reverse();
    }

    public int FindRandomToilet() => _toilets[Random.Range(0, _toilets.Count)].Index;
    public int FindRandomSocial() => _socializing[Random.Range(0, _socializing.Count)].Index;
    public int FindRandomPunch() => _punchTables[Random.Range(0, _punchTables.Count)].Index;

    public int FindClosestMoveTarget(Vector3 from)
    {
        float dist = float.MaxValue;
        int point = -1;

        for (int i = 0; i < _allPoints.Count; i++)
        {
            var item = _allPoints[i];

            float dst = Vector3.SqrMagnitude(from - item.Position);
            if(dst <= dist)
            {
                var ray = Physics2D.Raycast(from, item.Position, Mathf.Sqrt(dst), _levelMask);
                if (ray) 
                {
                    continue; 
                }

                dist = dst;
                point = item.Index;
            }
        }

        if(point > -1) { return point; }
        dist = float.MaxValue;

        for (int i = 0; i < _allPoints.Count; i++)
        {
            var item = _allPoints[i];
            float dst = Vector3.SqrMagnitude(from - item.Position);
            if (dst <= dist)
            {
                dist = dst;
                point = item.Index;
            }
        }
        return point;
    }
}