using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float _interactionRange;
    
    private List<ILocatable> _nearbyInteractables;

    private void Awake()
    {
        _nearbyInteractables = new List<ILocatable>();
    }
    
    // Update is called once per frame
    private void Update()
    {
        _nearbyInteractables.Clear();
        // TODO: Fetch nearby party goers

        foreach (var locatable in _nearbyInteractables)
        {
            if (locatable is not IInteractable interactable) continue;
            
            interactable.Interact();
        }
    }
    
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _interactionRange);
    }
}
