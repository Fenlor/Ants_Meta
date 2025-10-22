using UnityEngine;

public class IntroState : GameState
{
    public GameObject introCanvas;
    public GameObject locomotionObject;
    [Range(0f, 1f)]
    public float timeBuffer = 0.1f;
    private float timer = 0.0f;
    private Vector3 posAtTeleport;
    private bool doChangeState = false;   

    void Start()
    {
        stateName = GameStateMachine.GameStateName.INTRO;
    }
    override public void InitialiseState()
    {
        //disable locomotionObject, seems to work ok
        //this was too quick and stops locomotion before we can teleport
        //it is triggered when the teleport event is queued
        //locomotionObject.SetActive(false);

        //locomotionObject.SetActive(false);
        if (introCanvas != null)
        {
            introCanvas.SetActive(true);
        }
    }
    override public GameStateMachine.GameStateName UpdateState()
    {

        //Show tite name + some descriptive text
        //UI with next/ok button
      

        if (doChangeState)
        {
            return GameStateMachine.GameStateName.LOGIN;
        }

        return GameStateMachine.GameStateName.LOGIN;
    }
    override public void ShutDownState()
    {
        //tenable locomationObject
        //locomotionObject.SetActive(false);

        //just destroy intro canvas, we never come back to this state
        if(introCanvas != null)
        {
            Destroy(introCanvas);
        }        
    }
    override public void TeleOn()
    {
        //hasTeleportedIn = true;
    }

    //For Transition part of Update, use position as a backup, if xrorigin has moved away from teleport pos then back into intro state
    //should keep position at teleport then

    public void NextButtonPressed()
    {
        doChangeState = true;
    }
}
