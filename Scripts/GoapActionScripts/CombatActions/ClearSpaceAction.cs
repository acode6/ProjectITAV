using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClearSpaceAction", menuName = "AgentActions/ClearSpaceAction")]
public class ClearSpaceAction : GAction
{

    GameObject currentTarget;

    public void OnEnable()
    {
  
        preconditions.Add(AgentFact.CanHeavyAttack.ToString(), true);
        preconditions.Add(AgentFact.InMeleeRange.ToString(), true);
        effects.Add(AgentTask.AvoidDamage.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {
       

        currentTarget = ThisAgent.BlackBoard.CurrentEnemy.gameObject;


        if (currentTarget != null )
        {

            target = currentTarget;



            return true;
        }


        return false;
    }



    public override bool ActionEffect()
    {

        //VariablesManager.SetLocal(this.gameObject, "Attack", false, true);
       
        if (currentTarget.GetComponent<GAgent>() != null && currentTarget.GetComponent<GAgent>().BlackBoard.AgentHealth <= 0)
        {
            ThisAgent.BlackBoard.CurrentEnemy = null;
            target = ThisAgent.gameObject;
            ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;
            ThisAgent.UpdateAwareness();
            return true;
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


        if (ThisAgent.AgentFacts.HasState(AgentFact.InMeleeRange.ToString(), true))
        {



            ThisAgent.MeleeComponent.Execute(GameCreator.Melee.CharacterMelee.ActionKey.B);
          
            if (ThisAgent.MeleeComponent.IsAttacking == true)
            {

                ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = false;
             
                duration = 0.25f;

            }
            else
            {
                ThisAgent.currentAction.ActionEffect();
            }

        }
        else
        {
            ThisAgent.currentAction.ActionEffect();
        }


       

    }
}
