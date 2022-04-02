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


    void Start()
    {
        c = Camera.main;
        cfl = c.GetComponent<CinemachineFreeLook>();
    }

    void Update()
    {
        SetSpriteLook(this.gameObject);
    }

    private void SetSpriteLook(GameObject P)
    {
        angle = cfl.m_XAxis.Value ;
        RotationsAchse = P.gameObject.transform.GetChild(0);
        float eulerAngleY= P.transform.rotation.eulerAngles.y;
        float korrigiert = 0f;

        //TODO Code versuch zu refrakturieren in 2 Schlaufen und dann nochmal verbessern in eine Funktion
        //die Objekt oder was auch immer übergibt. 
        /*
        for (int i = 0; i < 8; i++){
            int z = 45 * i;
            if (eulerAngleY > (-22.5 + z) && eulerAngleY < (22.5 + z)){
                korrigiert = z;
                break;
            }
        }

        for (int i = 0; i < 8; i++){   
            int z = 45 * i;
            if (angle > -202f + z && angle < -157.5f + z)
            {
                
            }
        }
        */

        if (eulerAngleY > 0f && eulerAngleY < 22.5f){
            korrigiert = 0f;}
        if (eulerAngleY > 22.5f && eulerAngleY < 67.5f){
            korrigiert = 45f;}
        if (eulerAngleY > 67.5f && eulerAngleY < 112.5f){
            korrigiert = 90f;}
        if (eulerAngleY > 112.5f && eulerAngleY < 157.5f){
            korrigiert = 135f;}
        if (eulerAngleY > 157.5f && eulerAngleY < 202.5f){
            korrigiert = 180f;}
        if (eulerAngleY > 202.5f && eulerAngleY < 247.5f){
            korrigiert = 225f;}
        if (eulerAngleY > 247.5f && eulerAngleY < 292.5f){
            korrigiert = 270f;}
        if (eulerAngleY > 292.5f && eulerAngleY < 337.5f){
            korrigiert = 315f;}
        if (eulerAngleY > 337.5f && eulerAngleY < 360f){
            korrigiert = 360f;}


        if (angle < 22.5f && angle > -22.5f){
            RotationsAchse.rotation = Quaternion.Euler(0,korrigiert + angle,0);
            return;}
        if (angle < 67.5f && angle > 22.5f){
            RotationsAchse.rotation = Quaternion.Euler(0,korrigiert + angle - 45,0);
            return;}
        if (angle > -67.5f && angle < -22.5f){
            RotationsAchse.rotation = Quaternion.Euler(0,korrigiert + angle + 45,0);
            return;}
        if (angle < 112.5f && angle > 67.5f){
            RotationsAchse.rotation = Quaternion.Euler(0,korrigiert + angle - 90,0);
            return;}
        if (angle > -112.5f && angle < -67.5f){
            RotationsAchse.rotation = Quaternion.Euler(0,korrigiert + angle + 90,0);
            return;}
        if (angle < 157.5f && angle > 112.5f){
            RotationsAchse.rotation = Quaternion.Euler(0,korrigiert + angle - 135,0);
            return;}
        if (angle > -157.5f && angle < -112.5f){
            RotationsAchse.rotation = Quaternion.Euler(0,korrigiert + angle + 135,0);
            return;}
        if ((angle > 157.5f && angle < 180f) || (angle > -180f && angle < -157.5f)){
            RotationsAchse.rotation = Quaternion.Euler(0,korrigiert + angle - 180,0);
            return;}    
    }
    
}
