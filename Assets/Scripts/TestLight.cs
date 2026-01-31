using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLight : MonoBehaviour, ILightSource
{
    public bool IsActive => gameObject.activeInHierarchy;
    public bool IsSun => true;
    public int NumRays => _numRays;
    public bool Invert => _invert;

    public float Arc => _arc;
    public float Width => _width;

    public Vector2 Origin => transform.position;
    public Vector2 Direction => transform.up;

    [SerializeField, Range(8, 128)] private int _numRays = 16;
    [SerializeField, Range(0, 360)] private float _arc = 25;
    [SerializeField, Min(0)] private float _width = 2.0f;
    [SerializeField] private Gradient _tint;
    [SerializeField] private bool _invert;

    [SerializeField] private float _distance = 8.0f;
    public Color GetTint(float progress)
    {
        return _tint.Evaluate(progress);
    }  
    public float GetDistance(float progress)
    {
        return _distance;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        LightingSystem.Instance.Register(this);
    }

    public void DrawGizmos()
    {
    }
}
