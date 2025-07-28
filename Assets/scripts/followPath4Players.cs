using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followPath4Players : MonoBehaviour
{
   
    public Transform[] waypoints;

    [SerializeField]
    private float moveSpeed = 1f;

    //[HideInInspector]
    public int waypointIndex = 0;

    public bool moveAllowed = false;
    public bool movebackAllowed = false;

    IEnumerator coroutine;


    public static followPath4Players ins;
    // Use this for initialization
    private void Start()
    {
        ins = this;
        transform.position = waypoints[waypointIndex].transform.position;
     
    }

    // Update is called once per frame
     void Update()
     {
        if (moveAllowed == true)
        {
             coroutine = Move(1.0f);
            StartCoroutine(coroutine);
        }

        if (movebackAllowed == true)
            FightMove();

        if (waypointIndex <= 0)
            waypointIndex = 0;
     }

    IEnumerator Move(float waitTime)
    {
        // moving player forward each time rolled
        if (waypointIndex <= waypoints.Length - 1)
        {
            this.transform.position = Vector2.MoveTowards(transform.position,
            this.waypoints[waypointIndex].transform.position,
            moveSpeed * Time.deltaTime);


            if (transform.position == waypoints[waypointIndex].transform.position)
            {
                this.waypointIndex = this.waypointIndex + 1;
            //    movementsound.ins.audioSource.clip = movementsound.ins.audioClip;
             //   movementsound.ins.audioSource.Play();
                //this.waypointIndex +=  1;
            }
        }
        yield return new WaitForSeconds(waitTime);
    }

    public void FightMove()
    {
        // function for moving back if there is a fight
        if (gamecontrol4players.ins.losingplayerIndex >= gamecontrol4players.ins.losingplayerIndex - gamecontrol4players.ins.fightLoseNo
            && gamecontrol4players.ins.losingplayerIndex != 0  )
        {
            print(" losing player index---------- " + gamecontrol4players.ins.losingplayerIndex + " my name  -----  " + transform.name);
            // change the position of lost player 
            gamecontrol4players.ins.losingPlayer.transform.position = Vector2.MoveTowards(gamecontrol4players.ins.losingPlayer.transform.position,
            waypoints[gamecontrol4players.ins.losingplayerIndex].transform.position,
            moveSpeed * Time.deltaTime);
            // changing the waypoint index each time
            if (gamecontrol4players.ins.losingPlayer.transform.position == waypoints[gamecontrol4players.ins.losingplayerIndex].transform.position
                )
            {
                print(" moving back " + gamecontrol4players.ins.losingplayerIndex );
                gamecontrol4players.ins.losingplayerIndex -= 1;
                gamecontrol4players.ins.losingPlayer.GetComponent<followPath4Players>().waypointIndex -= 1;
                print(" moving back after   ------   " + gamecontrol4players.ins.losingPlayer.GetComponent<followPath4Players>().waypointIndex);
            }
        }
        else
        {
            movebackAllowed = false;
            gamecontrol4players.ins.Fightexplosion.SetActive(false);
        }



    }
}
