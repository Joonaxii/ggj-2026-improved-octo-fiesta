using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraMovement : MonoBehaviour
{
    [Header("In game movement behaviour")]
    [SerializeField] private PlayerMovement _playerReference;
    [SerializeField] private float _smoothStep;
    [SerializeField] private float _lookaheadDistance;
    [SerializeField] private float _smoothStepTime;
    
    [Header("Main menu movement behaviour")]
    [SerializeField] private GameObject _partyPointParent;
    [SerializeField] private Vector2 _cameraWaitTimeMinMax;
    [SerializeField] private float _panningSmoothing;
    [SerializeField] private float _distanceThreshold;
    
    private Vector3 _moveTargetPosition;
    private float _currentWaitTime = 0;
    private float _waitTime = 0;
    private bool _moving;
    
    private Vector3 _velocity;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if(GameManager.Instance.State == GameManager.GameState.Menu )
        {
            MainMenuBehaviour();
        }
        else
        {
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        if (!_playerReference) return;
        
        var moveDirection = _playerReference.Velocity.normalized; 
        var targetPosition = _playerReference.transform.position + (moveDirection * _lookaheadDistance);
        var newPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothStep);
        newPosition.z = -10;
        transform.position = newPosition;
    }

    private void MainMenuBehaviour()
    {
        if (!_partyPointParent) return;
        
        if (!_moving)
        {
            _currentWaitTime += Time.deltaTime;

            if (_currentWaitTime < _waitTime)
            {
                return;
            }
            
            _moveTargetPosition = _partyPointParent.transform
                    .GetChild(Random.Range(0, _partyPointParent.transform.childCount)).position;
        }
        
        _moving = true;
        
        var newPosition = Vector3.SmoothDamp(transform.position, _moveTargetPosition, ref _velocity, _panningSmoothing);
        newPosition.z = -10;
        transform.position = newPosition;

        if (Vector2.Distance(transform.position, _moveTargetPosition) <= _distanceThreshold)
        {
            _moving = false;
            _currentWaitTime = 0;
            _waitTime = Random.Range(_cameraWaitTimeMinMax.x, _cameraWaitTimeMinMax.y);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _distanceThreshold);
        Gizmos.DrawLine(transform.position, _moveTargetPosition);
    }
}
