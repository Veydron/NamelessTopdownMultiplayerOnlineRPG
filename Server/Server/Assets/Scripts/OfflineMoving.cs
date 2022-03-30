using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

public class OfflineMoving : NetworkBehaviour
{
    private Pathfinding pathfinding;
    public Vector3 targetPosition;
    private List<Vector3> waypoints;
    public bool isMoving = false;
    private int waypointIndex = 0;
    void Start(){
        pathfinding = new Pathfinding(20,20);
    }
    void Update(){
        if(Input.GetMouseButton(0)){
            SetTargetPosition();
            SearchPath();
            if (waypoints != null){
                isMoving = true;
                waypointIndex = 0;
            }
        }
        if (isMoving){
            Moving();
        }
    }
    void SetTargetPosition(){
		Plane plane = new Plane(Vector3.up, transform.position);
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float point = 0f;
		if(plane.Raycast (ray, out point)){
            targetPosition = ray.GetPoint(point);
        }
	}
    void SearchPath(){
        if (waypoints != null){
           waypoints = pathfinding.FindPath(waypoints[waypointIndex], targetPosition); 
        }
        else{
            waypoints = pathfinding.FindPath(transform.position, targetPosition);
        }
    }    
    void Moving(){
        transform.position = Vector3.MoveTowards(transform.position,waypoints[waypointIndex], 4f * Time.deltaTime); 
        if (Vector3.Distance(transform.position, waypoints[waypointIndex]) < 0.05f) {
            isMoving =  false;
			if (waypointIndex < (waypoints.Count -1)) {
				waypointIndex = waypointIndex +1;
                isMoving = true;
			}                
		}
    }
}
