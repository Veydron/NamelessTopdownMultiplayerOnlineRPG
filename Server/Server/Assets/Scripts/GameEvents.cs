using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using System;
using FishNet.Connection;

public class GameEvents : NetworkBehaviour
{
    public static GameEvents current;

    private void Awake() 
    {
        current = this;    
    }

    public event Action<GameObject,int,Vector2> onPlayerWantPath;
    public void PlayerWantPath(GameObject Caller, int Range, Vector2 PathTarget){
        if (onPlayerWantPath != null){
            onPlayerWantPath(Caller, Range, PathTarget);
        }
    }

}
