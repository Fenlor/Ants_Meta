using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static System.TimeZoneInfo;

//This will play out the first part of the scenario and then start timer and expect user to pick correct emotion on display.

//TODO, play out animation, start timer and show options once it has finished

public class ScenarioOneSocialState : GameState
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
    public GameObject scenarioOneSocialObject;
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

    public GameObject anchorPoint;

    //public ScenarioStateMachine scenarioStateMachine;

    [Range (0f, 2f)]
    public float radiusCheck = 1f;

    public GameObject cameraRig;

    private bool instructionsRead = false;
    private float interactionTimer = 0f;
    [Range(0f, 2f)]
    public float interactionTimeNeeded = 1f;
    private bool interactionComplete = false;

    public GameObject rightHandAnchor;
    public GameObject leftHandAnchor;
    public GameObject rightShoulderObject;
    public GameObject leftShoulderObject;

    public AudioClip successAudio;
    public AudioClip failureAudio;

    private bool isRightHandCloseToShoulder = false;
    private bool isLeftHandCloseToShoulder = false;
    private float shoulderTouchDistance = 0.1f;

    private float positiveFeedbackTime = 1f;
    private float positiveFeedbackTimer = 0;

    public AudioClip voiceInstructions;

    void Start()
    {
        stateName = GameStateMachine.GameStateName.SCENARIOONESOCIAL;

        //delay the anim trigger
        //lailaObject.GetComponent<Animator>().SetTrigger("SitToSad");

        
    }
    override public void InitialiseState()
    {
        instructionsRead = false;
        correctChoice = false;
        interactionComplete = false;
        transitionTimer = 0f;
        assessmentTimer = 0f;
        sittingIdleTimer = 0f;
        errors = 0;
        isRightHandCloseToShoulder = false;
        isLeftHandCloseToShoulder = false;

        if (voiceInstructions is not null)
        {
            GetComponent<AudioSource>().PlayOneShot(voiceInstructions);
        }
        //if (backGroundMusic is not null) 
        //{
        //    //backGroundMusic.oneshot
        //    //backGroundMusic.Play();
        //    GetComponent<AudioSource>().loop = true;
        //    GetComponent<AudioSource>().clip = backGroundMusic;
        //    GetComponent<AudioSource>().Play();
        //}

        //locomotionObject.SetActive(false);

        scenarioOneSocialObject.SetActive(true);
    }
    //private ScenarioStateMachine.STATE UpdateScenarioState()
    //{

    //    return ScenarioStateMachine.STATE.INTRO;
    //}
    override public GameStateMachine.GameStateName UpdateState()
    {
        //make sure user has pressed next on instructions
        if (!isAssessing && instructionsRead && !interactionComplete)
        {
            isAssessing = true;
        }

        if (isAssessing)
        {
            assessmentTimer += Time.deltaTime;
            //then check if player is standing within radius of Laila AND looking at her genral direction FOR a set amount of time (2 seconds to start testing>)
            if (lailaObject != null && cameraRig != null)
            {
                Vector3 targetPos = lailaObject.transform.position;
                Vector3 playerPos = cameraRig.transform.position;
                float distToTarget = Vector3.Distance(targetPos, playerPos);

                //CHECK CONTROLLER/HAND DIST TO EACH SHOULDER

                if (leftShoulderObject != null && rightShoulderObject != null)
                {
                    Vector3 leftShoulderPos = leftShoulderObject.transform.position;
                    Vector3 rightShoulderPos = rightShoulderObject.transform.position; 

                    if (rightHandAnchor != null)
                    {
                        Vector3 rightHandAnchorPos = rightHandAnchor.transform.position;
                        float rightHandDistToLeftShoulder = Vector3.Distance(rightHandAnchorPos, leftShoulderPos);
                        float rightHandDistToRightShoulder = Vector3.Distance(rightHandAnchorPos, rightShoulderPos);

                        if (rightHandDistToLeftShoulder <= shoulderTouchDistance || rightHandDistToRightShoulder <= shoulderTouchDistance)
                        {
                            isRightHandCloseToShoulder = true;
                        }
                    }
                    if (leftHandAnchor != null)
                    {
                        Vector3 leftHandAnchorPos = leftHandAnchor.transform.position;
                        float leftHandDistToLeftShoulder = Vector3.Distance(leftHandAnchorPos, leftShoulderPos);
                        float leftHandDistToRightShoulder = Vector3.Distance(leftHandAnchorPos, rightShoulderPos);

                        if (leftHandDistToLeftShoulder <= shoulderTouchDistance || leftHandDistToRightShoulder <= shoulderTouchDistance)
                        {
                            isLeftHandCloseToShoulder = true;
                        }
                    }

                    if(isRightHandCloseToShoulder || isLeftHandCloseToShoulder)
                    {
                        interactionTimer += Time.deltaTime;
                        if (interactionTimer > interactionTimeNeeded) //still need to check for facing, and if player looks away we start that again.
                        {
                            //should there also be a choice component

                            //we can give feedback to the player when they have gotten this far (close and looking) and then expect them to do another step

                            //stop timer
                            //set bool
                            //maybe lock player movement? unsure about this
                            interactionComplete = true;
                            isAssessing = false;

                            interactionTimer = 0f;

                            if (successAudio is not null)
                            {
                                //backGroundMusic.oneshot
                                //backGroundMusic.Play();
                                GetComponent<AudioSource>().clip = successAudio;
                                GetComponent<AudioSource>().Play();

                                lailaObject.GetComponent<Animator>().SetTrigger("Neutral");
                            }
                        }
                    }
                    else
                    {
                        interactionTimer = 0f;
                    }                    
                }              


                //get camera facing and make sure player is looking towards the front of Laila (90 degrees on side is ok)

                //if (distToTarget <= radiusCheck)
                //{
                //    //need to time how long player is in within

                //    interactionTimer += Time.deltaTime;
                //    if (interactionTimer > interactionTimeNeeded) //still need to check for facing, and if player looks away we start that again.
                //    {
                //        //should there also be a choice component

                //        //we can give feedback to the player when they have gotten this far (close and looking) and then expect them to do another step

                //        //stop timer
                //        //set bool
                //        //maybe lock player movement? unsure about this
                //        interactionComplete = true;

                //        interactionTimer = 0f;
                //    }
                //}
                //else
                //{
                //    interactionTimer = 0f;
                //}
            }
        }
        

        Animator lailaAnimator = lailaObject.GetComponent<Animator>();

        if (sittingIdleTimer >= sittingIdleTime)
        {
            //lailaAnimator.SetTrigger("Disbelief");
        }
        sittingIdleTimer += Time.deltaTime;

        //check to see which anim clip is playing
        //if (!isAssessing)
        //{
        //    string clipName = lailaAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        //    Debug.Log("LAILA ANIM CLIP NAME: " + clipName);
        //    if (clipName == "Human.rig|SitSadIdle")
        //    {
        //        isAssessing = true;
        //        //ScenarioOneEmotionObject.SetActive(true);
        //    }
        //}        


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

        scenarioOneSocialObject.SetActive(false);

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

        if (interactionComplete)
        {
            return GameStateMachine.GameStateName.SCENARIOONEEMPATHY;
        }

        return GameStateMachine.GameStateName.SCENARIOONESOCIAL;
    } 
    public void HandleBigBlueButton()
    {
        bblWasPressed = true;
    }

    public void NextButtonPressed()
    {
        bblWasPressed = true;
    }

    public void InstructionsButtonPressed()
    {
        instructionsRead = true;
        scenarioOneSocialObject.SetActive(false);
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
        

        if(guess == 1)
        {
            correctChoice = true;
        }
        else
        {
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
