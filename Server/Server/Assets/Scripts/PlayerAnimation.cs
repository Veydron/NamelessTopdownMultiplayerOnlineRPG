using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class PlayerAnimation : NetworkBehaviour
{
    private Animator _animator;
    private void Awake() {
        _animator = GetComponentInChildren<Animator>();
    }

    public void SetMoving(bool value)
    {
        _animator.SetBool("Moving", value);
    }

    public void Jump()
    {
        _animator.SetTrigger("Jump");
    }

    public void Attacking(bool value)
    {
        _animator.SetBool("Attacking", value);
    }

    private void Update() {
        
    }
}
