using UnityEngine;

public interface ILightSource
{
    bool IsActive { get; }
    int NumRays { get; }
    float Arc { get; }
    float Width { get; }
    Vector2 Origin { get; }
    Vector2 Direction { get; }
    
    void DrawGizmos();
}
