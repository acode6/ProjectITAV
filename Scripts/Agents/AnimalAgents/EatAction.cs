using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EatAction", menuName = "AgentActions/EatAction")]
public class EatAction : GAction
{
    bool Eating = false;
    GameObject TargetObject = null;
    public void OnEnable()
    {

        preconditions.Add(AgentFact.Hungry.ToString(), true);
        preconditions.Add(AgentFact.CanEat.ToString(), true);
        effects.Add(AgentTask.SatisfyNeed.ToString(), true);


    }

    public override bool ContextPreConditions()
    {
        Debug.Log("Trying To Eat");
        TargetObject = ThisAgent.AgentMemory.FindClosestFoodSource();
        Eating = false;

        if(TargetObject == null)
        {
            Debug.Log("No food sources");
            return false;
        }
        target = null;
        ThisAgent.BlackBoard.AgentEating = true;
 
        return true;
    }

    public override bool ActionEffect()
    {

        ThisAgent.ResetAgentState();
        ThisAgent.AgentMemory.FoodSources.Remove(TargetObject);
        Destroy(TargetObject);

        ThisAgent.BlackBoard.AgentEating = false;
        return false;


    }

    public override bool InterruptActionCleanUp()
    {
        ThisAgent.ResetAgentState();
        ThisAgent.BlackBoard.AgentEating = false;
        return false;
    }
    public override void ActionRunning()
    {

       
        if (Vector3.Distance(ThisAgent.BlackBoard.Position, TargetObject.transform.position) < 2.5f)
        {
            if (Eating == false)
            {
                Eating = true;
                ThisAgent.ChangeAgentState(ThisAgent.AgentData.EatingState);
            }


            if (Eating == true)
            {
                ThisAgent.LookAtTarget(TargetObject);
            }


        }
        if (Vector3.Distance(ThisAgent.BlackBoard.Position, TargetObject.transform.position) >= 2.5f)
        {
            Eating = false;
            duration = 5;
            ThisAgent.ResetAgentState();
            ThisAgent.LookAtTarget(TargetObject);
            ThisAgent.FollowTarget(TargetObject);


        }
      


    }
}