using UnityEngine;
using UnityEngine.Rendering;

public class TutorialState : GameState
{
    public GameObject locomotionObject;
    [Range(0f, 1f)]
    public float timeBuffer = 0.05f;
    private float timer = 0.0f;
    private Vector3 posAtTeleport;
    public GameObject[] happinessPanels;
    public GameObject[] sadnessPanels;
    public GameObject[] angerPanels;
    public GameObject[] fearPanels;
    public GameObject[] tut2HappinessPanels;
    public GameObject[] tut2SadPanels;
    public GameObject[] tut2AngerPanels;
    public GameObject[] tut2FearPanels;

    private bool bblWasPressed = false;
    private int activePanels = 0;
    private int prevActivePanels = 0;
    private int tutorialIndex = 0;

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

    void Start()
    {
        stateName = GameStateMachine.GameStateName.TUTORIALONE;
        happinessPanelPos = new Vector3[happinessPanels.Length];
        sadnessPanelPos = new Vector3[sadnessPanels.Length];
        angerPanelPos = new Vector3[angerPanels.Length];
        fearPanelPos = new Vector3[fearPanels.Length];

        activePanelPos = new Vector3[happinessPanels.Length];
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

        for (int panelIndex = 0; panelIndex < happinessPanels.Length; ++panelIndex)
        {
            GameObject happinessPanel = happinessPanels[panelIndex];
            happinessPanel.SetActive(true);

            //set activePanel pos from happiness Panels
            activePanelPos[panelIndex] = happinessPanel.transform.position;
        }
    }
    override public GameStateMachine.GameStateName UpdateState()
    {
        //need to handle multiple tutorial "states" so when button is pressed handle depending on state.
        //so if tutorialIndex = 0 then do the switch we already had created
        //if tutorialIndex is 0, this is more a free form tutorial. BBL moves the index to show the next emotion
        //at the end instead of  going back to frontend, we should move into the next tutorial index.
        //only save tutorial finised when all tutorials are done.
        switch (tutorialIndex)
        {
            case 0:
                UpdateTutorialOne();
                break;
            case 1:
                UpdateTutorialTwo();
                break;
            case 2:
                UpdateTutorialThree();
                break;
            default:
                break;
        }
        

        //GameStateMachine.GameStateName.FRONTEND;

        return GameStateMachine.GameStateName.TUTORIALONE;
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

        if (bblWasPressed)
        {
            if (activePanels < 4)
            {
                activePanels++;
            }

            if (prevActivePanels != activePanels)
            {
                Debug.Log("GAMESTATE Swapping panels, prevAvctivePanels, activePanels" + prevActivePanels + ", " + activePanels);
                switch (prevActivePanels)
                {
                    case 0:
                        for(int panelIndex = 0; panelIndex < happinessPanels.Length; ++panelIndex)
                        {
                            GameObject happinessPanel = happinessPanels[panelIndex];
                            happinessPanel.SetActive(false);
                            //happinessPanel.transform.position = activePanelPos[panelIndex];
                        }
                        for (int panelIndex = 0; panelIndex < sadnessPanels.Length; ++panelIndex)
                        {
                            GameObject sadnessPanel = sadnessPanels[panelIndex];
                            sadnessPanel.SetActive(true);
                            sadnessPanel.transform.position = activePanelPos[panelIndex];
                        }                       
                        break;
                    case 1:
                        for (int panelIndex = 0; panelIndex < sadnessPanels.Length; ++panelIndex)
                        {
                            GameObject sadnessPanel = sadnessPanels[panelIndex];
                            sadnessPanel.SetActive(false);
                            sadnessPanel.transform.position = sadnessPanelPos[panelIndex];
                        }
                        for (int panelIndex = 0; panelIndex < angerPanels.Length; ++panelIndex)
                        {
                            GameObject angerPanel = angerPanels[panelIndex];
                            angerPanel.SetActive(true);
                            angerPanel.transform.position = activePanelPos[panelIndex];
                        }                        
                        break;
                    case 2:
                        for (int panelIndex = 0; panelIndex < angerPanels.Length; ++panelIndex)
                        {
                            GameObject angerPanel = angerPanels[panelIndex];
                            angerPanel.SetActive(false);
                            angerPanel.transform.position= angerPanelPos[panelIndex];
                        }
                        for (int panelIndex = 0; panelIndex < fearPanels.Length; ++panelIndex)
                        {
                            GameObject fearPanel = fearPanels[panelIndex];
                            fearPanel.SetActive(true);
                            fearPanel.transform.position = activePanelPos[panelIndex];
                        }                        
                        break;
                    case 3:
                        for (int panelIndex = 0; panelIndex < fearPanels.Length; ++panelIndex)
                        {
                            GameObject fearPanel = fearPanels[panelIndex];
                            fearPanel.SetActive(false);
                            fearPanel.transform.position = fearPanelPos[panelIndex];
                        }
                        for (int panelIndex = 0; panelIndex < tut2HappinessPanels.Length; ++panelIndex)
                        {
                            GameObject happinessPanel = tut2HappinessPanels[panelIndex];
                            happinessPanel.SetActive(true);
                            happinessPanel.transform.position = activePanelPos[panelIndex];
                        }
                        activePanels = 0;
                        ++tutorialIndex;                        
                        break;
                    default:
                        break;
                }
            }
            bblWasPressed = false;
        }
        prevActivePanels = activePanels;

        return GameStateMachine.GameStateName.FRONTEND;
    }
    private GameStateMachine.GameStateName UpdateTutorialTwo()
    {
        //same amount of panels but with only 1 (or more) correct emotion to identify
        //user must ray interact and select correct one to progress to the next emotion
        //when correct choice flash green and move to next panels
        //if incorrect flash red and wait for user to correctly input, no punishment

        //move panels only when correct emotion picked
        //use tags to check correct emotion was picked?
        //can we have a bank of panels that are tagged with their emotion
        //and build the tutorials off that? randomising the picks
        //if we do not we should at least randomise the order
        //if (activePanels < 4)
        //{
        //    activePanels++;
        //}

        //fist time in should hit this
        if (prevActivePanels != activePanels)
        {
            Debug.Log("GAMESTATE Swapping panels, prevAvctivePanels, activePanels" + prevActivePanels + ", " + activePanels);
            switch (prevActivePanels)
            {
                case 0:
                    for (int panelIndex = 0; panelIndex < tut2HappinessPanels.Length; ++panelIndex)
                    {
                        GameObject happinessPanel = tut2HappinessPanels[panelIndex];
                        happinessPanel.SetActive(false);
                        //happinessPanel.transform.position = happinessPanelPos[panelIndex];
                    }
                    for (int panelIndex = 0; panelIndex < sadnessPanels.Length; ++panelIndex)
                    {
                        GameObject sadnessPanel = tut2SadPanels[panelIndex];
                        sadnessPanel.SetActive(true);
                        sadnessPanel.transform.position = activePanelPos[panelIndex];
                    }
                    break;
                case 1:
                    for (int panelIndex = 0; panelIndex < tut2SadPanels.Length; ++panelIndex)
                    {
                        GameObject sadnessPanel = tut2SadPanels[panelIndex];
                        sadnessPanel.SetActive(false);
                        sadnessPanel.transform.position = sadnessPanelPos[panelIndex];
                    }
                    for (int panelIndex = 0; panelIndex < tut2AngerPanels.Length; ++panelIndex)
                    {
                        GameObject angerPanel = tut2AngerPanels[panelIndex];
                        angerPanel.SetActive(true);
                        angerPanel.transform.position = activePanelPos[panelIndex];
                    }
                    break;
                case 2:
                    for (int panelIndex = 0; panelIndex < tut2AngerPanels.Length; ++panelIndex)
                    {
                        GameObject angerPanel = tut2AngerPanels[panelIndex];
                        angerPanel.SetActive(false);
                        angerPanel.transform.position = angerPanelPos[panelIndex];
                    }
                    for (int panelIndex = 0; panelIndex < tut2FearPanels.Length; ++panelIndex)
                    {
                        GameObject fearPanel = tut2FearPanels[panelIndex];
                        fearPanel.SetActive(true);
                        fearPanel.transform.position = activePanelPos[panelIndex];
                    }
                    break;
                case 3:
                    for (int panelIndex = 0; panelIndex < tut2FearPanels.Length; ++panelIndex)
                    {
                        GameObject fearPanel = tut2FearPanels[panelIndex];
                        fearPanel.SetActive(false);
                        fearPanel.transform.position = fearPanelPos[panelIndex];
                    }

                    //return GameStateMachine.GameStateName.FRONTEND;
                    ++tutorialIndex;
                    break;
                default:
                    break;
            }
        }

        //instead of next button increasing the panel index we wait until user has made correct choice.
        //need to make slide combinations that are 1-2 of the correct emotion and 2-3 incorrect, user must get all correct to increase activePanels
        //these should be always the same, save the randomisation of images for the assessments.
        //make thos in the editor?

        //set correct panel tag depending on active panel index
        //even for ray selection?

        //TutoriPanel.cs has IsSelected
        //check if any selected then check that is the correct tag
        //only need to check the panels that are active though

        //update states here, need to refactor this into other switch??
        //get panels, get component TutorialPanel, check is selected, check if correct
        switch(activePanels)
        {
            case 0:
                for (int panelIndex = 0; panelIndex < tut2HappinessPanels.Length; ++panelIndex)
                {
                    GameObject tutPanelObject = tut2HappinessPanels[panelIndex];
                    TutorialPanel tutPanel = tutPanelObject.GetComponent<TutorialPanel>();

                    if (tutPanel.IsSelected())
                    {
                        if(tutPanelObject.tag == "HappinessPanel")
                        {
                            //correct, flash green outline or something?

                            //go to next panel index
                            ++activePanels;
                        }
                        else
                        {
                            //incorrect, flash red outline or something?
                        }
                    }
                }

                break;

            case 1:
                for (int panelIndex = 0; panelIndex < tut2SadPanels.Length; ++panelIndex)
                {
                    GameObject tutPanelObject = tut2SadPanels[panelIndex];
                    TutorialPanel tutPanel = tutPanelObject.GetComponent<TutorialPanel>();

                    if (tutPanel.IsSelected())
                    {
                        if (tutPanelObject.tag == "SadnessPanel")
                        {
                            //correct, flash green outline or something?

                            //go to next panel index
                            ++activePanels;
                        }
                        else
                        {
                            //incorrect, flash red outline or something?
                        }
                    }
                }
                break;

            case 2:
                for (int panelIndex = 0; panelIndex < tut2AngerPanels.Length; ++panelIndex)
                {
                    GameObject tutPanelObject = tut2AngerPanels[panelIndex];
                    TutorialPanel tutPanel = tutPanelObject.GetComponent<TutorialPanel>();

                    if (tutPanel.IsSelected())
                    {
                        if (tutPanelObject.tag == "AngerPanel")
                        {
                            //correct, flash green outline or something?

                            //go to next panel index
                            ++activePanels;
                        }
                        else
                        {
                            //incorrect, flash red outline or something?
                        }
                    }
                }
                break;

            case 3:
                for (int panelIndex = 0; panelIndex < tut2FearPanels.Length; ++panelIndex)
                {
                    GameObject tutPanelObject = tut2FearPanels[panelIndex];
                    TutorialPanel tutPanel = tutPanelObject.GetComponent<TutorialPanel>();

                    if (tutPanel.IsSelected())
                    {
                        if (tutPanelObject.tag == "FearPanel")
                        {
                            //correct, flash green outline or something?

                            //go to next panel index
                            ++activePanels;
                        }
                        else
                        {
                            //incorrect, flash red outline or something?
                        }
                    }
                }
                break;

            default:
                break;

        }


        prevActivePanels = activePanels;

        return GameStateMachine.GameStateName.TUTORIALONE;
    }

    private GameStateMachine.GameStateName UpdateTutorialThree()
    {
        //AVATAR TUTORIAL
        return GameStateMachine.GameStateName.FRONTEND;
    }

    //For Transition part of Update, use position as a backup, if xrorigin has moved away from teleport pos then back into intro state
    //should keep position at teleport then
    public void HandleBigBlueButton()
    {
        bblWasPressed = true;
    }
}
