using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static System.TimeZoneInfo;

public class TutorialTwoState : GameState
{
    public GameObject locomotionObject;
    [Range(0f, 1f)]
    public float timeBuffer = 0.05f;
    private float timer = 0.0f;
    private Vector3 posAtTeleport;
    //public GameObject[] happinessPanels;
    //public GameObject[] sadnessPanels;
    //public GameObject[] angerPanels;
    //public GameObject[] fearPanels;
    //public GameObject[] tut2HappinessPanels;
    //public GameObject[] tut2SadPanels;
    //public GameObject[] tut2AngerPanels;
    //public GameObject[] tut2FearPanels;


    //take in a GameObject for each emotion that is a ui canvas
    //assume already inactive if desired
    public GameObject HappinessCanvas;
    public GameObject SadnessCanvas;
    public GameObject AngerCanvas;
    public GameObject FearCanvas;

    private bool bblWasPressed = false;
    public int activeCanvas = 0;
    public int prevActiveCanvas = 0;
    private int tutorialIndex = 0;
    private int currentGuess = -1;

    //store initial panel positions?
    //we have array of panel objects
    //create the tutorial panel sets from these
    //tutorial 1 just use as is
    //tutorial 2 make a mix of 1 emotion from each array
    //tutorial 3 is avatar but we need some cards for the user\
    //these arrays must be 1-1 to the panel object arrays
    private Vector3[] happinessPanelPos;
    private Vector3[] sadnessPanelPos;
    private Vector3[] angerPanelPos;
    private Vector3[] fearPanelPos;

    //just use 1 array of positions for the active panels
    private Vector3[] activePanelPos;

    public int correctHappyIndex = 1;
    public int correctSadIndex = 1;
    public int correctAngerIndex = 1;
    public int correctFearIndex = 1;

    private float transitionTimeDelta = 0.25f;
    public float transitionTimer = 0f;
    private bool startTransition = false;

    void Start()
    {
        stateName = GameStateMachine.GameStateName.TUTORIALTWO;
        //happinessPanelPos = new Vector3[happinessPanels.Length];
        //sadnessPanelPos = new Vector3[sadnessPanels.Length];
        //angerPanelPos = new Vector3[angerPanels.Length];
        //fearPanelPos = new Vector3[fearPanels.Length];

        //activePanelPos = new Vector3[happinessPanels.Length];
    }
    override public void InitialiseState()
    {
        //disable locomotionObject, seems to work ok
        //this was too quick and stops locomotion before we can teleport
        //it is triggered when the teleport event is queued
        //locomotionObject.SetActive(false);

        //turn on the happiness panels
        //we have four sets of panels, happy, sad, angry, fear
        //show one set at a time, press button to get next set
        //this is tutorial one, we are going to need to split the gameply states up for sure

        //foreach (GameObject panel in happinessPanels)
        //{
        //    panel.SetActive(true);
        //}

        //for (int panelIndex = 0; panelIndex < happinessPanels.Length; ++panelIndex)
        //{
        //    GameObject happinessPanel = happinessPanels[panelIndex];
        //    happinessPanel.SetActive(true);

        //    //set activePanel pos from happiness Panels
        //    activePanelPos[panelIndex] = happinessPanel.transform.position;
        //}

        HappinessCanvas.SetActive(true);
        activeCanvas = 0;
        prevActiveCanvas = 0;
        bblWasPressed = false;
        startTransition = false;
        transitionTimer = 0f;
    }
    override public GameStateMachine.GameStateName UpdateState()
    {    
        return UpdateTutorialOne();
    }
    override public void ShutDownState()
    {
        //tenable locomationObject
        //locomotionObject.SetActive(true);
    }
    override public void TeleOn()
    {
        //hasTeleportedIn = true;
    }

