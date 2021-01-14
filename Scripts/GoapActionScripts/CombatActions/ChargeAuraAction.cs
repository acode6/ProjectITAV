using GameCreator.Core;
using GameCreator.Shooter;
using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameCreator.Core.TargetCharacter;

[CreateAssetMenu(fileName = "ChargeAuraAction", menuName = "AgentActions/ChargeAuraAction")]
public class ChargeAuraAction : GAction
{

    GameObject currentTarget;

    private void OnEnable()
    {
     
       
       
        preconditions.Add(AgentFact.CanChargeAura.ToString(), true);     
        preconditions.Add(AgentFact.MagicWeapon.ToString(), true);         
        effects.Add(AgentFact.AuraCharged.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {
        target = null;
        ThisAgent.PlayAnimClip(ThisAgent.AgentData.AuraChargeAnim, ThisAgent.BlackBoard.MainWeapon.WeaponAnimMask);

       
        return true;
    }



    public override bool ActionEffect()
    {
      
        if(ThisAgent.MeleeComponent.IsStaggered == true)
        {
            
            return false;
        }

        GameObject instance = Instantiate(ThisAgent.BlackBoard.MainWeapon.WandAura, ThisAgent.transform);
        ThisAgent.agentCombatManager.ChargeAura = instance;
        ThisAgent.agentCombatManager.AuraChargedState = true;
        return true;
    }
    public override bool InterruptActionCleanUp()
    {
      
     
        return false;
    }

    public override void ActionRunning()
    {

        if (ThisAgent.MeleeComponent.IsStaggered == true)
        {
            ThisAgent.CancelCurrentGoal();
            
        }
      


       

       





    }
}
