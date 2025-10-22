using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static System.TimeZoneInfo;

//This will play out the first part of the scenario and then start timer and expect user to pick correct emotion on display.

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

        //if (backGroundMusic is not null && audioSourceObject is not null) 
        //{
        //    //backGroundMusic.oneshot
        //    //backGroundMusic.Play();
        //    audioSourceObject.GetComponent<AudioSource>().loop = true;
        //    audioSourceObject.GetComponent<AudioSource>().clip = backGroundMusic;
        //    audioSourceObject.GetComponent<AudioSource>().Play();
        //}
    }
    //private ScenarioStateMachine.STATE UpdateScenarioState()
    //{

    //    return ScenarioStateMachine.STATE.INTRO;
    //}
    override public GameStateMachine.GameStateName UpdateState()
    {    
        assessmentTimer += Time.deltaTime;
        return UpdateTutorialOne();
    }
    override public void ShutDownState()
    {
        //tenable locomationObject
        //locomotionObject.SetActive(true);
        if (recordManager != null)
        {
            RecordManager.User loggedInUser = recordManager.GetLoggedInUser();
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
   
    private GameStateMachine.GameStateName UpdateTutorialOne()
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
