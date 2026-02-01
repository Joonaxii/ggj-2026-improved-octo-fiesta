using UnityEngine;

public class Player : MonoBehaviour, ILocatable
{
    public bool IsActive => true;
    public Vector3 Position => _move.Position;
    public float Radius => _move.Radius;
    public ObjectKind Kind => ObjectKind.Player;

    public PlayerMovement Movement => _move;
    public PlayerInteract Interact => _interact;

    private PlayerMovement _move;
    private PlayerInteract _interact;

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

    public void TickMovement()
    {
        _move.TickMovement();
    }

    public void TickInteraction()
    {
        _interact.TickInteract();
    }
}
