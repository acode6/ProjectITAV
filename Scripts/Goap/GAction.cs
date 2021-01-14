using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using GameCreator.Variables;
using System.Linq;
using GameCreator.Characters;

[CreateAssetMenu(fileName = "Action", menuName = "AgentActions/BaseAction")]
public abstract class GAction : ScriptableObject
{

    public string actionName = "Action";
    public float cost = 1.0f;
    public GameObject target; //location where this action will take place
 
   
    public string targetTag; // tag for the heirchary
    public float duration = new float();
    public float remainingduration = new float();

    public WorldState[] preConditions;
    public WorldState[] afterEffects;
   
    public Dictionary<string, bool> preconditions;
    public Dictionary<string, bool> effects;

    public bool running = false;// if action is being performed

    public GAgent ThisAgent;
    public GAction()
    {
        preconditions = new Dictionary<string, bool>();
        effects = new Dictionary<string, bool>();
    }

    public void Awake()
    {

        
     
        if (preConditions != null)
            foreach(WorldState w in preConditions)
            {
                preconditions.Add(w.key, w.value);
            }

        if(afterEffects != null)
            foreach (WorldState w in afterEffects)
        {
            effects.Add(w.key, w.value);
        }
        
       
       
       
    }
    
   
    //Validating if actions are achievable
    public bool IsAchievable()
    {
        return true;

    }

    public bool IsAchievableGiven(Dictionary<string, bool> conditions)
    {
        Dictionary<string, bool> match = new Dictionary<string, bool>();

        foreach (KeyValuePair<string, bool> p in preconditions)
        {

          match = conditions.Where(x => x.Key == p.Key && x.Value == p.Value).ToDictionary(x => x.Key, x=>x.Value);

            if (!match.ContainsKey(p.Key))
            {
                return false;
            }

        
        }

    
      
        return true;
    }

 
   //checking preconditions
    public abstract bool ContextPreConditions();
    //Activate action
    public abstract void ActionRunning();
    //Actions world state at the end
    public abstract bool ActionEffect();
    //Incase of interruption
    public abstract bool InterruptActionCleanUp();
}
