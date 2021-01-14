using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StrafeAction", menuName = "AgentActions/StrafeAction")]
public class StrafeAction : GAction
{
   
    GameObject currentTarget = null;
    HumanoidAgent hAgent;
    public void OnEnable()
    {
        hAgent = ThisAgent as HumanoidAgent;

     
        preconditions.Add(AgentFact.CanAttack.ToString(), false);       
        preconditions.Add(AgentFact.InMeleeRange.ToString(), true);       
  
        effects.Add(AgentTask.AvoidDamage.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {
      
        
        
        currentTarget = ThisAgent.agentCombatManager.preyObject;

       if(currentTarget == null)
        {
            return false;
        }
            
        target = null;
             
               

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




        ThisAgent.myCharacter.characterLocomotion.canRun = false;
        ThisAgent.StrafeAroundTarget(ThisAgent.agentCombatManager.preyObject.transform.position);
     //   thisAgent.FollowTarget(thisAgent.AgentCombatManager.CurrentEnemy);






    }
}
