using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class diceScriptController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject diceScript;
    public GameObject diceScript2;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gamecontrol4players.ins.fight == true)
        {
            diceScript.SetActive(false);
            diceScript2.SetActive(true);
            diceOnFight.ins.whosTurn = 1;
        }
        if (gamecontrol4players.ins.fight == false)
        {
            diceScript.SetActive(true);
            diceScript2.SetActive(false);
        }
    }
}
