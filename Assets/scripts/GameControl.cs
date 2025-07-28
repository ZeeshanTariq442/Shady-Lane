using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameControl : MonoBehaviour
{

    public GameObject player1MoveText, player2MoveText, copMoveText;
    [Header("Game Panels")]
    public GameObject whoWinsTextShadow;
    public GameObject completePanel;
    public GameObject Fightexplosion;
    public GameObject pausePanel;
    public int player1atEnd = 0;
    public int player2atEnd = 0;
    [Header("Canvas Objects")]
    public Text player1Moveindex;
    public Text player2Moveindex;
    public Text player1CardsText;
    public Text player2CardsText;
    [Header("WayPoints ")]
    public GameObject[] points;
    public GameObject[] totalBoardwaypoints;
  
    public static GameObject player1, player2, Cop;
    [Header("Tasks")]
    public string[] tasks = {
        "Get Player out of Hospital",
        "Botched Robbery ",
        "Bank Mixup",
        "Bad Drugs",
        "You found 2 products",
        "Caught in sting (Jail) player pays $10",
        "Overdose (dead) lost player",
        "Snitched (back 2)",
        "Cops- All player roll",
        "You gained a player (xtra man)",
        "Conflict (lose next turn)",
        "ROB Contact ",
        "ROB Bank ",
        "Bad Drugs ",
        "Go to liquor store ",
        "ROB grow house ",
        "ROB GunStore ",
        "Bank Mixup receive $10",
        "To Hot (Back To Start)",
        "You found 3 product",
        "Lady came up $5 from every player",
        "Head to Vape ",
        "Get a man out of jail",
        "If man in jail he was killed (player lost)",
        "Drug sweep",
        "Your lucky day ( found $5)",
        "Lucky day move 2",
        "ROB Weed Store ",
        "VigiLante comes from the bar and move like the cop"
    };

    public static int diceSideThrown = 0;
    public int player1StartWaypoint = 1;
    public int player2StartWaypoint = 1;
    public int CopStartWaypoint = 1;
    [Header("Task Panel")]
    public GameObject taskPanel;
    public GameObject taskText;
    public GameObject taskOptions;
    public GameObject taskOk;
    public GameObject startpanel;
    [Header("Sprites")]
    public Sprite[] playerwithGun;

    public Text coins;
    public static bool gameOver = false;
    public bool fight = false;
    public int fightTurn = 0;
    public int fightindexPlayer1, fightindexPlayer2, player1dicenoforfight, player2dicenoforfight, fightLoseNo, losingplayerIndex;
    public int player1Cards, player2Cards;
    public GameObject losingPlayer;

    public GameObject currentPlayer;
    public int currentPlayerNo;
    public bool buygunBool;

    public int currentTask;

    public bool playSound;

    public int coin;
    public GameObject tutorial;
    public static GameControl ins;
    // Use this for initialization
    void Start()
    {

        coin = 100;
        ins = this;
        //   whoWinsTextShadow = GameObject.Find("WhoWinsText");
        player1MoveText = GameObject.Find("Player1MoveText");
        player2MoveText = GameObject.Find("Player2MoveText");
        copMoveText = GameObject.Find("copMoveText");
        player1 = GameObject.Find("Player1");
        player2 = GameObject.Find("Player2");
        Cop = GameObject.Find("Cop");
        player1.GetComponent<FollowThePath>().moveAllowed = false;
        player2.GetComponent<FollowThePath>().moveAllowed = false;

        whoWinsTextShadow.gameObject.SetActive(false);
        player1MoveText.gameObject.SetActive(true);
        player2MoveText.gameObject.SetActive(false);
        copMoveText.gameObject.SetActive(false);
        player1.GetComponent<FollowThePath>().waypointIndex = 1;
        player2.GetComponent<FollowThePath>().waypointIndex = 1;

        player1CardsText.text = player1.ToString();
        player2CardsText.text = player2.ToString();

        Time.timeScale = 0;
        if (PlayerPrefs.GetInt("tutoial") == 0)
        {
            tutorial.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {
        coins.text = coin.ToString();

        player1.GetComponent<PlayersScript>().PlayerAtHome = player1atEnd;
        player2.GetComponent<PlayersScript>().PlayerAtHome = player2atEnd;




        player1Moveindex.text = player1StartWaypoint.ToString();
        player2Moveindex.text = player2StartWaypoint.ToString();

        /// move forward 
        if (player1.GetComponent<FollowThePath>().waypointIndex >
            player1StartWaypoint + diceSideThrown)
        {
            player1.GetComponent<FollowThePath>().moveAllowed = false;
            playSound = false;
            currentPlayer = player1;
            currentPlayerNo = 0;
            player1MoveText.gameObject.SetActive(false);
            player2MoveText.gameObject.SetActive(true);
            copMoveText.gameObject.SetActive(false);
            player1StartWaypoint = player1.GetComponent<FollowThePath>().waypointIndex - 1;
        }

        if (player2.GetComponent<FollowThePath>().waypointIndex >
            player2StartWaypoint + diceSideThrown)
        {
            player2.GetComponent<FollowThePath>().moveAllowed = false;
            playSound = false;
            currentPlayer = player2;
            currentPlayerNo = 1;
            player2MoveText.gameObject.SetActive(false);
            player1MoveText.gameObject.SetActive(false);
            copMoveText.gameObject.SetActive(true);
            player2StartWaypoint = player2.GetComponent<FollowThePath>().waypointIndex - 1;
        }

        if (Cop.GetComponent<FollowThePath>().waypointIndex >
          CopStartWaypoint + diceSideThrown)
        {
            Cop.GetComponent<FollowThePath>().moveAllowed = false;
            playSound = false;
            currentPlayer = Cop;
            currentPlayerNo = 2;
            player2MoveText.gameObject.SetActive(false);
            player1MoveText.gameObject.SetActive(true);
            copMoveText.gameObject.SetActive(false);
            CopStartWaypoint = Cop.GetComponent<FollowThePath>().waypointIndex - 1;
        }


        // end forward 
        //start back
        if (player1.GetComponent<FollowThePath>().waypointIndex <= player1StartWaypoint - fightLoseNo + 1)
        {
            Fightexplosion.SetActive(false);
            player1.GetComponent<FollowThePath>().movebackAllowed = false;
            player1StartWaypoint = player1.GetComponent<FollowThePath>().waypointIndex - 1;
        }
        if (player2.GetComponent<FollowThePath>().waypointIndex <= player2StartWaypoint - fightLoseNo + 1)
        {
            Fightexplosion.SetActive(false);
            player2.GetComponent<FollowThePath>().movebackAllowed = false;
            player2StartWaypoint = player2.GetComponent<FollowThePath>().waypointIndex - 1;
        }
        // end back

        if (player1.GetComponent<FollowThePath>().waypointIndex ==
            player1.GetComponent<FollowThePath>().waypoints.Length)
        {
            player1atEnd = player1atEnd + 1;

            player1.GetComponent<FollowThePath>().waypointIndex = 0;
            player1StartWaypoint = 0;
            player1.SetActive(false);
            player1.transform.position = player1.GetComponent<FollowThePath>().waypoints[0].position;
            player1.SetActive(true);
        }

        if (player2.GetComponent<FollowThePath>().waypointIndex ==
            player2.GetComponent<FollowThePath>().waypoints.Length)
        {
            player2atEnd = player2atEnd + 1;


            player2.GetComponent<FollowThePath>().waypointIndex = 0;
            player2StartWaypoint = 0;
            player2.SetActive(false);
            player2.transform.position = player2.GetComponent<FollowThePath>().waypoints[0].position;
            player2.SetActive(true);
        }


        if (CopStartWaypoint >= Cop.GetComponent<FollowThePath>().waypoints.Length - 1)
        {
            Cop.GetComponent<FollowThePath>().waypointIndex = 0;
            CopStartWaypoint = 0;
            Cop.transform.position = Cop.GetComponent<FollowThePath>().waypoints[0].transform.position;
        }




        if (player1atEnd >= 4)
        {

            completePanel.SetActive(true);
            whoWinsTextShadow.gameObject.SetActive(true);
            whoWinsTextShadow.GetComponent<Text>().text = "Player 1 Wins";
            gameOver = true;
        }
        if (player2atEnd >= 4)
        {

            completePanel.SetActive(true);
            whoWinsTextShadow.gameObject.SetActive(true);
            whoWinsTextShadow.GetComponent<Text>().text = "Player 2 Wins";
            gameOver = true;
        }





        #region player fight
        // checking if both player are on same point 
        // for the fight
        if (player1.GetComponent<FollowThePath>().waypointIndex == player2.GetComponent<FollowThePath>().waypointIndex
            && player1.GetComponent<FollowThePath>().waypointIndex != 1
            && player2.GetComponent<FollowThePath>().waypointIndex != 1)
        {
            print(" fight condition");

            fightindexPlayer1 = player1StartWaypoint;
            fightindexPlayer2 = player2StartWaypoint;
            // check either both are on same point
            if (fightindexPlayer1 == fightindexPlayer2 && fight == false)
            {
                print(" fight");
                Fightexplosion.transform.position = player1.transform.position;
                Fightexplosion.SetActive(true);
                fight = true;
            }
        }
        // in fight both player roll dice the one with high number wins
        if (fightTurn >= 2 && fight == true)
        {

            if (player1dicenoforfight > player2dicenoforfight)
            {
                print("player 1 wins"); fight = false;
                fightLoseNo = player1dicenoforfight;
                MovePlayer(1);
                losingplayerIndex = player2StartWaypoint;
                losingPlayer = player2;
                player1.GetComponent<FollowThePath>().moveAllowed = true;
                //   player2.GetComponent<FollowThePath>().FightMove(player2StartWaypoint);
                player2.GetComponent<FollowThePath>().FightMove();
                player2.GetComponent<FollowThePath>().movebackAllowed = true;

                fightTurn = 0;
                player1dicenoforfight = 0;
                player2dicenoforfight = 0;
            }
            else if (player2dicenoforfight > player1dicenoforfight)
            {
                print("player 2 wins"); fight = false;
                fightLoseNo = player2dicenoforfight;
                MovePlayer(2);
                losingplayerIndex = player1StartWaypoint;
                losingPlayer = player1;
                player2.GetComponent<FollowThePath>().moveAllowed = true;
                player1.GetComponent<FollowThePath>().FightMove();
                player1.GetComponent<FollowThePath>().movebackAllowed = true;
                //   player1.GetComponent<FollowThePath>().FightMove(player1StartWaypoint);

                fightTurn = 0;
                player1dicenoforfight = 0;
                player2dicenoforfight = 0;
            }

        }// end of  if (fightTurn >= 2 && fight==true)
        #endregion
        // if cop and player have same position
        #region cop kill player
        if (player1.transform.position == Cop.transform.position && Cop.GetComponent<FollowThePath>().moveAllowed == false
            && player1.GetComponent<FollowThePath>().moveAllowed == false)
        {
            print(" player1 lost 1 life");
            player1Cards = 0;
            player1.GetComponent<PlayersScript>().playerweed = 0;
            player1.transform.GetChild(1).gameObject.SetActive(false);
            player1.transform.GetChild(0).gameObject.SetActive(true);
            player1.SetActive(false);
            player1.GetComponent<FollowThePath>().moveAllowed = false;
            player1.GetComponent<FollowThePath>().waypointIndex = 0;
            player1.transform.position = player1.GetComponent<FollowThePath>().waypoints[0].position;
            player1.SetActive(true);
            player1.GetComponent<PlayersScript>().PlayerAtHome = player1.GetComponent<PlayersScript>().PlayerAtHome - 1;
            player1.transform.GetChild(0).gameObject.SetActive(true);
            Invoke("RelivePlayer", 2f);
        }
        if (player2.transform.position == Cop.transform.position && Cop.GetComponent<FollowThePath>().moveAllowed == false
            && player2.GetComponent<FollowThePath>().moveAllowed == false)
        {
            print(" player2 lost 1 life");
            player2Cards = 0;
            player2.GetComponent<PlayersScript>().playerweed = 0;
            player2.transform.GetChild(1).gameObject.SetActive(false);
            player2.transform.GetChild(0).gameObject.SetActive(true);
            player2.GetComponent<FollowThePath>().moveAllowed = false;
            player2.SetActive(false);
            player2.GetComponent<FollowThePath>().waypointIndex = 0;
            player2.transform.position = player2.GetComponent<FollowThePath>().waypoints[0].position;
            player2.SetActive(true);
            player2.GetComponent<PlayersScript>().PlayerAtHome = player2.GetComponent<PlayersScript>().PlayerAtHome - 1;
            Invoke("RelivePlayer", 2f);
        }
        #endregion

        // update no of cards
        player1CardsText.text = player1.GetComponent<PlayersScript>().myCards.ToString();
        player2CardsText.text = player2.GetComponent<PlayersScript>().myCards.ToString();

    }

    public static void MovePlayer(int playerToMove)
    {
        // moving the player who has rolled the dice
        switch (playerToMove)
        {

            case 1:
                player1.GetComponent<FollowThePath>().moveAllowed = true;

                GameControl.ins.playSound = true;
                break;

            case 2:
                player2.GetComponent<FollowThePath>().moveAllowed = true;
                GameControl.ins.playSound = true;
                break;
            case 3:
                Cop.GetComponent<FollowThePath>().moveAllowed = true;
                GameControl.ins.playSound = true;
                break;
        }
    }

    public void TasksForPlayer(int waypointsindex, GameObject player)
    {
        if (waypointsindex == 9 || waypointsindex == 115 || waypointsindex == 45 || waypointsindex == 67)
        {

            taskText.GetComponent<Text>().text = "  You found 5 weed products";
            //taskOptions.SetActive(true);
            taskOk.SetActive(true);
            taskPanel.SetActive(true);
            print("player who got weed is : " + player.transform.tag);
            switch (player.transform.tag)
            {
                case "player1":
                    player1Cards = player1Cards + 5;
                    player1CardsText.text = player1Cards.ToString();
                    player.GetComponent<PlayersScript>().playerweed = player1Cards;
                    player.GetComponent<PlayersScript>().playerweedText.text = player.GetComponent<PlayersScript>().playerweed.ToString();
                    break;
                case "player2":
                    player2Cards = player2Cards + 5;
                    player2CardsText.text = player2Cards.ToString();
                    player.GetComponent<PlayersScript>().playerweed = player2Cards;
                    player.GetComponent<PlayersScript>().playerweedText.text = player.GetComponent<PlayersScript>().playerweed.ToString();
                    break;

            }
        }
        if (waypointsindex == 18 || waypointsindex == 72 || waypointsindex == 101)
        {
            // gun store

            taskText.GetComponent<Text>().text = " You got A Gun ";
            taskOk.SetActive(true);
            taskPanel.SetActive(true);
            buygunBool = true;
            player.transform.GetChild(1).gameObject.SetActive(true);
            player.transform.GetChild(0).gameObject.SetActive(false);

        }

    }

    public void replay()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
        gameOver = false;
        player1.GetComponent<FollowThePath>().waypointIndex = 0;
        player2.GetComponent<FollowThePath>().waypointIndex = 0;
    }
    public void home()
    {

        SceneManager.LoadScene(0);
        gameOver = false;
        player1.GetComponent<FollowThePath>().waypointIndex = 0;
        player2.GetComponent<FollowThePath>().waypointIndex = 0;
    }

    public void no()
    {
        taskPanel.SetActive(false);
    }
    public void startbutton()
    {
        startpanel.SetActive(false);
        Time.timeScale = 1;
        movementsound2players.ins.musicSource.clip = movementsound2players.ins.musicClip;
        movementsound2players.ins.musicSource.Play();
        //my logic 
        // Pause music
        movementsound2players.ins.musicSource.Pause();

        // Resume music
        movementsound2players.ins.musicSource.UnPause();
    }
    public void yes()
    {
        //switch (currentTask)
        //{
        //    case 0:
        //         // "Get Player out of Hospital"
        //        if(currentPlayer.GetComponent<PlayersScript>().PlayerAtHospital > 0)
        //        {
        //            currentPlayer.GetComponent<PlayersScript>().PlayerAtHospital = currentPlayer.GetComponent<PlayersScript>().PlayerAtHospital - 1;
        //        }
        //        break;
        //    case 1:
        //        //"Botched Robbery ",

        //        break;
        //    case 2:
        //       // "Bank Mixup",

        //            break;
        //    case 3:
        //        // "Bad Drugs",

        //        break;
        //    case 4:
        //        //"You found 2 products",

        //        break;
        //    case 5:
        //        //"Caught in sting (Jail) player pays $10",

        //        break;
        //    case 6:
        //        // "Overdose (dead) lost player",
        //        currentPlayer.GetComponent<PlayersScript>().PlayerAtHome = currentPlayer.GetComponent<PlayersScript>().PlayerAtHome - 1;
        //        break;
        //    case 7:
        //        //"Snitched (back 2)",

        //        break;
        //    case 8:
        //        //"Cops- All player roll",

        //        break;
        //    case 9:
        //        //"You gained a player (xtra man)",
        //        currentPlayer.GetComponent<PlayersScript>().PlayerAtHome = currentPlayer.GetComponent<PlayersScript>().PlayerAtHome + 1;
        //        currentPlayer.GetComponent<PlayersScript>().PlayerAtHomeText.text = currentPlayer.GetComponent<PlayersScript>().PlayerAtHome.ToString();
        //        break;
        //    case 10:
        //        // "Conflict (lose next turn)",

        //        break;
        //    case 11:
        //        // "ROB Contact ",

        //        break;
        //    case 12:
        //        // "ROB Bank ",
        //        currentPlayer.GetComponent<FollowThePath>().waypointIndex = 4;
        //        currentPlayer.GetComponent<FollowThePath>().moveAllowed = false;
        //        break;
        //    case 13:
        //        // "Bad Drugs ",

        //        break;
        //    case 14:
        //        //"Go to liquor store ",
        //        print("move to liqour store");
        //        currentPlayer.GetComponent<FollowThePath>().waypointIndex = 53;
        //        currentPlayer.GetComponent<FollowThePath>().moveAllowed = false;
        //        break;
        //    case 15:
        //        // "ROB grow house ",
        //        currentPlayer.GetComponent<FollowThePath>().waypointIndex = 44;
        //        currentPlayer.GetComponent<FollowThePath>().moveAllowed = false;
        //        break;
        //    case 16:
        //        // "ROB GunStore ",
        //        currentPlayer.GetComponent<FollowThePath>().waypointIndex = 75;
        //        currentPlayer.GetComponent<FollowThePath>().moveAllowed = false;
        //        break;
        //    case 17:
        //        // "Bank Mixup receive $10",

        //        break;
        //    case 18:
        //        //"To Hot (Back To Start)",
        //        print("move to start ");
        //        currentPlayer.GetComponent<FollowThePath>().waypointIndex = 0;
        //        currentPlayer.GetComponent<FollowThePath>().moveAllowed = false;
        //        break;
        //    case 19:
        //        //"You found 3 product",

        //        break;
        //    case 20:
        //        //"Lady came up $5 from every player",

        //        break;
        //    case 21:
        //        // "Head to Vape ",
        //        // waypoint no 39 
        //        print("head to vape");
        //        currentPlayer.GetComponent<FollowThePath>().waypointIndex = 40;
        //        currentPlayer.GetComponent<FollowThePath>().moveAllowed = false;
        //        break;
        //    case 22:
        //        //"Get a man out of jail",
        //        if (currentPlayer.GetComponent<PlayersScript>().playerInJail > 0)
        //        {
        //            currentPlayer.GetComponent<PlayersScript>().playerInJail = currentPlayer.GetComponent<PlayersScript>().playerInJail - 1;
        //            currentPlayer.GetComponent<PlayersScript>().PlayerAtHome = currentPlayer.GetComponent<PlayersScript>().PlayerAtHome + 1;
        //            print(" player in jail");
        //        }
        //        break;
        //    case 23:
        //        // "If man in jail he was killed (player lost)",
        //        if(currentPlayer.GetComponent<PlayersScript>().playerInJail > 0)
        //        {
        //            currentPlayer.GetComponent<PlayersScript>().playerInJail = currentPlayer.GetComponent<PlayersScript>().playerInJail - 1;
        //            print(" player in jail");
        //        }
        //        break;
        //    case 24:
        //        // "Drug sweep",

        //        break;
        //    case 25:
        //        //"Your lucky day ( found $5)",

        //        break;
        //    case 26:
        //        //   "Lucky day move 2",
        //        currentPlayer.GetComponent<FollowThePath>().waypointIndex = currentPlayer.GetComponent<FollowThePath>().waypointIndex + 2;
        //        currentPlayer.transform.position = currentPlayer.GetComponent<FollowThePath>().waypoints[currentPlayer.GetComponent<FollowThePath>().waypointIndex + 2].position;
        //        currentPlayer.GetComponent<FollowThePath>().moveAllowed = false;
        //        break;
        //    case 27:
        //        //    "ROB Weed Store ",
        //        print("weed store");
        //        currentPlayer.GetComponent<FollowThePath>().waypointIndex = 9;
        //        currentPlayer.GetComponent<FollowThePath>().moveAllowed = false;
        //        break;
        //    case 28:
        //        //"VigiLante comes from the bar and move like the cop"

        //        break;
        //    default:
        //        taskPanel.SetActive(false);
        //        break;
        //}
        if (buygunBool == true)
        {
            currentPlayer.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = playerwithGun[currentPlayerNo];
            currentPlayer.GetComponent<PlayersScript>().haveGun = true;
        }
        taskPanel.SetActive(false);
    }
    public void PlayerTut()
    {
        PlayerPrefs.SetInt("tutoial", 1);
        tutorial.SetActive(false);
    }
    //Relive the player 
    void RelivePlayer()
    {
        if (player1.transform.GetChild(0).gameObject.activeInHierarchy == false)
        {
            player1.transform.GetChild(0).gameObject.SetActive(true);

        }
        else if (player2.transform.GetChild(0).gameObject.activeInHierarchy == false)
        {
            player2.transform.GetChild(0).gameObject.SetActive(true);

        }
    }
}
