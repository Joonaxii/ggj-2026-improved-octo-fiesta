using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    
    // Update is called once per frame
    void Update()
    {
        LocatorSystem.Instance.Tick();
        LightingSystem.Instance.Tick(0.0f);
    }
    
    
}
