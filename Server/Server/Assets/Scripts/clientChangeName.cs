using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clientChangeName : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        TempCharNamesID.current.playerCount = TempCharNamesID.current.playerCount + 1;
        this.name = TempCharNamesID.current.playerCount.ToString();      
    }

}
