using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineMoving : MonoBehaviour
{
    private Pathfinding pathfinding;
    public Vector3 targetPosition;
    private List<Vector3> waypoints;
    private int waypointIndex = 0;
    private bool isMoving = false;
    
    // Start is called before the first frame update
    void Start()
    {
        pathfinding = new Pathfinding(20,20);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
		{
            SetTargetPosition();
            SearchPath();

            if (waypoints != null)
            {
                isMoving = true;
                waypointIndex = 0;
            }
        }

        if (isMoving)
        {
            Moving();
        }
        
    }

    void SetTargetPosition()
	{
		Plane plane = new Plane(Vector3.up, transform.position);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float point = 0f;
		
		if(plane.Raycast (ray, out point))
		{
            targetPosition = ray.GetPoint(point);
        }
        
	}

    void SearchPath()
    {
        if (waypoints != null)
        {
           waypoints = pathfinding.FindPath(waypoints[waypointIndex], targetPosition); 
        }
        else
        {
            waypoints = pathfinding.FindPath(transform.position, targetPosition);
        }
    }    

    void Moving()
    {
        transform.position = Vector3.MoveTowards(transform.position,waypoints[waypointIndex], 5f * Time.deltaTime); 

        //if (transform.position == waypoints[waypointIndex])
        if (Vector3.Distance(transform.position, waypoints[waypointIndex]) < 0.01f) 
        {
            isMoving =  false;

			if (waypointIndex < (waypoints.Count -1)) 
            {
				waypointIndex = waypointIndex +1;
                isMoving = true;
			}                
		}

    }

}
