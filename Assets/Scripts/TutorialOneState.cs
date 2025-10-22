using UnityEngine;
using UnityEngine.Rendering;

public class TutorialOneState : GameState
{
    public GameObject locomotionObject;
    [Range(0f, 1f)]
    public float timeBuffer = 0.05f;
    private float timer = 0.0f;
    private Vector3 posAtTeleport;

    //take in a GameObject for each emotion that is a ui canvas
    //assume already inactive if desired
    public GameObject HappinessCanvas;
    public GameObject SadnessCanvas;
    public GameObject AngerCanvas;
    public GameObject FearCanvas;

    private bool bblWasPressed = false;
    private int activeCanvas = 0;
    private int prevActiveCanvas = 0;       

    void Start()
    {
        stateName = GameStateMachine.GameStateName.TUTORIALONE;       
    }
    override public void InitialiseState()
    {
        //*new note, the locomotion idea was old, if we have a decent sized ray interactable canvas, we might be ok*
        //disable locomotionObject seems to work ok
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

        activeCanvas = 0;
        prevActiveCanvas = 0;
        HappinessCanvas.SetActive(true);
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

        if (bblWasPressed)
        {
            if (activeCanvas < 4)
            {
                activeCanvas++;
            }

            if (prevActiveCanvas != activeCanvas)
            {
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
                        break;                  
                    default:
                        break;
                }
            }
            bblWasPressed = false;
        }
        prevActiveCanvas = activeCanvas;

        if(activeCanvas == 4)
        {
            return GameStateMachine.GameStateName.TUTORIALTWO;
        }
        else
        {
            return GameStateMachine.GameStateName.TUTORIALONE;
        }           
    }
  
    public void HandleBigBlueButton()
    {
        bblWasPressed = true;
    }

    public void NextButtonPressed()
    {
        bblWasPressed = true;
    }
}
