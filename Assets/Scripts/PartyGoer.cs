using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Needs = PartyManager.Needs;

using Random = UnityEngine.Random;

public class PartyGoer : MonoBehaviour, IInteractable, ILocatable
{
    public SpriteRenderer SpriteRenderer;
    public AnimationController AnimationController;
    
    public Needs Needs;
    public float MovementSpeed;

    private CircleCollider2D _collider;

    public bool IsActive => _state != State.Dead;
    public Vector3 Position => transform == null ? Vector3.zero : transform.position;
    public float Radius => 0.5f;
    public ObjectKind Kind => ObjectKind.PartyGoer;

    private State _state;
    private State _prevState;

    private void Start()
    {
        LocatorSystem.Instance.Register(this);
        _collider = GetComponent<CircleCollider2D>();
    }

    private void OnDestroy()
    {
        LocatorSystem.Instance.Unregister(this);
    }

    public void Movement()
    {
        if (_state == State.Dead) { return; }
    }

    public void Behavior()
    {
        if (_state == State.Dead) { return; }

      
        switch (_state)
        {
            case State.LookingForTarget:

                break;
            case State.Walking:
                if (CheckDrink()) { break; }
                break;
            case State.Idle:
                if (CheckDrink()) { break; }
                break;
        }
    }

    private bool CheckDrink()
    {
        int rng = Random.Range(0, 100);
        if (rng <= 5)
        {
            _prevState = _state;
            _state = State.Drinking;
            return true;
        }
        return false;
    }

    public void Action()
    {
        if (_state == State.Dead) { return; }

        switch (_state)
        {
            case State.Idle:
                _state = State.LookingForTarget;
                _collider.enabled = false;
                break;
        }
    }

    public void Interact()
    {
        if (_state == State.Dead) { return; }
        AnimationController.DeathAnimation();
        _collider.enabled = false;
        _state = State.Dead;
        PartyManager.Instance.GoersLeft--;
        GameManager.Instance.AddScore(250);
    }


    public enum State
    {
        Idle,

        Drinking,
        Walking,
        Chatting,
        LookingForTarget,

        Dead,
    }
}
