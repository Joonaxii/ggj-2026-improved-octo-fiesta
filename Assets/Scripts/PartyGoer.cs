using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Needs = PartyManager.Needs;

using Random = UnityEngine.Random;

public class PartyGoer : MonoBehaviour, IInteractable, ILocatable
{
    public SpriteRenderer SpriteRenderer;
    public AnimationController AnimationController;
    public PartyGoerDialogue DialogueController;

    public GameObject TargetMarker;
    
    public Needs Needs;
    public float MovementSpeed;

    public bool IsActive => _state != State.Dead;
    public Vector3 Position => _position;
    public float Radius => 0.5f;
    public ObjectKind Kind => ObjectKind.PartyGoer;

    [SerializeField] private float _repulseDistance = 0.65f;
    [SerializeField] private float _repulseStrength = 0.25f;

    [SerializeField] private LayerMask _repulseMask;

    [Header("Timings")]
    [SerializeField] private float _minDrinkingDuration = 0.25f;
    [SerializeField] private float _maxDrinkingDuration = 2.5f;

    [SerializeField] private float _minPissDuration = 2.5f;
    [SerializeField] private float _maxPissDuration = 7.5f;

    [SerializeField] private float _minPunchDuration = 0.75f;
    [SerializeField] private float _maxPunchDuration = 2.5f;

    [SerializeField] private float _minSocializingDuration = 0.15f;
    [SerializeField] private float _maxSocializingDuration = 7.5f;

    [SerializeField, ReadOnly] private State _state;
    [SerializeField, ReadOnly] private State _prevState;
    [SerializeField, ReadOnly] private State _targetState;
    [SerializeField, ReadOnly] private float _timer;
    private Vector3 _position;
    private Vector3 _prevPos;
    private Vector3 _socialOffset;
    private bool _hasDrink;
    [SerializeField, ReadOnly] private Path _path = new Path();

    private void Start()
    {
        LocatorSystem.Instance.Register(this);
        AnimationController.InitializeAnimator(false);
        TargetMarker.SetActive(false);

        _state = State.Idle;
        _timer = Random.Range(0.25f, 1.75f);
    }

    private void OnDestroy()
    {
        LocatorSystem.Instance.Unregister(this);
    }

    public void SetState(State state)
    {
        _prevState = _state;
        _state = state;
    }

    public void UpdateInteractVisuals(bool newState)
    {
        TargetMarker.SetActive(newState);
    }

    public void WantsToSocialize()
    {
        if((_targetState == State.Chatting && _state != State.Idle) || _state == State.Chatting ||
            (_prevState == State.Chatting && _state == State.Drinking)) { return; }

        int from = NodeManager.Instance.FindClosestMoveTarget(_position);
        if (from < 0) { return; }
        DialogueController.ShowSocializingComment();

        int to = NodeManager.Instance.FindRandomSocial();
        NodeManager.Instance.BuildPath(_path, from, to);
        _state = State.Walking;
        _socialOffset = NodeManager.Instance.GetRandomOffsetTarget(to);
        _targetState = State.Chatting;
        _timer = Random.Range(_minSocializingDuration, _maxSocializingDuration);
    }

    public void WantsToPiss()
    {
        if ((_targetState == State.Pissing && _state != State.Idle) || _state == State.Pissing ||
            (_prevState == State.Pissing && _state == State.Drinking)) { return; }

        int from = NodeManager.Instance.FindClosestMoveTarget(_position);
        if (from < 0) { return; }

        DialogueController.ShowBathroomComment();
        int to = NodeManager.Instance.FindRandomToilet();
        NodeManager.Instance.BuildPath(_path, from, to);
        _state = State.Walking;
        _socialOffset = NodeManager.Instance.GetRandomOffsetTarget(to);
        _targetState = State.Pissing;
        _timer = Random.Range(_minPissDuration, _maxPissDuration);
    }

    public void WantsSomePunch()
    {
        if ((_targetState == State.GettingPunch && _state != State.Idle) || _state == State.GettingPunch ||
            (_prevState == State.GettingPunch && _state == State.Drinking)) { return; }

        int from = NodeManager.Instance.FindClosestMoveTarget(_position);
        if (from < 0) { return; }

        DialogueController.ShowDrinkComment();
        int to = NodeManager.Instance.FindRandomPunch();
        NodeManager.Instance.BuildPath(_path, from, to);
        _state = State.Walking;
        _socialOffset = NodeManager.Instance.GetRandomOffsetTarget(to);
        _targetState = State.GettingPunch;
        _timer = Random.Range(_minPunchDuration, _maxPunchDuration);
    }

