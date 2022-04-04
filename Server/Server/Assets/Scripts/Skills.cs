using System.Collections;
using System.Collections.Generic;
using FishNet.Example.Prediction.Transforms;
using FishNet.Object;
using UnityEngine;

public class Skills : NetworkBehaviour
{
    private Animator _animator;
    private TransformPrediction _transformPrediction;
    private void Awake() {
        _animator = GetComponentInChildren<Animator>();
        _transformPrediction = this.GetComponent<TransformPrediction>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            if (Input.GetKeyDown(KeyCode.S) && base.IsOwner)
            {
                Sit();
                
                //_transformPrediction.isSitting = !_transformPrediction.isSitting;
            }     

            _animator.SetBool("Sit", _transformPrediction.isSitting);
    }

    [ServerRpc(RequireOwnership = true)]
    private void Sit()
    {   
        //Debug.Log("Is Moving = " + _transformPrediction.isMoving);
        if (_transformPrediction.isMoving){
            return;
        }
        //_transformPrediction.isSitting = !_transformPrediction.isSitting;
        _transformPrediction.SittingDown(!_transformPrediction.isSitting);
        _animator.SetBool("Sit", _transformPrediction.isSitting);
    }


}
