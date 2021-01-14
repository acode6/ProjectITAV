using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockAction", menuName = "AgentActions/BlockAction")]
public class BlockAttackAction : GAction
{

    GameObject currentTarget = null;

    public void OnEnable()
    {
     

       
        preconditions.Add(AgentFact.Blocking.ToString(), true);

        effects.Add(AgentTask.AvoidDamage.ToString(), true);


    }

    public override bool ContextPreConditions()
    {

        if (ThisAgent.MeleeComponent.IsAttacking == true || ThisAgent.MeleeComponent.IsStaggered == true)
        {
            return false;
        }


        currentTarget = ThisAgent.agentCombatManager.preyObject;

        if (currentTarget == null)
        {
            return false;
        }

        target = null;


        ThisAgent.MeleeComponent.StartBlocking();
        return true;




    }



    public override bool ActionEffect()
    {



        ThisAgent.MeleeComponent.StopBlocking();
        ThisAgent.MeleeComponent.currentShield.perfectBlockWindow = 0;
        if (ThisAgent.AgentFacts.HasState(AgentFact.CounteredAttack.ToString(), true))
        {
            ThisAgent.agentCombatManager.AgentConfidence = ThisAgent.AgentData.AgentStartingConfidence;

        }
        return true;
    }
    public override bool InterruptActionCleanUp()
    {
        ThisAgent.MeleeComponent.StopBlocking();
        return false;
    }

    public override void ActionRunning()
    {


        if (currentTarget == null)
        {
            ThisAgent.CancelCurrentGoal();
        }

        if (currentTarget != null)
        {
           







            if (ThisAgent.agentCombatManager.EnemyAttacking.FactState == true )
            {
                if (ThisAgent.RequestCounter() == true)
                {
                    ThisAgent.MeleeComponent.currentShield.perfectBlockWindow = 1;
                   
                }
                else
                {
                    ThisAgent.MeleeComponent.currentShield.perfectBlockWindow = 0;
                }
            }


            
           
            duration = 1f;
        }









    }
}
