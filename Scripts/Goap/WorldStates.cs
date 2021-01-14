using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

[System.Serializable]
public class WorldState //holds a world state that will affect the world state 
{
    public string key; //holds world state name
    public bool value; //value to hold the amount of the state in the world (for overall UI)
}

//Class to hold the states of the world 
public class WorldStates 
{
    //ditionary to hold the state and how much of it is in the world 
    public Dictionary<string, bool> states;

    public WorldStates()
    {
        states = new Dictionary<string, bool>(); //declare new states dictionary
    }

    //checks to see if state is in dictionary
    public bool HasState(string key , bool value)
    {
        Dictionary<string, bool> match = new Dictionary<string, bool>();
     

        match = states.Where(x => x.Key == key && x.Value == value).ToDictionary(x => x.Key, x => x.Value);

          


        

        return match.ContainsKey(key); 
    }

    //adds state to dictionary
    void AddState(string key, bool value)
    {    
        states.Add(key, value);
    }

    //modify states and how much is added to the world 
    public void ModifyState( string key, bool value)
    {
        if (states.ContainsKey(key))
        {
           // states[key] += value;
         //   if (states[key] <= 0)
             //   RemoveState(key);
        }
        else
            states.Add(key, value);
    }

    //remove the state 
    public void RemoveState(string key)
    {
        if (states.ContainsKey(key))
            states.Remove(key);
    }

    //change the value of a state
    public void SetState(string key, bool value)
    {
        if (states.ContainsKey(key))
            states[key] = value;
        else
            states.Add(key, value);
    }

    //get the state
    public Dictionary<string, bool> GetStates()
    {
        return states;
    }
}
