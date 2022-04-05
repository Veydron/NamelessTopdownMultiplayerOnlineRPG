using System.Collections;
using System.Collections.Generic;
using FishNet.Example.Prediction.Transforms;
using FishNet.Object;
using FishNet;
using UnityEngine;

public class Skills : NetworkBehaviour
{
    private Animator _animator;
    private TransformPrediction _transformPrediction;
    private PopupEmote _popupEmote;
    private CombatSystem _CB;
    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _transformPrediction = this.GetComponent<TransformPrediction>();
        _popupEmote = GetComponentInChildren<PopupEmote>();
        _CB = GetComponent<CombatSystem>();
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
            //Debug.Log("Player sits");

            if (_CB.CharacterStateIs == CombatSystem.State.IDLE)
            {
                Sit();
            }


            //_transformPrediction.isSitting = !_transformPrediction.isSitting;
        }

        if (Input.GetKeyDown(KeyCode.F1) && base.IsOwner)
        {
            _popupEmote.CloseEmote();
            string[] emoteNames = _popupEmote.EmoteNames;
            foreach (var name in emoteNames)
            {
                //Debug.Log(name);
            }
            //PlayEmote();
        }
        if (Input.GetKeyDown(KeyCode.A) && base.IsOwner)
        {
            //Debug.Log("Player startet emote");
            //_popupEmote.ShowEmote("cool");
            Debug.Log("Player sagt server er startete Emote");
            PlayerPlayedEmote();
            //Invoke("closeEmote", 2);

        }

        _animator.SetBool("Sit", _transformPrediction.isSitting);
    }

    void closeEmote()
    {
        _popupEmote.CloseEmote();
    }

    [ServerRpc(RequireOwnership = true)]
    private void Sit()
    {
        //Debug.Log("Is Moving = " + _transformPrediction.isMoving);
        if (_transformPrediction.isMoving || _CB.CharacterStateIs != CombatSystem.State.IDLE)
        {
            return;
        }
        //_transformPrediction.isSitting = !_transformPrediction.isSitting;
        _transformPrediction.SittingDown(!_transformPrediction.isSitting);
        _animator.SetBool("Sit", _transformPrediction.isSitting);

    }

    [ServerRpc(RequireOwnership = true)]
    private void PlayerPlayedEmote()
    {
        Debug.Log("Server macht RPC");
        PlayEmoteForAllObservers();
    }

    [ObserversRpc(BufferLast = false, IncludeOwner = true)]
    private void PlayEmoteForAllObservers()
    {
        Debug.Log("Server macht ObserveRPC");
        _popupEmote.ShowEmote("cool");
        //Invoke("closeEmote", 2);
    }


}
