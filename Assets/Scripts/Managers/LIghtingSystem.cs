using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightingSystem : Singleton<LightingSystem>
{
    [SerializeField] private Material _material;
    [SerializeField] private LayerMask _occluders;
    [SerializeField, Range(4, 64)] private int _numLightSegments = 8;

    [SerializeField] private float _maxDistance = 2.0f;
    [SerializeField] private AnimationCurve _strengtCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [SerializeField] private Gradient _lightColor;
    [SerializeField] private AnimationCurve _falloff = AnimationCurve.Linear(0, 0, 1, 1);

    private Mesh _mesh;
    private Vector3[] _vertPositions = new Vector3[256];
    private Vector2[] _uvs = new Vector2[256];
    private Color[] _vertColors = new Color[256];
    private int[] _indexBuffer = new int[256 * 3];
    private int _indices;
    private int _verts;

    private static Ray[] TEMP = new Ray[256];

    protected override void Awake()
    {
        base.Awake();
    }

    private HashSet<ILightSource> _lights = new HashSet<ILightSource>();

    public void Register(ILightSource source)
        => _lights.Add(source);

    public void Unregister(ILightSource source)
        => _lights.Remove(source);

    public void Tick(float progress)
    {
        _verts = 0;
        _indices = 0;

        foreach(var light in _lights)
        {
            HandleLight(light);
        }

        if(_mesh == null)
        {
            _mesh = new Mesh();
        }

        _mesh.Clear();
        _mesh.SetVertices(_vertPositions, 0, _verts);
        _mesh.SetColors(_vertColors, 0, _verts);
        _mesh.SetUVs(0, _uvs, 0, _verts);
        _mesh.SetIndices(_indexBuffer, 0, _indices, MeshTopology.Triangles, 0);

        Graphics.DrawMesh(_mesh, transform.localToWorldMatrix, _material, gameObject.layer);
    }

    public void HandleLight(ILightSource source)
    {
        if(source == null || !source.IsActive || source.NumRays < 2) { return; }

        int nRays = Mathf.Min(source.NumRays, 256);
        Span<Ray> hits = TEMP.AsSpan(0, nRays);

        float arc = (-source.Arc * 0.5f) * Mathf.Deg2Rad;
        float perI = (source.Arc / nRays) * Mathf.Deg2Rad;

        Vector2 sPos = source.Origin - Utils.Perpendicular(source.Direction) * 0.5f * source.Width;
        Vector2 shift = Utils.Perpendicular(source.Direction) * (source.Width / nRays);
        for (int i = 0; i < nRays; i++)
        {
            Vector2 dir = Utils.Rotate(source.Direction, arc);
            var hit = Physics2D.Raycast(sPos, dir, _maxDistance, _occluders);
            hits[i] = default;

            ref var hitOut = ref hits[i];
            hitOut.length = _maxDistance;
            hitOut.origin = source.Origin;
            hitOut.direction = dir;
            hitOut.hitDist = _maxDistance;

            if (hit)
            {
                hitOut.hitPoint = hit.point;
                hitOut.hitObject = hit.collider.gameObject;
                hitOut.hitDist = Vector2.Distance(hit.point, source.Origin);
            }

            arc += perI;
            sPos += shift;
        }

        int verts = (_numLightSegments + 1) * nRays;
        int nVert = _verts + verts;

        if(_vertPositions.Length < nVert)
        {
            Array.Resize(ref _vertPositions, nVert + (_vertPositions.Length >> 1));
            Array.Resize(ref _vertColors, _vertPositions.Length);
            Array.Resize(ref _uvs, _vertPositions.Length);
            Array.Resize(ref _indexBuffer, _vertPositions.Length * 3);
        }

        Color color = Color.white;
        float alpha = 1;

        int curVert = _verts;
        int rowStride = _numLightSegments + 1;
        for (int i = 0; i < hits.Length; i++)
        {
            alpha = 1;
            ref var hit = ref hits[i];
            for (int s = 0; s <= _numLightSegments; s++)
            {
                float tRad = s / (float)_numLightSegments;
                float dist = tRad * _maxDistance;
                Vector2 pos = hit.origin + hit.direction * dist;

                alpha = Mathf.Min(alpha, _falloff.Evaluate(tRad));
                if (dist >= hit.hitDist && s > 0)
                {
                    float pDist = ((s - 1.0f) / _numLightSegments) * _maxDistance;
                    Vector2 prevPos = hit.origin + hit.direction * pDist;

                    float lerp = (hit.hitDist - pDist) / (dist - pDist);
                    pos = Vector2.Lerp(prevPos, pos, lerp);
                    alpha = 0;
                }
                color.a = alpha;

                _vertPositions[curVert] = pos;
                _vertColors[curVert] = color;
                curVert++;

                if(s < _numLightSegments)
                {
                    int i0 = _verts + i * rowStride + s;
                    int i1 = i0 + 1;
                    int i2 = i0 + rowStride;
                    int i3 = i2 + 1;

                    _indexBuffer[_indices++] = i0;
                    _indexBuffer[_indices++] = i2;
                    _indexBuffer[_indices++] = i1;

                    _indexBuffer[_indices++] = i1;
                    _indexBuffer[_indices++] = i2;
                    _indexBuffer[_indices++] = i3;
                }
            }
        }
        _verts = curVert;
    }

    private struct Ray
    {
        public GameObject hitObject;
        public Vector2 hitPoint;
        public Vector2 origin;
        public Vector2 direction;
        public float hitDist;
        public float length;
    }
}
