using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class LightingManager : Singleton<LightingManager>
{
    [SerializeField] private Material _material;
    [SerializeField] private LayerMask _occluders;
    [SerializeField, Range(0, 64)] private int _numLightSegments = 8;
    [SerializeField, Range(0, 360)] private float _sunAngle = 0;
    
    [SerializeField] private AnimationCurve _falloff = AnimationCurve.Linear(1, 1, 0, 0);
    [SerializeField, Range(0.1f, 1.0f)] private float _downscaling = 0.25f;  
    [SerializeField] private Gradient _skyColorOverTime;  
    [SerializeField] private AnimationCurve _distanceOverTime = AnimationCurve.Linear(1, 1, 1,1);  
    
    private Mesh _mesh;
    private Vector3[] _vertPositions = new Vector3[256];
    private Vector2[] _uvs = new Vector2[256];
    private Color[] _vertColors = new Color[256];
    private int[] _indexBuffer = new int[256 * 3];
    private int _indices;
    private int _verts;

    private LightPostProcessing _lightPP;
    private Vector2Int _resolution = default;
    private float _curDownscaling;
    private RenderTexture _texture;
    private Camera _lightCam;
    private SkyBG _skyBG;
    private bool _rtInit;
    private int _drawLayer;
    private float _distanceMult = 1.0f;

    private static Ray[] TEMP = new Ray[256];

    protected override void Awake()
    {
        _rtInit = false;
        _drawLayer = LayerMask.NameToLayer("Lights");
        base.Awake();
        _lightPP = Camera.main.GetComponent<LightPostProcessing>();

        if (_skyBG == null)
        {
            _skyBG = GameObject.FindObjectOfType<SkyBG>();
        }
    }

    private HashSet<ILightSource> _lights = new HashSet<ILightSource>();

    public void Register(ILightSource source)
        => _lights.Add(source);

    public void Unregister(ILightSource source)
        => _lights.Remove(source);


    private Color _skyColor;
    private Color _skyColorVelocity;

    public void Tick(float progress)
    {
        _sunAngle = Mathf.LerpUnclamped(-90.0f, 180.0f, progress);
        _distanceMult = _distanceOverTime.Evaluate(Mathf.Repeat(_sunAngle, 360.0f) / 360.0f);

        var targetColor = GetColor();
        // Kind of ugly but should work.
        _skyColor = targetColor;

        if (_skyBG != null)
        {
            _skyBG.SetColor(_skyColor);
        }

        Vector2Int cReso = new Vector2Int(Screen.width, Screen.height);
        if (cReso != _resolution || _texture == null || !_rtInit || _curDownscaling != _downscaling)
        {
            if (_lightCam == null)
            {
                _lightCam = GameObject.FindWithTag("LightCamera").GetComponent<Camera>();
            }

            _curDownscaling = _downscaling;
            float funcScaling = _downscaling / (cReso.y / 1080.0f);

            _resolution = cReso;
            cReso.x = Mathf.CeilToInt(_resolution.x * funcScaling);
            cReso.y = Mathf.CeilToInt(_resolution.y * funcScaling);

            if (_texture != null)
            {
                _texture.Release();
                GameObject.Destroy(_texture);
            }
            _texture = new RenderTexture(cReso.x, cReso.y, GraphicsFormat.R8G8B8A8_UNorm, GraphicsFormat.None);
            _texture.filterMode =  FilterMode.Point;
            
            _lightCam.targetTexture = _texture;
            _lightPP.Texture = _texture;

            _rtInit = true;
        }
        
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

        Graphics.DrawMesh(_mesh, Matrix4x4.identity, _material, _drawLayer);
    }

    private Color GetColor()
    {
        float cur = Mathf.Repeat(_sunAngle, 360.0f) / 360.0f;
        Color skyColor = _skyColorOverTime.Evaluate(cur);
        return GetColor(skyColor);
    }

    private static Color GetColor(Color baseColor)
    {
        var mngr = GameManager.Instance;
        var rgb = mngr.RainbowColor;
        if (mngr.State == GameManager.GameState.GoingToWin ||
           mngr.State == GameManager.GameState.AtWinScreen)
        {
            return new Color(1.0f, 0.0f, 0.0f);
        }
        else if(mngr.State == GameManager.GameState.GoingToLose ||
           mngr.State == GameManager.GameState.AtLoseScreen)
        {
            return baseColor;
        }
        else if (mngr.RainbowMode)
        {
            baseColor.r = rgb.r;
            baseColor.g = rgb.g;
            baseColor.b = rgb.b;
        }
        return baseColor;
    }

    private static float GetSunAngle(float current)
    {
        var mngr = GameManager.Instance;
        var rgb = mngr.RainbowColor;

        if (mngr.State == GameManager.GameState.GoingToWin || 
            mngr.State == GameManager.GameState.AtWinScreen ||
            mngr.State == GameManager.GameState.GoingToLose ||
            mngr.State == GameManager.GameState.AtLoseScreen)
        {
            return 180.0f;
        }
        return current;
    }

    public void HandleLight(ILightSource source)
    {
        if(source == null || !source.IsActive || source.NumRays < 2) { return; }
        float sunAngle =  GetSunAngle(Mathf.Repeat(_sunAngle, 360));
        
        int numRays = Mathf.Min(source.NumRays, 256);
        Span<Ray> hits = TEMP.AsSpan(0, numRays);

        float add = 0;
        if (source.IsSun)
        {
            Vector2 sunDir = Utils.Rotate(source.Invert ? Vector2.down : Vector2.up, sunAngle * Mathf.Deg2Rad);
            add = Vector2.SignedAngle(source.Direction, sunDir);
        }

        add *= Mathf.Deg2Rad;
        
        float progress = sunAngle / 360.0f;

        float arc = (source.Arc * 0.5f) * Mathf.Deg2Rad + add; 
        float perI = (-source.Arc / (numRays - 1.0f)) * Mathf.Deg2Rad;

        Vector2 sPos = source.Origin - Utils.Perpendicular(source.Direction) * (0.5f * source.Width);
        Vector2 shift = Utils.Perpendicular(source.Direction) * (source.Width / (numRays - 1.0f));
        
        float distance = source.GetDistance(progress) * (source.IsSun ? _distanceMult : 1.0f);
        Color tint = source.GetTint(progress);

        if (source.IsSun)
        {
            tint.r = _skyColor.r;
            tint.g = _skyColor.g;
            tint.b = _skyColor.b;
            tint.a *= _skyColor.a;
        }


        for (int i = 0; i < numRays; i++)
        {
            float effArc = source.IsSun ? Mathf.Clamp(arc, -Mathf.PI * 0.5f, Mathf.PI * 0.5f) : arc;
            Vector2 dir = Utils.Rotate(source.Direction, effArc);
            
            var hit = Physics2D.Raycast(sPos, dir, distance, _occluders);
            hits[i] = default;

            ref var hitOut = ref hits[i];
            hitOut.length = distance;
            hitOut.origin = sPos;
            hitOut.direction = dir;
            hitOut.hitDist = distance;

            if (hit)
            {
                hitOut.hitPoint = hit.point;
                hitOut.hitObject = hit.collider.gameObject;
                hitOut.hitDist = Vector2.Distance(hit.point, source.Origin);

                if(hitOut.hitObject.gameObject.tag == "Player")
                {
                    GameManager.Instance.HitRay(1.0f - (hitOut.hitDist / distance));
                }
            }
            
            arc += perI;
            sPos += shift;
        }

        int numSegments = (_numLightSegments + 2);
        int verts = numSegments * numRays;
        int nVerts = _verts + verts;
        int numQuads = (numSegments - 1) * (numRays - 1);
        if(_vertPositions.Length < nVerts || _indexBuffer.Length < _indices + numQuads * 6)
        {
            Array.Resize(ref _vertPositions, nVerts + (_vertPositions.Length >> 1));
            Array.Resize(ref _vertColors, _vertPositions.Length);
            Array.Resize(ref _uvs, _vertPositions.Length);
            Array.Resize(ref _indexBuffer, _indices + numQuads * 6);
        }
        
        Span<Vector3> vertPos = _vertPositions.AsSpan(_verts, verts);
        Span<Color> vertCol = _vertColors.AsSpan(_verts, verts);
        Span<Vector2> vertUV = _uvs.AsSpan(_verts, verts);

        int qS = 0;

        for (int j = 0; j < numSegments - 1; j++, qS += numRays)
        {
            for (int i = 0; i < numRays - 1; i++)
            {
                int i0 = qS + i;
                int i1 = i0 + 1;
                int i2 = i0 + numRays; 
                int i3 = i2 + 1;
                
                _indexBuffer[_indices++] = _verts + i0;
                _indexBuffer[_indices++] = _verts + i2;
                _indexBuffer[_indices++] = _verts + i1;

                _indexBuffer[_indices++] = _verts + i1;
                _indexBuffer[_indices++] = _verts + i2;
                _indexBuffer[_indices++] = _verts + i3;
            }
        }
        
        _verts = nVerts;

        for (int y = 0; y < numSegments; y++)
        {
            float v = 1.0f -  y / (numSegments - 1.0f);
            float fade = Mathf.Max(_falloff.Evaluate(v), 0);
            for (int x = 0; x < numRays; x++)
            {
                ref var ray = ref hits[x];
                
                float u = x / (numRays - 1.0f);
                int idx = y * numRays + x;
                vertUV[idx] = new Vector2(u, v);
                vertCol[idx] = new Color(tint.r, tint.g, tint.b, tint.a * fade);
                vertPos[idx] = ray.origin + ray.direction * (ray.length * v);
            }
        }
        
        for (int x = 0; x < numRays; x++)
        {
            ref var ray = ref hits[x];
            if(ray.hitObject == null) { continue; }

            FindNearestSegment(ray.hitPoint, vertPos, x, numRays, numSegments, out int clipIdx);
            if(clipIdx < 0) { continue;}

            int prevIdx = clipIdx - numRays;
            vertPos[clipIdx] = ray.hitPoint;

            if(prevIdx >= numRays)
            {
                vertPos[prevIdx] = ray.hitPoint + ray.direction * 0.05f;
            }

            for (int i = clipIdx; i >= 0; i -= numRays)
            {
                vertCol[i].a = 0;
            }
        }
    }

    private static void FindNearestSegment(Vector3 target, ReadOnlySpan<Vector3> verts, int ray, int stride, int numSegments, out int index)
    {
        index = -1;

        float dist = float.MaxValue;
        for (int i = 1, j = (stride * numSegments - stride * 2) + ray; i < numSegments; i++, j -= stride)
        {
            float dst = Vector3.Distance(target, verts[j]);
            if (dst < dist)
            {
                dist = dst;
                index = j;
            }
        }
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
