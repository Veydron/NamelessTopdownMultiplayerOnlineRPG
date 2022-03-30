using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
public class Moving : NetworkBehaviour
{
    public float moveSpeed = 5f;
    private Vector3 target;
    private Vector3 position; 
    
    void Awake()
    {
        position = gameObject.transform.position;
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (!base.IsOwner)
            return;

        float step = moveSpeed * Time.deltaTime;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        target = new Vector3(transform.position.x + horizontal, 0, transform.position.z + vertical);
        transform.position = Vector3.MoveTowards(transform.position, target, step);
    }

}
