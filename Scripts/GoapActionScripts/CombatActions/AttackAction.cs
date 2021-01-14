using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackAction", menuName = "AgentActions/AttackAction")]
public class AttackAction : GAction
{

    GameObject currentTarget;

    public void OnEnable()
    {

        preconditions.Add(AgentFact.InMeleeRange.ToString(), true);
        preconditions.Add(AgentFact.CanAttack.ToString(), true);      
        effects.Add(AgentTask.AttackEnemy.ToString(), true);


    }

    public override bool ContextPreConditions()
    {

        

        currentTarget = ThisAgent.BlackBoard.CurrentEnemy.gameObject;


        if (currentTarget != null)
        {

            target = null;



            return true;
        }


        return false;
    }



    public override bool ActionEffect()
    {

   
     
        return true;
    }
    public override bool InterruptActionCleanUp()
    {
        ThisAgent.MeleeComponent.StopAttack();
        Debug.Log("Melee cancel");
        return false;
    }

    public override void ActionRunning()
    {
        if (ThisAgent.AgentFacts.HasState(AgentFact.InMeleeRange.ToString(), true) )
        {



            ThisAgent.MeleeComponent.Execute(GameCreator.Melee.CharacterMelee.ActionKey.A);
            if (ThisAgent.MeleeComponent.IsAttacking == true)
            {

                ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = false;

                duration = ThisAgent.MeleeComponent.ReturnCurrentMeleeClip().animationClip.length;

            }



        }
        else
        {
            ThisAgent.currentAction.ActionEffect();
        }

        if(ThisAgent.AgentFacts.HasState(AgentFact.CanCounter.ToString(), true) )
        {
            ThisAgent.CancelCurrentGoal();
        }







    }
}
