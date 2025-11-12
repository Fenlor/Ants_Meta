using System;
using TMPro;
using UnityEngine;

public class LoginState : GameState
{
    public GameObject canvas;
    public GameObject locomotionObject;
    [Range(0f, 1f)]
    public float timeBuffer = 0.1f;
    private float timer = 0.0f;
    private Vector3 posAtTeleport;
    private bool isUserLoggedIn = false;
    public RecordManager recordManager;
    public GameObject idInputField;
    public GameObject textMeshProObject;
    
    
    void Start()
    {
        stateName = GameStateMachine.GameStateName.LOGIN;

    }
    override public void InitialiseState()
    {
        //disable locomotionObject, seems to work ok
        //this was too quick and stops locomotion before we can teleport
        //it is triggered when the teleport event is queued
        //locomotionObject.SetActive(false);
        if (canvas != null)
        {
            canvas.SetActive(true);
        }        
    }
    override public GameStateMachine.GameStateName UpdateState()
    {
        //if (locomotionObject.activeSelf)
        //{
        //    timer += Time.deltaTime;
        //    if (timer >= timeBuffer)
        //    {
        //        if (locomotionObject.activeSelf)
        //        {
        //            locomotionObject.SetActive(false);
        //        }
        //    }
        //}

        //NEW USER BUTTON which provides the user with a number that can later be used to look up data and to log back in
        //how are we holding the player data? need to dump it to csv but also we have the ability to log back in.
        //can we write over the old data? or can we just create a new row with the same id and the data ranglers can handle that after?
        //either of these make it a lot easier, I'm assuming we create another row for now.
        //How do we get the next id though? or can we randomly generic a cute name or something instead of a number. how about Colour/Animal but
        //how to know we are not reusing. Let's read in the csv, it's not hard and this app is all about data. Can always suggest a cutename generator 
        //but only if we have time later

        //RETURNING USER TEXT INPUT FIELD with enter/confirm button
        //User to enter ID number, can we just show a number pad so they can't eneter anything else. 
        //still need to check if it's a number though
        //then check if it exists in loaded csv dataset
        //if not return message saying so.
        //Debug.Log("LoginState - isUserLoggedIn: " + isUserLoggedIn);
        if (isUserLoggedIn)
        {
            //set TMP element to Id value
            textMeshProObject.GetComponent<TMPro.TextMeshProUGUI>().text = "Your Id number is: " + recordManager.loggedInUserId.ToString();
            //
            //Debug.Log("LoginState - returning GameStateName.GAMEPLAY");
            return GameStateMachine.GameStateName.TUTORIALONE;
        }
        else
        {
            return GameStateMachine.GameStateName.LOGIN;
        }            
    }
    override public void ShutDownState()
    {
        //tenable locomationObject

        //locomotionObject.SetActive(true);

        if(canvas != null)
        {
            canvas.SetActive(false);
        }        
    }
    override public void TeleOn()
    {
        //hasTeleportedIn = true;
    }

    //For Transition part of Update, use position as a backup, if xrorigin has moved away from teleport pos then back into intro state
    //should keep position at teleport then

    public void UserLoggedIn()
    {
        isUserLoggedIn = true;
    }

    //AttmeptLogin should be in recordmanager maybe?
    public void AttemptLogin()
    {
        int userId = int.Parse(idInputField.GetComponent<TMP_InputField>().text);
        //Debug.Log("userId: " + userId);
        if (recordManager.AttemptLogin(userId))
        {
            //Debug.Log("LoginState, Login successful, userId: " + userId);
            isUserLoggedIn = true;
            textMeshProObject.GetComponent<TMPro.TextMeshProUGUI>().text = "Your Id number is: " + recordManager.loggedInUserId.ToString();
        }
        else
        {
            //Debug.Log("LoginState, Login unsuccessful, userId: " + userId);
        }
    }
}
