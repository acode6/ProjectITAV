using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UseSeatAction", menuName = "AgentActions/IdleActions/UseSeatAction")]
public class UseSeatAction : GAction
{

    GameObject currentTarget;
    bool blocking;
    GObjectInfo ObjectInfo;
    public void OnEnable()
    {


        preconditions.Add(AgentFact.AvailableSeat.ToString(), true);
        effects.Add(AgentTask.Rest.ToString(), true);


    }

    public override bool ContextPreConditions()
    {

        ThisAgent.randomExploration = false;
        blocking = false;

        if (ThisAgent.BlackBoard.myAwareness != AIAwareness.Idle)
        {
            return false;
        }


        currentTarget = Gworld.Instance.GetQueue(WorldObject.AvailableSeat.ToString()).FindResourceClosestToMe(ThisAgent.BlackBoard.Position);
        // currentTarget = Gworld.Instance.GetQueue(AgentFact.AvailableSeat.ToString()).FindResourceClosestToMe(ThisAgent.BlackBoard.Position);
        if (currentTarget != null)
        {
            target = Gworld.Instance.GetQueue(AgentFact.AvailableSeat.ToString()).RemoveResource(currentTarget);
        }
        // Debug.Log(" Seat Queue Count " + Gworld.Instance.GetQueue(AgentFact.AvailableSeat.ToString()).que.Count);
        //  Debug.Log(" Seat Target  " + target);
        //  Debug.Log(" Seat Target Parent " + target.transform.parent);
        if (target == null)
        {
            Debug.Log("use seat returning false");
            return false;

        }
        else
        {

            ObjectInfo = target.transform.parent.GetComponent<GObjectInfo>();
            if (ObjectInfo == null)
            {
                ObjectInfo = target.transform.GetComponent<GObjectInfo>();
                if (ObjectInfo == null)
                {
                    Debug.Log(target + "Object info problem");
                    return false;
                }
            }
            if (ObjectInfo.WorldObject.CurrentObjectOwner == null)
            {
                ObjectInfo.WorldObject.CurrentObjectOwner = ThisAgent;
                ObjectInfo.WorldObject.ObjectPrimaryFact = AgentFact.Fact_InvalidType;
            }


        }

        ThisAgent.walk = true;



        return true;


    }



    public override bool ActionEffect()
    {
        ThisAgent.BlackBoard.AgentStates.resting = false;
        ObjectInfo.WorldObject.CurrentObjectOwner = null;
        ObjectInfo.WorldObject.ObjectPrimaryFact = AgentFact.AvailableSeat;
        Gworld.Instance.GetQueue(AgentFact.AvailableSeat.ToString()).AddResource(target);
        ThisAgent.updateAgentMov = true;
        ThisAgent.ResetAgentState();

        return true;
    }
    public override bool InterruptActionCleanUp()
    {
        ThisAgent.BlackBoard.AgentStates.resting = false;
        ObjectInfo.WorldObject.CurrentObjectOwner = null;
        Gworld.Instance.GetQueue(AgentFact.AvailableSeat.ToString()).AddResource(target);
        ObjectInfo.WorldObject.ObjectPrimaryFact = AgentFact.AvailableSeat;
        ThisAgent.updateAgentMov = true;
        ThisAgent.ResetAgentState();



        return false;
    }

    public override void ActionRunning()
    {



        if (blocking == false)
        {

            if (ThisAgent.pathAi.reachedDestination == true)
            {

                //  thisAgent.gameObject.transform.LookAt(target.transform);
                blocking = true;


                ObjectInfo.UseObjectLogic();
                ThisAgent.updateAgentMov = false;
                duration = ObjectInfo.WorldObject.ObjectUseTime;


            }




        }
        if (blocking == true)
        {


            ThisAgent.BlackBoard.AgentStates.resting = true;






        }

        if (ThisAgent.BlackBoard.myAwareness != AIAwareness.Idle)
        {
            ThisAgent.CancelCurrentGoal();
        }



    }
}
