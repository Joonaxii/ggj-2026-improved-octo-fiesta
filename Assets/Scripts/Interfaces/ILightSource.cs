using UnityEngine;

public interface ILightSource
{
    bool IsActive { get; }
    bool Invert { get; }
    bool IsSun { get; }
    int NumRays { get; }
    float Arc { get; }
    float Width { get; }
    Vector2 Origin { get; }
    Vector2 Direction { get; }
    float GetDistance(float progress);
    Color GetTint(float progress);
}
