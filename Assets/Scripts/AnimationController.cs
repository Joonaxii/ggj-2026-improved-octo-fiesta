using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    private const string DIE_ANIM = "Die";
    private const string IDLE_ANIM = "Idle";
    private const string MOVE_ANIM = "Move";

    public void MoveAnimation() => _animator.Play(MOVE_ANIM);
    public void IdleAnimation() => _animator.Play(IDLE_ANIM);
    public void DeathAnimation() => _animator.Play(DIE_ANIM);
}
