using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StationaryBlockAction", menuName = "AgentActions/StationaryBlockAction")]
public class StationaryBlockAction : GAction
{
   
    GameObject currentTarget = null;
  
    public void OnEnable()
    {
    

        preconditions.Add(AgentFact.MeleeWeapon.ToString(), true);
        preconditions.Add(AgentFact.Blocking.ToString(), true);       
  
        effects.Add(AgentTask.AvoidDamage.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {
      
        if(ThisAgent.MeleeComponent.IsAttacking == true || ThisAgent.MeleeComponent.IsStaggered == true)
        {
            return false;
        }

        
        currentTarget = ThisAgent.agentCombatManager.preyObject;

       if(currentTarget == null)
        {
            return false;
        }
            
        target = ThisAgent.gameObject;
             
               

                return true;
            
        

        
    }



    public override bool ActionEffect()
    {

     

        ThisAgent.MeleeComponent.StopBlocking();

        return true;
    }
    public override bool InterruptActionCleanUp()
    {
        ThisAgent.MeleeComponent.StopBlocking();
        return false;
    }

    public override void ActionRunning()
    {
        
   //     
       
         
            
        if (ThisAgent.AgentFacts.HasState(AgentFact.Blocking.ToString(), true))
        {
            if (ThisAgent.AgentFacts.HasState(AgentFact.BlockedAttack.ToString(), true))
            {
                ThisAgent.agentCombatManager.AgentConfidence += Time.deltaTime * ThisAgent.AgentData.AgentConfidenceIncreaseRate;
            }
            ThisAgent.MeleeComponent.StartBlocking();
            duration = 1f;
        }



     



    }
}
