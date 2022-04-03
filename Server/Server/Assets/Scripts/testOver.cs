using System.Collections;
using System.Collections.Generic;
using FishNet.Example.Prediction.Transforms;
using UnityEngine;

public class testOver : MonoBehaviour
{
    public Transform transHead;
    public Transform transBody;

    //Referenz to Script
    private TransformPrediction _TP;

    public float headX = 0f;
    public float headY = 0f;
    public float headZ = -90f;

    public float bodyX = 260f;
    public float bodyY = 0f;
    public float bodyZ = 90f;

    private float rotationLookDirection = 0f;
    private float oldLookDirection = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Transform[] allchildren = this.transform.GetComponentsInChildren<Transform>();   
        
        for (int i = 0; i < allchildren.Length-1; i++)
        { 
            Debug.Log(i + " = " + allchildren[i].name);
        }
        
        transBody = allchildren[6].GetComponent<Transform>();
        transHead = allchildren[61].GetComponent<Transform>();
        allchildren = null;

        _TP = this.GetComponent<TransformPrediction>();

        //transHead.rotation = Quaternion.Euler(Headx,Heady,Headz);
    }

    // Update is called once per frame
    void Update()
    {
        rotationLookDirection = transform.eulerAngles.y;
        //Debug.Log(rotationLookDirection);
    }
    void LateUpdate()
    { 
        //Debug.Log("Is Sitting: " + _TP.isSitting);
        //transHead.rotation = Quaternion.Euler(Headx,Heady,Headz);
        if (_TP.isSitting){

            //tramsform body = oldloockdirection
            
            if (oldLookDirection == 0){
                oldLookDirection = rotationLookDirection;
            }

            if (oldLookDirection + 67.5f < rotationLookDirection)
            {
                Debug.Log("Look Direction: " + rotationLookDirection);
                Debug.Log("Old Look Direction bev: " + oldLookDirection);
                //transform Body +45
                oldLookDirection = oldLookDirection +67.5f;
                transBody.rotation = Quaternion.Euler(0,oldLookDirection,0);
                Debug.Log("Old Look Direction +: " + oldLookDirection);
            }
            if (oldLookDirection - 67.5 > rotationLookDirection)
            {
                Debug.Log("Look Direction: " + rotationLookDirection);
                Debug.Log("Old Look Direction bev: " + oldLookDirection);
                //transform Body -45
                oldLookDirection = oldLookDirection -67.5f;
                transBody.rotation = Quaternion.Euler(0,oldLookDirection,0);            
                Debug.Log("Old Look Direction -: " + oldLookDirection);    
            }

            transHead.rotation = Quaternion.Euler(headX,rotationLookDirection-90,headZ);
            transBody.rotation = Quaternion.Euler(bodyX,oldLookDirection,bodyZ);
        }
    }
}
