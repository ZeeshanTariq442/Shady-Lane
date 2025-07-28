using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCheckZoneScript : MonoBehaviour {

	Vector3 diceVelocity;

	public int diceNo;
	public static DiceCheckZoneScript ins;
    private void Start()
    {
		ins = this;
    }
    // Update is called once per frame
    void FixedUpdate () {
		diceVelocity = DiceScript.diceVelocity;
	}

	void OnTriggerStay(Collider col)
	{
		if (diceVelocity.x == 0f && diceVelocity.y == 0f && diceVelocity.z == 0f)
		{
			switch (col.gameObject.name) {
			case "Side1":
				    DiceNumberTextScript.diceNumber = 6;
					stone.ins.movePlayer();
					diceNo = 6;
				break;
			case "Side2":
				    DiceNumberTextScript.diceNumber = 5;
					stone.ins.movePlayer();
					diceNo = 5;
					break;
			case "Side3":
				    DiceNumberTextScript.diceNumber = 4;
					stone.ins.movePlayer();
					diceNo = 4;
					break;
			case "Side4":
			    	DiceNumberTextScript.diceNumber = 3;
					stone.ins.movePlayer();
					diceNo = 3;
					break;
			case "Side5":
				    DiceNumberTextScript.diceNumber = 2; 
					stone.ins.movePlayer();
					diceNo = 5;
					break;
			case "Side6":
				    DiceNumberTextScript.diceNumber = 1;
					stone.ins.movePlayer();
					diceNo = 1;
					break;
			}
		}
	}
}
