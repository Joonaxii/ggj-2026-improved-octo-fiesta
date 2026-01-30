using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocatorSystem : Singleton<LocatorSystem>
{
    private HashSet<ILocatable> _locatables = new HashSet<ILocatable>();
    private List<ILocatable> _activeLocatables = new List<ILocatable>();
    
    public void Register(ILocatable locatable)
        => _locatables.Add(locatable);
    public void Unregister(ILocatable locatable)
        => _locatables.Remove(locatable);

    public void Tick()
    {
        _activeLocatables.Clear();
        // TODO: Add spatial hashing if performance is really bad.
        foreach (var locatable in _locatables)
        {
            if(!locatable.IsActive) { continue; }
            _activeLocatables.Add(locatable);
        }
    }

    public static bool IsAllowed(ObjectKindMask mask, ObjectKind kind)
        => ((int)mask & (1 << (int)kind)) != 0;
    
    public void Fetch(Vector3 position, float radius, List<ILocatable> results, ObjectKindMask allowed = ObjectKindMask.All)
    {
        foreach (var locatable in _activeLocatables)
        {
            if(locatable == null) { continue; }
            if (IsAllowed(allowed, locatable.Kind) && Overlaps(position, radius, locatable.Position, locatable.Radius))
            {
                results.Add(locatable);
            }
        }
    }
    private static bool Overlaps(Vector3 p0, float r0, Vector3 p1, float r1)
    {
        float r = r0 + r1;
        return Vector3.SqrMagnitude(p1 - p0) <= r * r;
    }
}
