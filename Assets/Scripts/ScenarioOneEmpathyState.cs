using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static System.TimeZoneInfo;

//This will play out the first part of the scenario and then start timer and expect user to pick correct emotion on display.

//TODO, play out animation, start timer and show options once it has finished

public class ScenarioOneEmpathyState : GameState
{
    //public GameObject locomotionObject;
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
    public GameObject scenarioOneEmpathyObject;
    public GameObject empathyAnswersObject;
    public RecordManager recordManager;

    private float transitionTimeDelta = 0.2f;
    public float transitionTimer = 0f;
    private bool bblWasPressed = false;
    public bool correctChoice = false;
    private int errors;
    public bool isAssessing = false;
    private float assessmentTimer;
    public GameObject lailaObject;
    public GameObject hintMenuObject;
    //public GameObject cameraRig;
    public AudioSource socialAudio;
    private bool instructionsRead = false;
    public AudioSource successAudio;
    public AudioSource failureAudio;
    public AudioSource dialogueLeaveLaila;
    public AudioSource dialogueStayAndPlay;
    public AudioSource dialogueStaySilent;

    public GameObject hintMenu;

    //private bool isRightHandCloseToShoulder = false;
    //private bool isLeftHandCloseToShoulder = false;
    //private float shoulderTouchDistance = 0.1f;


    //EMPATHY STATE NEEDS
    //Canvas with Question prompt
    //"Think about why Laila is feeling sad"
    //"What would you say to her now"

    public GameObject empathyChoiceOne;
    public GameObject empathyChoiceTwo;
    public GameObject empathyChoiceThree;
    public GameObject empathyChoiceVerify;

    public int currentChoice = -1;

    void Start()
    {
        stateName = GameStateMachine.GameStateName.SCENARIOONEEMPATHY;

        //delay the anim trigger
        //lailaObject.GetComponent<Animator>().SetTrigger("SitToSad");
    }
    override public void InitialiseState()
    {
        scenarioOneEmpathyObject.SetActive(true);

        instructionsRead = false;
        correctChoice = false;
        //interactionComplete = false;
        transitionTimer = 0f;
        assessmentTimer = 0f;
        sittingIdleTimer = 0f;
        errors = 0;
        //isRightHandCloseToShoulder = false;
        //isLeftHandCloseToShoulder = false;


        //if (backGroundMusic is not null) 
        //{
        //    //backGroundMusic.oneshot
        //    //backGroundMusic.Play();
        //    GetComponent<AudioSource>().loop = true;
        //    GetComponent<AudioSource>().clip = backGroundMusic;
        //    GetComponent<AudioSource>().Play();
        //}

        //locomotionObject.SetActive(false);

        if(socialAudio is not null)
        {
            socialAudio.Play();
        }

        //if(hintMenu is not null)
        //{
        //    hintMenu.SetActive(true);
        //}

        empathyChoiceOne.SetActive(true);
        empathyChoiceTwo.SetActive(true);
        empathyChoiceThree.SetActive(true);
        empathyChoiceVerify.SetActive(true);

        currentChoice = -1;

    }
    //private ScenarioStateMachine.STATE UpdateScenarioState()
    //{

