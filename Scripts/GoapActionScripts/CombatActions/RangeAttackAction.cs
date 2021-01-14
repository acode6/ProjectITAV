using GameCreator.Core;
using GameCreator.Shooter;
using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameCreator.Core.TargetCharacter;

[CreateAssetMenu(fileName = "RangeAttackAction", menuName = "AgentActions/RangeAttackAction")]
public class RangeAttackAction : GAction
{

    GameObject currentTarget;

    private void OnEnable()
    {

        preconditions.Add(AgentFact.CanAttack.ToString(), true);
        preconditions.Add(AgentFact.RangeWeapon.ToString(), true);
        effects.Add(AgentTask.AttackEnemy.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {
        if (ThisAgent.ShooterComponent.isChargingShot == true)
        {
            Debug.Log("Already charging shot error");
            return false;
        }

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

        ThisAgent.ShooterComponent.ExecuteChargedShot();

        ThisAgent.ShooterComponent.StopAiming();



        return true;
    }
    public override bool InterruptActionCleanUp()
    {
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
        if (ThisAgent.AgentFacts.HasState(AgentFact.InViewSight.ToString(), true))
        {

            if (ThisAgent.ShooterComponent.isChargingShot == false)
            {

                ThisAgent.ShooterComponent.StartChargedShot();
            }

            if (ThisAgent.ShooterComponent.isAiming == false)
            {

                ThisAgent.ShooterComponent.StartAiming(ThisAgent.agentCombatManager.aimingTarget);
            }




        }
        else
        {
            ThisAgent.CancelCurrentGoal();
        }
       

        








    }
}
