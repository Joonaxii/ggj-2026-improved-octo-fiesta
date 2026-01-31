using System;
using System.Collections.Generic;
using UnityEngine;

public class SpatialHashGrid<T> 
{
    public const int CHUNK_SIZE = 64;

    public int Width => _width;
    public int Height => _height;
    public Vector2 CellSize => _cellSize;
    public Vector2 Min => _min;
    public Vector2 Max => _max;

    public Vector2 Center
    {
        get => _center;
        set
        {
            if(_center == value) { return; }
            _center = value;
            UpdateBounds();
        }
    }

    private Cell[] _cells = new Cell[0];

    private Chunk[] _chunks = new Chunk[0];
    private T[] _entries = new T[0];
    private int _numChunks = 0;

    private Vector2 _cellSize = Vector2.one;
    private Vector2 _min = default;
    private Vector2 _max = default;
    private Vector2 _center = default;
    private int _width = 32;
    private int _height = 32;

    public void Setup(int width, int height, Vector2 cellSize)
    {
        _cellSize = cellSize;
        _width = Mathf.Clamp(width, 4, 256);
        _height = Mathf.Clamp(height, 4, 256);

        UpdateBounds();

        Array.Resize(ref _cells, _width * _height);
        Clear();
    }

    public void UpdateBounds()
    {
        _min.x = _center.x - (_width * _cellSize.x * 0.5f);
        _min.y = _center.y - (_height * _cellSize.y * 0.5f);
        _max.x = _min.x + _width * _cellSize.x;
        _max.y = _min.y + _height * _cellSize.y;
    }

    public int CountNumInCell(int cellIdx)
    {
        if(cellIdx >= _cells.Length || cellIdx < 0) { return 0; }

        int count = 0;
        ref var cell = ref _cells[cellIdx];
        int cur = cell.head;

        while(cur > -1)
        {
            ref var chunk = ref _chunks[cur];
            count += chunk.numInChunk;
            cur = chunk.next;
        }
        return count;
    }

    public void Push(Vector2 position, float radius, T entry)
        => Push(position - new Vector2(radius, radius), position + new Vector2(radius, radius), entry);

    public void Push(Vector2 min, Vector2 max, T entry)
    {
        if (Utils.AABBVSAABB(min, max, _min, _max))
        {
            int minX = Mathf.Clamp((Mathf.FloorToInt((min.x - _min.x) / _cellSize.x)), 0, _width - 1);
            int minY = Mathf.Clamp((Mathf.FloorToInt((min.y - _min.y) / _cellSize.y)), 0, _height - 1);
            int maxX = Mathf.Clamp((Mathf.FloorToInt((max.x - _min.x) / _cellSize.x)), 0, _width - 1);
            int maxY = Mathf.Clamp((Mathf.FloorToInt((max.y - _min.y) / _cellSize.y)), 0, _height - 1);

            for (int y = minY, yP = minY * _width; y <= maxY; y++, yP += _width)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    PushToCell(yP + x, entry);
                }
            }
        }
    }

    public void Fetch(Vector2 position, float radius, HashSet<T> buffer, Func<T, bool> isAllowed = null)
        => Fetch(position - new Vector2(radius, radius), position + new Vector2(radius, radius), buffer, isAllowed);
 
    public void Fetch(Vector2 min, Vector2 max, HashSet<T> buffer, Func<T, bool> isAllowed = null)
    {
        if (buffer == null) { return; }

        if (Utils.AABBVSAABB(min, max, _min, _max))
        {
            int minX = Mathf.Clamp((Mathf.FloorToInt((min.x - _min.x) / _cellSize.x)), 0, _width - 1);
            int minY = Mathf.Clamp((Mathf.FloorToInt((min.y - _min.y) / _cellSize.y)), 0, _height - 1);
            int maxX = Mathf.Clamp((Mathf.FloorToInt((max.x - _min.x) / _cellSize.x)), 0, _width - 1);
            int maxY = Mathf.Clamp((Mathf.FloorToInt((max.y - _min.y) / _cellSize.y)), 0, _height - 1);

            for (int y = minY, yP = minY * _width; y <= maxY; y++, yP += _width)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    ref var cell = ref _cells[yP + x];

                    int cur = cell.head;
                    while(cur > -1)
                    {
                        ref var chunk = ref _chunks[cur];
                        int offset = cur * CHUNK_SIZE;
                        for (int i = 0; i < chunk.numInChunk; i++)
                        {
                            var value = _entries[offset + i];
                            if(isAllowed == null || isAllowed.Invoke(value))
                            {
                                buffer.Add(value);
                            }
                        }
                        cur = chunk.next;
                    }
                }
            }
        }
    }

    private void PushToCell(int cellIdx, T entry)
    {
        ref var cell = ref _cells[cellIdx];
        int chunkIdx = cell.tail;
        if(chunkIdx < 0)
        {
            GetNewChunk(out chunkIdx);
            cell.head = cell.tail = chunkIdx;
        }
        else
        {
            ref var tail = ref _chunks[chunkIdx];
            if(tail.numInChunk >= CHUNK_SIZE)
            {
                GetNewChunk(out chunkIdx);
                cell.tail = chunkIdx;
                tail.next = chunkIdx;
            }
        }

        ref var chunk = ref _chunks[chunkIdx];

        int offset = chunkIdx * CHUNK_SIZE;
        _entries[offset + chunk.numInChunk] = entry;
        chunk.numInChunk++;
    }

    private ref Chunk GetNewChunk(out int chunkIdx)
    {
        chunkIdx = _numChunks++;
        if (_numChunks >= _chunks.Length)
        {
            int nCap = _chunks.Length;
            int tCap = _numChunks + 1;
            while (nCap < tCap)
            {
                nCap += Math.Max(_chunks.Length >> 1, 8);
            }

            Array.Resize(ref _chunks, nCap);
            Array.Resize(ref _entries, nCap * CHUNK_SIZE);
        }

        ref Chunk chunk = ref _chunks[chunkIdx];
        chunk.next = -1;
        chunk.numInChunk = 0;
        return ref chunk;
    }

    public void Clear()
    {
        Array.Fill(_entries, default);
        Array.Fill(_cells, Cell.Null);
        _numChunks = 0;
    }

    private struct Cell
    {
        public static Cell Null => new Cell() { head = -1, tail = -1 };

        public int head;
        public int tail;
    }

    private struct Chunk
    {
        public int numInChunk;
        public int next;
    }
}