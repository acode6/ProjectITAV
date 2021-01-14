using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BreakBlockAttackAction", menuName = "AgentActions/MeleeBreakBlockAttackAction")]
public class MeleeBreakBlockAction : GAction
{

    GameObject currentTarget;

    public void OnEnable()
    {
     
        preconditions.Add(AgentFact.CanAttack.ToString(), true);
        preconditions.Add(AgentFact.CanBreakBlock.ToString(), true);
        preconditions.Add(AgentFact.InMeleeRange.ToString(), true);
        effects.Add(AgentTask.AttackEnemy.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {
       

        currentTarget = ThisAgent.BlackBoard.CurrentEnemy.gameObject;


        if (currentTarget != null )
        {

            target = null;


            return true;
        }


        return false;
    }



    public override bool ActionEffect()
    {
    
       
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



            ThisAgent.MeleeComponent.Execute(GameCreator.Melee.CharacterMelee.ActionKey.E);




            if (ThisAgent.MeleeComponent.IsAttacking == true)
            {

                ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = false;

                duration = 1f;

            }



        }
        else
        {
            if (ThisAgent.MeleeComponent.IsAttacking == false)
            {

                ThisAgent.currentAction.ActionEffect();

            }

        }

    }
}
