using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
public class Moving : NetworkBehaviour
{
    public float MoveSpeed = 5f;
    private CharacterController _characterController;
    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    //[Client(RequireOwnership = true)]
    private void Move()
    {
        if (!base.IsOwner)
            return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 offset = new Vector3(horizontal, Physics.gravity.y, vertical) * (MoveSpeed * Time.deltaTime);
    
        _characterController.Move(offset);
    }
}
