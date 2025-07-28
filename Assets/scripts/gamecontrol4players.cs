using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class gamecontrol4players : MonoBehaviour
{
    [Header("Move turn text")]
    public GameObject player1MoveText;
    public GameObject player2MoveText;
    public GameObject copMoveText;
    public GameObject player3MoveText;
    public GameObject player4MoveText;
    public PlayersScript player1_text;
    [Header("Game Panels")]
    public GameObject whoWinsTextShadow;
    public GameObject completePanel;
    public GameObject Fightexplosion;
    public GameObject pausePanel;
    [Header("Canvas Objects")]
    public Text player1Moveindex;
    public Text player2Moveindex;
    public Text player3Moveindex;
    public Text player4Moveindex;
    public Text player1CardsText;
    public Text player2CardsText;
    public Text player3CardsText;
    public Text player4CardsText;
    [Header("WayPoints ")]
    public GameObject[] points;
    public GameObject[] totalBoardwaypoints;

    public GameObject Cop;
    public GameObject Cop1;
    public GameObject Cop2, tutorial;
    public GameObject[] players;
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

    public int diceSideThrown = 0;
    public int[] playersStartWaypoint = { 0, 0, 0, 0 };
    public int CopStartWaypoint = 0;
    public int Cop1StartWaypoint = 0;
    public int Cop2StartWaypoint = 0;
    [Header("Task Panel")]
    public GameObject taskPanel;
    public GameObject taskText;
    public GameObject taskOptions;
    public GameObject taskOk;
    public GameObject startpanel;
    [Header("Sprites")]
    public Sprite[] playerwithGun;

    public Text coins;
    public bool gameOver = false;

    [Header("Fight")]
    public bool fight = false;
    public int fightTurn = 0;
    public int fightindexPlayer1;
    public int fightindexPlayer2;
    public int player1dicenoforfight;
    public int player2dicenoforfight;
    public int fightLoseNo;
    public int losingplayerIndex;
    public GameObject player1OnFight;
    public GameObject player2Onfight;
    public int Player1fightturnWhilefight = 0;
    public int Player2fightturnWhilefight = 0;

    [Header("Player Canvas Objects")]
    public int player1Cards;
    public int player2Cards;
    public int player3Cards;
    public int player4Cards;

    public GameObject losingPlayer;

    public GameObject currentPlayer;
    public int currentPlayerNo;

    public int currentTask;

    public bool buygunBool;
    public bool playSound;
    public int coin;

    // check the player pick gun for all 4 player 
    public bool player1_pick_gun, player2_pick_gun, player3_pick_gun, player4_pick_gun;

    public AudioSource new_sound;
    public AudioClip expo_sound,fail_sound;
    public static gamecontrol4players ins;
    // Use this for initialization
    void Start()
    {

        coin = 100;
        ins = this;

        if (PlayerPrefs.GetInt("tut") == 0)
        {
            tutorial.SetActive(true);
            PlayerPrefs.SetInt("tut", 1);
        }
        //   whoWinsTextShadow = GameObject.Find("WhoWinsText");
        player1MoveText = GameObject.Find("Player1MoveText");
        player2MoveText = GameObject.Find("Player2MoveText");
        player3MoveText = GameObject.Find("Player3MoveText");
        player4MoveText = GameObject.Find("Player4MoveText");
        copMoveText = GameObject.Find("copMoveText");

        players[0] = GameObject.Find("Player1");
        players[1] = GameObject.Find("Player2");
        players[2] = GameObject.Find("Player3");
        players[3] = GameObject.Find("Player4");
        Cop = GameObject.Find("Cop");
        Cop1 = GameObject.Find("Cop1");
        Cop2 = GameObject.Find("Cop2");

        players[0].GetComponent<followPath4Players>().moveAllowed = false;
        players[1].GetComponent<followPath4Players>().moveAllowed = false;
        players[2].GetComponent<followPath4Players>().moveAllowed = false;
        players[3].GetComponent<followPath4Players>().moveAllowed = false;


        whoWinsTextShadow.gameObject.SetActive(false);
        player1MoveText.gameObject.SetActive(true);
        player2MoveText.gameObject.SetActive(false);
        player3MoveText.gameObject.SetActive(false);
        player4MoveText.gameObject.SetActive(false);
        copMoveText.gameObject.SetActive(false);




        player1CardsText.text = player1Cards.ToString();
        player2CardsText.text = player2Cards.ToString();
        player3CardsText.text = player3Cards.ToString();
        player4CardsText.text = player4Cards.ToString();
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        coins.text = coin.ToString();

        //player1Moveindex.text = player1.GetComponent<followPath4Players>().waypointIndex.ToString();
        //player2Moveindex.text = player2.GetComponent<followPath4Players>().waypointIndex.ToString();

        player1Moveindex.text = playersStartWaypoint[0].ToString();
        player2Moveindex.text = playersStartWaypoint[1].ToString();
        player3Moveindex.text = playersStartWaypoint[2].ToString();
        player4Moveindex.text = playersStartWaypoint[3].ToString();

        player1CardsText.text = player1Cards.ToString();
        player2CardsText.text = player2Cards.ToString();
        player3CardsText.text = player3Cards.ToString();
        player4CardsText.text = player4Cards.ToString();


        if (playersStartWaypoint[0] <= 0)
        {
            playersStartWaypoint[0] = 0;
        }
        if (playersStartWaypoint[1] <= 0)
        {
            playersStartWaypoint[1] = 0;
        }
        if (playersStartWaypoint[2] <= 0)
        {
            playersStartWaypoint[2] = 0;
        }
        if (playersStartWaypoint[3] <= 0)
        {
            playersStartWaypoint[3] = 0;
        }
        //     && players[0].GetComponent<Transform>().position == players[0].GetComponent<followPath4Players>().waypoints[playersStartWaypoint[0] + diceSideThrown - 1].position






        #region Moving

        //StopMoving();
        if (players[0].GetComponent<followPath4Players>().waypointIndex >
            playersStartWaypoint[0] + diceSideThrown)
        {
            players[0].GetComponent<followPath4Players>().moveAllowed = false;
            playersStartWaypoint[0] = players[0].GetComponent<followPath4Players>().waypointIndex;
            playSound = false;
            //   players[0].GetComponent<Transform>().position = players[0].GetComponent<followPath4Players>().waypoints[playersStartWaypoint[0] + diceSideThrown+1].position;
            currentPlayer = players[0];
            currentPlayerNo = 0;
            player1MoveText.gameObject.SetActive(false);
            player2MoveText.gameObject.SetActive(true);
            player3MoveText.gameObject.SetActive(false);
            player4MoveText.gameObject.SetActive(false);
            copMoveText.gameObject.SetActive(false);

            players[0].transform.position = players[0].GetComponent<followPath4Players>().waypoints[players[0].GetComponent<followPath4Players>().waypointIndex - 1].position;
            playersStartWaypoint[0] = players[0].GetComponent<followPath4Players>().waypointIndex - 1;
            diceSideThrown = 0;
        }

        if (players[1].GetComponent<followPath4Players>().waypointIndex >
            playersStartWaypoint[1] + diceSideThrown)
        {
            players[1].GetComponent<followPath4Players>().moveAllowed = false;
            playersStartWaypoint[1] = players[1].GetComponent<followPath4Players>().waypointIndex;
            playSound = false;
            // players[1].GetComponent<Transform>().position = players[1].GetComponent<followPath4Players>().waypoints[playersStartWaypoint[1] + diceSideThrown + 1].position;
            currentPlayer = players[1];
            currentPlayerNo = 1;

            player1MoveText.gameObject.SetActive(false);
            player2MoveText.gameObject.SetActive(false);
            player3MoveText.gameObject.SetActive(true);
            player4MoveText.gameObject.SetActive(false);
            copMoveText.gameObject.SetActive(false);

            players[1].transform.position = players[1].GetComponent<followPath4Players>().waypoints[players[1].GetComponent<followPath4Players>().waypointIndex - 1].position;
            playersStartWaypoint[1] = players[1].GetComponent<followPath4Players>().waypointIndex - 1;
            diceSideThrown = 0;
        }


        if (players[2].GetComponent<followPath4Players>().waypointIndex >
         playersStartWaypoint[2] + diceSideThrown)
        {
            players[2].GetComponent<followPath4Players>().moveAllowed = false;
            playersStartWaypoint[2] = players[2].GetComponent<followPath4Players>().waypointIndex;
            playSound = false;
            //  players[2].GetComponent<Transform>().position = players[2].GetComponent<followPath4Players>().waypoints[playersStartWaypoint[2] + diceSideThrown + 1].position;

            currentPlayer = players[2];
            currentPlayerNo = 2;

            player1MoveText.gameObject.SetActive(false);
            player2MoveText.gameObject.SetActive(false);
            player3MoveText.gameObject.SetActive(false);
            player4MoveText.gameObject.SetActive(true);
            copMoveText.gameObject.SetActive(false);

            playersStartWaypoint[2] = players[2].GetComponent<followPath4Players>().waypointIndex - 1;
            players[2].transform.position = players[2].GetComponent<followPath4Players>().waypoints[players[2].GetComponent<followPath4Players>().waypointIndex - 1].position;
            diceSideThrown = 0;
        }

        if (players[3].GetComponent<followPath4Players>().waypointIndex >
            playersStartWaypoint[3] + diceSideThrown)
        {
            players[3].GetComponent<followPath4Players>().moveAllowed = false;
            playersStartWaypoint[3] = players[3].GetComponent<followPath4Players>().waypointIndex;
            playSound = false;
            //    players[3].GetComponent<Transform>().position = players[3].GetComponent<followPath4Players>().waypoints[playersStartWaypoint[3] + diceSideThrown + 1].position;

            currentPlayer = players[3];
            currentPlayerNo = 3;

            player1MoveText.gameObject.SetActive(false);
            player2MoveText.gameObject.SetActive(false);
            player3MoveText.gameObject.SetActive(false);
            player4MoveText.gameObject.SetActive(false);
            copMoveText.gameObject.SetActive(true);

            players[3].transform.position = players[3].GetComponent<followPath4Players>().waypoints[players[3].GetComponent<followPath4Players>().waypointIndex - 1].position;
            playersStartWaypoint[3] = players[3].GetComponent<followPath4Players>().waypointIndex - 1;
            diceSideThrown = 0;
        }

        if (Cop.GetComponent<followPath4Players>().waypointIndex >
          CopStartWaypoint + diceSideThrown &&
          Cop1.GetComponent<followPath4Players>().waypointIndex >
          Cop1StartWaypoint + diceSideThrown)
        {
            Cop.GetComponent<followPath4Players>().moveAllowed = false;
            Cop1.GetComponent<followPath4Players>().moveAllowed = false;
            Cop2.GetComponent<followPath4Players>().moveAllowed = false;
            playSound = false;
            float step = 3 * Time.deltaTime;
            Cop.GetComponent<Transform>().position = Vector3.MoveTowards(Cop.GetComponent<Transform>().position, Cop.GetComponent<followPath4Players>().waypoints[CopStartWaypoint + diceSideThrown + 1].position, step);
            Cop1.GetComponent<Transform>().position = Vector3.MoveTowards(Cop1.GetComponent<Transform>().position, Cop1.GetComponent<followPath4Players>().waypoints[Cop1StartWaypoint + diceSideThrown + 1].position, step);
            Cop2.GetComponent<Transform>().position = Vector3.MoveTowards(Cop2.GetComponent<Transform>().position, Cop2.GetComponent<followPath4Players>().waypoints[Cop2StartWaypoint + diceSideThrown + 1].position, step);


            //Cop.GetComponent<Transform>().position = Cop.GetComponent<followPath4Players>().waypoints[CopStartWaypoint + diceSideThrown + 1].position;
            //Cop1.GetComponent<Transform>().position = Cop1.GetComponent<followPath4Players>().waypoints[Cop1StartWaypoint + diceSideThrown + 1].position;
            currentPlayer = Cop;
            currentPlayerNo = 4;

            player1MoveText.gameObject.SetActive(true);
            player2MoveText.gameObject.SetActive(false);
            player3MoveText.gameObject.SetActive(false);
            player4MoveText.gameObject.SetActive(false);
            copMoveText.gameObject.SetActive(false);

            CopStartWaypoint = Cop.GetComponent<followPath4Players>().waypointIndex - 1;
            Cop1StartWaypoint = Cop1.GetComponent<followPath4Players>().waypointIndex - 1;
            Cop2StartWaypoint = Cop2.GetComponent<followPath4Players>().waypointIndex - 1;

            diceSideThrown = 0;
        }
        //if (Cop1.GetComponent<followPath4Players>().waypointIndex >
        //  Cop1StartWaypoint + diceSideThrown )
        //{
        //    Cop1.GetComponent<followPath4Players>().moveAllowed = false;


        //    currentPlayerNo = 4;

        //    player1MoveText.gameObject.SetActive(true);
        //    player2MoveText.gameObject.SetActive(false);
        //    player3MoveText.gameObject.SetActive(false);
        //    player4MoveText.gameObject.SetActive(false);
        //    copMoveText.gameObject.SetActive(false);

        //    Cop1StartWaypoint = Cop1.GetComponent<followPath4Players>().waypointIndex - 1;
        //    diceSideThrown = 0;
        //}

        // end forward 

        #endregion


        // if cop and player have same position
        #region cop kill player




        if (players[0].transform.position == Cop.transform.position && Cop.GetComponent<followPath4Players>().moveAllowed == false
            && players[0].GetComponent<followPath4Players>().moveAllowed == false)
        {
            print(" player1 lost 1 life");
            player1Cards = 0;
            players[0].GetComponent<PlayersScript>().playerweed = 0;
            PlayNewSound(fail_sound);
            if (player1_pick_gun == true)
            {
                players[0].transform.GetChild(0).gameObject.SetActive(false);
                players[0].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                players[0].transform.GetChild(1).gameObject.SetActive(false);
                players[0].transform.GetChild(0).gameObject.SetActive(true);
            }
          //  players[0].transform.GetChild(1).gameObject.SetActive(false);
          //  players[0].transform.GetChild(0).gameObject.SetActive(true);
            players[0].SetActive(false);
            players[0].GetComponent<followPath4Players>().moveAllowed = false;
            players[0].GetComponent<followPath4Players>().waypointIndex = 0;
            playersStartWaypoint[0] = 0;
            players[0].transform.position = players[0].GetComponent<followPath4Players>().waypoints[0].position;
            players[0].SetActive(true);
            players[0].GetComponent<PlayersScript>().PlayerAtHome = players[0].GetComponent<PlayersScript>().PlayerAtHome - 1;

        }
        if (players[1].transform.position == Cop.transform.position && Cop.GetComponent<followPath4Players>().moveAllowed == false
            && players[1].GetComponent<followPath4Players>().moveAllowed == false)
        {
             PlayNewSound(fail_sound);
            print(" player2 lost 1 life");
            player2Cards = 0;
            players[1].GetComponent<PlayersScript>().playerweed = 0;
            players[1].transform.GetChild(1).gameObject.SetActive(false);
            players[1].transform.GetChild(0).gameObject.SetActive(true);
            players[1].GetComponent<followPath4Players>().moveAllowed = false;
            players[1].SetActive(false);
            players[1].GetComponent<followPath4Players>().waypointIndex = 0;
            playersStartWaypoint[1] = 0;
            players[1].transform.position = players[1].GetComponent<followPath4Players>().waypoints[0].position;
            players[1].SetActive(true);
            players[1].GetComponent<PlayersScript>().PlayerAtHome = players[1].GetComponent<PlayersScript>().PlayerAtHome - 1;
            
        }
        if (players[2].transform.position == Cop.transform.position && Cop.GetComponent<followPath4Players>().moveAllowed == false
            && players[2].GetComponent<followPath4Players>().moveAllowed == false)
        {
             PlayNewSound(fail_sound);
            print(" player3 lost 1 life");
            player3Cards = 0;
            players[2].GetComponent<PlayersScript>().playerweed = 0;
            players[2].transform.GetChild(1).gameObject.SetActive(false);
            players[2].transform.GetChild(0).gameObject.SetActive(true);
            players[2].SetActive(false);
            players[2].GetComponent<followPath4Players>().moveAllowed = false;
            players[2].GetComponent<followPath4Players>().waypointIndex = 0;
            playersStartWaypoint[2] = 0;
            players[2].transform.position = players[2].GetComponent<followPath4Players>().waypoints[0].position;
            players[2].SetActive(true);
            players[2].GetComponent<PlayersScript>().PlayerAtHome = players[2].GetComponent<PlayersScript>().PlayerAtHome - 1;

        }
        if (players[3].transform.position == Cop.transform.position && Cop.GetComponent<followPath4Players>().moveAllowed == false
            && players[3].GetComponent<followPath4Players>().moveAllowed == false)
        {
             PlayNewSound(fail_sound);
            print(" player4 lost 1 life");
            player4Cards = 0;
            players[3].GetComponent<PlayersScript>().playerweed = 0;
            players[3].transform.GetChild(1).gameObject.SetActive(false);
            players[3].transform.GetChild(0).gameObject.SetActive(true);
            players[3].GetComponent<followPath4Players>().moveAllowed = false;
            players[3].SetActive(false);
            players[3].GetComponent<followPath4Players>().waypointIndex = 0;
            playersStartWaypoint[3] = 0;
            players[3].transform.position = players[3].GetComponent<followPath4Players>().waypoints[0].position;
            players[3].SetActive(true);
            players[3].GetComponent<PlayersScript>().PlayerAtHome = players[3].GetComponent<PlayersScript>().PlayerAtHome - 1;

        }

        if (players[0].transform.position == Cop1.transform.position && Cop1.GetComponent<followPath4Players>().moveAllowed == false
           && players[0].GetComponent<followPath4Players>().moveAllowed == false)
        {
             PlayNewSound(fail_sound);
            print(" player1 lost 1 life");
            player1Cards = 0;
            players[0].GetComponent<PlayersScript>().playerweed = 0;
             if (player1_pick_gun == true)
            {
                players[0].transform.GetChild(0).gameObject.SetActive(false);
                players[0].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                players[0].transform.GetChild(1).gameObject.SetActive(false);
                players[0].transform.GetChild(0).gameObject.SetActive(true);
            }
            //players[0].transform.GetChild(1).gameObject.SetActive(false);
            //players[0].transform.GetChild(0).gameObject.SetActive(true);
            players[0].SetActive(false);
            players[0].GetComponent<followPath4Players>().moveAllowed = false;
            players[0].GetComponent<followPath4Players>().waypointIndex = 0;
            playersStartWaypoint[0] = 0;
            players[0].transform.position = players[0].GetComponent<followPath4Players>().waypoints[0].position;
            players[0].SetActive(true);
            players[0].GetComponent<PlayersScript>().PlayerAtHome = players[0].GetComponent<PlayersScript>().PlayerAtHome - 1;

        }
        if (players[1].transform.position == Cop1.transform.position && Cop1.GetComponent<followPath4Players>().moveAllowed == false
            && players[1].GetComponent<followPath4Players>().moveAllowed == false)
        {
             PlayNewSound(fail_sound);
            print(" player2 lost 1 life");
            player2Cards = 0;
            players[1].GetComponent<PlayersScript>().playerweed = 0;
            players[1].transform.GetChild(1).gameObject.SetActive(false);
            players[1].transform.GetChild(0).gameObject.SetActive(true);
            players[1].GetComponent<followPath4Players>().moveAllowed = false;
            players[1].SetActive(false);
            players[1].GetComponent<followPath4Players>().waypointIndex = 0;
            playersStartWaypoint[1] = 0;
            players[1].transform.position = players[1].GetComponent<followPath4Players>().waypoints[0].position;
            players[1].SetActive(true);
            players[1].GetComponent<PlayersScript>().PlayerAtHome = players[1].GetComponent<PlayersScript>().PlayerAtHome - 1;

        }
        if (players[2].transform.position == Cop1.transform.position && Cop1.GetComponent<followPath4Players>().moveAllowed == false
            && players[2].GetComponent<followPath4Players>().moveAllowed == false)
        {
             PlayNewSound(fail_sound);
            print(" player3 lost 1 life");
            player3Cards = 0;
            players[2].GetComponent<PlayersScript>().playerweed = 0;
            players[2].transform.GetChild(1).gameObject.SetActive(false);
            players[2].transform.GetChild(0).gameObject.SetActive(true);
            players[2].SetActive(false);
            players[2].GetComponent<followPath4Players>().moveAllowed = false;
            players[2].GetComponent<followPath4Players>().waypointIndex = 0;
            playersStartWaypoint[2] = 0;
            players[2].transform.position = players[2].GetComponent<followPath4Players>().waypoints[0].position;
            players[2].SetActive(true);
            players[2].GetComponent<PlayersScript>().PlayerAtHome = players[2].GetComponent<PlayersScript>().PlayerAtHome - 1;

        }
        if (players[3].transform.position == Cop1.transform.position && Cop1.GetComponent<followPath4Players>().moveAllowed == false
            && players[3].GetComponent<followPath4Players>().moveAllowed == false)
        {
             PlayNewSound(fail_sound);
            print(" player4 lost 1 life");
            player4Cards = 0;
            players[3].GetComponent<PlayersScript>().playerweed = 0;
            players[3].transform.GetChild(1).gameObject.SetActive(false);
            players[3].transform.GetChild(0).gameObject.SetActive(true);
            players[3].GetComponent<followPath4Players>().moveAllowed = false;
            players[3].SetActive(false);
            players[3].GetComponent<followPath4Players>().waypointIndex = 0;
            playersStartWaypoint[3] = 0;
            players[3].transform.position = players[3].GetComponent<followPath4Players>().waypoints[0].position;
            players[3].SetActive(true);
            players[3].GetComponent<PlayersScript>().PlayerAtHome = players[3].GetComponent<PlayersScript>().PlayerAtHome - 1;

        }

        if (players[0].transform.position == Cop2.transform.position && Cop2.GetComponent<followPath4Players>().moveAllowed == false
   && players[0].GetComponent<followPath4Players>().moveAllowed == false)
        {
             PlayNewSound(fail_sound);
            print(" player1 lost 1 life");
            player1Cards = 0;
            players[0].GetComponent<PlayersScript>().playerweed = 0;
            if (player1_pick_gun == true)
            {
                players[0].transform.GetChild(0).gameObject.SetActive(false);
                players[0].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                players[0].transform.GetChild(1).gameObject.SetActive(false);
                players[0].transform.GetChild(0).gameObject.SetActive(true);
            }
           // players[0].transform.GetChild(1).gameObject.SetActive(false);
            //players[0].transform.GetChild(0).gameObject.SetActive(true);
            players[0].SetActive(false);
            players[0].GetComponent<followPath4Players>().moveAllowed = false;
            players[0].GetComponent<followPath4Players>().waypointIndex = 0;
            playersStartWaypoint[0] = 0;
            players[0].transform.position = players[0].GetComponent<followPath4Players>().waypoints[0].position;
            players[0].SetActive(true);
            players[0].GetComponent<PlayersScript>().PlayerAtHome = players[0].GetComponent<PlayersScript>().PlayerAtHome - 1;

        }
        if (players[1].transform.position == Cop2.transform.position && Cop1.GetComponent<followPath4Players>().moveAllowed == false
            && players[1].GetComponent<followPath4Players>().moveAllowed == false)
        {
             PlayNewSound(fail_sound);
            print(" player2 lost 1 life");
            player2Cards = 0;
            players[1].GetComponent<PlayersScript>().playerweed = 0;
            players[1].transform.GetChild(1).gameObject.SetActive(false);
            players[1].transform.GetChild(0).gameObject.SetActive(true);
            players[1].GetComponent<followPath4Players>().moveAllowed = false;
            players[1].SetActive(false);
            players[1].GetComponent<followPath4Players>().waypointIndex = 0;
            playersStartWaypoint[1] = 0;
            players[1].transform.position = players[1].GetComponent<followPath4Players>().waypoints[0].position;
            players[1].SetActive(true);
            players[1].GetComponent<PlayersScript>().PlayerAtHome = players[1].GetComponent<PlayersScript>().PlayerAtHome - 1;

        }
        if (players[2].transform.position == Cop2.transform.position && Cop2.GetComponent<followPath4Players>().moveAllowed == false
            && players[2].GetComponent<followPath4Players>().moveAllowed == false)
        {
             PlayNewSound(fail_sound);
            print(" player3 lost 1 life");
            player3Cards = 0;
            players[2].GetComponent<PlayersScript>().playerweed = 0;
            players[2].transform.GetChild(1).gameObject.SetActive(false);
            players[2].transform.GetChild(0).gameObject.SetActive(true);
            players[2].SetActive(false);
            players[2].GetComponent<followPath4Players>().moveAllowed = false;
            players[2].GetComponent<followPath4Players>().waypointIndex = 0;
            playersStartWaypoint[2] = 0;
            players[2].transform.position = players[2].GetComponent<followPath4Players>().waypoints[0].position;
            players[2].SetActive(true);
            players[2].GetComponent<PlayersScript>().PlayerAtHome = players[2].GetComponent<PlayersScript>().PlayerAtHome - 1;

        }
        if (players[3].transform.position == Cop2.transform.position && Cop2.GetComponent<followPath4Players>().moveAllowed == false
            && players[3].GetComponent<followPath4Players>().moveAllowed == false)
        {
             PlayNewSound(fail_sound);
            player4Cards = 0;
            players[3].GetComponent<PlayersScript>().playerweed = 0;
            players[3].transform.GetChild(1).gameObject.SetActive(false);
            players[3].transform.GetChild(0).gameObject.SetActive(true);
            print(" player4 lost 1 life");
            players[3].GetComponent<followPath4Players>().moveAllowed = false;
            players[3].SetActive(false);
            players[3].GetComponent<followPath4Players>().waypointIndex = 0;
            playersStartWaypoint[3] = 0;
            players[3].transform.position = players[3].GetComponent<followPath4Players>().waypoints[0].position;
            players[3].SetActive(true);
            players[3].GetComponent<PlayersScript>().PlayerAtHome = players[3].GetComponent<PlayersScript>().PlayerAtHome - 1;

        }

        #endregion


        if (dice4Players.ins.whosTurn == 1)
        {
            dice4Players.ins.MaterialOn(1);
            player1_text.player_text.text = "Your Turn";
            player1_text.player_with_gun_text.text = "Your Turn";
            player1MoveText.gameObject.SetActive(true);
            player2MoveText.gameObject.SetActive(false);
            player3MoveText.gameObject.SetActive(false);
            player4MoveText.gameObject.SetActive(false);
            copMoveText.gameObject.SetActive(false);
            
        }
        else if (dice4Players.ins.whosTurn == 2)
        {
            dice4Players.ins.OutlineOn(2);
            player1_text.player_text.text = "......";
             player1_text.player_with_gun_text.text = "........";
            player1MoveText.gameObject.SetActive(false);
            player2MoveText.gameObject.SetActive(true);
            player3MoveText.gameObject.SetActive(false);
            player4MoveText.gameObject.SetActive(false);
            copMoveText.gameObject.SetActive(false);

        }
        else if (dice4Players.ins.whosTurn == 3)
        {
            dice4Players.ins.OutlineOn(2);
            player1_text.player_text.text = "......";
             player1_text.player_with_gun_text.text = "........";
            player1MoveText.gameObject.SetActive(false);
            player2MoveText.gameObject.SetActive(false);
            player3MoveText.gameObject.SetActive(true);
            player4MoveText.gameObject.SetActive(false);
            copMoveText.gameObject.SetActive(false);
        }
        else if (dice4Players.ins.whosTurn == 4)
        {
            dice4Players.ins.OutlineOn(2);
            player1_text.player_text.text = "......";
             player1_text.player_with_gun_text.text = "........";
            player1MoveText.gameObject.SetActive(false);
            player2MoveText.gameObject.SetActive(false);
            player3MoveText.gameObject.SetActive(false);
            player4MoveText.gameObject.SetActive(true);
            copMoveText.gameObject.SetActive(false);
        }
        else if (dice4Players.ins.whosTurn == 5)
        {
            dice4Players.ins.OutlineOn(2);
            player1_text.player_text.text = "......";
             player1_text.player_with_gun_text.text = "........";
            player1MoveText.gameObject.SetActive(false);
            player2MoveText.gameObject.SetActive(false);
            player3MoveText.gameObject.SetActive(false);
            player4MoveText.gameObject.SetActive(false);
            copMoveText.gameObject.SetActive(true);
        }
        // update no of cards
        player1CardsText.text = players[0].GetComponent<PlayersScript>().myCards.ToString();
        player2CardsText.text = players[1].GetComponent<PlayersScript>().myCards.ToString();
        player3CardsText.text = players[2].GetComponent<PlayersScript>().myCards.ToString();
        player4CardsText.text = players[3].GetComponent<PlayersScript>().myCards.ToString();
    }

    public void MovePlayer(int playerToMove)
    {
        // moving the player who has rolled the dice
        switch (playerToMove)
        {
            case 1:
                players[0].GetComponent<followPath4Players>().moveAllowed = true;
                playSound = true;
                break;

            case 2:
                players[1].GetComponent<followPath4Players>().moveAllowed = true;
                playSound = true;
                break;
            case 3:
                players[2].GetComponent<followPath4Players>().moveAllowed = true;
                playSound = true;
                break;
            case 4:
                players[3].GetComponent<followPath4Players>().moveAllowed = true;
                playSound = true;
                break;
            case 5:
                Cop.GetComponent<followPath4Players>().moveAllowed = true;
                Cop1.GetComponent<followPath4Players>().moveAllowed = true;
                Cop2.GetComponent<followPath4Players>().moveAllowed = true;
                playSound = true;
                break;
        }
    }
    //play sound 

    public void PlayNewSound(AudioClip clip)
    {
        new_sound.PlayOneShot(clip);
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
                    break;
                case "player2":
                    player2Cards = player2Cards + 5;
                    player2CardsText.text = player2Cards.ToString();
                    player.GetComponent<PlayersScript>().playerweed = player2Cards;
                    break;
                case "player3":
                    player3Cards = player3Cards + 5;
                    player3CardsText.text = player3Cards.ToString();
                    player.GetComponent<PlayersScript>().playerweed = player3Cards;
                    break;
                case "player4":
                    player4Cards = player4Cards + 5;
                    player.GetComponent<PlayersScript>().playerweed = player4Cards;
                    player4CardsText.text = player4Cards.ToString();
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
            if (player.tag == "player1")
            {
                player1_pick_gun = true;
            }
            else if (player.tag == "player2")
            {
                player2_pick_gun = true;
            }
            else if (player.tag == "player3")
            {
                player3_pick_gun = true;
            }
            else if (player.tag == "player4")
            {
                player4_pick_gun = true;
            }

        }
        //else
        //{
        //     taskOk.SetActive(true);
        //     taskOptions.SetActive(false);
        //}


        //  print(" my tag _______ " + points[waypointsindex].tag+" task ");


    }
    void Relive()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].activeInHierarchy == false)
            {
                players[i].SetActive(true);
        }
    }
}
    public void getProducts(int waypointindex)
    {

        if (waypointindex == 18 || waypointindex == 76 || waypointindex == 109)
        {
            // gun store

            taskText.GetComponent<Text>().text = " Do You Want To Buy A Gun ??";
            taskOptions.SetActive(true);
            taskPanel.SetActive(true);
            buygunBool = true;
        }
    }


    public void replay()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
        gameOver = false;
        players[0].GetComponent<followPath4Players>().waypointIndex = 0;
        players[1].GetComponent<followPath4Players>().waypointIndex = 0;
        players[2].GetComponent<followPath4Players>().waypointIndex = 0;
        players[3].GetComponent<followPath4Players>().waypointIndex = 0;
    }

    public void no()
    {
        taskPanel.SetActive(false);
    }
    public void startbutton()
    {
        startpanel.SetActive(false);
        Time.timeScale = 1;
        movementsound.ins.musicSource.clip = movementsound.ins.musicClip;
        movementsound.ins.musicSource.Play();
    }
    public void yes()
    {
        //switch (currentTask)
        //{

        //    case 0:
        //        //"Botched Robbery ",

        //        break;

        //    case 1:
        //        // "Bad Drugs",

        //        break;

        //    case 2:
        //        //"Caught in sting (Jail) player pays $10",

        //        break;

        //    case 3:
        //        //"Snitched (back 2)",

        //        break;

        //    case 4:
        //        //"You gained a player (xtra man)",
        //        currentPlayer.GetComponent<PlayersScript>().PlayerAtHome = currentPlayer.GetComponent<PlayersScript>().PlayerAtHome + 1;
        //        break;

        //    case 5:
        //        // "ROB Contact ",

        //        break;

        //    case 6:
        //        // "Bad Drugs ",

        //        break;

        //    case 7:
        //        // "ROB grow house ",
        //        currentPlayer.GetComponent<followPath4Players>().waypointIndex = 44;
        //        currentPlayer.GetComponent<followPath4Players>().moveAllowed = false;
        //        break;

        //    case 8:
        //        // "Bank Mixup receive $10",

        //        break;

        //    case 9:
        //        //"You found 3 product",

        //        break;

        //    case 10:
        //        // "Head to Vape ",
        //        // waypoint no 39 
        //        print("head to vape");
        //        currentPlayer.GetComponent<followPath4Players>().waypointIndex = 40;
        //        currentPlayer.GetComponent<followPath4Players>().moveAllowed = false;
        //        break;

        //    case 11:
        //        // "If man in jail he was killed (player lost)",
        //        if (currentPlayer.GetComponent<PlayersScript>().playerInJail > 0)
        //        {
        //            currentPlayer.GetComponent<PlayersScript>().playerInJail = currentPlayer.GetComponent<PlayersScript>().playerInJail - 1;
        //            print(" player in jail");
        //        }
        //        break;

        //    case 12:
        //        //"Your lucky day ( found $5)",

        //        break;

        //    case 13:
        //        //    "ROB Weed Store ",
        //        print("weed store");
        //        currentPlayer.GetComponent<followPath4Players>().waypointIndex = 9;
        //        currentPlayer.GetComponent<followPath4Players>().moveAllowed = false;
        //        break;

        //    default:
        //        taskPanel.SetActive(false);
        //        break;
        //}


        //if(buygunBool == true)
        //{
        //    currentPlayer.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = playerwithGun[currentPlayerNo];
        //    currentPlayer.GetComponent<PlayersScript>().haveGun = true;
        //}
        taskPanel.SetActive(false);
    }

    void StopMoving()
    {
        /// move forward 
        if (players[0].GetComponent<followPath4Players>().waypointIndex >
            playersStartWaypoint[0] + diceSideThrown)
        {
            players[0].GetComponent<followPath4Players>().moveAllowed = false;

            currentPlayer = players[0];

            player1MoveText.gameObject.SetActive(false);
            player2MoveText.gameObject.SetActive(true);
            player3MoveText.gameObject.SetActive(false);
            player4MoveText.gameObject.SetActive(false);
            copMoveText.gameObject.SetActive(false);


            playersStartWaypoint[0] = players[0].GetComponent<followPath4Players>().waypointIndex - 1;
            diceSideThrown = 0;
        }

        if (players[1].GetComponent<followPath4Players>().waypointIndex >
            playersStartWaypoint[1] + diceSideThrown)
        {
            players[1].GetComponent<followPath4Players>().moveAllowed = false;

            currentPlayer = players[1];

            player1MoveText.gameObject.SetActive(false);
            player2MoveText.gameObject.SetActive(false);
            player3MoveText.gameObject.SetActive(true);
            player4MoveText.gameObject.SetActive(false);
            copMoveText.gameObject.SetActive(false);


            playersStartWaypoint[1] = players[1].GetComponent<followPath4Players>().waypointIndex - 1;
            diceSideThrown = 0;
        }


        if (players[2].GetComponent<followPath4Players>().waypointIndex >
         playersStartWaypoint[2] + diceSideThrown)
        {
            players[2].GetComponent<followPath4Players>().moveAllowed = false;

            currentPlayer = players[2];

            player1MoveText.gameObject.SetActive(false);
            player2MoveText.gameObject.SetActive(false);
            player3MoveText.gameObject.SetActive(false);
            player4MoveText.gameObject.SetActive(true);
            copMoveText.gameObject.SetActive(false);

            playersStartWaypoint[2] = players[2].GetComponent<followPath4Players>().waypointIndex - 1;
            diceSideThrown = 0;
        }

        if (players[3].GetComponent<followPath4Players>().waypointIndex >
            playersStartWaypoint[3] + diceSideThrown)
        {
            players[3].GetComponent<followPath4Players>().moveAllowed = false;

            currentPlayer = players[3];

            player1MoveText.gameObject.SetActive(false);
            player2MoveText.gameObject.SetActive(false);
            player3MoveText.gameObject.SetActive(false);
            player4MoveText.gameObject.SetActive(false);
            copMoveText.gameObject.SetActive(true);

            playersStartWaypoint[3] = players[3].GetComponent<followPath4Players>().waypointIndex - 1;
            diceSideThrown = 0;
        }

        if (Cop.GetComponent<followPath4Players>().waypointIndex >
          CopStartWaypoint + diceSideThrown)
        {
            Cop.GetComponent<followPath4Players>().moveAllowed = false;

            player1MoveText.gameObject.SetActive(true);
            player2MoveText.gameObject.SetActive(false);
            player3MoveText.gameObject.SetActive(false);
            player4MoveText.gameObject.SetActive(false);
            copMoveText.gameObject.SetActive(false);

            CopStartWaypoint = Cop.GetComponent<followPath4Players>().waypointIndex - 1;
            diceSideThrown = 0;
        }


    }
    public void PlayerTut()
    {
        tutorial.SetActive(false);
        PlayerPrefs.SetInt("tut", 1);
    }

}
