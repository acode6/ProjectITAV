using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeMidRangeAction", menuName = "AgentActions/MeleeMidRangeAction")]
public class MeleeMidRangeAction : GAction
{

    GameObject currentTarget;

    public void OnEnable()
    {
        preconditions.Add(AgentFact.MeleeWeapon.ToString(), true);
        preconditions.Add(AgentFact.InLungeRange.ToString(), true);
        preconditions.Add(AgentFact.CanLungeAttack.ToString(), true);
     
        effects.Add(AgentTask.FindOpening.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {
        if(ThisAgent.AgentFacts.HasState(AgentFact.InMeleeRange.ToString(), true) || ThisAgent.MeleeComponent.IsStaggered == true || ThisAgent.MeleeComponent.IsAttacking == true || ThisAgent.MeleeComponent.IsBlocking == true)
        {
            return false;
        }
       
        currentTarget = ThisAgent.BlackBoard.CurrentEnemy.gameObject;


        if (currentTarget != null )
        {


            target = ThisAgent.gameObject;
            if (ThisAgent.MeleeComponent != null)
                ThisAgent.MeleeComponent.Execute(GameCreator.Melee.CharacterMelee.ActionKey.D);

            return true;
        }


        return false;
    }



    public override bool ActionEffect()
    {
        if (ThisAgent.agentCombatManager.EnemyMeleeComponent.IsStaggered == false)
        {
            ThisAgent.agentCombatManager.AgentConfidence -= ThisAgent.AgentData.ConfidenceDecreaseRate;


        }
        //VariablesManager.SetLocal(this.gameObject, "Attack", false, true);

        if (currentTarget.GetComponent<GAgent>() != null && currentTarget.GetComponent<GAgent>().BlackBoard.AgentHealth <= 0)
        {
            ThisAgent.BlackBoard.CurrentEnemy = null;
            target = null;
            ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;
            ThisAgent.UpdateAwareness();
            return true;
        }

        ThisAgent.currentAction.running = false;
        return false;
    }
    public override bool InterruptActionCleanUp()
    {
        ThisAgent.MeleeComponent.StopAttack();
        return false;
    }

    public override void ActionRunning()
    {
      


        if (ThisAgent.MeleeComponent.IsAttacking == true)
        {
            ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = true;
            ThisAgent.FollowTarget(currentTarget);
      
           
          
            duration = 1;
        }
        else
        {
            ThisAgent.currentAction.ActionEffect();
        }

        if (ThisAgent.MeleeComponent.IsStaggered)
        {
            ThisAgent.CancelCurrentGoal();
        }





      


    }
}
