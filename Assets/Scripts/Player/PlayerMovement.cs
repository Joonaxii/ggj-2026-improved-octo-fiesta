using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;

    private Vector3 _velocity;
    public Vector3 Velocity => _velocity;
    private Vector3 _moveDirection;

    private const float MOVE_LIMITER = 0.7f;
    
    void Start()
    {
        _moveDirection = Vector3.zero;
        _velocity = Vector3.zero;
    }
    
    void Update()
    {
        _moveDirection.x = Input.GetAxisRaw("Horizontal");
        _moveDirection.y = Input.GetAxisRaw("Vertical");
        
        _velocity = _moveDirection * _movementSpeed;
        _velocity *= IsDiagonalMovement() ? MOVE_LIMITER : 1f;
        transform.position += _velocity * Time.deltaTime;
    }

    private bool IsDiagonalMovement()
    {
        return _moveDirection.x != 0 && _moveDirection.y != 0;
    }
}
