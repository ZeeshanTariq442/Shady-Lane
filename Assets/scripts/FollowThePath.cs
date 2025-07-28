using UnityEngine;

public class FollowThePath : MonoBehaviour {

    public Transform[] waypoints;

    [SerializeField]
    private float moveSpeed = 1f;

    //[HideInInspector]
    public int waypointIndex = 0;

    public bool moveAllowed = false;
    public bool movebackAllowed = false;

    public static FollowThePath ins;
	// Use this for initialization
	private void Start () {
        ins = this;
        transform.position = waypoints[waypointIndex].transform.position;
	}
	
	// Update is called once per frame
	private void Update () {
        if (moveAllowed)
        {
            Move();
        }
           
        if (movebackAllowed)
            FightMove();
    }

    private void Move()
    {
        // moving player forward each time rolled
        if (waypointIndex <= waypoints.Length - 1)
        {
            transform.position = Vector2.MoveTowards(transform.position,
            waypoints[waypointIndex].transform.position,
            moveSpeed * Time.deltaTime);

            if (transform.position == waypoints[waypointIndex].transform.position)
            {
                waypointIndex += 1;
            }
        }
    }
 
    public void FightMove()
    {
        // function for moving back if there is a fight
        if (GameControl.ins.losingplayerIndex >= GameControl.ins.losingplayerIndex - GameControl.ins.fightLoseNo && GameControl.ins.losingplayerIndex != 0)
        {
            print(" losing player index---------- "+ GameControl.ins.losingplayerIndex+" my name  -----  "+transform.name);
            // change the position of lost player 
            GameControl.ins.losingPlayer.transform.position = Vector2.MoveTowards(GameControl.ins.losingPlayer.transform.position,
            waypoints[GameControl.ins.losingplayerIndex].transform.position,
            moveSpeed * Time.deltaTime);
            // changing the waypoint index each time
            if (GameControl.ins.losingPlayer.transform.position == waypoints[GameControl.ins.losingplayerIndex].transform.position)
            {
                GameControl.ins.losingplayerIndex -= 1;
                GameControl.ins.losingPlayer.GetComponent<FollowThePath>().waypointIndex -= 1;
            }
        }
        else
        {
            movebackAllowed = false;
            GameControl.ins.Fightexplosion.SetActive(false);
        }



    }
}
