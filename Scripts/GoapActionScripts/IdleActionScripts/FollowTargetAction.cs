using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FollowTargetAction", menuName = "AgentActions/LocomotionActions/FollowTargetAction")]
public class FollowTargetAction : GAction
{

    GameObject currentTarget;
    bool blocking;
    public void OnEnable()
    {


        preconditions.Add(AgentFact.HasFollowTarget.ToString(), true);
        effects.Add(AgentTask.GoToTarget.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {
       
     

        if(target == null)
        {
            return false;
        }


     
        ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = false;

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
            ThisAgent.CancelCurrentGoal();
        }

        

    }
}
