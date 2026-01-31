using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLight : MonoBehaviour, ILightSource
{
    public bool IsActive => gameObject.activeInHierarchy;
    public int NumRays => _numRays;

    public float Arc => _arc;
    public float Width => _width;

    public Vector2 Origin => transform.position;
    public Vector2 Direction => transform.up;

    [SerializeField, Range(2, 64)] private int _numRays = 16;
    [SerializeField, Range(0, 360)] private float _arc = 25;
    [SerializeField, Min(0)] private float _width = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        LightingSystem.Instance.Register(this);
    }

    public void DrawGizmos()
    {
    }
}
