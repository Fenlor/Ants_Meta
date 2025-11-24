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

    [Range(0f, 1f)]
    public float timeBuffer = 0.05f;
    [Range(0f, 5f)]
    public float sittingIdleTime;
    private float sittingIdleTimer = 0f;

    //ScenarioOneEmotionObject is just the things for this, take out the environment and other things
    //we are using through out the same scenario
    //probably doesnt need the audio listener and clip either as intro should have started this and outro should end it when it wraps up
    public GameObject ScenarioOneEmotionObject;
    public GameObject ScenarioOneAnswersObject;    
    public RecordManager recordManager;
    public bool correctChoice = false;
    private int errors;
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
    public AudioSource successAudio;
    public AudioSource failureAudio;
    public AudioSource instructionVoiceOver;
    public AudioSource happy;
    public AudioSource sad;
    public AudioSource fear;
    public AudioSource anger;
    public int currentChoice = -1;
    public GameObject coinEffectPrefab;
    public int coinScore = 5;
    public int consolationCoinScore = 1;
    public GameObject happyCanvas;
    public GameObject sadCanvas;
    public GameObject angryCanvas;
    public GameObject fearCanvas;

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

        ScenarioOneEmotionObject.SetActive(true);
    }

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
            instructionVoiceOver.Play();            
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
                happy.Play();
            }

            happyCanvas.GetComponent<Outline>().enabled = true;
            sadCanvas.GetComponent<Outline>().enabled = false;
            angryCanvas.GetComponent<Outline>().enabled = false;
            fearCanvas.GetComponent<Outline>().enabled = false;
        }
        else if (currentChoice == 1)
        {
            if (sad != null)
            {
                sad.Play();
            }

            happyCanvas.GetComponent<Outline>().enabled = false;
            sadCanvas.GetComponent<Outline>().enabled = true;
            angryCanvas.GetComponent<Outline>().enabled = false;
            fearCanvas.GetComponent<Outline>().enabled = false;
        }
        else if (currentChoice == 2)
        {
            if (anger != null)
            {
                anger.Play();
            }

            happyCanvas.GetComponent<Outline>().enabled = false;
            sadCanvas.GetComponent<Outline>().enabled = false;
            angryCanvas.GetComponent<Outline>().enabled = true;
            fearCanvas.GetComponent<Outline>().enabled = false;
        }
        else if (currentChoice == 3)
        {
            if (fear != null)
            {
                fear.Play();
            }

            happyCanvas.GetComponent<Outline>().enabled = false;
            sadCanvas.GetComponent<Outline>().enabled = false;
            angryCanvas.GetComponent<Outline>().enabled = false;
            fearCanvas.GetComponent<Outline>().enabled = true;
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
                    successAudio.Play();
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
                    failureAudio.Play();
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

            //any choice resets currentChoice to 0 and disables outlines
            currentChoice = 0;
            happyCanvas.GetComponent<Outline>().enabled = false;
            sadCanvas.GetComponent<Outline>().enabled = false;
            angryCanvas.GetComponent<Outline>().enabled = false;
            fearCanvas.GetComponent<Outline>().enabled = false;
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
