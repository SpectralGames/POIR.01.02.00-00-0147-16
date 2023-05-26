using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FSMSystem
{
    private List<FSMState> states;

    private StateID currentStateID, lastStateID;
    public StateID CurrentStateID { get { return currentStateID; } }
	public StateID LastStateID { get { return lastStateID; } }
    private FSMState currentState;
    public FSMState CurrentState { get { return currentState; } }

    public FSMSystem ()
    {
        states = new List<FSMState>();
    }


    public void AddState (FSMState s)
    {

        if (s == null)
        {
            Debug.LogError("FSM ERROR: Null reference is not allowed");
        }
        
        if (states.Count == 0)
        {
            states.Add(s);
            currentState = s;
			lastStateID = currentStateID = s.StateID;
            return;
        }


        foreach (FSMState state in states)
        {
            if (state.StateID == s.StateID)
            {
                Debug.LogError("FSM ERROR: Impossible to add state " + s.StateID.ToString() +
                               " because state has already been added");
                return;
            }
        }
        states.Add(s);
    }


    public void DeleteState (StateID id)
    {
        
        if (id == StateID.Null)
        {
            Debug.LogError("FSM ERROR: NullStateID is not allowed for a real state");
            return;
        }

        
        foreach (FSMState state in states)
        {
            if (state.StateID == id)
            {
                states.Remove(state);
                return;
            }
        }
        Debug.LogError("FSM ERROR: Impossible to delete state " + id.ToString() +
                       ". It was not on the list of states");
    }


	public void PerformTransition (StateID id, FSMStateArgs additionalLeavingArgs = null, FSMStateArgs additionalEnteringArgs = null)  //Transition trans)
    {
        /*if (trans == Transition.Null)
        {
            Debug.LogError("FSM ERROR: NullTransition is not allowed for a real transition");
            return;
        }*/
		
		if(id == currentStateID)
		{
			return;
		}
        
        //StateID id = currentState.GetOutputState(trans);
		if(currentState.HasTransitionStateID(id) == false)
		{
			Debug.LogWarning("FSM ERROR: State " + currentStateID.ToString() + " does not have a transition to state " + id.ToString());
			return;
		}
        /*if (id == StateID.Null)
        {
			Debug.LogError("FSM ERROR: State " + currentStateID.ToString() + " does not have a transition to state " + id.ToString());
            Debug.LogError("FSM ERROR: State " + currentStateID.ToString() + " does not have a target state " +
                           " for transition " + trans.ToString());
            return;
        }*/
        
        //currentStateID = id;
	
        foreach (FSMState state in states)
        {
            if (state.StateID == id)
            {
                var currentStateName = currentState.GetType().Name;
				currentState.DoBeforeLeaving(additionalLeavingArgs);

				lastStateID = currentStateID;
                currentState = state;
				currentStateID = id;
                var nextStateName = currentState.GetType().Name;

                Debug.LogFormat("Form state {0} to state {1}", currentStateName, nextStateName);

                currentState.DoBeforeEntering(additionalEnteringArgs);
                break;
            }
        }
    } 

}