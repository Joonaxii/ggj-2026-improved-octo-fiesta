using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILocatable
{
    bool IsActive { get; }
    Vector3 Position { get; }
    float Radius { get; }
    ObjectKind Kind { get; }
}

public enum ObjectKind : int
{
    Default,
    PartyGoer,
    Guard,
    Prop,
    Player,
}

[System.Flags]
public enum ObjectKindMask : int
{
    Default = 0x1,
    PartyGoer = 0x2,
    Guard = 0x4,
    Prop = 0x8,
    Player = 0x10,
    
    All = -1
}

