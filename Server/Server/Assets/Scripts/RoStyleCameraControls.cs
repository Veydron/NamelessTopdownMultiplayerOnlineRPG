using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RoStyleCameraControls : MonoBehaviour
{
    public CinemachineFreeLook Cinema;

    public GameObject PlayerChar;
    
    public GameObject FreeLookObject;

    public Texture2D cursorRotation;
    public Texture2D cursorZoom;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    public CinemachineFreeLook _fl;

    public float lastClickTime;
    public const float DOUBLE_CLICK_TIME = 0.5f;

    public Vector3 mouseXY;

    public float lastCamRotValue;
    public float lastCamHighValue;

    public bool LockAll;

    // Start is called before the first frame update
    void Start()
    { 
        _fl = Cinema.GetComponent<CinemachineFreeLook>();

        _fl.m_YAxis.Value = 0.6f;
        _fl.m_XAxis.Value = 0;

        OnMouseExit();  
        Reseten();
          
    }

    void Drehen()
    {   
        OnMouseEnter(cursorRotation);
        _fl.m_XAxis.m_MaxSpeed = 400f;
    }

       void Scrollen()
    {   
        //OnMouseEnter(cursorZoom);
        _fl.m_YAxis.m_MaxSpeed = 10.75f;
    }

       void Reseten()
    {   
        if (_fl.m_XAxis.m_MaxSpeed > 0)
        {
          _fl.m_XAxis.m_MaxSpeed = 0f;
          OnMouseExit();  
        }
    }

        void CamReset()
    {   
        _fl.m_XAxis.Value = 0;
    }

    void FreeLookMovement()
    {
        mouseXY = new Vector3(Input.GetAxis("Mouse X")/2, 0, Input.GetAxis("Mouse Y")/2);
        
        FreeLookObject.transform.position = FreeLookObject.transform.position + mouseXY;
        
        

        //Debug.Log(mouseXY)
    }

    // Update is called once per frame
    void Update()
    {   
        FreeLookMovement();

        if (Input.GetMouseButtonDown(2))
        {
            LockAll = true;
            _fl.m_YAxis.m_MaxSpeed = 10f;
            lastCamRotValue = _fl.m_XAxis.Value;
            lastCamHighValue = _fl.m_YAxis.Value;
            _fl.m_XAxis.Value = 0;
            _fl.m_YAxis.Value = 1;
            FreeLookObject.transform.position = new Vector3(PlayerChar.transform.position.x,10f,PlayerChar.transform.position.z);
            _fl.Follow = FreeLookObject.transform;
            _fl.LookAt = FreeLookObject.transform;
        }

        if (Input.GetMouseButtonUp(2))
        {
            _fl.m_XAxis.Value = lastCamRotValue;
            _fl.m_YAxis.Value = lastCamHighValue;
            _fl.Follow = PlayerChar.transform;
            _fl.LookAt = PlayerChar.transform;
            LockAll = false;
        }

        if (LockAll == false)
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

    void OnMouseEnter(Texture2D cursorTexture)
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }


}


/*
Was will ich können ?

Camera immer zentriert auf Player


Kamera dreht um den player bei rechter maustaste gedrückthalten und Mausbewegung


Kamera snappt auf Standart 0 Grad drehung bei doppel klick rechte maustauste


Kamera zoomt näher ran und ändert den Winkel bei mausrad scrolling


Kamera lösst sich von playerzentrierung, Zoomt weit raus und bewegt sich mit der Maus bewegung


Maus bekommt Icons je nachdem in welchem Modus man ist


*/