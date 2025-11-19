using Oculus.Interaction.Locomotion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using static System.TimeZoneInfo;

//This will play out the first part of the scenario and then start timer and expect user to pick correct emotion on display.


//Oct 22 changes
//Intro state will allow player to explore the space before they teleport to the chair and trigger the start
//ScenarioIntroObject now has some text and voice over and the button plays the voice over again.

public class ScenarioOneIntroState : GameState
{
    //[Range(0f, 1f)]
    //public float timeBuffer = 0.05f;
    //private float timer = 0.0f;
    //private Vector3 posAtTeleport;

    //ScenarioOneEmotionObject is just the things for this, take out the environment and other things
    //we are using through out the same scenario
    public GameObject ScenarioOneEnvironmentObject;
    public GameObject ScenarioOnePlayers;
    public GameObject ScenarioOneIntroObject;
    public AudioClip backGroundMusic;
    //public AudioListener audioListener;
    public RecordManager recordManager;
    //public AudioClip intructionVoiceOver;
    private bool isVoiceOverPlaying = false;

    //private float transitionTimeDelta = 0.2f;
    public float transitionTimer = 0f;

    //private bool bblWasPressed = false;
    //public int activeEmotion = 0;
    //private int prevActiveEmotion = 0;
    //private int tutorialIndex = 0;
    public bool nextButtonPressed = false;
    private int errors;
    //private int guessIndex = -1;

    private float assessmentTimer;

    public AudioClip voiceInstructions;
    private bool havePlayedVoice = false;

    //public ScenarioStateMachine scenarioStateMachine;

    public GameObject SplashScreenObject;
    private bool shouldStartScenario = false;

    public float voiceStartDelay = 1.0f;
    private float voiceDelayTimer = 0.0f;

    public AudioClip successAudio;

    public GameObject coinEffectObject;

    public GameObject chairObject;
    public Transform chairTeleportTransform;

    public GameObject teleportUser;
        
    //How to handle difficulty levels?
    //starting with two
    //front end state will choose route
    //no locomotion in easy (fixed positions and we teleport or move the player)
    //so similar states but update will differ.
    //clean this state up and then it'll make it easier to figure out which direction\
    //just split into two states for now, make this one the easy state

    public Transform xrOrigin;
    //public class TeleportUser : MonoBehaviour
    //{
    //    public Transform cameraRig;

    //    public void TeleportTo(Vector3 targetPos)
    //    {
    //        Vector3 cameraOffset = cameraRig.GetComponentInChildren<Camera>().transform.localPosition;
    //        Vector3 offset = new Vector3(cameraOffset.x, 0, cameraOffset.z);
    //        cameraRig.transform.position = targetPos - offset;
    //    }
    //}
    //private TeleportUser teleportUser;

    public GameObject playerMovement;
    public FirstPersonLocomotor locomotor;
    public OVRPlayerController playerController;

    void Start()
    {
        stateName = GameStateMachine.GameStateName.SCENARIOONEINTRO;

        teleportUser.GetComponent<TeleportUser>().cameraRig = xrOrigin;
    }
    override public void InitialiseState()
    {
        ScenarioOneEnvironmentObject.SetActive(true);
        ScenarioOnePlayers.SetActive(true);
        //ScenarioOneIntroObject.SetActive(true);
        //SplashScreenObject.SetActive(true);

        nextButtonPressed = false;
        transitionTimer = 0f;
        assessmentTimer = 0f;
        errors = 0;

        if (backGroundMusic is not null) 
        {          
            GetComponent<AudioSource>().loop = true;
            GetComponent<AudioSource>().clip = backGroundMusic;
            GetComponent<AudioSource>().Play();
        }


        voiceDelayTimer = 0f;
        shouldStartScenario = false;
        havePlayedVoice = false;
        isVoiceOverPlaying = false;


        //init loggedInUser? for coins and for debug
        //Vector3 teleToPos = chairTeleportTransform.position;
        //teleportUser.GetComponent<TeleportUser>().TeleportTo(chairObject.transform.position, chairTeleportTransform.localRotation);

        //teleportUser.GetComponent<TeleportUser>().TeleportTo(chairTeleportTransform.position, chairTeleportTransform.localRotation);
        if (locomotor != null)
        {
           //locomotor.DisableMovement();
           //locomotor.
        }
        if(playerController != null)
        {

        }
    }
    
    override public GameStateMachine.GameStateName UpdateState()
    {
        if(voiceDelayTimer > voiceStartDelay)
        {
            shouldStartScenario = true;
        }
        else
        {
            voiceDelayTimer += Time.deltaTime;
        }

        if (shouldStartScenario && !havePlayedVoice)
        {
            SplashScreenObject.SetActive(false);
            ScenarioOneIntroObject.SetActive(true);
            if (voiceInstructions is not null)
            {
                GetComponent<AudioSource>().PlayOneShot(voiceInstructions);
                havePlayedVoice = true;
            }

            //teleportUser.TeleportTo(chairObject.transform.position);

            //playerMovement.GetComponent<RigMoverWithPreRotation>().StartFullTransition();
        }

        assessmentTimer += Time.deltaTime;
        return UpdateTutorialOne();
    }
    override public void ShutDownState()
    {
        //Vector3 teleToPos = chairTeleportTransform.position;
        //teleportUser.GetComponent<TeleportUser>().TeleportTo(chairObject.transform.position, chairObject.transform.rotation);

        

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

        ScenarioOneIntroObject.SetActive(false);

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
            //teleportUser.GetComponent<TeleportUser>().TeleportTo(chairTeleportTransform.position, chairTeleportTransform.localRotation);
            
            //if(locomotor != null)
            //{
            //    locomotor.DisableMovement();
            //}

            if (successAudio is not null)
            {
                //backGroundMusic.oneshot
                //backGroundMusic.Play();
                //GetComponent<AudioSource>().clip = successAudio;
                GetComponent<AudioSource>().PlayOneShot(successAudio);

                //always start with giving the user a coin(or however many we decide to give)
                //how do we know where to spawn the coin though?
                //does the chair object just do it?
                //also need to add to the coin count
                //what if chair teleport calls the function to instantiate and set off coin effects
                //THEN when the coin reaches its target it adds to the UI element and record manager?
                if(coinEffectObject is not null)
                {

                }
            }
            nextButtonPressed = false;
            return GameStateMachine.GameStateName.SCENARIOONEEMOTION;
        }

        return GameStateMachine.GameStateName.SCENARIOONEINTRO;
    } 
    //public void HandleBigBlueButton()
    //{
    //    bblWasPressed = true;
    //}

    public void NextButtonPressed()
    {
        //bblWasPressed = true;
        nextButtonPressed = true;
    }
    public void CorrectChoice()
    {
        //activeEmotion++;
    }

    public void NextPressed()
    {
        nextButtonPressed = true;
    }

    public void SetShouldStartScenario()
    {
        shouldStartScenario = true;

       
    }

    public void PlayVoiceOver()
    {
        havePlayedVoice = false;
    }

    //make a ultities object? or just put in base class
   
}
