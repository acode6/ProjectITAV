using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExploreAction", menuName = "AgentActions/ExploreAction")]
public class ExploreAction : GAction
{

    GameObject currentTarget;
    bool blocking;
    public void OnEnable()
    {

     
        preconditions.Add(AgentFact.InSpeciesBiome.ToString(), true);
        preconditions.Add(AgentFact.Tired.ToString(), false);
        effects.Add(AgentTask.ExploreArea.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {
       
     


        if (ThisAgent.BlackBoard.myAwareness != AIAwareness.Idle  )
        {
            return false;
        }


        target = null;
        ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = false;
        ThisAgent.randomExploration = true;
        Debug.Log("EXPLORING");
        return true;

   
    }



    public override bool ActionEffect()
    {

  

        return true;
    }
    public override bool InterruptActionCleanUp()
    {
      
        return false;
    }

    public override void ActionRunning()
    {



     
        
        if (ThisAgent.BlackBoard.myAwareness != AIAwareness.Idle)
        {
            ThisAgent.currentAction.ActionEffect();
        }

        

    }
}
