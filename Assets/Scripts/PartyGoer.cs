using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyGoer : MonoBehaviour, IInteractable, ILocatable
{
    public bool IsActive { get; set; }
    public Vector3 Position { get; }
    public float Radius { get; }
    public int Layer { get; }

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
        Debug.Log("suck");
    }

}
