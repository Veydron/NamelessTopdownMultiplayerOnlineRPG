using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testEmote : MonoBehaviour
{    
    private PopupEmote _popupEmote;
    // Start is called before the first frame update
    void Start()
    {
        _popupEmote = GetComponent<PopupEmote>();
        _popupEmote.ShowEmote("cool");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
