using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;


public class StartEmpty : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MapGrid grid = new MapGrid(4,2,1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
