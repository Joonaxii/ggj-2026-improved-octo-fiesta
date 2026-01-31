using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool RainbowMode
    {
        get => _rainbowMode;
        set => _rainbowMode = value;
    }
    [SerializeField] private bool _rainbowMode; 
    
    
    // Update is called once per frame
    void Update()
    {
        LocatorSystem.Instance.Tick();
        LightingSystem.Instance.Tick(0.0f);
        
        PartyManager.Instance.TickMovement();
        PartyManager.Instance.TickBehaviour();
    }
    
    
}
