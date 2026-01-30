using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILocatable
{
    bool IsActive { get; set; }
    Vector3 Position { get; }
    float Radius { get; }
    ObjectKind Kind { get; }
}

public enum ObjectKind : int
{
    Default,
    PartyGore,
    Guard,
    Prop,
}

[System.Flags]
public enum ObjectKindMask : int
{
    Default = 0x1,
    Baller = 0x2,
    Guard = 0x4,
    Prop = 0x8,
    
    All = -1
}

