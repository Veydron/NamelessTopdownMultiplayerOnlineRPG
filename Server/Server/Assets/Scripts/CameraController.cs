using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using FishNet.Object;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    private CinemachineFreeLook cfl;
    Camera c;
    private Vector3 mouseXY;
    public float lastClickTime;
    public const float DOUBLE_CLICK_TIME = 0.5f;
    public GameObject FreeLookObject;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            c = Camera.main;
            cfl = c.GetComponent<CinemachineFreeLook>();
            cfl.Follow = transform;
            cfl.LookAt = transform;

            cfl.m_YAxis.Value = 0.6f;
            cfl.m_XAxis.Value = 0;
            cfl.m_XAxis.m_MaxSpeed = 0f;
        }
    }

    void Update()
    {
        if(base.IsOwner && cfl != null)
        {

        if (Input.GetMouseButtonDown(1))
        {
            float timeSinceLastClick = Time.time - lastClickTime;
            //Debug.Log(timeSinceLastClick);

            if (timeSinceLastClick <= DOUBLE_CLICK_TIME)
            {
                CamReset();
            }
            lastClickTime = Time.time;
        }
        
        if (Input.GetMouseButton(1))
        {
        Drehen();
        return;
        } 

        if (Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetAxis("Mouse ScrollWheel") < 0)
        {
        Scrollen();
        return;
        }

        Reseten();   
        }
    }
    void FreeLookMovement(){
        mouseXY = new Vector3(Input.GetAxis("Mouse X")/2, 0, Input.GetAxis("Mouse Y")/2);
        FreeLookObject.transform.position = FreeLookObject.transform.position + mouseXY;
    }
    void Reseten()
    {   
        if (cfl.m_XAxis.m_MaxSpeed > 0){
          cfl.m_XAxis.m_MaxSpeed = 0f;
          //OnMouseExit();  
        }
    }
    void CamReset()
    {   
        cfl.m_XAxis.Value = 0;
    }

    void Drehen()
    {   
        //OnMouseEnter(cursorRotation);
        cfl.m_XAxis.m_MaxSpeed = 2.5f;
    }

    void Scrollen()
    {   
        //OnMouseEnter(cursorZoom);
        cfl.m_YAxis.m_MaxSpeed = 0.14f;
    }
    
}