    private GameStateMachine.GameStateName UpdateTutorialOne()
    {
        //JUST USE ONE SET OF POSITIONS FOR PANELS
        //
        
        if (prevActiveCanvas != activeCanvas)
        {
            transitionTimer += Time.deltaTime;
            if (transitionTimer >= transitionTimeDelta)
            {                
                startTransition = false;
                transitionTimer = 0f;
                Debug.Log("GAMESTATE Swapping panels, prevAvctivePanels, activePanels" + prevActiveCanvas + ", " + activeCanvas);
                switch (prevActiveCanvas)
                {
                    case 0:
                        HappinessCanvas.SetActive(false);
                        SadnessCanvas.SetActive(true);
                        break;
                    case 1:
                        SadnessCanvas.SetActive(false);
                        AngerCanvas.SetActive(true);
                        break;
                    case 2:
                        AngerCanvas.SetActive(false);
                        FearCanvas.SetActive(true);
                        break;
                    case 3:
                        FearCanvas.SetActive(false);
                        //activeCanvas = 0;
                        return GameStateMachine.GameStateName.TUTORIALTHREE;
                    default:
                        break;
                }

                prevActiveCanvas = activeCanvas;
            }            
        }
        
        //need to check the toggles, or do they fire an event in here? event in here would be easier I think

        return GameStateMachine.GameStateName.TUTORIALTWO;
    }
    

    //    //update states here, need to refactor this into other switch??
    //    //get panels, get component TutorialPanel, check is selected, check if correct
    //    switch(activePanels)
    //    {
    //        case 0:
    //            for (int panelIndex = 0; panelIndex < tut2HappinessPanels.Length; ++panelIndex)
    //            {
    //                GameObject tutPanelObject = tut2HappinessPanels[panelIndex];
    //                TutorialPanel tutPanel = tutPanelObject.GetComponent<TutorialPanel>();

    //                if (tutPanel.IsSelected())
    //                {
    //                    if(tutPanelObject.tag == "HappinessPanel")
    //                    {
    //                        //correct, flash green outline or something?

    //                        //go to next panel index
    //                        ++activePanels;
    //                    }
    //                    else
    //                    {
    //                        //incorrect, flash red outline or something?
    //                    }
    //                }
    //            }

    //            break;

    //        case 1:
    //            for (int panelIndex = 0; panelIndex < tut2SadPanels.Length; ++panelIndex)
    //            {
    //                GameObject tutPanelObject = tut2SadPanels[panelIndex];
    //                TutorialPanel tutPanel = tutPanelObject.GetComponent<TutorialPanel>();

    //                if (tutPanel.IsSelected())
    //                {
    //                    if (tutPanelObject.tag == "SadnessPanel")
    //                    {
    //                        //correct, flash green outline or something?

    //                        //go to next panel index
    //                        ++activePanels;
    //                    }
    //                    else
    //                    {
    //                        //incorrect, flash red outline or something?
    //                    }
    //                }
    //            }
    //            break;

    //        case 2:
    //            for (int panelIndex = 0; panelIndex < tut2AngerPanels.Length; ++panelIndex)
    //            {
    //                GameObject tutPanelObject = tut2AngerPanels[panelIndex];
    //                TutorialPanel tutPanel = tutPanelObject.GetComponent<TutorialPanel>();

    //                if (tutPanel.IsSelected())
    //                {
    //                    if (tutPanelObject.tag == "AngerPanel")
    //                    {
    //                        //correct, flash green outline or something?

    //                        //go to next panel index
    //                        ++activePanels;
    //                    }
    //                    else
    //                    {
    //                        //incorrect, flash red outline or something?
    //                    }
    //                }
    //            }
    //            break;

    //        case 3:
    //            for (int panelIndex = 0; panelIndex < tut2FearPanels.Length; ++panelIndex)
    //            {
    //                GameObject tutPanelObject = tut2FearPanels[panelIndex];
    //                TutorialPanel tutPanel = tutPanelObject.GetComponent<TutorialPanel>();

    //                if (tutPanel.IsSelected())
    //                {
    //                    if (tutPanelObject.tag == "FearPanel")
    //                    {
    //                        //correct, flash green outline or something?

    //                        //go to next panel index
    //                        ++activePanels;
    //                    }
    //                    else
    //                    {
    //                        //incorrect, flash red outline or something?
    //                    }
    //                }
    //            }
    //            break;

    //        default:
    //            break;

    //    }


    //    prevActivePanels = activePanels;

    //    return GameStateMachine.GameStateName.GAMEPLAY;
    //}

  
    //For Transition part of Update, use position as a backup, if xrorigin has moved away from teleport pos then back into intro state
    //should keep position at teleport then
    public void HandleBigBlueButton()
    {
        bblWasPressed = true;
    }

    public void NextButtonPressed()
    {
        bblWasPressed = true;
    }

    //guess input should be the index
    public void IsCorrectGuess(int guess)
    {
        currentGuess = guess;

        if(guess == activeCanvas)
        {
            startTransition = true;
            ++activeCanvas;
        }
    }
}
