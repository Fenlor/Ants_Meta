using JetBrains.Annotations;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
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

    private bool isInChair = false;

    //for each cue or action found it unlocks the corresponding dialogue option or action interaction
    //for easy mode user can also pick the action instead of doing it
    //normal mode user has to perform action, either interacting with chair or placing hand on shoulder

    public GameObject hiddenCueOne;
    public GameObject hiddenCueTwo;
    public GameObject hiddenCueThree;
    public GameObject hiddenCueFour;
    public GameObject hiddenCueActionOne;
    public GameObject hiddenCueActionTwo;

    public GameObject cueOneCanvas;
    public GameObject cueTwoCanvas;
    public GameObject cueThreeCanvas;
    public GameObject cueFourCanvas;
    public GameObject cueVerifyCanvas;

    public AudioSource dialogueOneVoiceOver;
    public AudioSource dialogueTwoVoiceOver;
    public AudioSource dialogueThreeVoiceOver;
    public AudioSource dialogueFourVoiceOver;
    public AudioSource actionOneVoiceOver;
    public AudioSource actionTwoVoiceOver;

    //for easy mode we show the following action prompts so user doesn't have to 
    //do we then teleport the user to the chair or move into comfort position? or just let them move where they want to?
    //USE THE ACTION ICON THAT USER FOUND. Just set them active again (or leave active?)

    private bool hasCollectedCueOne = false;
    private bool hasCollectedCueTwo = false;
    private bool hasCollectedCueThree = false;
    private bool hasCollectedCueFour = false;
    private bool hasCollectedActionOne = false;
    private bool hasCollectedActionTwo = false;
    private int currentDialogueChoice = -1;
    public int dialogueChoicesFound;

    public GameObject coinEffectPrefab;

    private enum SpinState
    {
        Accelerating,
        Holding,
        Decelerating,
        Effects,
        Moving
    }
    private SpinState state = SpinState.Accelerating;

    public bool isSpinning = true;
    public float maxRotSpeed = 180f;
    public float acceleration = 90f;
    public float holdTime = 1;
    public float spinTimer = 0;

    private float currentSpeed = 0f;
    //private float timer = 0f;

    //public Button actionOneButton;
    private int lockedInAction = 0;


    public bool isActionOneSelected = false;
    public bool isActionTwoSelected = false;
    public bool isActionLockedIn = false;

    public Color selectedButtonColour = Color.white;
    public Color defaultButtonColour = Color.blue;

    public GameObject heroScreenPanelOne;
    public GameObject heroScreenPanelTwo;

    //shall we intruduce a state object?
    //we have DIALOGUELOST, DIALOGUEFOUND, ACTIONCHOSEN, DIALOGUECHOSEN
    //start of DIALOGUELOSTG, when all 4 are found return DIALOGUEFOUND

    public enum SocialState
    {
        DialogueLost,
        DialogueFound,
        ActionChosen,
        DialogueChosen
    }
    public SocialState socialState;

    void Start()
    {
        stateName = GameStateMachine.GameStateName.SCENARIOONESOCIAL;

        //delay the anim trigger
        //lailaObject.GetComponent<Animator>().SetTrigger("SitToSad");
        //UnityEngine.UI.Button button = hiddenCueActionOne.GetComponentInChildren<UnityEngine.UI.Button>();
        //SelectButton(button);

        //heroScreenPanelOne.GetComponent<UnityEngine.UI.Image>().color = selectedButtonColour;
        //heroScreenPanelTwo.GetComponent<UnityEngine.UI.Image>().color = defaultButtonColour;
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
        isInChair = false;

        if (voiceInstructions is not null)
        {
            GetComponent<AudioSource>().PlayOneShot(voiceInstructions);
        }
      
        //locomotionObject.SetActive(false);

        scenarioOneSocialObject.SetActive(true);

        hiddenCueOne.SetActive(true);
        hiddenCueTwo.SetActive(true);
        hiddenCueThree.SetActive(true);
        hiddenCueFour.SetActive(true);
        //hiddenCueActionOne.SetActive(true);
        //hiddenCueActionTwo.SetActive(true);

        isActionOneSelected = false;
        isActionTwoSelected = false;

        //hiddenCueActionOne.GetComponentInChildren<Canvas>().GetComponent<Outline>().enabled = true;
        //hiddenCueActionTwo.GetComponentInChildren<Canvas>().GetComponent<Outline>().enabled = true;

        hasCollectedCueOne = false;
        hasCollectedCueTwo = false;
        hasCollectedCueThree = false;
        hasCollectedCueFour = false;
        hasCollectedActionOne = false;
        hasCollectedActionTwo = false;
        currentDialogueChoice = -1;
        dialogueChoicesFound = 0;
        isActionLockedIn = false;

        socialState = SocialState.DialogueLost;
    }
    //private ScenarioStateMachine.STATE UpdateScenarioState()
    //{

    //    return ScenarioStateMachine.STATE.INTRO;
    //}
    override public GameStateMachine.GameStateName UpdateState()
    {
        //when should we start the timer?

        switch (socialState)
        {
            case(SocialState.DialogueLost):
                //starting state, user must find at least 3 of the 4 dialogue options
                socialState = UpdateDialgoueLost();
                break;
            case(SocialState.DialogueFound):
                //user can still add the 4th dialogue option
                //user can choose action which teleports them into place for dialogue options
                socialState = UpdateDialogueFound();
                break;
            case(SocialState.ActionChosen):
                //User is in one of two positions (sitting or standing next to
                //choose from found dialogue options to continue
                socialState = UpdateActionChosen();
                break;
            case(SocialState.DialogueChosen):
                socialState = UpdateDialogueChosen();
                break;
            default:
                break; 
        }

        //RotateAroundY(hiddenCueActionOne);
        //RotateAroundY(hiddenCueActionTwo);

        //make sure user has pressed next on instructions
        if (!isAssessing && instructionsRead && !interactionComplete)
        {
            isAssessing = true;
        }

        if (isAssessing)
        {
            assessmentTimer += Time.deltaTime;

            //check if player has sat back down in chair OR patting Laila on the shoudler
            if (isInChair) 
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

                    PlaySuccessAudio();

                    lailaObject.GetComponent<Animator>().SetTrigger("Neutral");
                }
            }
            else 
            {
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

                        if (isRightHandCloseToShoulder || isLeftHandCloseToShoulder)
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

                                PlaySuccessAudio();

                                lailaObject.GetComponent<Animator>().SetTrigger("Neutral");

                            }
                        }
                        else
                        {
                            interactionTimer = 0f;
                        }
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


        //change colour of selected button
        //selectedButtonColour
        //defaultButtonColour
        //UpdateButtons
        //only change if they are dirty though.

        return UpdateSocialIntroOne();
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

        scenarioOneSocialObject.SetActive(false);

        //if (backGroundMusic is not null)
        //{
        //    //backGroundMusic.oneshot
        //    GetComponent<AudioSource>().Stop();
        //}

        hiddenCueOne.SetActive(false);
        hiddenCueTwo.SetActive(false);
        hiddenCueThree.SetActive(false);
        hiddenCueFour.SetActive(false);
        hiddenCueActionOne.SetActive(false);
        hiddenCueActionTwo.SetActive(false);

        cueOneCanvas.SetActive(false);
        cueTwoCanvas.SetActive(false);
        cueThreeCanvas.SetActive(false);
        cueFourCanvas.SetActive(false);
        cueVerifyCanvas.SetActive(false);
    }
    override public void TeleOn()
    {
        //hasTeleportedIn = true;
    }
   
    private GameStateMachine.GameStateName UpdateSocialIntroOne()
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

    //BIG BLUE BUTTON TO RETURN!!!?

    //public void HandleBigBlueButton()
    //{
    //    bblWasPressed = true;
    //}

    //public void NextButtonPressed()
    //{
    //    bblWasPressed = true;
    //}

    SocialState UpdateDialgoueLost()
    {
        if (dialogueChoicesFound >= 3)
        {
            //go to next state but don't hide the last dialogue option 
            
            hiddenCueActionOne.SetActive(true);
            hiddenCueActionTwo.SetActive(true);

            return SocialState.DialogueFound;
        }        
        else
        {
            //turn on interactive dialogue objects the player has to find
            //does user have to find all of them?
            //what if after 3 of 4 we tell the user (with audio voice over) that
            //they can proceed to the next bit or finish looking for the last one
            //then have an audio voice over finding all the dialogue option
            //easy are very easy to find as they need to be visiable from the camera locations we lock them in
            //do we allow telport anchor movement for easy? could also have them for hard
            


            return SocialState.DialogueLost;
        }
    }
    SocialState UpdateDialogueFound()
    {
        //start timer now?
        //or several timers. e.g. 1 minute to gatehr dialgoue options. make easy not so hidden
        //definetley put in difficulty levels. cant even expect any of these users to have
        //used VR before, let alone not get nauses when moving around and other issues

        if (!isActionLockedIn)
        {
            return SocialState.ActionChosen;
        }
        else
        {
            //turn on Action icons for player to choose
            //wait for player choice
            //wait for last dialgoue option to be found (optinal)
            if (dialogueChoicesFound >= 4)
            {
                //play audio for "That's great, you have found all dialogue options!"

            }

            //how do we lock in an action
            //don't want user to have too many clicks
            //click and hold? that seems like too much all of a sudden if first used here
            //how about a button or if user clicks it a second time in a row?
            //how about first time it is selected since that already takes two clicks
            //make sure audio plays and locks the second click
            //how to add a universal lock to all interactions while voice over or some other
            //timer is running


            if (isActionOneSelected || isActionTwoSelected)
            {
                
                isActionLockedIn = true;
                
            }

            return SocialState.DialogueFound;
        }

    }
    SocialState UpdateActionChosen()
    {
        //Teleport player to location and stop tracking position
        //Dialogue options

        
        if (!true)
        {

            return SocialState.DialogueChosen;
        }
        else
        {
            //turn on Dialogue options
            //wait for player choice
            cueOneCanvas.SetActive(hasCollectedCueOne);
            cueTwoCanvas.SetActive(hasCollectedCueTwo);
            cueThreeCanvas.SetActive(hasCollectedCueThree);

            cueFourCanvas.SetActive(hasCollectedCueFour);
            cueVerifyCanvas.SetActive(true);

            return SocialState.ActionChosen;
        }
    }
    SocialState UpdateDialogueChosen() 
    {
        //Do we let user get here on incorrect dialogue?


        if (interactionComplete)
        {

            return SocialState.DialogueChosen;
        }
        else
        {

            return SocialState.DialogueChosen;
        }
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

    public void MakeGuess()
    {
        //0 - Happy
        //1 - Sad
        //2 - Anger
        //3 - Fear

        //if(guess == activeEmotion)
        //{
        //    activeEmotion++;
        //}      

        //so if an action button is selected then use that and dialogue together
        //easy mode: move/teleport player to position
        //normal: player have to interact with chair or move close/pat either shoulder with either hand.

        //what if we did an action and that locks us into that position on any difficulty level, then 
        //we choose the dialogue?
        //hmm, maybe not. wait for both to be selected so less buttons and then we can teleport player into position.

        if (lockedInAction > 0)
        {
            //what do we do now? teleport/move player into position, play out dialogue and watch Laila either get happy or stay sad.

            switch (currentDialogueChoice)
            {
                case 1:
                    PlaySuccessAudio();
                    lailaObject.GetComponent<Animator>().SetTrigger("Neutral");
                    interactionComplete = true;
                    break;
                case 2:
                    PlaySuccessAudio();
                    lailaObject.GetComponent<Animator>().SetTrigger("Neutral");
                    interactionComplete = true;
                    break;
                case 3:
                    //PlaySuccessAudio();
                    //lailaObject.GetComponent<Animator>().SetTrigger("Neutral");
                    //interactionComplete = true;

                    if (failureAudio is not null)
                    {

                        GetComponent<AudioSource>().clip = failureAudio;
                        GetComponent<AudioSource>().Play();
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

                    break;
                case 4:
                    if (failureAudio is not null)
                    {

                        GetComponent<AudioSource>().clip = failureAudio;
                        GetComponent<AudioSource>().Play();
                    }

                    //PlayFailureAudio
                    //this is the "don't cry, it's not that hard" option
                    ++errors;

                    if (recordManager != null)
                    {
                        User loggedInUser = recordManager.GetLoggedInUser();
                        if (loggedInUser != null)
                        {
                            loggedInUser.scenarioOneErrors++;
                        }
                    }
                    break;
                default:
                    
                    break;
            }
        }
    }   

    public void PlaySuccessAudio()
    {
        if (successAudio is not null)        {
           
            GetComponent<AudioSource>().clip = successAudio;
            GetComponent<AudioSource>().Play();

            lailaObject.GetComponent<Animator>().SetTrigger("Neutral");
        }
    }

    public void SitInChair()
    {
        isInChair = true;
    }

  
    public void DialogueCueFound(int cueNumber)
    {
        scenarioOneSocialObject.SetActive(false);
        //if cue found, deactivate object and give coin(s)
        switch (cueNumber) 
        {
            case 1:
                if (!hasCollectedCueOne)
                {
                    ++dialogueChoicesFound;
                    hasCollectedCueOne = true;
                    hiddenCueOne.SetActive(false);
                    //cueOneCanvas.SetActive(true);
                    //cueVerifyCanvas.SetActive(true);
                    dialogueOneVoiceOver.Play();
                    SpawnCoin(1, hiddenCueOne.transform.position);                    
                }               
                break;
            case 2:
                if (!hasCollectedCueTwo)
                {
                    ++dialogueChoicesFound;
                    hasCollectedCueTwo = true;
                    hiddenCueTwo.SetActive(false);
                    //cueTwoCanvas.SetActive(true);
                    //cueVerifyCanvas.SetActive(true);
                    dialogueTwoVoiceOver.Play();
                    SpawnCoin(1, hiddenCueTwo.transform.position);                    
                }                    
                break;
            case 3:
                if (!hasCollectedCueThree)
                {
                    ++dialogueChoicesFound;
                    hiddenCueThree.SetActive(false);
                    hasCollectedCueThree = true;
                    //cueThreeCanvas.SetActive(true);
                    //cueVerifyCanvas.SetActive(true);
                    dialogueThreeVoiceOver.Play();
                    SpawnCoin(1, hiddenCueThree.transform.position);                   
                }               
                break;
            case 4:
                if (!hasCollectedCueFour)
                {
                    ++dialogueChoicesFound;
                    hiddenCueFour.SetActive(false);
                    hasCollectedCueFour = true;
                    //cueFourCanvas.SetActive(true);
                    //cueVerifyCanvas.SetActive(true);
                    dialogueFourVoiceOver.Play();
                    SpawnCoin(1, hiddenCueFour.transform.position);
                }                
                break;
            default:
                break;
        }
    }
    public void ActionFound(int actionNumber)
    {
        scenarioOneSocialObject.SetActive(false);
        //if action found, deactivate object and give coin(s)
        switch (actionNumber)
        {
            case 1:
                if (!hasCollectedActionOne)
                {
                    
                    hasCollectedActionOne = true;
                    //first time, give coin and unlock ability to use
                    //Play Voice over
                    //do we then select this action
                    //hiddenCueActionOne.SetActive(false);                    
                    SpawnCoin(1, hiddenCueActionOne.transform.position);
                    //have it spin in place                    
                }
                else
                {
                }

                actionOneVoiceOver.Play();

                isActionOneSelected = true;
                isActionTwoSelected = false;                             

                break;
            case 2:
                if (!hasCollectedActionTwo)
                {
                    hasCollectedActionTwo = true;
                    //hiddenCueActionTwo.SetActive(false);                    
                    SpawnCoin(1, hiddenCueActionTwo.transform.position);
                }
                else
                {
                }

                actionTwoVoiceOver.Play();

                isActionOneSelected = false;
                isActionTwoSelected = true;
                
                break;
            default:
                break;
        }

        hiddenCueActionOne.GetComponentInChildren<Canvas>().GetComponent<Outline>().enabled = isActionOneSelected;
        hiddenCueActionTwo.GetComponentInChildren<Canvas>().GetComponent<Outline>().enabled = isActionTwoSelected;
    }

    public void LockInSelectedAction()
    {
        if (isActionOneSelected || isActionTwoSelected)
        {
            lockedInAction = isActionOneSelected ? 1 : 2;            
        }
    }

    public void SetCurrentDialogueOption(int currentChoice)
    {
        //play choice voice over
        //set to currentChoice and allow verify button 

        switch (currentChoice)
        {
            case 1:
                dialogueOneVoiceOver.Play();
                currentDialogueChoice = 1;
                break;
            case 2:
                dialogueTwoVoiceOver.Play();
                currentDialogueChoice = 2;
                break;
            case 3:
                dialogueThreeVoiceOver.Play();
                currentDialogueChoice = 3;
                break;
            case 4:
                dialogueThreeVoiceOver.Play();
                currentDialogueChoice = 4;
                break;
            default:
                break;
        }
    }

    //pass in position to spawn from
    public void SpawnCoin(int value, Vector3 spawnPosition)
    {
        if (coinEffectPrefab != null)
        {
            GameObject coin = Instantiate(coinEffectPrefab, spawnPosition, Quaternion.identity);
            coin.SetActive(true);
            coin.GetComponent<Coin>().coinValue = value;
        }

        //if (teleportationProvider != null)
        //{
        //    TeleportRequest request = new TeleportRequest
        //    {
        //        destinationPosition = transform.position,
        //        destinationRotation = transform.rotation,
        //        matchOrientation = MatchOrientation.TargetUpAndForward,
        //    };

        //    teleportationProvider.QueueTeleportRequest(request);

        //}
    }

    //this next one should be on the object itself and not in this class?
    public void RotateAroundY(GameObject gameObject)
    {
        if (isSpinning)
        {
            switch (state)
            {
                case SpinState.Accelerating:
                    currentSpeed += acceleration * Time.deltaTime;
                    if (currentSpeed >= maxRotSpeed)
                    {
                        currentSpeed = maxRotSpeed;
                        state = SpinState.Holding;
                    }

                    //StartCoroutine(MoveToHand());

                    break;
                case SpinState.Holding:
                    spinTimer += Time.deltaTime;
                    if (spinTimer >= holdTime)
                    {
                        state = SpinState.Decelerating;
                    }
                    break;
                case SpinState.Decelerating:
                    currentSpeed -= acceleration * Time.deltaTime;
                    if (currentSpeed <= 0f)
                    {
                        currentSpeed = 0f;
                        state = SpinState.Effects;
                        spinTimer = 0f;
                    }
                    break;
                case SpinState.Effects:
                    //if (sparkleEffect != null)
                    //{
                    //    sparkleEffect.Play();
                    //}
                    //if (spinSound != null)
                    //{
                    //    spinSound.Play();
                    //}

                    state = SpinState.Moving;
                    break;
                case SpinState.Moving:
                    //StartCoroutine(MoveToHand());
                    break;
                default:
                    break;
            }
            gameObject.transform.Rotate(Vector3.up, currentSpeed * Time.deltaTime);
        }
    }
    public void StartSpining()
    {
        isSpinning = true;
    }
    public void StopSpinning()
    {
        isSpinning = false;

    }
}
