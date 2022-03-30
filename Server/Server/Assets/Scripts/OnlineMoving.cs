using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Transporting;
using UnityEngine;

public class OnlineMoving : NetworkBehaviour
{
    private Pathfinding pathfinding;
    public Vector3 targetPosition;
    private List<Vector3> clientWaypoints;
    private List<Vector3> oldServerWaypoints;
    private List<Vector3> newServerWaypoints;

    [SyncVar(Channel = Channel.Unreliable, ReadPermissions = ReadPermission.OwnerOnly, SendRate = 0.1f)]
    public bool isMoving = false;
    
    public float movementSpeed = 5f;
    
    [SyncVar(Channel = Channel.Unreliable, ReadPermissions = ReadPermission.OwnerOnly, SendRate = 0.1f)]
    public int waypointIndex;

    public bool newPointReady = false;

    // Start is called before the first frame update
    void Start()
    {
        pathfinding = new Pathfinding(20,20);
    }

     void Update(){
        if(Input.GetMouseButton(0) && base.IsOwner){
            SetTargetPosition();
            SearchPath();
            if (clientWaypoints == null){
                Debug.Log("A) clientWaypoint is NULL");
                return;           
            }
            NewPath(clientWaypoints); 
        }
        
        if (base.IsServer && isMoving){
             Moving();
        }
    }

    [ServerRpc(RequireOwnership = true)]
    void NewPath(List<Vector3> wps){
        newPointReady = true;
        newServerWaypoints = wps;
        isMoving = true;
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
        if (clientWaypoints != null){
           clientWaypoints = pathfinding.FindPath(clientWaypoints[waypointIndex], targetPosition); 
        }
        else{
            clientWaypoints = pathfinding.FindPath(transform.position, targetPosition);
        }
    }
    void Moving(){
        if(oldServerWaypoints == null)
        {
            oldServerWaypoints = newServerWaypoints;
        }

        transform.position = Vector3.MoveTowards(transform.position,oldServerWaypoints[waypointIndex], movementSpeed * Time.deltaTime); 
        if (Vector3.Distance(transform.position, oldServerWaypoints[waypointIndex]) < 0.1f) {
            
            if(newPointReady)
            {
                oldServerWaypoints = newServerWaypoints;
                waypointIndex = 0;
                isMoving = true;
                newPointReady = false;
                return;        
            }

            isMoving =  false;

			if (waypointIndex < (oldServerWaypoints.Count -1)) {
				waypointIndex = waypointIndex +1;
                isMoving = true;
			}     
		}
    }        
}
