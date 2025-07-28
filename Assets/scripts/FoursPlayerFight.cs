using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoursPlayerFight : MonoBehaviour
{
    public int player1no;
    public int player2no;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        #region player fight




        if (gamecontrol4players.ins.players[0].GetComponent<followPath4Players>().moveAllowed == false
            && gamecontrol4players.ins.players[1].GetComponent<followPath4Players>().moveAllowed == false
            && gamecontrol4players.ins.players[2].GetComponent<followPath4Players>().moveAllowed == false
            && gamecontrol4players.ins.players[3].GetComponent<followPath4Players>().moveAllowed == false)
        {

            for (int i = 0; i < gamecontrol4players.ins.players.Length; i++)
            {
                for (int j = i + 1; j < gamecontrol4players.ins.players.Length; j++)
                {
                    if (gamecontrol4players.ins.players[i].GetComponent<followPath4Players>().waypointIndex == gamecontrol4players.ins.players[j].GetComponent<followPath4Players>().waypointIndex
                        && gamecontrol4players.ins.fight == false && gamecontrol4players.ins.players[i].GetComponent<followPath4Players>().moveAllowed == false
                        && gamecontrol4players.ins.players[j].GetComponent<followPath4Players>().moveAllowed == false)
                    {
                        gamecontrol4players.ins.fightindexPlayer1 = gamecontrol4players.ins.playersStartWaypoint[i];
                        gamecontrol4players.ins.fightindexPlayer2 = gamecontrol4players.ins.playersStartWaypoint[i];
                        if (gamecontrol4players.ins.fightindexPlayer1 != 0 || gamecontrol4players.ins.fightindexPlayer1 != 0)
                        {
                            gamecontrol4players.ins.Fightexplosion.transform.position = gamecontrol4players.ins.players[i].transform.position;
                            gamecontrol4players.ins.player1OnFight = gamecontrol4players.ins.players[i];
                            gamecontrol4players.ins.player2Onfight = gamecontrol4players.ins.players[j];
                            gamecontrol4players.ins.Player1fightturnWhilefight = i + 1;
                            gamecontrol4players.ins.Player2fightturnWhilefight = j + 1;
                            gamecontrol4players.ins.Fightexplosion.SetActive(true);
                            print("fight explosion");
                            gamecontrol4players.ins.fight = true;
                            player1no = i + 1;
                            player2no = j + 1;
                        }
                        print(" fight");


                    }
                }
            }

        }

        // in fight both player roll dice the one with high number wins
        if (gamecontrol4players.ins.fightTurn >= 2 && gamecontrol4players.ins.fight == true)
        {

            if (gamecontrol4players.ins.player1dicenoforfight > gamecontrol4players.ins.player2dicenoforfight)
            {
                print(gamecontrol4players.ins.player1OnFight + " wins"); gamecontrol4players.ins.fight = false;
                gamecontrol4players.ins.fightLoseNo = gamecontrol4players.ins.player1dicenoforfight;
                gamecontrol4players.ins.MovePlayer(1);
                gamecontrol4players.ins.losingplayerIndex = gamecontrol4players.ins.player2Onfight.GetComponent<followPath4Players>().waypointIndex;
                gamecontrol4players.ins.losingPlayer = gamecontrol4players.ins.player2Onfight;
                for (int i = 0; i < gamecontrol4players.ins.players.Length; i++)
                {
                    gamecontrol4players.ins.players[i].GetComponent<followPath4Players>().moveAllowed = false;
                }
                gamecontrol4players.ins.player1OnFight.GetComponent<followPath4Players>().moveAllowed = true;
                //   player2.GetComponent<followPath4Players>().FightMove(player2StartWaypoint); 
                gamecontrol4players.ins.player2Onfight.GetComponent<followPath4Players>().movebackAllowed = true;
                gamecontrol4players.ins.player2Onfight.GetComponent<followPath4Players>().FightMove();

                print(" losing player index " + gamecontrol4players.ins.losingplayerIndex);

                gamecontrol4players.ins.fightTurn = 0;
                //gamecontrol4players.ins.player1dicenoforfight = 0;
                //gamecontrol4players.ins.player2dicenoforfight = 0;

            }
            else if (gamecontrol4players.ins.player2dicenoforfight > gamecontrol4players.ins.player1dicenoforfight)
            {
                print(gamecontrol4players.ins.player2Onfight + " wins"); gamecontrol4players.ins.fight = false;
                gamecontrol4players.ins.fightLoseNo = gamecontrol4players.ins.player2dicenoforfight;
                gamecontrol4players.ins.MovePlayer(2);
                gamecontrol4players.ins.losingplayerIndex = gamecontrol4players.ins.player1OnFight.GetComponent<followPath4Players>().waypointIndex;
                gamecontrol4players.ins.losingPlayer = gamecontrol4players.ins.player1OnFight;

                for(int i = 0; i < gamecontrol4players.ins.players.Length; i++)
                {
                    gamecontrol4players.ins.players[i].GetComponent<followPath4Players>().moveAllowed = false;
                }

                gamecontrol4players.ins.player2Onfight.GetComponent<followPath4Players>().moveAllowed = true;

                gamecontrol4players.ins.player1OnFight.GetComponent<followPath4Players>().movebackAllowed = true;
                gamecontrol4players.ins.player1OnFight.GetComponent<followPath4Players>().FightMove();
                //   player1.GetComponent<followPath4Players>().FightMove(player1StartWaypoint);

                gamecontrol4players.ins.fightTurn = 0;
                //gamecontrol4players.ins.player1dicenoforfight = 0;
                //gamecontrol4players.ins.player2dicenoforfight = 0;

            }

        }// end of  if (fightTurn >= 2 && fight==true)
        #endregion




        if (gamecontrol4players.ins.player1OnFight != null && gamecontrol4players.ins.player2Onfight != null)
        {
            if (gamecontrol4players.ins.player1OnFight.GetComponent<followPath4Players>().movebackAllowed == true
                &&
                 gamecontrol4players.ins.player1OnFight.GetComponent<followPath4Players>().waypointIndex < 
                 gamecontrol4players.ins.fightindexPlayer1 - gamecontrol4players.ins.fightLoseNo + 1)
            {
                gamecontrol4players.ins.Fightexplosion.SetActive(false);
                gamecontrol4players.ins.player1OnFight.GetComponent<followPath4Players>().movebackAllowed = false;
                gamecontrol4players.ins.playersStartWaypoint[player1no] = gamecontrol4players.ins.losingPlayer.GetComponent<followPath4Players>().waypointIndex - 1;
               
            }
            if (gamecontrol4players.ins.player2Onfight.GetComponent<followPath4Players>().movebackAllowed == true &&
                 gamecontrol4players.ins.player2Onfight.GetComponent<followPath4Players>().waypointIndex <
                 gamecontrol4players.ins.fightindexPlayer2 - gamecontrol4players.ins.fightLoseNo + 1)
            {
                gamecontrol4players.ins.Fightexplosion.SetActive(false);
                gamecontrol4players.ins.player2Onfight.GetComponent<followPath4Players>().movebackAllowed = false;
                gamecontrol4players.ins.playersStartWaypoint[player2no] = gamecontrol4players.ins.losingPlayer.GetComponent<followPath4Players>().waypointIndex - 1;
             
            }
            //gamecontrol4players.ins.player2Onfight = null;
            //gamecontrol4players.ins.player1OnFight = null;
        }



      






    }
}
