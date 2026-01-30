using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Needs = PartyManager.Needs;

public class PartyGoer : MonoBehaviour, IInteractable, ILocatable
{
    public Needs Needs;
    public float MovementSpeed;

    public bool IsActive => true;
    public Vector3 Position => transform.position;
    public float Radius => 1;
    public ObjectKind Kind => ObjectKind.PartyGoer;

    private void Awake()
    {
        LocatorSystem.Instance.Register(this);
    }

    private void OnDestroy()
    {
        LocatorSystem.Instance.Unregister(this);
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
