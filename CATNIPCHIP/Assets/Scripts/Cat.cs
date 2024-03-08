using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : MonoBehaviour
{
    [SerializeField]
    private WaypointManager waypointManager = new WaypointManager();

    private void Start()
    {
        transform.position = waypointManager.CurrentWaypoint.transform.position;
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.E)) { 
            waypointManager.SetNextWaypoint();
            GetComponent<SmoothAgentMovement>().SetDestination(waypointManager.CurrentWaypoint.transform.position);
        }
    }

}