    public void HasToDrink()
    {
        if (_state == State.Drinking) { return; }

        DialogueController.ShowSipDrinkComment();
        _prevState = _state;
        _state = State.Drinking;
        _timer = Random.Range(_minDrinkingDuration, _maxDrinkingDuration);
    }

    public void Movement()
    {
        _position = transform.position;
        if (_state == State.Dead) { return; }

        switch (_state)
        {
            case State.Walking:
                {
                    if(_path.Current != null)
                    {
                        var targetPos = _path.Current.Position;

                        var moveDir = (targetPos - _position);
                        if (_path.IsAtTarget(_position, out var next))
                        {
                            if(next == null)
                            {
                                _state = _targetState;
                            }
                        }
                        else
                        {
                            _position += moveDir.normalized * MovementSpeed * Time.deltaTime;
                        }
                    }
                    else
                    {
                        _state = State.Idle;
                    }

                    _others.Clear();
                    LocatorSystem.Instance.Fetch(_position, _repulseDistance, _others, ObjectKindMask.PartyGoer);

                    Utils.CollisionContact contact = default;
                    foreach (var other in _others)
                    {
                        if (other is PartyGoer pGoer)
                        {
                            contact = default;
                            Utils.CircleVSCircle(_position, _repulseDistance, other.Position, other.Radius, ref contact);
                            if (contact.wasHit)
                            {
                                pGoer._position += (Vector3)contact.normal * other.Radius * _repulseStrength * Time.deltaTime;
                            }
                        }
                    }
                }
                break;
            case State.Chatting:
                {
                    const float MAX_SOCIAL_POINT_DIST = 0.05f * 0.05f;
                    var moveDir = (_socialOffset - _position);
                    float dist = Vector3.SqrMagnitude(moveDir);
                    if(dist > MAX_SOCIAL_POINT_DIST)
                    {
                        _position += moveDir.normalized * MovementSpeed * Time.deltaTime * 0.125f;
                    }
                }
                break;
        }

    }

    private static List<ILocatable> _others = new List<ILocatable>();

    public void PostMovement()
    {
        if (_state == State.Dead) { return; }

        switch (_state)
        {
            case State.Walking:
            case State.Chatting:
                RepulseFromPlayer();
                break;
        }

        var overlap = Physics2D.CircleCast(_position, _repulseDistance, Vector2.up, 0.0f, _repulseMask);
        if (overlap)
        {
            _position += (Vector3)overlap.normal * _repulseStrength * Time.deltaTime;
        }

        var delta = _position - _prevPos;
        if (delta.sqrMagnitude > 0)
        {
            SpriteRenderer.flipX = delta.x < 0 ? true : false;
            AnimationController.MoveAnimation();
        }
        else
        {
            AnimationController.IdleAnimation();
        }
        _prevPos = _position;
        transform.position = _position;
    }

    private static ILocatable[] TEMP = new ILocatable[1];
    private void RepulseFromPlayer()
    {
        int num = LocatorSystem.Instance.Fetch(_position, _repulseDistance, TEMP, ObjectKindMask.Player);
        if(num <= 0) { return; }

        var plr = TEMP[0];

        Utils.CollisionContact contact = default;
        Utils.CircleVSCircle(_position, _repulseDistance, plr.Position, plr.Radius, ref contact);

        if (contact.wasHit)
        {
            _position -= (Vector3)(contact.normal * contact.depth) * _repulseStrength * Time.deltaTime;
        }

    }

    public void Behavior()
    {
        if (_state == State.Dead) { return; }

        switch (_state)
        {
            case State.Chatting:
                if (CheckDrink()) { break; }
                CheckNeeds();
                break;
            case State.Walking:
                if (CheckDrink()) { break; }
                CheckNeeds();
                break;
            case State.Idle:
                if (CheckDrink()) { break; }
                CheckNeeds();
                break;
        }
    }

