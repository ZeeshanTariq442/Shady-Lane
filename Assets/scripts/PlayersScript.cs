using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayersScript : MonoBehaviour
{
    // Start is called before the first frame update
    public int myCards = 0 ;
    public int PlayerAtHome;
    public int myHoods;
    public int PlayerAtHospital;
    public int playerInJail;
    public int playerweed;

    public Text PlayerAtHomeText;
    public Text myHoodsText;
    public Text PlayerAtHospitalText;
    public Text playerInJailText;
    public Text playerweedText;



    public bool haveGun;

    public Text player_text,player_with_gun_text;
    public string player_name;
    
    public GameObject player_panel,player_panel2;

    public static PlayersScript ins;
    void Start()
    {
        ins = this;
        PlayerAtHome = 0;
        myHoods = 0;
        PlayerAtHospital = 0;
        playerInJail = 0;

        PlayerAtHomeText.text = PlayerAtHome.ToString();
        myHoodsText.text = myHoods.ToString();
        PlayerAtHospitalText.text = PlayerAtHospital.ToString();
        playerInJailText.text = playerInJail.ToString();
        playerweedText.text = playerweed.ToString();
        player_text.text = player_name;
        if (gameObject.tag == "player1")
        {
            player_panel.SetActive(true);
            player_panel2.SetActive(true);
            Debug.Log("Panel on");
        }
    
    }

    // Update is called once per frame
    void Update()
    {
        PlayerAtHomeText.text = PlayerAtHome.ToString();
        myHoodsText.text = myHoods.ToString();
        PlayerAtHospitalText.text = PlayerAtHospital.ToString();
        playerInJailText.text = playerInJail.ToString();
        playerweedText.text = playerweed.ToString();
       
    }
}