    //    return ScenarioStateMachine.STATE.INTRO;
    //}
    override public GameStateMachine.GameStateName UpdateState()
    {
        if (bblWasPressed)
        {

        }
        //make sure user has pressed next on instructions
        if (!isAssessing && instructionsRead)
        {
            isAssessing = true;

            //show answer options
            //put in error answers?
            //have several different canvas with one answer each
            empathyAnswersObject.SetActive(true);
        }

        if (isAssessing)
        {
            assessmentTimer += Time.deltaTime;
            ////then check if player is standing within radius of Laila AND looking at her genral direction FOR a set amount of time (2 seconds to start testing>)
            //if (lailaObject != null && cameraRig != null)
            //{
            //    Vector3 targetPos = lailaObject.transform.position;
            //    Vector3 playerPos = cameraRig.transform.position;
            //    float distToTarget = Vector3.Distance(targetPos, playerPos);

            //    //CHECK CONTROLLER/HAND DIST TO EACH SHOULDER

            //    if (leftShoulderObject != null && rightShoulderObject != null)
            //    {
            //        Vector3 leftShoulderPos = leftShoulderObject.transform.position;
            //        Vector3 rightShoulderPos = rightShoulderObject.transform.position; 

            //        if (rightHandAnchor != null)
            //        {
            //            Vector3 rightHandAnchorPos = rightHandAnchor.transform.position;
            //            float rightHandDistToLeftShoulder = Vector3.Distance(rightHandAnchorPos, leftShoulderPos);
            //            float rightHandDistToRightShoulder = Vector3.Distance(rightHandAnchorPos, rightShoulderPos);

            //            if (rightHandDistToLeftShoulder <= shoulderTouchDistance || rightHandDistToRightShoulder <= shoulderTouchDistance)
            //            {
            //                isRightHandCloseToShoulder = true;
            //            }
            //        }
            //        if (leftHandAnchor != null)
            //        {
            //            Vector3 leftHandAnchorPos = leftHandAnchor.transform.position;
            //            float leftHandDistToLeftShoulder = Vector3.Distance(leftHandAnchorPos, leftShoulderPos);
            //            float leftHandDistToRightShoulder = Vector3.Distance(leftHandAnchorPos, rightShoulderPos);

            //            if (leftHandDistToLeftShoulder <= shoulderTouchDistance || leftHandDistToRightShoulder <= shoulderTouchDistance)
            //            {
            //                isLeftHandCloseToShoulder = true;
            //            }
            //        }

            //        if(isRightHandCloseToShoulder || isLeftHandCloseToShoulder)
            //        {
            //            interactionTimer += Time.deltaTime;
            //            if (interactionTimer > interactionTimeNeeded) //still need to check for facing, and if player looks away we start that again.
            //            {
            //                //should there also be a choice component

            //                //we can give feedback to the player when they have gotten this far (close and looking) and then expect them to do another step

            //                //stop timer
            //                //set bool
            //                //maybe lock player movement? unsure about this
            //                interactionComplete = true;

            //                interactionTimer = 0f;
            //            }
            //        }
            //        else
            //        {
            //            interactionTimer = 0f;
            //        }                    
            //    }              


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
            UpdateHintMenu();



        }
        
            
        

        //Animator lailaAnimator = lailaObject.GetComponent<Animator>();

       


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
                loggedInUser.scenarioOneTime = assessmentTimer;
                recordManager.WriteCSV();
            }
        }

        scenarioOneEmpathyObject.SetActive(false);
        empathyAnswersObject.SetActive(false);

        //if (backGroundMusic is not null)
        //{
        //    //backGroundMusic.oneshot
        //    GetComponent<AudioSource>().Stop();
        //}

        empathyChoiceOne.SetActive(false);
        empathyChoiceTwo.SetActive(false);
        empathyChoiceThree.SetActive(false);
        empathyChoiceVerify.SetActive(false);
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
            return GameStateMachine.GameStateName.SCENARIOONEOUTRO;
        }

        return GameStateMachine.GameStateName.SCENARIOONEEMPATHY;
    } 
    public void HandleBigBlueButton()
    {
        bblWasPressed = true;
    }

    public void NextButtonPressed()
    {
        bblWasPressed = true;

        instructionsRead = true;
        scenarioOneEmpathyObject.SetActive(false);
    }

    public void InstructionsButtonPressed()
    {
        instructionsRead = true;        
    }
    public void CorrectChoice()
    {
        //activeEmotion++;
    }

    public void SetCurrentChoice(int newChoice)
    {
        currentChoice = newChoice;

        //need to play audio as well herez

        switch(currentChoice) 
        {
            case 1:
                empathyChoiceOne.GetComponent<Outline>().enabled = true;
                empathyChoiceTwo.GetComponent<Outline>().enabled = false;
                empathyChoiceThree.GetComponent<Outline>().enabled = false;
                empathyChoiceVerify.GetComponent<Outline>().enabled = false;



                break;
            case 2:
                empathyChoiceOne.GetComponent<Outline>().enabled = false;
                empathyChoiceTwo.GetComponent<Outline>().enabled = true;
                empathyChoiceThree.GetComponent<Outline>().enabled = false;
                empathyChoiceVerify.GetComponent<Outline>().enabled = false;
                break;
            case 3:
                empathyChoiceOne.GetComponent<Outline>().enabled = false;
                empathyChoiceTwo.GetComponent<Outline>().enabled = false;
                empathyChoiceThree.GetComponent<Outline>().enabled = true;
                empathyChoiceVerify.GetComponent<Outline>().enabled = false;
                break;
            default:
                empathyChoiceOne.GetComponent<Outline>().enabled = false;
                empathyChoiceTwo.GetComponent<Outline>().enabled = false;
                empathyChoiceThree.GetComponent<Outline>().enabled = false;
                empathyChoiceVerify.GetComponent<Outline>().enabled = false;
                break;
        }
    }

    public void MakeGuess()
    {

        //1 - Correct
        //2 - Correct
        //3 - Incorrect

        //if(guess == activeEmotion)
        //{
        //    activeEmotion++;
        //}        

        if (currentChoice == 2)
        {
            //this is the most correct answer
            correctChoice = true;


            if (successAudio is not null)
            {
                successAudio.Play();
            }
        }
        if(currentChoice == 3)
        {
            //this is the semi-correct answer
            correctChoice = true;

            if (successAudio is not null)
            {
                successAudio.Play();
            }
        }
        else if(currentChoice == 1)
        {
            ++errors;

            if (recordManager != null)
            {
                User loggedInUser = recordManager.GetLoggedInUser();
                if (loggedInUser != null)
                {
                    loggedInUser.scenarioOneErrors++;
                }
            }

            if (failureAudio is not null)
            {
                failureAudio.Play();
            }
        }
    }


    //hint menu is attached to the wrist, only shows when direction is close to looking at headset forward
   
    public void UpdateHintMenu()
    {
        if(hintMenuObject  != null)
        {

        }
    }
}
