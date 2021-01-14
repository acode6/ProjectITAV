using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RunAwayAction", menuName = "AgentActions / RunAwayAction")]
public class RunAwayAction : GAction
{

    GameObject currentTarget;
    bool hiding;
    private void OnEnable()
    {
 
       
        preconditions.Add(AgentFact.currentEnemy.ToString(), true);       

        effects.Add(AgentTask.StaySafe.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {
             hiding = false;
       
            currentTarget = ThisAgent.BlackBoard.CurrentEnemy.gameObject;


            if (currentTarget != null )
            {

                target = null;

               ThisAgent.walk = false;
            return true;
            }
        

        return false;
    }



    public override bool ActionEffect()
    {

        if(hiding == true)
        {
            ThisAgent.gameObject.SetActive(true);
        }

        ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;
        ThisAgent.UpdateAwareness();
        ThisAgent.BlackBoard.CurrentEnemy = null;
        ThisAgent.ResetAgentState();
        return true;
    }
    public override bool InterruptActionCleanUp()
    {
        if (hiding == true)
        {
            ThisAgent.gameObject.SetActive(true);
        }

        ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;
        ThisAgent.UpdateAwareness();
        ThisAgent.BlackBoard.CurrentEnemy = null;
        ThisAgent.ResetAgentState();
        return false;
    }

    public override void ActionRunning()
    {




       

        




        //if (ThisAgent.AgentFacts.HasState(ThisAgent.AgentData.Home.ToString(), true) && hiding == false)
        //{



        //    duration += .5f ;
            
        //       ThisAgent.pathAi.destination = Gworld.Instance.GetQueue("hidingspots").FindResourceClosestToMe(ThisAgent.BlackBoard.Position).transform.position;
        //       VariablesManager.SetLocal(ThisAgent.gameObject, "Target", ThisAgent.pathAi.steeringTarget, true);
        //    if (ThisAgent.AgentData.CanHide == true)
        //    {

                
                    
                
        //        if (Vector3.Distance(ThisAgent.transform.position, ThisAgent.pathAi.destination) < 1.2f)
        //        {

        //            if (hiding == false)
        //            {
        //                hiding = true;
        //                duration = ThisAgent.AgentDataCopy.HideTime;
                       
        //                ThisAgent.ChangeAgentState(ThisAgent.AgentDataCopy.HidingState);
                       
        //            }
        //            if(hiding == true)
        //            {
        //                //delay
        //               ThisAgent.StartCoroutine(SetAgentActive()); 

        //            }
        //        }
               

        //    }





        //}
      
           // Debug.Log("Run Away");
            ThisAgent.FleeFromTarget(currentTarget);
            duration = 15f;
         
            
        

        
    }
    IEnumerator SetAgentActive()
    {
        yield return new WaitForSeconds(1f);

        ThisAgent.gameObject.SetActive(false);
    }

}
