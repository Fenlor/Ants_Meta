using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static System.TimeZoneInfo;

//Shows end of scenario canvas along with voice over
//add coins found in scenario to users total and save to file
//either go back to frontend or next activity if applicable

public class ScenarioOneOutroState : GameState
{
    public GameObject locomotionObject;
    [Range(0f, 1f)]
    public float timeBuffer = 0.05f;
    private float timer = 0.0f;
    private Vector3 posAtTeleport;

    //ScenarioOneEmotionObject is just the things for this, take out the environment and other things
    //we are using through out the same scenario
    public GameObject ScenarioOneEnvironmentObject;
    public GameObject ScenarioOnePlayers;
    public GameObject ScenarioOneOutroObject;
    public AudioClip backGroundMusic;
    public AudioListener audioListener;
    public RecordManager recordManager;

    private float transitionTimeDelta = 0.2f;
    public float transitionTimer = 0f;

    private bool bblWasPressed = false;
    //public int activeEmotion = 0;
    //private int prevActiveEmotion = 0;
    //private int tutorialIndex = 0;
    public bool nextButtonPressed = false;
    private int errors;
    //private int guessIndex = -1;

    private float assessmentTimer;

    public GameObject hintMenu;

    public GameObject experienceRing;

    private int totalCoins = 0;

    //public ScenarioStateMachine scenarioStateMachine;

    void Start()
    {
        stateName = GameStateMachine.GameStateName.SCENARIOONEOUTRO;
    }
    override public void InitialiseState()
    {
        ScenarioOneEnvironmentObject.SetActive(true);
        ScenarioOnePlayers.SetActive(true);
        ScenarioOneOutroObject.SetActive(true);

        nextButtonPressed = false;
        transitionTimer = 0f;
        assessmentTimer = 0f;
        errors = 0;
        totalCoins = 0;

        if (experienceRing != null && recordManager != null)
        {
            totalCoins = recordManager.GetComponent<RecordManager>().GetBankedCoins();
            experienceRing.GetComponent<ExperienceRing>().SetStartingXP(totalCoins);            
        }
    }
    override public GameStateMachine.GameStateName UpdateState()
    {    
        assessmentTimer += Time.deltaTime;
        Debug.Log("assessmentTimer: " + assessmentTimer);
        if (assessmentTimer > 1)
        {
            if (experienceRing != null && recordManager != null)
            {
                int coinsToAdd = recordManager.GetActivityCoinValue();
                experienceRing.GetComponent<ExperienceRing>().AddXP(coinsToAdd);
                recordManager.activityCoinCount = 0;
            }
        }
        return UpdateOutro();
    }
    override public void ShutDownState()
    {
        //tenable locomationObject
        //locomotionObject.SetActive(true);
        if (recordManager != null)
        {
            User loggedInUser = recordManager.GetLoggedInUser();
            if (loggedInUser != null)
            {
                loggedInUser.scenarioOneTime = assessmentTimer;
                recordManager.WriteCSV();
            }
        }

        ScenarioOneOutroObject.SetActive(false);
        ScenarioOneEnvironmentObject.SetActive(false);
        ScenarioOnePlayers.SetActive(false);


        //if (backGroundMusic is not null)
        //{
        //    //backGroundMusic.oneshot
        //    GetComponent<AudioSource>().Stop();
        //}
    }
    override public void TeleOn()
    {
        //hasTeleportedIn = true;
    }
   
    private GameStateMachine.GameStateName UpdateOutro()
    {
        //so, we have active emotion and previou emotion, use to turn on and off the models.
        //do we float panels with emotion names on them for the user to pick? sure!
        //use timer like in tutorial two

        if (nextButtonPressed)
        {
            nextButtonPressed = false;
            return GameStateMachine.GameStateName.FRONTEND;
        }

        return GameStateMachine.GameStateName.SCENARIOONEOUTRO;
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
        //activeEmotion++;
    }

    public void NextPressed()
    {
        nextButtonPressed = true;
    }
}
