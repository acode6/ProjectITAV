using GameCreator.Core;
using GameCreator.Shooter;
using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameCreator.Core.TargetCharacter;

[CreateAssetMenu(fileName = "MagicHealAction", menuName = "AgentActions/MagicHealAction")]
public class MagicHealAction : GAction
{

    GameObject currentTarget;
    GAgent TargetAgent;
    private void OnEnable()
    {
     
       
       
        preconditions.Add(AgentFact.NeedHealth.ToString(), true);     
        preconditions.Add(AgentFact.InEnemyMeleeRange.ToString(), false);             
        preconditions.Add(AgentFact.HasMana.ToString(), true);         
        effects.Add(AgentTask.AvoidDamage.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {
        target = null;
        TargetAgent = null;
        currentTarget = null;
       
        currentTarget = ThisAgent.BlackBoard.AgentMemory.FindClosestObjectMatchingFact(AgentFact.NeedHealth, true);
        if(currentTarget == null)
        {
            return false;
        }

        if(currentTarget.gameObject.GetComponent<GAgent>() != null)
        {

            TargetAgent = currentTarget.gameObject.GetComponent<GAgent>();
        }
        ThisAgent.PlayAnimClip(ThisAgent.AgentData.AuraChargeAnim, ThisAgent.BlackBoard.MainWeapon.WeaponAnimMask);
        return true;
    }



    public override bool ActionEffect()
    {
      
        if( TargetAgent != null)
        {
            GameObject instance = Instantiate(ThisAgent.BlackBoard.MainWeapon.WandHealPrefab, currentTarget.transform);
            TargetAgent.myCharacterStats.SetAttrValue("health", TargetAgent.BlackBoard.AgentHealth + (TargetAgent.AgentData.AgentHealth/4), true);
        }
        
        ThisAgent.myCharacterStats.SetAttrValue("mana", ThisAgent.BlackBoard.AgentMana - 50, true);

        return true;
    }
    public override bool InterruptActionCleanUp()
    {
      
     
        return false;
    }

    public override void ActionRunning()
    {
        if(TargetAgent != ThisAgent)
        {
            ThisAgent.TrackTarget(currentTarget);
        }
        if (ThisAgent.MeleeComponent.IsStaggered == true)
        {
            ThisAgent.CancelCurrentGoal();
            
        }
      


       

       





    }
}
