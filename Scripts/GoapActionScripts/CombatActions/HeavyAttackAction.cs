using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeavyAttackAction", menuName = "AgentActions/HeavyAttackAction")]
public class HeavyAttackAction : GAction
{

    GameObject currentTarget;

    public void OnEnable()
    {

        preconditions.Add(AgentFact.CanAttack.ToString(), true);
        preconditions.Add(AgentFact.CanHeavyAttack.ToString(), true);
        preconditions.Add(AgentFact.InMeleeRange.ToString(), true);
        effects.Add(AgentTask.AttackEnemy.ToString(), true);


    }

    public override bool ContextPreConditions()
    {
        if ( ThisAgent.AgentFacts.HasState(AgentFact.CanCounter.ToString(), true))
        {
            return false;
        }

        currentTarget = ThisAgent.BlackBoard.CurrentEnemy.gameObject;


        if (currentTarget != null)
        {

            target = currentTarget;
            ThisAgent.LookAtTarget(currentTarget);


            return true;
        }


        return false;
    }



    public override bool ActionEffect()
    {



        if (currentTarget != null)
        {
            if (currentTarget.GetComponent<GAgent>() != null && currentTarget.GetComponent<GAgent>().BlackBoard.AgentHealth <= 0)
            {
                ThisAgent.BlackBoard.CurrentEnemy = null;
                ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;
                ThisAgent.UpdateAwareness();
                return true;
            }
            if (!ThisAgent.AgentFacts.HasState(AgentFact.AttackLanded.ToString(), true))
            {
                ThisAgent.agentCombatManager.AgentConfidence -= ThisAgent.AgentData.AgentConfidenceIncreaseRate ;
            }
        }

        ThisAgent.currentAction.running = false;
        return false;
    }
    public override bool InterruptActionCleanUp()
    {
        return false;
    }

    public override void ActionRunning()
    {


        if (ThisAgent.AgentFacts.HasState(AgentFact.InMeleeRange.ToString(), true) && ThisAgent.agentCombatManager.CanHeavyFact.FactState == true)
        {



            ThisAgent.MeleeComponent.Execute(GameCreator.Melee.CharacterMelee.ActionKey.B);
            if (ThisAgent.MeleeComponent.IsAttacking == true)
            {

                ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = false;

                duration = 1f;
              
            }
          
          

        }
        else
        {
            ThisAgent.currentAction.ActionEffect();
        }




    }
}
