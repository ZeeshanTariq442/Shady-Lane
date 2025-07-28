using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class copMoveCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gamecontrol4players.ins.CopStartWaypoint >= gamecontrol4players.ins.Cop.GetComponent<followPath4Players>().waypoints.Length-1)
        {
            gamecontrol4players.ins.Cop.GetComponent<followPath4Players>().waypointIndex = 0;
            gamecontrol4players.ins.CopStartWaypoint = 0;
            gamecontrol4players.ins.Cop.transform.position = gamecontrol4players.ins.Cop.GetComponent<followPath4Players>().waypoints[0].transform.position;
        }

        if (gamecontrol4players.ins.Cop1StartWaypoint >= gamecontrol4players.ins.Cop1.GetComponent<followPath4Players>().waypoints.Length - 1)
        {
            gamecontrol4players.ins.Cop1.GetComponent<followPath4Players>().waypointIndex = 0;
            gamecontrol4players.ins.Cop1StartWaypoint = 0;
            gamecontrol4players.ins.Cop1.transform.position = gamecontrol4players.ins.Cop1.GetComponent<followPath4Players>().waypoints[0].transform.position;
        }
    
    
    }
}
