using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class TempCharNamesID : NetworkBehaviour
{
    public static TempCharNamesID current;

    public int playerCount = 0;

    private void Awake() 
    {
        current = this;    
    }


}
