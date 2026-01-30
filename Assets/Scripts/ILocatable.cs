using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILocatable
{
    bool IsActive { get; set; }
    Vector3 Position { get; }
    float Radius { get; }
    int Layer { get; }
}
