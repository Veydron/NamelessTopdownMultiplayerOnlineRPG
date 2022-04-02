using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SpriteStyleRendering : MonoBehaviour
{
    private Camera c;
    private CinemachineFreeLook cfl;
    private float angle;
    private Transform RotationsAchse;

    // Start is called before the first frame update
    void Start()
    {
        c = Camera.main;
        cfl = c.GetComponent<CinemachineFreeLook>();
        RotationsAchse = this.gameObject.transform.GetChild(0);
    }

    // Update is called once per frame
    
    
    void Update()
    {
        //Debug.Log("Rotation y:" + this.transform.rotation.y);
        //Debug.Log("Rotation Euler y:" + this.transform.rotation.eulerAngles.y);
        angle = cfl.m_XAxis.Value ;

        if (angle < 22.5f && angle > -22.5f){
            RotationsAchse.rotation = Quaternion.Euler(0,this.transform.rotation.eulerAngles.y + angle,0);
        }
        if (angle < 67.5f && angle > 22.5f){
            RotationsAchse.rotation = Quaternion.Euler(0,this.transform.rotation.eulerAngles.y + angle - 45,0);
        }
        if (angle > -67.5f && angle < -22.5f){
            RotationsAchse.rotation = Quaternion.Euler(0,this.transform.rotation.eulerAngles.y + angle + 45,0);
        }
        if (angle < 112.5f && angle > 67.5f){
            RotationsAchse.rotation = Quaternion.Euler(0,this.transform.rotation.eulerAngles.y + angle - 90,0);
        }
        if (angle > -112.5f && angle < -67.5f){
            RotationsAchse.rotation = Quaternion.Euler(0,this.transform.rotation.eulerAngles.y + angle + 90,0);
        }
        if (angle < 157.5f && angle > 112.5f){
            RotationsAchse.rotation = Quaternion.Euler(0,this.transform.rotation.eulerAngles.y + angle - 135,0);
        }
        if (angle > -157.5f && angle < -112.5f){
            RotationsAchse.rotation = Quaternion.Euler(0,this.transform.rotation.eulerAngles.y + angle + 135,0);
        }
        if ((angle > 157.5f && angle < 180.1f) || (angle > -180.1f && angle < -157.5f)){
            RotationsAchse.rotation = Quaternion.Euler(0,this.transform.rotation.eulerAngles.y + angle - 180,0);
        }    
        
    }
    
}
