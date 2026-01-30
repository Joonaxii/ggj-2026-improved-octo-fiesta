using UnityEngine;

public interface ILightSource
{
    int NumRays { get; }
    float Arc { get; }
    Vector2 Direction { get; } 
    float Angle { get; }
    
    void DrawGizmos();
}
