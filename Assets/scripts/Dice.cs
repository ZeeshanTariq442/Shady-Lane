using System.Collections;
using UnityEngine;

using UnityEngine.UI;

public class Dice : MonoBehaviour
{

    private Sprite[] diceSides;
    private SpriteRenderer rend;
    private int whosTurn = 1;
    private bool coroutineAllowed = true;
    bool copTurnCheck;

    private static GameObject player1, player2;

    public Material player1_material, player2_material, cop_material;

    // Use this for initialization
    private void Start()
    {
        copTurnCheck = false;
        //  rend = GetComponent<SpriteRenderer>();
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");
        //   rend.sprite = diceSides[5];
        player1 = GameObject.Find("Player1");
        player2 = GameObject.Find("Player2");
    }
    void Update()
    {
        if (whosTurn == 3 && copTurnCheck == true)
        {
            copTurnCheck = false;
            rollMe();
        }
    }
    private void OnMouseDown()
    {
        if (!GameControl.gameOver && coroutineAllowed)
            StartCoroutine("RollTheDice");
    }

    private IEnumerator RollTheDice()
    {
        coroutineAllowed = false;
        int randomDiceSide = 0;
        for (int i = 0; i <= 20; i++)
        {
            randomDiceSide = Random.Range(0, 6);
            //   rend.sprite = diceSides[randomDiceSide];
            this.GetComponent<Image>().sprite = diceSides[randomDiceSide];
            yield return new WaitForSeconds(0.05f);
        }

        GameControl.diceSideThrown = randomDiceSide + 1;
        if (GameControl.ins.fight == true)
        {
            if (whosTurn == 1)
            {
                GameControl.ins.player1dicenoforfight = GameControl.diceSideThrown;
                GameControl.ins.fightTurn++;
                print("fight turn");
            }
            else if (whosTurn == 2)
            {
                GameControl.ins.player2dicenoforfight = GameControl.diceSideThrown;
                GameControl.ins.fightTurn++;
                print("fight turn");
            }
            else if (whosTurn == 3)
            {
                copTurnCheck = true;
                GameControl.ins.player2dicenoforfight = GameControl.diceSideThrown;
                GameControl.ins.fightTurn++;
                print("fight turn");
            }


        }
        else
        {
            if (whosTurn == 1)
            {
                Debug.Log("Hello trun1");

                player1_material.SetFloat("_OutlineEnabled", 0f);

                player2_material.SetFloat("_OutlineEnabled", 1f);

                GameControl.MovePlayer(1);
                GameControl.ins.TasksForPlayer(GameControl.diceSideThrown + GameControl.ins.player1StartWaypoint, player1);
                int myindex = GameControl.diceSideThrown + GameControl.ins.player1StartWaypoint;
                print("my index " + myindex);
            }
            else if (whosTurn == 2)
            {
                player2_material.SetFloat("_OutlineEnabled", 0f);
                cop_material.SetFloat("_OutlineEnabled", 1f);
                GameControl.MovePlayer(2);
                GameControl.ins.TasksForPlayer(GameControl.diceSideThrown + GameControl.ins.player2StartWaypoint, player2);
                int myindex = GameControl.diceSideThrown + GameControl.ins.player2StartWaypoint;
                print("my index " + myindex);
            }
            else if (whosTurn == 3)
            {
                cop_material.SetFloat("_OutlineEnabled", 0f);
                player1_material.SetFloat("_OutlineEnabled", 1f);
                copTurnCheck = true;
                GameControl.MovePlayer(3);
                GameControl.ins.TasksForPlayer(GameControl.diceSideThrown + GameControl.ins.player2StartWaypoint, player2);
                int myindex = GameControl.diceSideThrown + GameControl.ins.player2StartWaypoint;
                print("my index " + myindex);
            }
        }
        if (whosTurn == 1)
        {
            whosTurn = 2;
        }
        else if (whosTurn == 2)
        {
            whosTurn = 3;
        }
        else if (whosTurn == 3)
        {
            whosTurn = 1;
        }
        //  whosTurn *= -1;
        coroutineAllowed = true;
    }
    public void rollMe()
    {
        if (!GameControl.gameOver && coroutineAllowed)
            StartCoroutine("RollTheDice");
    }
    void OnDisable()
    {
        player1_material.SetFloat("_OutlineEnabled", 1f);
        player2_material.SetFloat("_OutlineEnabled", 0f);
        cop_material.SetFloat("_OutlineEnabled", 0f);
    }
}
