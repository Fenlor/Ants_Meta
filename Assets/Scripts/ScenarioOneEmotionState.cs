using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using static System.TimeZoneInfo;

//This will play out the first part of the scenario and then start timer and expect user to pick correct emotion on display.

//TODO, play out animation, start timer and show options once it has finished

public class ScenarioOneEmotionState : GameState
{
    //public GameObject locomotionObject;
    [Range(0f, 1f)]
    public float timeBuffer = 0.05f;
    //private float timer = 0.0f;
    //private Vector3 posAtTeleport;

    [Range(0f, 5f)]
    public float sittingIdleTime;
    private float sittingIdleTimer = 0f;

    //ScenarioOneEmotionObject is just the things for this, take out the environment and other things
    //we are using through out the same scenario
    //probably doesnt need the audio listener and clip either as intro should have started this and outro should end it when it wraps up
    public GameObject ScenarioOneEmotionObject;
    public GameObject ScenarioOneAnswersObject;
    //public AudioClip backGroundMusic;
    //public AudioListener audioListener;
    public RecordManager recordManager;

    //private float transitionTimeDelta = 0.2f;
    //public float transitionTimer = 0f;

    //private bool bblWasPressed = false;
    //public int activeEmotion = 0;
    //private int prevActiveEmotion = 0;
    //private int tutorialIndex = 0;
    public bool correctChoice = false;
    private int errors;
    //private int guessIndex = -1;
    private bool shouldStartScenario = false;
    //can we stop the user from spamming the read again?
    private bool hasPlayedVoiceOver = false;

    public bool isAssessing = false;
    private float assessmentTimer;
    public float scoreThresholdMax = 25f;
    public AudioSource incorrectVoiceInstructions;//also use a text bubble with this

    public GameObject lailaObject;

    private float positiveFeedbackTime = 1f;
    private float positiveFeedbackTimer = 0;

    public AudioClip successAudio;
    public AudioClip failureAudio;
    public AudioClip instructionVoiceOver;
    public AudioClip happy;
    public AudioClip sad;
    public AudioClip fear;
    public AudioClip anger;

    public int currentChoice = -1;

    public GameObject coinEffectPrefab;
    public int coinScore = 5;
    public int consolationCoinScore = 1;


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
        //transitionTimer = 0f;
        assessmentTimer = 0f;
        sittingIdleTimer = 0f;
        errors = 0;
        currentChoice = -1;

        //if (backGroundMusic is not null) 
        //{
        //    //backGroundMusic.oneshot
        //    //backGroundMusic.Play();
        //    GetComponent<AudioSource>().loop = true;
        //    GetComponent<AudioSource>().clip = backGroundMusic;
        //    GetComponent<AudioSource>().Play();
        //}

        ScenarioOneEmotionObject.SetActive(true);
    }

    //private ScenarioStateMachine.STATE UpdateScenarioState()
    //{

    //    return ScenarioStateMachine.STATE.INTRO;
    //}
    override public GameStateMachine.GameStateName UpdateState()
    {
        //what until user presses next button
        //they can press the read button before this as well
        if (shouldStartScenario)
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
                    ScenarioOneAnswersObject.SetActive(true);
                }
            }

            if (isAssessing)
            {
                assessmentTimer += Time.deltaTime;
            }
        }
        else if(!hasPlayedVoiceOver)
        {
            ReadIntro();
            hasPlayedVoiceOver = true;
        }
       

        return UpdateTutorialOne();
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
                //TODO
                //ADD ERRORS TO CSV!!! and whatever else is needed
                //NEED TO ADD TIMERS FOR EACH STAGE OF THE SCENARIOS
                loggedInUser.scenarioOneTime = assessmentTimer;
                recordManager.WriteCSV();
            }
        }

        ScenarioOneEmotionObject.SetActive(false);
        ScenarioOneAnswersObject.SetActive(false);

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

        //if plauer is too slow, play hint audio and show text to match
        if (assessmentTimer > scoreThresholdMax)
        {
            //make sure incorrectVoiceInstructions doesn't play over the top of itself
            
            if (incorrectVoiceInstructions != null && !incorrectVoiceInstructions.isPlaying)
            {
                incorrectVoiceInstructions.Play();
            }
        }
        
        return GameStateMachine.GameStateName.SCENARIOONEEMOTION;
    } 
    //public void HandleBigBlueButton()
    //{
    //    bblWasPressed = true;
    //}

    public void NextButtonPressed()
    {
        shouldStartScenario = true;

        ScenarioOneEmotionObject.SetActive(false);
    }
    public void ReadIntro()
    {
        if (instructionVoiceOver is not null)
        {
            GetComponent<AudioSource>().PlayOneShot(instructionVoiceOver);            
        }
    }
    public void CorrectChoice()
    {
        //activeEmotion++;
    }

    public void SetCurrentChoice(int newChoice)
    {
        //set choice int and read voice over
        currentChoice = newChoice;

        if (currentChoice == 0)
        {
            if(happy != null)
            {
                GetComponent<AudioSource>().PlayOneShot(happy);
            }
        }
        else if (currentChoice == 1)
        {
            if (sad != null)
            {
                GetComponent<AudioSource>().PlayOneShot(sad);
            }
        }
        else if (currentChoice == 2)
        {
            if (fear != null)
            {
                GetComponent<AudioSource>().PlayOneShot(anger);
            }
        }
        else if (currentChoice == 3)
        {
            if (anger != null)
            {
                GetComponent<AudioSource>().PlayOneShot(fear);
            }
        }
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

        if (currentChoice >= 0)
        {
            if (!correctChoice && currentChoice == 1)
            {
                //PLAY POSITIVE FEEDBACK
                //"That's right, well done" text plus voice 
                correctChoice = true;

                if (successAudio is not null)
                {
                    GetComponent<AudioSource>().PlayOneShot(successAudio);
                }

                //if within time threshold, give 5 coins. otherwise give 1 coin.
                if (assessmentTimer <= scoreThresholdMax)
                {
                    SpawnCoin(coinScore);
                }
                else
                {
                    SpawnCoin(consolationCoinScore);
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
                    GetComponent<AudioSource>().PlayOneShot(failureAudio);
                }

                //do we queue audio up so important audio still plays but not over the top of other important audio
                if(incorrectVoiceInstructions != null)
                {
                    incorrectVoiceInstructions.Play();
                }

                ++errors;

                if (recordManager != null)
                {
                    User loggedInUser = recordManager.GetLoggedInUser();
                    if (loggedInUser != null)
                    {
                        loggedInUser.scenarioOneErrors++;
                    }
                }
            }
        }
    }

    public void SpawnCoin(int value)
    {
        if (coinEffectPrefab != null)
        {
            GameObject coin = Instantiate(coinEffectPrefab, ScenarioOneAnswersObject.transform.position, Quaternion.identity);
            coin.SetActive(true);
            coin.GetComponent<Coin>().coinValue = value;
        }        
    }
}
