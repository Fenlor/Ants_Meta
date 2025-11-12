using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
//using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;

//not sure how to do this
//looking for a generic way to call init, update, and end calls in GameState objects?
//what about transition, we just call a transition func that returns a bool?
//hmm do we need an enum for this or just the GameState objects themselves?
public class GameStateMachine : MonoBehaviour
{
    public int stateCount { get; set; } = 12;
    public GameObject[] gameStateObjects;
    private GameState[] gameStates;    
    public GameState currentGameState { get; set; }
    private GameStateName newStateName;

    //debug
    public GameObject stateTMPObject;

    public GameStateName startingState = GameStateName.INTRO;
    //so, each state has init, update, and shutdown funcs as well as transitions from each
    //so, maybe a transition could return a state instead of a bool?
    //we can run update and transition functions for whichever is the current gameState;
    //safe to assume to start at index 0?

    //an enum that matches index to the array?
    //use the enum value when addressing array
    //use the enum name when using API
    //These names need to make sense for the classes of course, is there a better way to init this?
    //keep it here for now
    public enum GameStateName//CUSTOM THIS, not the nicest but it works. ue config or something? could use as public editor?
    {
        INTRO = 0,
        LOGIN = 1,
        AVATAR = 2,
        FRONTEND = 3,
        TUTORIALONE = 4,
        TUTORIALTWO = 5,
        TUTORIALTHREE = 6,
        SCENARIOONEINTRO = 7,
        SCENARIOONEEMOTION = 8,
        SCENARIOONESOCIAL = 9,
        SCENARIOONEEMPATHY = 10,
        SCENARIOONEOUTRO = 11,
    }

    void Start()
    {
        gameStates = new GameState[stateCount];
        for(int stateIndex = 0; stateIndex < stateCount; ++stateIndex)
        {
            gameStates[stateIndex] = gameStateObjects[stateIndex].GetComponent<GameState>();
            //Assert.IsNotNull(gameStates[stateIndex]);
        }
        currentGameState = gameStates[(int)startingState];
        currentGameState.InitialiseState();        
    }

    // Update is called once per frame
    //Should we assume we want to write to the users record .csv every state swap? makes sense I think
    void Update()
    {
        //Debug.Log("GameStateMachine - Update, current state: " + currentGameState);
        newStateName = currentGameState.UpdateState();
        if(newStateName != currentGameState.stateName)
        {
            currentGameState.ShutDownState(); //can this include transitions in the current state? should expand so we can do some little animations and fades, etc...
            //use name of new gamestate to use as index to change gameState 
            int newIndex = (int)newStateName;
            //Debug.Log("GameStateMachine - changing state, current state: " + currentGameState.stateName + ", newState, newIndex: " + newStateName + ", " + newIndex);
            currentGameState = gameStates[newIndex];
            currentGameState.InitialiseState();           
        }

        if (stateTMPObject != null)
        {
            stateTMPObject.GetComponent<TMPro.TextMeshProUGUI>().text = currentGameState.stateName.ToString();
        }
    }
}
