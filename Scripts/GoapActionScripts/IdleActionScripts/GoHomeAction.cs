using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GoHomeAction", menuName = "AgentActions/IdleActions/GoHomeAction")]
public class GoHomeAction : GAction
{

    GameObject currentTarget;
    bool blocking;
    GObjectInfo ObjectInfo;
    public void OnEnable()
    {


        preconditions.Add(AgentFact.AtHome.ToString(), false);
        preconditions.Add(TimeOfDay.Night.ToString(), true);
        effects.Add(AgentTask.Rest.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {

        ThisAgent.randomExploration = false;
        blocking = false;
      
        if (ThisAgent.BlackBoard.myAwareness != AIAwareness.Idle  )
        {
            return false;
        }
        Debug.Log("Agents home " + ThisAgent.BlackBoard.AgentHome);
        target = ThisAgent.BlackBoard.AgentHome.ActionDestination;

        if (target == null)
        {
            Debug.Log("Check agent Home");
            return false;  

        }

        Debug.Log("Going home");
        ThisAgent.walk = true;



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

   
     
     
        if (ThisAgent.BlackBoard.myAwareness != AIAwareness.Idle)
        {
            ThisAgent.CancelCurrentGoal();
        }

        

    }
}
