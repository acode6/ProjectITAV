using GameCreator.Core;
using GameCreator.Shooter;
using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameCreator.Core.TargetCharacter;

[CreateAssetMenu(fileName = "SummonMinionAction", menuName = "AgentActions/SummonMinionAction")]
public class SummonMinionAction : GAction
{

    GameObject currentTarget;
    GameObject summon;
    private void OnEnable()
    {
     
      
        preconditions.Add(AgentFact.CanChargeAura.ToString(), false);     
        //preconditions.Add(AgentFact.InEnemyMeleeRange.ToString(), false);         
        effects.Add(AgentTask.AvoidDamage.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {
       

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

        summon = Instantiate(ThisAgent.BlackBoard.MainWeapon.SummoningSpell.SpellPreFab, ThisAgent.BlackBoard.Position , ThisAgent.BlackBoard.MainWeapon.SummoningSpell.SpellPreFab.transform.rotation);
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

      
      //   ThisAgent.TrackTarget(currentTarget);







    }
}
