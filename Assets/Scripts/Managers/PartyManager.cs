using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PartyManager : Singleton<PartyManager>
{
    public int GoersLeft
    {
        get => _goersLeft;
        set
        {
            _goersLeft = value;
            UIManager.Instance.UpdateGoers();

            if(_goersLeft <= 0)
            {
                GameManager.Instance.Win("Manor Reclaimed!");
            }
        }
    }
    public int TotalGoers => _partyGoers.Count;

    [SerializeField] private LayerMask _spawnMask;
    [SerializeField] private float _pollRate = 1f;
    [SerializeField] private GameObject _partyGoerPrefab;
    [SerializeField] private int _numberOfPartyGoers = 16;
    [SerializeField] private Vector2 _partyGoerSpeedRange;
    [SerializeField] private Sprite[] _partyGoerSprites = new Sprite[6];
    
    [SerializeField] private GameObject _movementPointParent;

    private int _goersLeft;
 
    private List<PartyGoer> _partyGoers = new List<PartyGoer>();
    
    [Serializable]
    public struct Needs
    {
        public int SocialThreshold;
        public float Social;
        
        public int ThirstThreshold;
        public float Thirst;
        
        public int PissThreshold;
        public float Piss;

        public void MaxOut()
        {
            Social = 100;
            Thirst = 100;
            Piss = 100;
        }

        public void RandomizeThresholds()
        {
            SocialThreshold = Random.Range(15, 85);
            ThirstThreshold = Random.Range(15, 85);
            PissThreshold = Random.Range(15, 85);
        }
    }
    
    protected override void Awake()
    {
        base.Awake();
        _partyGoers = new List<PartyGoer>();
    }

    public void KillAll()
    {
        foreach (var goer in _partyGoers)
        {
            if (goer != null)
            {
                goer.Interact();
            }
        }
    }

    public void SpawnParty()
    {
        _pollTimer = 0;
        _goersLeft += _numberOfPartyGoers;
        for (int i = 0; i < _numberOfPartyGoers; i++)
        {
            var gameObject = Instantiate(_partyGoerPrefab, transform.position, Quaternion.identity);
            var partyGoer = gameObject.GetComponent<PartyGoer>();

            partyGoer.SpriteRenderer.sprite =
                _partyGoerSprites[Random.Range(0, _partyGoerSprites.Length)];
            
            var needs = new Needs();
            needs.MaxOut();
            needs.RandomizeThresholds();
            partyGoer.Needs = needs;
            
            partyGoer.MovementSpeed = Random.Range(0.75f, 3.25f);
            
            _partyGoers.Add(partyGoer);
            gameObject.transform.SetParent(transform);

            Vector2 dir = Random.insideUnitCircle;
            
            var point = _movementPointParent.transform.GetChild(Random.Range(0, _movementPointParent.transform.childCount));
            RaycastHit2D ray = Physics2D.Raycast(point.position, dir, _spawnMask);

            Vector2 position = point.position + (Vector3)dir;
            if (ray)
            {
                position = ray.point + ray.normal * 0.25f;
            }

            gameObject.transform.position = position;
            partyGoer.SpriteRenderer.flipX = gameObject.transform.position.x > point.position.x;
            
            partyGoer.DialogueController.StartSpeaker();
            partyGoer.DialogueController.ChatterModeToggle(true);
        }
    }
    
    public void GetOutOfMyHouse()
    {
        _pollTimer = 0;
        _goersLeft = 0;
        foreach (var partyGoer in _partyGoers)
        {
            if (partyGoer != null)
            {
                GameObject.Destroy(partyGoer.gameObject);
            }
        }
        _partyGoers.Clear();
    }

    public void TickMovement()
    {
        foreach(var partyGoer in _partyGoers)
        {
            partyGoer.Movement();
        }

        foreach(var partyGoer in _partyGoers)
        {
            partyGoer.PostMovement();
        }
    }
    
    public void TickActions()
    {
        foreach(var partyGoer in _partyGoers)
        {
            partyGoer.Action();
        }
    }

    private float _pollTimer;
    public void PollBehavior()
    {
        _pollTimer += Time.deltaTime;
        if(_pollTimer >= _pollRate)
        {
            _pollTimer = 0;
            foreach (var partyGoer in _partyGoers)
            {
                partyGoer.Behavior();
            }
        }

        //if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.X) && Input.GetKey(KeyCode.K))
        //{
        //    KillAll();
        //}
    }
}
