using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveToPosition", menuName = "AgentActions/IdleActions/MoveToPositionAction")]
public class MoveToPositionAction : GAction
{

    GameObject currentTarget;
    bool blocking;
    GObjectInfo ObjectInfo;
    public void OnEnable()
    {


       // preconditions.Add(AgentFact.NeedPath.ToString(), true);
        effects.Add(AgentTask.GoToTarget.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {

       
        if (ThisAgent.AgentFacts.HasState(AgentFact.NeedPath.ToString(), true))
        {
            Debug.Log("Here");
            return false;
        }
        target = ThisAgent.locotionTester;

        if (target == null)
        {
            return false;
        }


        return true;

   
    }



    public override bool ActionEffect()
    {
        
      

        return true;
    }
    public override bool InterruptActionCleanUp()
    {
      
      
        return false;
    }

    public override void ActionRunning()
    {


        duration = Vector3.Distance(ThisAgent.myLocation, ThisAgent.pathAi.destination);
       
      

        

    }
}
