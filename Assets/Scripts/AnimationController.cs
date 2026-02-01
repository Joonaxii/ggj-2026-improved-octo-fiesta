using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    [SerializeField] private AudioClip _swoon;
    [SerializeField] private AudioClip _hitfloor;
    
    private const string DIE_ANIM = "Die";
    private const string IDLE_ANIM = "Idle";
    private const string MOVE_ANIM = "Move";

    public void MoveAnimation() => _animator.Play(MOVE_ANIM);
    public void IdleAnimation() => _animator.Play(IDLE_ANIM);
    public void DeathAnimation() => _animator.Play(DIE_ANIM);
    
    public void PlaySwoonSound() => AudioManager.Instance.PlaySoundAtLocation(_swoon, Random.Range(0.95f, 1.05f), transform.position);
    public void PlayHitFloorSound() => AudioManager.Instance.PlaySoundAtLocation(_hitfloor, Random.Range(0.9f, 1.1f), transform.position);
    
}
