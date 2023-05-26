using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// powod przejscia w jakis stan
/// </summary>
public enum Transition // bodziec
{
    Null = 0,
    VirginIsClose,
    Any,
    Stop,
    Hit,
    SpellThrew, // dodac jakas szanse 50% na wykonanie tego stanu
    Random,
    SawPlayer,
    LostPlayer, // gracz nie jest w polu widzenia
    DeadlyHit,
    TakeDamage,
    Exhausted,
    Rage,
	SawTowerBase,
	TowerBaseDie
}


/// <summary>
/// id danego stanu
/// </summary>
public enum StateID   // dzialania na bodziec
{
    Null = 0,
    CarryVirign,
    Movement,
    TakingDamage,
    AvoidingSpell,
    Idle,
    Random,
    Stumble,
    AttackingPlayer,
    Defense,
    Death,
	AttackingTower,
	Frozen,
	StopMovement
}

public abstract class FSMStateArgs
{
}

public abstract class FSMState
{
    //protected Dictionary<Transition, StateID> map = new Dictionary<Transition, StateID>();
	protected List<StateID> map = new List<StateID>();
    protected StateID stateID;
    public StateID StateID { get { return stateID; } }

	public FSMState(params StateID[] statesToTransition)
	{
		for (int i=0; i<statesToTransition.Length; i++)
		{
			if(map.Contains(statesToTransition[i]) == false)
				map.Add(statesToTransition[i]);
		}
	}

	public void AddTransition (StateID id) //Transition trans,   
    {
       /* if (trans == Transition.Null)
        {
            Debug.LogError("FSMState ERROR: NullTransition is not allowed for a real transition");
            return;
        }*/

        if (id == StateID.Null)
        {
            Debug.LogError("FSMState ERROR: NullStateID is not allowed for a real ID");
            return;
        }

		if(map.Contains(id))
		{
			Debug.LogError("FSMState ERROR: State " + stateID.ToString() + " is already added");
			return;
		}

		map.Add(id);
        /*if (map.ContainsKey(trans))
        {
            Debug.LogError("FSMState ERROR: State " + stateID.ToString() + " already has transition " + trans.ToString() +
                           "Impossible to assign to another state");
            return;
        }*/

        //map.Add(trans, id);
    }


	public void DeleteTransition (StateID id) //Transition trans,  
    {

        /*if (trans == Transition.Null)
        {
            Debug.LogError("FSMState ERROR: NullTransition is not allowed");
            return;
        }*/

		if(map.Contains(id))
		{
			map.Remove(id);
			return;
		}
        /*if (map.ContainsKey(trans))
        {
            map.Remove(trans);
            return;
        }*/
		Debug.LogError("FSMState ERROR: State " + stateID.ToString() + " was not in the transition list");
        /*Debug.LogError("FSMState ERROR: Transition " + trans.ToString() + " passed to " + stateID.ToString() +
                       " was not on the state's transition list"); */
    }


    /// <summary>
    /// pobierz stan do ktorego mozna dojsc przez dane transition
    /// </summary>
    /// <param name="trans"></param>
    /// <returns></returns>
    /*
	public StateID GetOutputState (Transition trans)
    {
        if (map.ContainsKey(trans))
        {
            return map[trans];
        }
        return StateID.Null;
    }
`	*/

	public bool HasTransitionStateID(StateID id)
	{
		return map.Contains(id);
	}

	public virtual void DoBeforeEntering (FSMStateArgs additionalEnteringArgs = null) { }

	public virtual void DoBeforeLeaving (FSMStateArgs additionalLeavingArgs = null) { }

    /// <summary>
    /// powod do przejscia do innego stanu
    /// </summary>
    /// <param name="player"></param>
    /// <param name="npc"></param>
    // public abstract void Reason (GameObject player, GameObject npc);


    public abstract void Reason ();

    /// <summary>
    /// rob w danym stanie
    /// </summary>
    /// <param name="player"></param>
    /// <param name="npc"></param>
   // public abstract void Act (GameObject player, GameObject npc);


    public abstract void Act ();

    /// <summary>
    /// ackje wykonywane w LateUpdatecie
    /// </summary>
    /// <param name="player"></param>
    /// <param name="npc"></param>
    //public abstract void LateAct (GameObject player, GameObject npc);
}