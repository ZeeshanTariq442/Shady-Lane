using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class dice4Players : MonoBehaviour
{
    // --- Variables ---
    private Sprite[] diceSides;
    public int whosTurn = 1;
    private static GameObject player1, player2, player3, player4;
    public static dice4Players ins;

    [Tooltip("Set this to the total number of players/entities in a round (e.g., 5 if you have players 1-4 and a cop).")]
    public int totalEntities = 5;
    public Button rollButton;
    public float delayBetweenAITurns = 1.0f;
    private bool coroutineAllowed = true;
    public Material player1_material, player2_material, player3_material, player4_material;

    // --- NEW VARIABLE TO FIX THE BUG ---
    // This remembers whose turn it was before a fight interrupted the sequence.
    private int turnBeforeFight;

    void Start()
    {
        ins = this;
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");
        player1 = GameObject.Find("Player1");
        player2 = GameObject.Find("Player2");
        player3 = GameObject.Find("Player3");
        player4 = GameObject.Find("Player4");

        if (rollButton != null)
        {
            rollButton.interactable = true;
        }
    }

    public void rollMe()
    {
        if (whosTurn == 1 && coroutineAllowed)
        {
            StartCoroutine(TurnSequenceCoroutine());
        }
    }

    private IEnumerator TurnSequenceCoroutine()
    {
        Debug.Log("yeh chala");
        coroutineAllowed = false;
        if (rollButton != null) rollButton.interactable = false;

        // --- Dice Rolling Animation ---
        int randomDiceSide = 0;
        for (int i = 0; i <= 20; i++)
        {

            Debug.Log("yeh chala" + i);
            randomDiceSide = Random.Range(0, 6);
         
            this.GetComponent<Image>().sprite = diceSides[randomDiceSide];
            yield return new WaitForSeconds(0.05f);
            rollButton.interactable = true;
            
        }

        gamecontrol4players.ins.diceSideThrown = randomDiceSide + 1;

        // --- Main Game Logic with Fix ---
        if (gamecontrol4players.ins.fight)
        {
            
            HandleFightRoll();
            if (gamecontrol4players.ins.fightTurn < 2)
            {
                // Set turn to the second fighter
                whosTurn = gamecontrol4players.ins.Player2fightturnWhilefight;
            }
            else
            {
                // Fight is over, resolve it
                ResolveFight();
                // **FIX:** Restore the turn order to the player AFTER the one who was interrupted.
                whosTurn = turnBeforeFight + 1;
            }
        }
        else
        {
           
            // Normal turn
            HandleNormalTurn();
            CheckForFight();
            if (gamecontrol4players.ins.fight)
            {
                // A fight was JUST detected.
                // **FIX:** Save the current turn before starting the fight sequence.
                turnBeforeFight = whosTurn;

                // Set the turn to the first fighter
                whosTurn = gamecontrol4players.ins.Player1fightturnWhilefight;
            }
            else
            {
                // No fight, normal turn progression.
                whosTurn++;
            }
        }

        // Cycle Turn Number
        if (whosTurn > totalEntities)
        {
            whosTurn = 1;
        }

        // Auto-Roll or Wait for Player
        if (whosTurn == 1)
        {
            print("Your turn, Player 1!");
            coroutineAllowed = true;
            if (rollButton != null) rollButton.interactable = true;
        }
        else
        {
            print("Automatic turn for entity " + whosTurn + "...");
            yield return new WaitForSeconds(delayBetweenAITurns);
            StartCoroutine(TurnSequenceCoroutine());
            print("Your turn, Player !");
        }
    }

    // --- Helper Functions (no changes below this line) ---
    void HandleNormalTurn()
    {
        if (whosTurn == 1) { gamecontrol4players.ins.MovePlayer(1); gamecontrol4players.ins.TasksForPlayer(gamecontrol4players.ins.diceSideThrown + gamecontrol4players.ins.playersStartWaypoint[0], player1); }
        else if (whosTurn == 2) { gamecontrol4players.ins.MovePlayer(2); gamecontrol4players.ins.TasksForPlayer(gamecontrol4players.ins.diceSideThrown + gamecontrol4players.ins.playersStartWaypoint[1], player2); }
        else if (whosTurn == 3) { gamecontrol4players.ins.MovePlayer(3); gamecontrol4players.ins.TasksForPlayer(gamecontrol4players.ins.diceSideThrown + gamecontrol4players.ins.playersStartWaypoint[2], player3); }
        else if (whosTurn == 4) { gamecontrol4players.ins.MovePlayer(4); gamecontrol4players.ins.TasksForPlayer(gamecontrol4players.ins.diceSideThrown + gamecontrol4players.ins.playersStartWaypoint[3], player4); }
        else if (whosTurn == 5) { gamecontrol4players.ins.MovePlayer(5); }
        
    }

    void HandleFightRoll()
    {
        if (whosTurn == gamecontrol4players.ins.Player1fightturnWhilefight) { gamecontrol4players.ins.player1dicenoforfight = gamecontrol4players.ins.diceSideThrown; }
        else if (whosTurn == gamecontrol4players.ins.Player2fightturnWhilefight) { gamecontrol4players.ins.player2dicenoforfight = gamecontrol4players.ins.diceSideThrown; }
        gamecontrol4players.ins.fightTurn++;
    }

    void CheckForFight()
    {
        for (int i = 0; i < gamecontrol4players.ins.players.Length; i++)
        {
            for (int j = i + 1; j < gamecontrol4players.ins.players.Length; j++)
            {
                if (gamecontrol4players.ins.players[i].GetComponent<followPath4Players>().waypointIndex == gamecontrol4players.ins.players[j].GetComponent<followPath4Players>().waypointIndex && gamecontrol4players.ins.players[i].GetComponent<followPath4Players>().waypointIndex != 0)
                {
                    print("FIGHT DETECTED between Player " + (i + 1) + " and Player " + (j + 1));
                    gamecontrol4players.ins.fight = true;
                    gamecontrol4players.ins.player1OnFight = gamecontrol4players.ins.players[i];
                    gamecontrol4players.ins.player2Onfight = gamecontrol4players.ins.players[j];
                    gamecontrol4players.ins.Player1fightturnWhilefight = i + 1;
                    gamecontrol4players.ins.Player2fightturnWhilefight = j + 1;
                    if (gamecontrol4players.ins.Fightexplosion != null)
                    {
                        //gamecontrol4players.ins.Fightexplosion.transform.position = gamecontrol4players.ins.players[i].transform.position;
                        //gamecontrol4players.ins.Fightexplosion.SetActive(true);
                        Vector3 fxPos = gamecontrol4players.ins.players[i].transform.position;
                        fxPos.z = 0f; // important for 2D
                        gamecontrol4players.ins.Fightexplosion.transform.position = fxPos;
                        gamecontrol4players.ins.Fightexplosion.SetActive(true);
                        gamecontrol4players.ins.PlayNewSound(gamecontrol4players.ins.expo_sound);
                         }
                    else { Debug.LogError("Fightexplosion is not assigned!"); }
                    return;
                }
            }
        }
    }

    void ResolveFight()
    {
        GameObject winner = null;
        GameObject loser = null;
        if (gamecontrol4players.ins.player1dicenoforfight > gamecontrol4players.ins.player2dicenoforfight) { winner = gamecontrol4players.ins.player1OnFight; loser = gamecontrol4players.ins.player2Onfight; }
        else if (gamecontrol4players.ins.player2dicenoforfight > gamecontrol4players.ins.player1dicenoforfight) { winner = gamecontrol4players.ins.player2Onfight; loser = gamecontrol4players.ins.player1OnFight; }
        else
        {
            print("DRAW!");
            if (gamecontrol4players.ins.Fightexplosion != null) gamecontrol4players.ins.Fightexplosion.SetActive(false);
            gamecontrol4players.ins.fight = false;
            gamecontrol4players.ins.fightTurn = 0;
            return;
        }
        print(winner.name + " wins the fight!");
        if (gamecontrol4players.ins.Fightexplosion != null) gamecontrol4players.ins.Fightexplosion.SetActive(false);
        loser.GetComponent<followPath4Players>().movebackAllowed = true;
        loser.GetComponent<followPath4Players>().FightMove();
        gamecontrol4players.ins.fight = false;
        gamecontrol4players.ins.fightTurn = 0;
        gamecontrol4players.ins.player1dicenoforfight = 0;
        gamecontrol4players.ins.player2dicenoforfight = 0;
    }

    public void MaterialOn(int turn)
    {
        if (turn == 1)
        {
            player1_material.SetFloat("_OutlineEnabled", 1f);
            player2_material.SetFloat("_OutlineEnabled", 0f);
        }
        else if (turn == 2)
        {
            player3_material.SetFloat("_OutlineEnabled", 1f);
            player2_material.SetFloat("_OutlineEnabled", 0f);
        }
        else if (turn == 3)
        {
            player3_material.SetFloat("_OutlineEnabled", 1f);
            player2_material.SetFloat("_OutlineEnabled", 0f);
        }
        else if (turn == 4)
        {
            player4_material.SetFloat("_OutlineEnabled", 1f);
            player3_material.SetFloat("_OutlineEnabled", 0f);
        }
        else if (turn == 5)
        {
            player1_material.SetFloat("_OutlineEnabled", 1f);
            player4_material.SetFloat("_OutlineEnabled", 0f);
        }
        
    }

    public void OutlineOn(int turn)
    {
        if (turn == 1)
        {
            player1_material.SetFloat("_OutlineEnabled", 1f);
        }
        else
        {
            player1_material.SetFloat("_OutlineEnabled", 0f);
        }
    }
    void OnDisable()
    {
        
            player1_material.SetFloat("_OutlineEnabled", 0f);
            player2_material.SetFloat("_OutlineEnabled", 0f);
            player3_material.SetFloat("_OutlineEnabled", 0f);
            player4_material.SetFloat("_OutlineEnabled", 0f);
    }
}