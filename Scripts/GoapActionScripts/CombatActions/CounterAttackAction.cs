using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CounterAttackAction", menuName = "AgentActions/CounterAttackAction")]
public class CounterAttackAction : GAction
{

    GameObject currentTarget = null;
    bool counter = false;
    public void OnEnable()
    {


        preconditions.Add(AgentFact.MeleeWeapon.ToString(), true);
        preconditions.Add(AgentFact.CanCounter.ToString(), true);

        effects.Add(AgentTask.AvoidDamage.ToString(), true);


    }

    public override bool ContextPreConditions()
    {

        if (  ThisAgent.AgentFacts.HasState(AgentFact.CounteredAttack.ToString(), false))
        {
            return false;
        }

        counter = false;
        currentTarget = ThisAgent.agentCombatManager.preyObject;

        if (currentTarget == null)
        {
            return false;
        }

        target = ThisAgent.gameObject;


        duration = .2f;
        return true;




    }



    public override bool ActionEffect()
    {
      


        ThisAgent.MeleeComponent.StopBlocking();
        ThisAgent.MeleeComponent.currentShield.perfectBlockWindow = 0;
     

            ThisAgent.agentCombatManager.AgentConfidence = ThisAgent.AgentData.AgentStartingConfidence;
          //  Debug.Log("Successful Counter");
        



        return true;
    }
    public override bool InterruptActionCleanUp()
    {
        ThisAgent.MeleeComponent.StopBlocking();
        ThisAgent.agentCombatManager.AgentConfidence = ThisAgent.AgentData.AgentStartingConfidence;
       // Debug.Log("Countered Attack cancelled");
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


            if (ThisAgent.AgentFacts.HasState(AgentFact.InMeleeRange.ToString(), true))
            {

                ThisAgent.MeleeComponent.StartBlocking();


                ThisAgent.MeleeComponent.currentShield.perfectBlockWindow = 1;



                if (counter == false)
                {
                    if (ThisAgent.AgentFacts.HasState(AgentFact.CounteredAttack.ToString(), false))
                    {
                        counter = true;
                        Debug.Log("Countered Attack");
                        ThisAgent.currentAction.ActionEffect();
                    }
                }
            }
            else
            {
                ThisAgent.CancelCurrentGoal();
            }
        }









    }
}
