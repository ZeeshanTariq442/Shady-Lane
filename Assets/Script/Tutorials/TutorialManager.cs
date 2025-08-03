using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum TutorialState
{
    FirstMove,
    Undo,
    SecondMove
}
public class TutorialManager : MonoBehaviour
{
    public HandIndicator handIndicator;
    public List<Transform> indicatorUI = new List<Transform>();
    public GameObject indicatorPanel;

    private int currentStep = 0;
    private bool undoItem;

    public static TutorialState state; 

    public static TutorialState SetState
    {
        get { return state; }  
        set { state = value; }  
    }

    public bool UndoTutorial
    {
        get
        {
            return undoItem;
        }
        set
        {
            undoItem = value;
        }
    }
    public int CurrentStep
    {
        get { return currentStep; }
    }

    private void Start()
    {
        // StartTutorial();
        if(PlayerPrefsHandler.GetInt(PlayerPrefsHandler.TutorialPrefKey, 0 ) == 0)
            indicatorPanel.SetActive(true);
        else
            indicatorPanel.SetActive(false);
    }

    public void StartTutorial()
    {
        handIndicator.gameObject.SetActive(true);
        currentStep = 0;
        ShowNextStep();
    }
    private void ShowNextStep()
    {
        switch (currentStep)
        {
            case 0:
                handIndicator.ShowHandAt(indicatorUI[currentStep].position);
                break;

            case 1:
                handIndicator.ShowHandAt(indicatorUI[currentStep].position);
                break;

            default:
                handIndicator.HideHand();
                Debug.Log("Tutorial Completed!");
                indicatorPanel.SetActive(false);
                PlayerPrefsHandler.SetInt(PlayerPrefsHandler.TutorialPrefKey, 1);
                break;
        }
    }

    public void StepCompleted()
    {
        currentStep++;
        ShowNextStep();
    }
}
