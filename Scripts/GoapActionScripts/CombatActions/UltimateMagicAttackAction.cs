using GameCreator.Core;
using GameCreator.Shooter;
using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameCreator.Core.TargetCharacter;

[CreateAssetMenu(fileName = "UltimateMagicAttackAction", menuName = "AgentActions/UltimateMagicAttackAction")]
public class UltimateMagicAttackAction : GAction
{

    GameObject currentTarget;
    GameObject instance;
    private void OnEnable()
    {
     
      
        preconditions.Add(AgentFact.CanUseUltimate.ToString(), true);     
        preconditions.Add(AgentFact.AuraCharged.ToString(), true);     
        preconditions.Add(AgentFact.InEnemyMeleeRange.ToString(), false);         
        effects.Add(AgentTask.AttackEnemy.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {
        if (ThisAgent.ShooterComponent.isChargingShot == true || ThisAgent.agentCombatManager.CanAttackFact.FactState == false)
        {
            return false;
        }

        currentTarget = ThisAgent.BlackBoard.CurrentEnemy.gameObject;


        if (currentTarget != null)
        {

            target = ThisAgent.gameObject;
           
            VariablesManager.SetLocal(ThisAgent.agentCombatManager.MyShooterComponent.modelWeapon, "ChargeShot", true, true);
            ThisAgent.PlayAnimClip(ThisAgent.AgentData.UltimateAttackAnim);
            return true;
        }


        return false;


      
    }



    public override bool ActionEffect()
    {

        instance = Instantiate(ThisAgent.BlackBoard.MainWeapon.UltimateSpell.SpellPreFab, new Vector3(ThisAgent.transform.position.x, ThisAgent.transform.position.y, ThisAgent.transform.position.z ), ThisAgent.BlackBoard.MainWeapon.UltimateSpell.SpellPreFab.transform.rotation);
        VariablesManager.SetLocal(ThisAgent.agentCombatManager.MyCurrentWeapon.gameObject, "ChargeShot", false, true);
        if (ThisAgent.agentCombatManager.AgentConfidence >= 1f)
        {
            ThisAgent.agentCombatManager.AgentConfidence = ThisAgent.AgentData.AgentStartingConfidence;
        }
       
      

     
        return true;
    }
    public override bool InterruptActionCleanUp()
    {
        
      
        
        return false;
    }

    public override void ActionRunning()
    {

      
         ThisAgent.LookAtTarget(currentTarget);







    }
}
