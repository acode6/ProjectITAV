using GameCreator.Core;
using GameCreator.Shooter;
using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameCreator.Core.TargetCharacter;

[CreateAssetMenu(fileName = "ContingencyAttackAction", menuName = "AgentActions/ContingencyAttackAction")]
public class ContingencyAttackAction : GAction
{

    GameObject currentTarget;
    GameObject instance;
    private void OnEnable()
    {
     
      
       
        preconditions.Add(AgentFact.LowHealth.ToString(), true);       
        effects.Add(AgentTask.StaySafe.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {

       
        currentTarget = ThisAgent.BlackBoard.CurrentEnemy.gameObject;


        if (currentTarget != null)
        {

            target = null;
           
          
            ThisAgent.ChangeAgentState(ThisAgent.AgentData.ChargingState);
            Debug.Log("CONTINGENCY");
            return true;
        }

        Debug.Log("CONTINGENCY fail");
        return false;


      
    }



    public override bool ActionEffect()
    {

          instance = Instantiate(ThisAgent.BlackBoard.MainWeapon.UltimateSpell.SpellPreFab, new Vector3(ThisAgent.transform.position.x, ThisAgent.transform.position.y, ThisAgent.transform.position.z ), ThisAgent.BlackBoard.MainWeapon.UltimateSpell.SpellPreFab.transform.rotation);


        ThisAgent.ResetAgentState();

        ThisAgent.myCharacterStats.SetAttrValue("health",0,true);
     
        return true;
    }
    public override bool InterruptActionCleanUp()
    {

        ThisAgent.ResetAgentState();

        return false;
    }

    public override void ActionRunning()
    {


    
         ThisAgent.FollowTarget(currentTarget);
        






    }
}
