using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameplay : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void buttons(int no)
    {
        switch (no)
        {
            case 1:
                // pause
                gamecontrol4players.ins.pausePanel.SetActive(true);
                break;
            case 2:
                // unpause
                gamecontrol4players.ins.pausePanel.SetActive(false);
                break;
            case 3:
                // home
                SceneManager.LoadScene(0);
                break;
            case 4:
                // reload
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
        }
    }
}
