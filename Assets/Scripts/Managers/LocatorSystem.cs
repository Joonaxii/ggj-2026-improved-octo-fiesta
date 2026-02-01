using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocatorSystem : Singleton<LocatorSystem>
{
    [SerializeField]
    private GridSettings _gridSettings = new GridSettings()
    {
        cellSize = new Vector2(1, 1),
        width = 32,
        height = 32,
    };

    private HashSet<ILocatable> _locatables = new HashSet<ILocatable>();
    private HashSet<ILocatable> _tempLut = new HashSet<ILocatable>();

    private SpatialHashGrid<ILocatable> _grid = new SpatialHashGrid<ILocatable>();

    protected override void Awake()
    {
        base.Awake();
        _grid.Setup(_gridSettings.width, _gridSettings.height, _gridSettings.cellSize);
    }

    public void Register(ILocatable locatable)
        => _locatables.Add(locatable);
    public void Unregister(ILocatable locatable)
        => _locatables.Remove(locatable);

    public void Tick()
    {
        _grid.Center = transform.position;
        _grid.Clear();

        foreach (var locatable in _locatables)
        {
            if(!locatable.IsActive) { continue; }
            _grid.Push(locatable.Position, locatable.Radius, locatable);
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (!GameManager.DebugFlags.HasFlag(GameManager.DebugView.LocatorGrid)) { return; }

        Vector3 pos = transform.position;
        _grid.Center = pos;

        float max = 0;
        int reso = _grid.Width * _grid.Height;
        Span<int> nums = stackalloc int[reso];
        for (int i = 0; i < reso; i++)
        {
            int num = _grid.CountNumInCell(i);
            max = Mathf.Max(num, max);
            nums[i] = num;
        }

        float sX = _grid.Min.x + _grid.CellSize.x * 0.5f;
        float sY = _grid.Min.y + _grid.CellSize.y * 0.5f;

        float cX = sX;
        float cY = sY;

        for (int y = 0; y < _grid.Height; y++)
        {
            cX = sX;
            for (int x = 0; x < _grid.Width; x++)
            { 
                int idx = y * _grid.Width + x;
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(new Vector3(cX, cY), _grid.CellSize);

                float alpha = (max < 1 ? 0 : nums[idx] / max) * 0.85f;
                if(alpha > 0)
                {
                    Gizmos.color = new Color(1.0f, 1.0f, 0.0f, alpha);
                    Gizmos.DrawCube(new Vector3(cX, cY), _grid.CellSize);
                }
                cX += _grid.CellSize.x;
            }
            cY += _grid.CellSize.y;
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(_grid.Min + (_grid.Max - _grid.Min) * 0.5f, _grid.Max - _grid.Min);
    }

    public void OnValidate()
    {
        if(_grid.CellSize != _gridSettings.cellSize || _grid.Width != _gridSettings.width || _grid.Height != _gridSettings.height)
        {
            _grid.Setup(_gridSettings.width, _gridSettings.height, _gridSettings.cellSize);
        }
    }

    public static bool IsAllowed(ObjectKindMask mask, ObjectKind kind)
        => ((int)mask & (1 << (int)kind)) != 0;


    private static ObjectKindMask CHECK_MASK;
    private static Func<ILocatable, bool> VALIDITY_CHECK = CheckValidity;
    private struct FetchEntry
    {
        public ILocatable locatable;
        public float distance;
    }
    private static FetchEntry[] TEMP_ENTRIES = new FetchEntry[64];

    private static bool CheckValidity(ILocatable locatable)
    {
        return locatable != null && IsAllowed(CHECK_MASK, locatable.Kind);
    }

    private class FEntryComparer : IComparer<FetchEntry>
    {
        public static FEntryComparer Default { get; } = new FEntryComparer();
        public int Compare(FetchEntry x, FetchEntry y) => x.distance.CompareTo(y.distance);
    }

    public void Fetch(Vector3 position, float radius, List<ILocatable> results, ObjectKindMask allowed = ObjectKindMask.All)
    {
        CHECK_MASK = allowed;
        _tempLut.Clear();
        _grid.Fetch(position, radius, _tempLut, VALIDITY_CHECK);

        if(TEMP_ENTRIES.Length < _tempLut.Count)
        {
            Array.Resize(ref TEMP_ENTRIES, _tempLut.Count + (_tempLut.Count >> 1));
        }

        int hit = 0;

        Vector2 p2D = position;
        foreach(var entry in _tempLut)
        {
            if(Utils.CircleVSCircle(p2D, radius, entry.Position, entry.Radius))
            {
                float sqrDist = Vector2.SqrMagnitude(entry.Position - position);
                TEMP_ENTRIES[hit++] = new FetchEntry()
                {
                    distance = sqrDist,
                    locatable = entry
                };
            }
        }

        Array.Sort(TEMP_ENTRIES, 0, hit, FEntryComparer.Default);
        for (int i = 0; i < hit; i++)
        {
            results.Add(TEMP_ENTRIES[i].locatable);
        }
    }

    public int Fetch(Vector3 position, float radius, Span<ILocatable> results, ObjectKindMask allowed = ObjectKindMask.All)
    {
        CHECK_MASK = allowed;
        _tempLut.Clear();
        _grid.Fetch(position, radius, _tempLut, VALIDITY_CHECK);

        if(TEMP_ENTRIES.Length < _tempLut.Count)
        {
            Array.Resize(ref TEMP_ENTRIES, _tempLut.Count + (_tempLut.Count >> 1));
        }

        int hit = 0;

        Vector2 p2D = position;
        foreach(var entry in _tempLut)
        {
            if(Utils.CircleVSCircle(p2D, radius, entry.Position, entry.Radius))
            {
                float sqrDist = Vector2.SqrMagnitude(entry.Position - position);
                TEMP_ENTRIES[hit++] = new FetchEntry()
                {
                    distance = sqrDist,
                    locatable = entry
                };
            }
        }

        Array.Sort(TEMP_ENTRIES, 0, hit, FEntryComparer.Default);

        int len = Mathf.Min(results.Length, hit);
        for (int i = 0; i < len; i++)
        {
            results[i] = TEMP_ENTRIES[i].locatable;
        }
        return len;
    }

    [System.Serializable]
    public struct GridSettings
    {
        [Range(1, 64)] public int width;
        [Range(1, 64)] public int height;
        public Vector2 cellSize;
    }
}
