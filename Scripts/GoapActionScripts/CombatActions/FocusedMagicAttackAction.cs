using GameCreator.Core;
using GameCreator.Shooter;
using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameCreator.Core.TargetCharacter;

[CreateAssetMenu(fileName = "FocusedMagicAttackAction", menuName = "AgentActions/FocusedMagicAttackAction")]
public class FocusedMagicAttackAction : GAction
{

    GameObject currentTarget;
    GameObject instance;
    private void OnEnable()
    {
     
        preconditions.Add(AgentFact.CanAttack.ToString(), true);     
        preconditions.Add(AgentFact.AuraCharged.ToString(), true);     
        preconditions.Add(AgentFact.InEnemyMeleeRange.ToString(), true);     
        preconditions.Add(AgentFact.MagicWeapon.ToString(), true);         
        effects.Add(AgentTask.AttackEnemy.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {
        if (ThisAgent.ShooterComponent.isChargingShot == true || ThisAgent.agentCombatManager.CanAttackFact.FactState == false)
        {
            return false;
        }

        currentTarget = ThisAgent.BlackBoard.CurrentEnemy.gameObject;

     
        if (currentTarget != null )
        {

            target = null;
          instance = Instantiate(ThisAgent.BlackBoard.MainWeapon.CrowdControllSpell.SpellPreFab, ThisAgent.agentCombatManager.MyShooterComponent.modelWeapon.transform);
            VariablesManager.SetLocal(ThisAgent.agentCombatManager.MyShooterComponent.modelWeapon, "ChargeShot", true, true);

            return true;
        }


        return false;
    }



    public override bool ActionEffect()
    {

        Destroy(instance);
        VariablesManager.SetLocal(ThisAgent.agentCombatManager.MyCurrentWeapon.gameObject, "ChargeShot", false, true);

        if (currentTarget.GetComponent<GAgent>() != null && currentTarget.GetComponent<GAgent>().BlackBoard.AgentHealth <= 0)
        {
            ThisAgent.BlackBoard.CurrentEnemy = null;
           
            ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;
            ThisAgent.UpdateAwareness();
        }
      

     
        return true;
    }
    public override bool InterruptActionCleanUp()
    {

        Destroy(instance);
        VariablesManager.SetLocal(ThisAgent.agentCombatManager.MyCurrentWeapon.gameObject, "ChargeShot", false, true);
        return false;
    }

    public override void ActionRunning()
    {

        if (ThisAgent.agentCombatManager.InEnemyMeleeRange.FactState == true && ThisAgent.AgentFacts.HasState(AgentFact.InViewSight.ToString(), true))
        {
            if (ThisAgent.ShooterComponent.isAiming == false)
            {

                ThisAgent.ShooterComponent.StartAiming(ThisAgent.agentCombatManager.aimingTarget);
            }


            duration = 1.5f;

        }
        else
        {
            ThisAgent.CancelCurrentGoal();
        }

      



       





    }
}
