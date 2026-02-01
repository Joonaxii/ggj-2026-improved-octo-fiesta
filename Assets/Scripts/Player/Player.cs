
using UnityEngine;

public class Player : MonoBehaviour, ILocatable
{
    public bool IsActive => true;
    public bool WasSus
    {
        get => _wasSus;
        set => _wasSus = value;
    }

    public Vector3 Position => _move.Position;
    public float Radius => _move.Radius;
    public ObjectKind Kind => ObjectKind.Player;

    public PlayerMovement Movement => _move;
    public PlayerInteract Interact => _interact;

    public float BurnVal => _burnLevel / MAX_BURN;

    public AudioClip damage;

    private PlayerMovement _move;
    private PlayerInteract _interact;

    const float MAX_BURN = 16.0f;

    private int _lives = 4;
    private float _susLevel = 0;
    private bool _wasSus = false;
    private float _burnLevel;

    public void Init()
    {
        _move = GetComponent<PlayerMovement>();
        _interact = GetComponent<PlayerInteract>();
    }

    private void Start()
    {
        LocatorSystem.Instance.Register(this);
    }

    private void OnDestroy()
    {
        LocatorSystem.Instance.Unregister(this);
    }

    public void AddBurn(float amount)
    {
        _burnLevel += amount;
        _burnLevel = Mathf.Clamp(_burnLevel, 0, MAX_BURN);

        if(_burnLevel >= MAX_BURN)
        {
            _burnLevel = 0;
            _lives--;

            AudioManager.Instance.PlaySound(damage, 0.75f);

            _wasSus = true;
            if (_lives <= 0)
            {
                _move.Die();
                GameManager.Instance.Lose("You Died!");
            }
        }
    }

    public void Respawn()
    {
        _move.SetBurnLevel(BurnVal);
        _wasSus = false;
        _burnLevel = 0;
        _lives = 4; 
        _susLevel = 0;
        UIManager.Instance.UpdateSus(0.0f);
    }

    const float MAX_SUS = 8.0f;
    public void NoticeSus()
    {
        _susLevel += 0.8f;
        UIManager.Instance.UpdateSus(_susLevel / MAX_SUS);
    }

    public void TickMovement()
    {
        _move.SetBurnLevel(BurnVal);
        _move.TickMovement();
    }

    public void TickInteraction()
    {
        _interact.TickInteract();
        _burnLevel -= Time.deltaTime * 0.95f;
        _burnLevel = Mathf.Clamp(_burnLevel, 0, MAX_BURN);

        if(_susLevel >= MAX_SUS)
        {
            _move.Die();
            GameManager.Instance.Lose("You got, got!");
        }
    }
}
