using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HideFormAction", menuName = "AgentActions/CreatureIdles/HideFormAction")]
public class HideFormAction : GAction
{

    GameObject currentTarget;
    bool hiding;
    private void OnEnable()
    {
 
       
      //  preconditions.Add(AgentFact.currentEnemy.ToString(), false);       

        effects.Add(AgentTask.StaySafe.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {
        if (ThisAgent.BlackBoard.myAwareness != AIAwareness.Idle)
        {
            return false;
        }

        target = ThisAgent.gameObject;
        hiding = false;
        ThisAgent.ChangeAgentState(ThisAgent.AgentData.HidingState);


        return true;
    }



    public override bool ActionEffect()
    {

      

        ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;
        ThisAgent.UpdateAwareness();
        ThisAgent.BlackBoard.CurrentEnemy = null;
        ThisAgent.ResetAgentState();
        return true;
    }
    public override bool InterruptActionCleanUp()
    {
        

        ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;
        ThisAgent.UpdateAwareness();
        ThisAgent.BlackBoard.CurrentEnemy = null;
        ThisAgent.ResetAgentState();
        return false;
    }

    public override void ActionRunning()
    {

        if (hiding == false)
        {
            hiding = true;
            
           
        }

        duration += ThisAgent.BlackBoard.Time;

       

        
         
            
        

        
    }

    IEnumerator SetAgentActive()
    {
        yield return new WaitForSeconds(1f);

        ThisAgent.gameObject.SetActive(false);
    }

}
