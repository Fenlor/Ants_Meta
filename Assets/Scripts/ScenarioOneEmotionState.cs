using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static System.TimeZoneInfo;

//This will play out the first part of the scenario and then start timer and expect user to pick correct emotion on display.

//TODO, play out animation, start timer and show options once it has finished

public class ScenarioOneEmotionState : GameState
{
    public GameObject locomotionObject;
    [Range(0f, 1f)]
    public float timeBuffer = 0.05f;
    private float timer = 0.0f;
    private Vector3 posAtTeleport;

    [Range(0f, 5f)]
    public float sittingIdleTime;
    private float sittingIdleTimer = 0f;

    //ScenarioOneEmotionObject is just the things for this, take out the environment and other things
    //we are using through out the same scenario
    //probably doesnt need the audio listener and clip either as intro should have started this and outro should end it when it wraps up
    public GameObject ScenarioOneEmotionObject;
    //public AudioClip backGroundMusic;
    //public AudioListener audioListener;
    public RecordManager recordManager;

    private float transitionTimeDelta = 0.2f;
    public float transitionTimer = 0f;

    private bool bblWasPressed = false;
    //public int activeEmotion = 0;
    //private int prevActiveEmotion = 0;
    //private int tutorialIndex = 0;
    public bool correctChoice = false;
    private int errors;
    //private int guessIndex = -1;

    public bool isAssessing = false;
    private float assessmentTimer;

    public GameObject lailaObject;

    private float positiveFeedbackTime = 1f;
    private float positiveFeedbackTimer = 0;

    public AudioClip successAudio;
    public AudioClip failureAudio;
    
    //public ScenarioStateMachine scenarioStateMachine;

    void Start()
    {
        stateName = GameStateMachine.GameStateName.SCENARIOONEEMOTION;

        //delay the anim trigger
        //lailaObject.GetComponent<Animator>().SetTrigger("SitToSad");
    }
    override public void InitialiseState()
    {      
        correctChoice = false;
        transitionTimer = 0f;
        assessmentTimer = 0f;
        sittingIdleTimer = 0f;
        errors = 0;

        //if (backGroundMusic is not null) 
        //{
        //    //backGroundMusic.oneshot
        //    //backGroundMusic.Play();
        //    GetComponent<AudioSource>().loop = true;
        //    GetComponent<AudioSource>().clip = backGroundMusic;
        //    GetComponent<AudioSource>().Play();
        //}
    }
    //private ScenarioStateMachine.STATE UpdateScenarioState()
    //{

    //    return ScenarioStateMachine.STATE.INTRO;
    //}
    override public GameStateMachine.GameStateName UpdateState()
    {
        Animator lailaAnimator = lailaObject.GetComponent<Animator>();

        if (sittingIdleTimer >= sittingIdleTime)
        {
            lailaAnimator.SetTrigger("Disbelief");
        }
        sittingIdleTimer += Time.deltaTime;

        //check to see which anim clip is playing
        if (!isAssessing)
        {
            string clipName = lailaAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            Debug.Log("LAILA ANIM CLIP NAME: " + clipName);
            if (clipName == "Human.rig|SitSadIdle")
            {
                isAssessing = true;
                ScenarioOneEmotionObject.SetActive(true);
            }
        }        

        if(isAssessing)
        {
            assessmentTimer += Time.deltaTime;
        }        

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
                //TODO
                //ADD ERRORS TO CSV!!! and whatever else is needed
                //NEED TO ADD TIMERS FOR EACH STAGE OF THE SCENARIOS
                loggedInUser.scenarioOneTime = assessmentTimer;
                recordManager.WriteCSV();
            }
        }

        ScenarioOneEmotionObject.SetActive(false);

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

        if (correctChoice)
        {
            //
            positiveFeedbackTimer += Time.deltaTime;
            if (positiveFeedbackTimer > positiveFeedbackTime)
            {
                return GameStateMachine.GameStateName.SCENARIOONESOCIAL;
            }            
        }

        return GameStateMachine.GameStateName.SCENARIOONEEMOTION;
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

    public void MakeGuess(int guess)
    {
        //0 - Happy
        //1 - Sad
        //2 - Anger
        //3 - Fear

        //if(guess == activeEmotion)
        //{
        //    activeEmotion++;
        //}      
        

        if(!correctChoice && guess == 1)
        {   
            //PLAY POSITIVE FEEDBACK
            //"That's right, well done" text plus voice 
            correctChoice = true;

            if (successAudio is not null)
            {
                //backGroundMusic.oneshot
                //backGroundMusic.Play();
                GetComponent<AudioSource>().clip = successAudio;
                GetComponent<AudioSource>().Play();
            }
        }
        else
        {
            //SHOW FEEDBACK, IS NEGATIVE THE RIGHT WORD OR IS IT REINFORCMENT OR SOMETHING
            //already showing red and perhaps an error noise so the text should provide support to try again
            //"Let's try that again, try and put yourself in Laila's shoes"
            //What about different pentaly scores? If happy or fearful they are way off, if angry then its close

            if (failureAudio is not null)
            {
                //backGroundMusic.oneshot
                //backGroundMusic.Play();
                GetComponent<AudioSource>().clip = failureAudio;
                GetComponent<AudioSource>().Play();
            }            

            ++errors;

            if (recordManager != null)
            {
                RecordManager.User loggedInUser = recordManager.GetLoggedInUser();
                if (loggedInUser != null)
                {
                    loggedInUser.scenarioOneErrors++;
                }
            }
        }
    }
}
