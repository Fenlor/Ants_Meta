using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static System.TimeZoneInfo;

public class TutorialThreeState : GameState
{
    public GameObject locomotionObject;
    [Range(0f, 1f)]
    public float timeBuffer = 0.05f;
    private float timer = 0.0f;
    private Vector3 posAtTeleport;

    public GameObject happyTutorialObject;
    public GameObject sadTutorialObject;
    public GameObject angerTutorialObject;
    public GameObject fearTutorialObject;

    public GameObject happyCanvas;
    public GameObject sadCanvas;
    public GameObject angerCanvas;
    public GameObject fearCanvas;
    
    public RecordManager recordManager;

    private float transitionTimeDelta = 0.2f;
    public float transitionTimer = 0f;

    private bool bblWasPressed = false;
    public int activeEmotion = 0;
    private int prevActiveEmotion = 0;
    private int tutorialIndex = 0;
    private bool correctChoice = false;
    private int guessIndex = -1;

    void Start()
    {
        stateName = GameStateMachine.GameStateName.TUTORIALTHREE;
    }
    override public void InitialiseState()
    {
        happyTutorialObject.SetActive(true);
        happyCanvas.SetActive(true);
        sadCanvas.SetActive(false);
        angerCanvas.SetActive(false);
        fearCanvas.SetActive(false);

        sadTutorialObject.SetActive(false);
        angerTutorialObject.SetActive(false);
        fearTutorialObject.SetActive(false);
        activeEmotion = 0;
        prevActiveEmotion = 0;
        correctChoice = false;
        transitionTimer = 0f;
        guessIndex = -1;
    }
    override public GameStateMachine.GameStateName UpdateState()
    {    
        return UpdateTutorialOne();
    }
    override public void ShutDownState()
    {
        //tenable locomationObject
        //locomotionObject.SetActive(true);

        happyTutorialObject.SetActive(false);
        sadTutorialObject.SetActive(false);
        angerTutorialObject.SetActive(false);
        fearTutorialObject.SetActive(false);
    }
    override public void TeleOn()
    {
        //hasTeleportedIn = true;
    }

    private GameStateMachine.GameStateName UpdateTutorialOne()
    {
        //so, we have active emotion and previou emotion, use to turn on and off the models.
        //do we float panels with emotion names on them for the user to pick? sure!
        //use timer like in tutorial two

        if (prevActiveEmotion != activeEmotion)
        {
            transitionTimer += Time.deltaTime;
            if (transitionTimer >= transitionTimeDelta)
            {
                transitionTimer = 0;
                correctChoice = false;

                switch (prevActiveEmotion)
                {
                    case 0:
                        happyTutorialObject.SetActive(false);
                        sadTutorialObject.SetActive(true);
                        happyCanvas.SetActive(false);
                        sadCanvas.SetActive(true);
                        break;
                    case 1:
                        sadTutorialObject.SetActive(false);
                        angerTutorialObject.SetActive(true);
                        sadCanvas.SetActive(false);
                        angerCanvas.SetActive(true);
                        break;
                    case 2:
                        angerTutorialObject.SetActive(false);
                        fearTutorialObject.SetActive(true);
                        angerCanvas.SetActive(false);
                        fearCanvas.SetActive(true);
                        break;
                    case 3:
                        fearTutorialObject.SetActive(false);
                        fearCanvas.SetActive(false);
                        //Go straigt to scenario one if we havn't completed it yet
                        if(recordManager != null)
                        {
                            RecordManager.User loggedInUser = recordManager.GetLoggedInUser();
                            if (loggedInUser != null)
                            {
                                if(loggedInUser.scenarioOneTime == 0f)
                                {
                                    return GameStateMachine.GameStateName.SCENARIOONEINTRO;
                                }
                            }
                        }
                        return GameStateMachine.GameStateName.SCENARIOONEINTRO;                        
                    default:
                        break;
                }

                prevActiveEmotion = activeEmotion;
            }
        }

        return GameStateMachine.GameStateName.TUTORIALTHREE;
    } 
    public void HandleBigBlueButton()
    {
        bblWasPressed = true;
    }

    public void NextButtonPressed()
    {
        bblWasPressed = true;
    }

    public void CorrectChoice()
    {
        activeEmotion++;
    }

    public void MakeGuess(int guess)
    {
        //0 - Happy
        //1 - Sad
        //2 - Anger
        //3 - Fear

        if(guess == activeEmotion)
        {
            activeEmotion++;
        }      
    }
}
