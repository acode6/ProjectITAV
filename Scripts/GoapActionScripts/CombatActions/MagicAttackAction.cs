using GameCreator.Core;
using GameCreator.Shooter;
using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameCreator.Core.TargetCharacter;

[CreateAssetMenu(fileName = "MagicAttackAction", menuName = "AgentActions/MagicAttackAction")]
public class MagicAttackAction : GAction
{

    GameObject currentTarget;

    private void OnEnable()
    {

        preconditions.Add(AgentFact.CanAttack.ToString(), true);
        preconditions.Add(AgentFact.MagicWeapon.ToString(), true);
        effects.Add(AgentTask.AttackEnemy.ToString(), true);


    }

    public override bool ContextPreConditions()
    {
        if (ThisAgent.ShooterComponent.isChargingShot == true || ThisAgent.MeleeComponent.IsStaggered || ThisAgent.MeleeComponent.IsAttacking)
        {
            Debug.Log("Charging spell fail");
            return false;
        }

        currentTarget = ThisAgent.BlackBoard.CurrentEnemy.gameObject;


        if (currentTarget != null)
        {

            target = null;

            VariablesManager.SetLocal(ThisAgent.agentCombatManager.MyShooterComponent.modelWeapon, "ChargeShot", true, true);

            return true;
        }


        return false;
    }



    public override bool ActionEffect()
    {

        VariablesManager.SetLocal(ThisAgent.agentCombatManager.MyShooterComponent.modelWeapon, "ChargeShot", false, true);

        ThisAgent.ShooterComponent.ExecuteChargedShot();
        ThisAgent.ShooterComponent.StopAiming();



       




        return true;
    }
    public override bool InterruptActionCleanUp()
    {
        Debug.Log("Interrupted");
        VariablesManager.SetLocal(ThisAgent.agentCombatManager.MyShooterComponent.modelWeapon, "ChargeShot", false, true);
        if (ThisAgent.ShooterComponent.isChargingShot == true)
        {

            ThisAgent.ShooterComponent.currentAmmo.StopCharge(ThisAgent.ShooterComponent);
        }
        ThisAgent.ShooterComponent.StopAiming();
        return false;
    }

    public override void ActionRunning()
    {

        ThisAgent.myCharacter.characterLocomotion.canRun = false;


        if (ThisAgent.ShooterComponent.isChargingShot == false)
        {

            ThisAgent.ShooterComponent.StartChargedShot();
        }

        if (ThisAgent.ShooterComponent.isAiming == false)
        {
            Debug.Log("Aiming Target " + ThisAgent.agentCombatManager.aimingTarget);
            ThisAgent.ShooterComponent.StartAiming(ThisAgent.agentCombatManager.aimingTarget);
        }

        if (ThisAgent.MeleeComponent.IsStaggered == true)
        {
            ThisAgent.CancelCurrentGoal();
        }














    }
}