    private void CheckNeeds()
    {
        // Toilet -> Drink -> Social
        if(Needs.Piss >= Needs.PissThreshold)
        {
            WantsToPiss();
            return;
        }
        else if (Needs.Thirst >= (_hasDrink ? Needs.ThirstThreshold : Needs.ThirstThreshold * 0.25f))
        {
            WantsSomePunch();
            return;
        }
        else if (Needs.Social >= Needs.SocialThreshold)
        {
            WantsToSocialize();
            return;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Color pathColor = Color.yellow;
        switch (_state)
        {
            case State.Idle:
            case State.Chatting:
            case State.Dead:
            case State.GettingPunch:
                return;
            case State.Walking:
                pathColor = Color.yellow;
                break;
            case State.Drinking:
                pathColor = Color.blue;
                break;
        }

        var pathPast = pathColor;
        pathPast.r *= 0.333f;
        pathPast.g *= 0.333f;
        pathPast.b *= 0.333f;

        for (int i = 0; i < _path.nodes.Count; i++)
        {
            Gizmos.color = i <= _path.current ? pathPast : pathColor;
            var node = _path.nodes[i];
            if(i < _path.nodes.Count - 1)
            {
                Gizmos.DrawLine(node.Position, _path.nodes[i + 1].Position);
            }
            Gizmos.DrawSphere(node.Position, 0.25f);
        }
    }

    private bool CheckDrink()
    {
        int rng = Random.Range(0, 100);
        if (rng <= 3)
        {
            HasToDrink();
            return true;
        }
        return false;
    }

    public void Action()
    {
        if (_state == State.Dead) { return; }

        if (GameManager.Instance.WasSus)
        {
            const float SUS_RADIUS = 1.55f;
            int num = LocatorSystem.Instance.Fetch(_position, SUS_RADIUS, TEMP, ObjectKindMask.Player);
            if(num > 0 && Utils.CircleVSCircle(_position, SUS_RADIUS, TEMP[0].Position, 0.05f))
            {
                GameManager.Instance.NoticeSus();
            }
        }

        switch (_state)
        {
            case State.Idle:
                Needs.Social += 0.25f * Time.deltaTime;
                break;

            case State.Drinking:
                _timer -= Time.deltaTime;
                Needs.Piss += 3.5f * Time.deltaTime;
                Needs.Thirst -= 6.66f * Time.deltaTime;
                Needs.Social += 0.25f * Time.deltaTime;

                if (Needs.Thirst < 15)
                {
                    Needs.Thirst = 15;
                }

                if (_timer <= 0)
                {
                    _state = _prevState;
                }
                break;
            case State.Pissing:
                _timer -= Time.deltaTime;
                Needs.Piss -= 12.0f * Time.deltaTime;
                Needs.Social += 0.85f * Time.deltaTime;

                if (Needs.Piss < 15)
                {
                    Needs.Piss = 15;
                }

                if (_timer <= 0)
                {
                    _state = _prevState;
                }
                break;

            case State.GettingPunch:
                _timer -= Time.deltaTime;
                Needs.Thirst -= 4.25f * Time.deltaTime;
                Needs.Piss += 1.5f * Time.deltaTime;

                if (Needs.Thirst < 15)
                {
                    Needs.Thirst = 15;
                }

                if (_timer <= 0)
                {
                    _hasDrink = true;
                    _state = _prevState;
                }
                break;
                
            case State.Chatting:
                _timer -= Time.deltaTime;
                Needs.Social -= 2.85f * Time.deltaTime;

                if(Needs.Social < 15)
                {
                    Needs.Social = 15;
                }

                if (_timer <= 0)
                {
                    _state = _prevState;
                }
                break;

            case State.Walking:
                Needs.Thirst += 0.25f * Time.deltaTime;
                Needs.Piss += Mathf.LerpUnclamped(0.015f, 0.15f, (Needs.Thirst - 15) / 85.0f) * Time.deltaTime;
                Needs.Social += 0.015f * Time.deltaTime;
                break;
        }
    }

    public void Interact()
    {
        if (_state == State.Dead) { return; }
        AnimationController.DeathAnimation();
        _state = State.Dead;
        DialogueController.Shutup();
        PartyManager.Instance.GoersLeft--;
        GameManager.Instance.AddScore(250);
        GameManager.Instance.WasSus = true;
    }

    public enum State
    {
        Idle,

        Drinking,
        Walking,
        Chatting,
        GettingPunch,
        Pissing,

        Dead,
    }
}
