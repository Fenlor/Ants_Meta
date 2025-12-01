using UnityEngine;

public class GameplayState : GameState
{
    public GameObject locomotionObject;
    [Range(0f, 1f)]
    public float timeBuffer = 0.1f;
    private float timer = 0.0f;    
    public GameObject[] happinessPanels;
    public GameObject[] sadnessPanels;
    public GameObject[] angerPanels;
    public GameObject[] fearPanels;
    private bool bblWasPressed = false;
    private int activePanels = 0;
    private int prevAvctivePanels = 0;

    void Start()
    {
        stateName = GameStateMachine.GameStateName.TUTORIALONE;

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

        foreach (GameObject panel in happinessPanels)
        {
            panel.SetActive(true);
        }
    }
    override public GameStateMachine.GameStateName UpdateState()
    {
        if (bblWasPressed)
        {
            if(activePanels < 4)
            {
                activePanels++;
            }
            
            if (prevAvctivePanels != activePanels)
            {
                //Debug.Log("GAMESTATE Swapping panels, prevAvctivePanels, activePanels" + prevAvctivePanels + ", " + activePanels);
                switch (prevAvctivePanels)
                {
                    case 0:
                        foreach (GameObject panel in happinessPanels)
                        {
                            panel.SetActive(false);
                        }
                        foreach (GameObject panel in sadnessPanels)
                        {
                            panel.SetActive(true);
                        }
                        break;
                    case 1:
                        foreach (GameObject panel in sadnessPanels)
                        {
                            panel.SetActive(false);
                        }
                        foreach (GameObject panel in angerPanels)
                        {
                            panel.SetActive(true);
                        }
                        break;
                    case 2:
                        foreach (GameObject panel in angerPanels)
                        {
                            panel.SetActive(false);
                        }
                        foreach (GameObject panel in fearPanels)
                        {
                            panel.SetActive(true);
                        }
                        break;
                    case 3:
                        foreach (GameObject panel in fearPanels)
                        {
                            panel.SetActive(false);
                        }
                        bblWasPressed = false;
                        return GameStateMachine.GameStateName.FRONTEND;
                    default:
                        break;
                }
            }
            bblWasPressed = false;
        }
        prevAvctivePanels = activePanels;

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

    //For Transition part of Update, use position as a backup, if xrorigin has moved away from teleport pos then back into intro state
    //should keep position at teleport then
    public void HandleBigBlueButton()
    {
        bblWasPressed = true;
    }
}
