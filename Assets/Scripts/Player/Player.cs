using UnityEngine;

public class Player : MonoBehaviour, ILocatable
{
    public bool IsActive => true;
    public Vector3 Position => _move.Position;
    public float Radius => _move.Radius;
    public ObjectKind Kind => ObjectKind.Player;

    private PlayerMovement _move;
    private void Awake()
    {
        _move = GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        LocatorSystem.Instance.Register(this);
    }
    private void OnDestroy()
    {
        LocatorSystem.Instance.Unregister(this);
    }
}
