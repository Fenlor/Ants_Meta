using NUnit.Framework.Constraints;
using UnityEngine;
using static GameStateMachine;

public abstract class GameState : MonoBehaviour
{

    //How do we tell it which state to transisition into?
    //can we instantiate each state with possible transition states and then return that? how to use that nicely though?
    //enum for states then use that to check if we need to transition?
            
    //GameState should be inherited by each game state
    //GameState should have an init, update, end functions
    //Something will have a list of GameStates updating the active one, calling init and end funcs when needed
    //so each state will have its own state functions
    //whichever class holds the states, how do they init and where is the clever bit where we just call Update(State)etc...

    //OK, NEWEST HERE
    //we have an enum in GameStateMachine.GameStateName whos value matches 1-1 to the GameStateArray
    //we need to accept an array of GameStateName enums as possible transistions which the Transition functions can return
    private int stateCount { get; set; } = 5; //get stateCount from machine? yes for sure it should be resposible for that
    public GameStateName[] transistionStates;
    public GameStateName stateName { get; set; }
    public bool hasTeleported { get; set; } = false;


    private void Start()
    {
        transistionStates = new GameStateName[stateCount];
    }
    public abstract void InitialiseState();

    //update needs to return its own state name or whichever it wants to transition into
    public abstract GameStateName UpdateState();

    //this needs to contain update and then
    //check if transition needed
    //one way state transition for now?
    //that's boring but it kinda makes sense for this game
    //intro, game set up... wait a min we need to reverse
    //hmm, we need a better way to traverse the states/89+

    //We NEED transitionsss, either camera animations or fades or wipes
    //WAIT A MIN, NOT IN VR I DONT THINK, RIGHT? unless its a replay maybe but will be mostly in 1st person
    //actually replays from a cameras point of view would be sweet
    //could record the last little bit of gameplay from set cameras in scene, or check out how it is commonly done in other games or examples

    public abstract void ShutDownState();

    public abstract void TeleOn();

}
