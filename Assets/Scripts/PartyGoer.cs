using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Needs = PartyManager.Needs;

public class PartyGoer : MonoBehaviour, IInteractable, ILocatable
{
    public Needs Needs;
    public float MovementSpeed;
    
    public bool IsActive { get; set; }
    public Vector3 Position { get; }
    public float Radius { get; }
    public ObjectKind Kind => ObjectKind.PartyGoer;

    private void Awake()
    {
        // TODO: register Ilocatable to system
    }

    private void OnDestroy()
    {
        // TODO: remove from registery
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        Debug.Log("SUCK");
    }

}
