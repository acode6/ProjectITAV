using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PatrolAction", menuName = "AgentActions/PatrolAction")]
public class PatrolAction : GAction
{

    GameObject currentTarget;
    bool blocking;
    public void OnEnable()
    {

     
       
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
        //Debug.Log("Patroling");
        return true;

   
    }



    public override bool ActionEffect()
    {

  

        return true;
    }
    public override bool InterruptActionCleanUp()
    {
        ThisAgent.randomExploration = false;
      
        return false;
    }

    public override void ActionRunning()
    {



     
        
        if (ThisAgent.BlackBoard.myAwareness != AIAwareness.Idle)
        {
            ThisAgent.CancelCurrentGoal();
        }

        

    }
}
