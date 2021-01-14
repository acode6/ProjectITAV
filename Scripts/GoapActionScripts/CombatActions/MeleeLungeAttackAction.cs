using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeLungeAttackAction", menuName = "AgentActions/MeleeLungeAttackAction")]
public class MeleeLungeAttackAction : GAction
{

    GameObject currentTarget;
    float agentsTrackSpeed = 0f;
    public void OnEnable()
    {


        preconditions.Add(AgentFact.CanLungeAttack.ToString(), true);
        preconditions.Add(AgentFact.InLungeRange.ToString(), true);

        effects.Add(AgentFact.InMeleeRange.ToString(), true);
        effects.Add(AgentFact.CanAttack.ToString(), true);


    }

    public override bool ContextPreConditions()
    {

        Debug.Log("Lunge Pre");
        currentTarget = ThisAgent.BlackBoard.CurrentEnemy.gameObject;


        if (currentTarget != null)
        {


            target = null;
            agentsTrackSpeed = ThisAgent.AgentData.trackSpeed;
            ThisAgent.agentCombatManager.trackSpeed = 0;
            ThisAgent.MeleeComponent.Execute(GameCreator.Melee.CharacterMelee.ActionKey.D);

            return true;
        }


        return false;
    }



    public override bool ActionEffect()
    {
        ThisAgent.agentCombatManager.trackSpeed = agentsTrackSpeed;
        if (currentTarget != null)
        {         

            if (!ThisAgent.AgentFacts.HasState(AgentFact.AttackLanded.ToString(), true))
            {
                ThisAgent.agentCombatManager.AgentConfidence -= ThisAgent.AgentData.AgentConfidenceIncreaseRate / 2;
            }
        }
      
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
            //   ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = true;
            //   thisAgent.FollowTarget(currentTarget);


          //  Debug.Log("Duration " + ThisAgent.MeleeComponent.ReturnCurrentMeleeClip().animationClip.length);
            duration = ThisAgent.MeleeComponent.ReturnCurrentMeleeClip().animationClip.length; 
        }
        else
        {
            ThisAgent.currentAction.ActionEffect();
        }








    }
}
