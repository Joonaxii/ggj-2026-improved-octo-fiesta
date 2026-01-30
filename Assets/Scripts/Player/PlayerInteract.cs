using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float _interactionRange;
    
    private List<ILocatable> _nearbyInteractables = new List<ILocatable>();

    private void Awake()
    {
    }
    
    // Update is called once per frame
    private void Update()
    {
        _nearbyInteractables.Clear();
        LocatorSystem.Instance.Fetch(transform.position, _interactionRange, _nearbyInteractables, ObjectKindMask.PartyGoer);

        foreach (var locatable in _nearbyInteractables)
        {
            if (locatable is not IInteractable interactable) continue;
            
            if (Input.GetKeyDown(KeyCode.C))
            {
                // TODO: Look for the closest interactable and interact with that
                interactable.Interact();
            }
        }
    }
    
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _interactionRange);

        if (_nearbyInteractables == null) return;
        
        foreach (var interactable in _nearbyInteractables)
        {
            if (interactable == null) continue;
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, interactable.Position);
        }
    }
}
