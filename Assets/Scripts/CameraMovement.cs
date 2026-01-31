using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private PlayerMovement _playerReference;
    [SerializeField] private float _smoothStep;
    [SerializeField] private float _lookaheadDistance;
    
    private Vector3 _velocity;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_playerReference) return;
        
        var moveDirection = _playerReference.Velocity.normalized; 
        var targetPosition = _playerReference.transform.position + (moveDirection * _lookaheadDistance);
        var newPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothStep);
        newPosition.z = -10;
        transform.position = newPosition;
    }
}
