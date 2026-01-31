using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class PartyManager : Singleton<PartyManager>
{
    [SerializeField] private float _pollRate = 1f;
    [SerializeField] private GameObject _partyGoerPrefab;
    [SerializeField] private int _numberOfPartyGoers = 16;
    [SerializeField] private Vector2 _partyGoerSpeedRange;
    [SerializeField] private Sprite[] _partyGoerSprites = new Sprite[6];
    
    [SerializeField] private GameObject _movementPointParent;

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
            
            partyGoer.MovementSpeed = Random.Range(2.5f, 10f);
            
            _partyGoers.Add(partyGoer);
            gameObject.transform.SetParent(transform);

            var point = _movementPointParent.transform.GetChild(Random.Range(0, _movementPointParent.transform.childCount));
            gameObject.transform.position = point.position + (Vector3)Random.insideUnitCircle;


            // gameObject.GetComponent<SpriteRenderer>().color = new Color(
            //     needs.PissThreshold * 0.01f,
            //     needs.SocialThreshold * 0.01f,
            //     needs.ThirstThreshold * 0.01f);
        }
    }
    
    public void TickMovement()
    {
        
    }

    public void TickBehaviour()
    {
        
    }
}
