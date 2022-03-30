using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;


public class StartEmpty : MonoBehaviour
{
    private Pathfinding pathfinding;

    // Start is called before the first frame update
    void Start()
    {
        pathfinding = new Pathfinding(10,10);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            List<PathNode> path = pathfinding.FindPath(0,0,6,8);
            if (path != null)
            {
                for (int i=0; i<path.Count - 1; i++)
                {
                    
                    Debug.DrawLine(new Vector3(path[i].x,0, path[i].z) * 1f + Vector3.one * 0.5f, new Vector3(path[i+1].x,0, path[i+1].z) * 1f + Vector3.one * 0.5f, Color.green, 5f);

                }
            }        
        }
    }
}
