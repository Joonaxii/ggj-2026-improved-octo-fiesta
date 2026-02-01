using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;
    
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private AnimationController _animationController;
    
    [SerializeField] private AudioClip _footStepSound;

    public Vector3 Position => _rb != null ? _rb.position : Vector3.zero;
    public float Radius => _collider != null ? _collider.radius : 0;
    
    private Rigidbody2D _rb;
    private CircleCollider2D _collider;
    private Vector3 _velocity;
    public Vector3 Velocity => _velocity;
    private Vector3 _moveDirection;

    private const float MOVE_LIMITER = 0.7f;
    
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CircleCollider2D>();
        
        _moveDirection = Vector3.zero;
        _velocity = Vector3.zero;
        _rb.velocity = _velocity;

        _animationController.InitializeAnimator(true);
    }

    public void ResetMove()
    {
        _moveDirection = Vector3.zero;
        _velocity = Vector3.zero;
        _rb.velocity = _velocity;
    }
    
    public void TickMovement()
    {
        _moveDirection.x = Input.GetAxisRaw("Horizontal");
        _moveDirection.y = Input.GetAxisRaw("Vertical");
        
        _velocity = _moveDirection * _movementSpeed;
        _velocity *= IsDiagonalMovement() ? MOVE_LIMITER : 1f;
        _rb.velocity = _velocity;
        
        if (_velocity != Vector3.zero)
        {
            _spriteRenderer.flipX = _moveDirection.x < 0 ? true : false;
            _animationController.MoveAnimation();
        }
        else
        {
            _animationController.IdleAnimation();
        }
    }

    private bool IsDiagonalMovement()
    {
        return _moveDirection.x != 0 && _moveDirection.y != 0;
    }
}
