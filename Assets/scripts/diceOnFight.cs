using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class diceOnFight : MonoBehaviour
{

    private Sprite[] diceSides;
    private SpriteRenderer rend;
    public int whosTurn = 1;
    private bool coroutineAllowed = true;

    public static diceOnFight ins;

    // Use this for initialization
    private void Start()
    {

        ins = this;
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");

      
    }
    void Update()
    {
       
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
          
            this.GetComponent<Image>().sprite = diceSides[randomDiceSide];
            yield return new WaitForSeconds(0.05f);
        }

        gamecontrol4players.ins.diceSideThrown = randomDiceSide + 1;
        if (gamecontrol4players.ins.fight == true)
        {
            if (whosTurn == 1)
            {
                gamecontrol4players.ins.player1dicenoforfight = gamecontrol4players.ins.diceSideThrown;
                gamecontrol4players.ins.fightTurn = gamecontrol4players.ins.fightTurn + 1;
          
                print("fight turn");
            }
            else if (whosTurn == 2)
            {
                gamecontrol4players.ins.player2dicenoforfight = gamecontrol4players.ins.diceSideThrown;
                gamecontrol4players.ins.fightTurn = gamecontrol4players.ins.fightTurn + 1;
              
                print("fight turn");
            }



        }

        if (whosTurn == 0)
        {
            whosTurn = 1;
        }
        else if (whosTurn == 1)
        {
            whosTurn = 2;
        }
        else if (whosTurn == 2)
        {
            whosTurn = 3;
        }

        coroutineAllowed = true;
    }
    public void rollMe()
    {
        if (!GameControl.gameOver && coroutineAllowed)
            StartCoroutine("RollTheDice");
    }
}