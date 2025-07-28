using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class winning : MonoBehaviour
{
    public int player1atEnd = 0;
    public int player2atEnd = 0;
    public int player3atEnd = 0;
    public int player4atEnd = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        gamecontrol4players.ins.players[0].GetComponent<PlayersScript>().PlayerAtHome = player1atEnd;
        gamecontrol4players.ins.players[1].GetComponent<PlayersScript>().PlayerAtHome = player2atEnd;
        gamecontrol4players.ins.players[2].GetComponent<PlayersScript>().PlayerAtHome = player3atEnd;
        gamecontrol4players.ins.players[3].GetComponent<PlayersScript>().PlayerAtHome = player4atEnd;

        #region player win condition

        if (player1atEnd > 0 && gamecontrol4players.ins.player1Cards > 0)
        {
            gamecontrol4players.ins.completePanel.SetActive(true);
            gamecontrol4players.ins.whoWinsTextShadow.gameObject.SetActive(true);
            gamecontrol4players.ins.player1MoveText.gameObject.SetActive(false);
            gamecontrol4players.ins.player2MoveText.gameObject.SetActive(false);
            gamecontrol4players.ins.player3MoveText.gameObject.SetActive(false);
            gamecontrol4players.ins.player4MoveText.gameObject.SetActive(false);
            gamecontrol4players.ins.whoWinsTextShadow.GetComponent<Text>().text = "Player 1 Wins";
            gamecontrol4players.ins.gameOver = true;
            Time.timeScale = 0;
        }
        else if (player2atEnd > 0 && gamecontrol4players.ins.player2Cards > 0)
        {
            gamecontrol4players.ins.completePanel.SetActive(true);
            gamecontrol4players.ins.whoWinsTextShadow.gameObject.SetActive(true);
            gamecontrol4players.ins.player1MoveText.gameObject.SetActive(false);
            gamecontrol4players.ins.player2MoveText.gameObject.SetActive(false);
            gamecontrol4players.ins.player3MoveText.gameObject.SetActive(false);
            gamecontrol4players.ins.player4MoveText.gameObject.SetActive(false);
            gamecontrol4players.ins.whoWinsTextShadow.GetComponent<Text>().text = "Player 2 Wins";
            gamecontrol4players.ins.gameOver = true;
            Time.timeScale = 0;
        }

        else if (player3atEnd > 0 && gamecontrol4players.ins.player3Cards > 0)
        {
            gamecontrol4players.ins.completePanel.SetActive(true);
            gamecontrol4players.ins.whoWinsTextShadow.gameObject.SetActive(true);
            gamecontrol4players.ins.player1MoveText.gameObject.SetActive(false);
            gamecontrol4players.ins.player2MoveText.gameObject.SetActive(false);
            gamecontrol4players.ins.player3MoveText.gameObject.SetActive(false);
            gamecontrol4players.ins.player4MoveText.gameObject.SetActive(false);
            gamecontrol4players.ins.whoWinsTextShadow.GetComponent<Text>().text = "Player 3 Wins";
            gamecontrol4players.ins.gameOver = true;
            Time.timeScale = 0;
        }
        else if (player4atEnd > 0 && gamecontrol4players.ins.player4Cards > 0)
        {
            gamecontrol4players.ins.completePanel.SetActive(true);
            gamecontrol4players.ins.whoWinsTextShadow.gameObject.SetActive(true);
            gamecontrol4players.ins.player1MoveText.gameObject.SetActive(false);
            gamecontrol4players.ins.player2MoveText.gameObject.SetActive(false);
            gamecontrol4players.ins.player3MoveText.gameObject.SetActive(false);
            gamecontrol4players.ins.player4MoveText.gameObject.SetActive(false);
            gamecontrol4players.ins.whoWinsTextShadow.GetComponent<Text>().text = "Player 4 Wins";
            gamecontrol4players.ins.gameOver = true;
            Time.timeScale = 0;
        }
        else
        {
            #region winning region

            if (gamecontrol4players.ins.players[0].GetComponent<followPath4Players>().waypointIndex ==
                gamecontrol4players.ins.players[0].GetComponent<followPath4Players>().waypoints.Length)
            {
                player1atEnd = player1atEnd + 1;
                gamecontrol4players.ins.players[0].GetComponent<followPath4Players>().waypointIndex = 0;
                gamecontrol4players.ins.playersStartWaypoint[0] = 0;
                gamecontrol4players.ins.players[0].SetActive(false);
                gamecontrol4players.ins.players[0].transform.position = gamecontrol4players.ins.players[0].GetComponent<followPath4Players>().waypoints[0].position;
                gamecontrol4players.ins.players[0].SetActive(true);
            }

            if (gamecontrol4players.ins.players[1].GetComponent<followPath4Players>().waypointIndex ==
               gamecontrol4players.ins.players[1].GetComponent<followPath4Players>().waypoints.Length)
            {
                player2atEnd = player2atEnd + 1;
                gamecontrol4players.ins.players[1].GetComponent<followPath4Players>().waypointIndex = 0;
                gamecontrol4players.ins.playersStartWaypoint[1] = 0;
                gamecontrol4players.ins.players[1].SetActive(false);
                gamecontrol4players.ins.players[1].transform.position = gamecontrol4players.ins.players[1].GetComponent<followPath4Players>().waypoints[0].position;
                gamecontrol4players.ins.players[1].SetActive(true);
            }

            if (gamecontrol4players.ins.players[2].GetComponent<followPath4Players>().waypointIndex ==
              gamecontrol4players.ins.players[2].GetComponent<followPath4Players>().waypoints.Length)
            {
                player3atEnd = player3atEnd + 1;
                gamecontrol4players.ins.players[2].GetComponent<followPath4Players>().waypointIndex = 0;
                gamecontrol4players.ins.playersStartWaypoint[2] = 0;
                gamecontrol4players.ins.players[2].SetActive(false);
                gamecontrol4players.ins.players[2].transform.position = gamecontrol4players.ins.players[2].GetComponent<followPath4Players>().waypoints[0].position;
                gamecontrol4players.ins.players[2].SetActive(true);
            }

            if (gamecontrol4players.ins.players[3].GetComponent<followPath4Players>().waypointIndex ==
                gamecontrol4players.ins.players[3].GetComponent<followPath4Players>().waypoints.Length)
            {
                player4atEnd = player4atEnd + 1;
                gamecontrol4players.ins.players[3].GetComponent<followPath4Players>().waypointIndex = 0;
                gamecontrol4players.ins.playersStartWaypoint[3] = 0;
                gamecontrol4players.ins.players[3].SetActive(false);
                gamecontrol4players.ins.players[3].transform.position = gamecontrol4players.ins.players[3].GetComponent<followPath4Players>().waypoints[0].position;
                gamecontrol4players.ins.players[3].SetActive(true);
            }

            #endregion
        }
        #endregion




    }
}
