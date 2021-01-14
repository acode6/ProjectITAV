using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UseBedAction", menuName = "AgentActions/IdleActions/UseBedAction")]
public class UseBedAction : GAction
{

    GameObject currentTarget;
    bool blocking;
    GObjectInfo ObjectInfo;
    public void OnEnable()
    {


        preconditions.Add(AgentFact.AtHome.ToString(), true);
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

      
       // target = ThisAgent.AgentMemory.FindClosestObjectMatchingFact(AgentFact.AvailableSeat, true);
       // Debug.Log(" Seat Queue Count " + Gworld.Instance.GetQueue(AgentFact.AvailableSeat.ToString()).que.Count);
      //  Debug.Log(" Seat Target  " + target);
      //  Debug.Log(" Seat Target Parent " + target.transform.parent);
        if (target == null)
        {
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

            ObjectInfo.WorldObject.CurrentObjectOwner = ThisAgent;
        }

        ThisAgent.walk = true;



        return true;

   
    }



    public override bool ActionEffect()
    {

        ObjectInfo.WorldObject.CurrentObjectOwner = null;
        ThisAgent.ResetAgentState();
        ThisAgent.updateAgentMov = true;
        return true;
    }
    public override bool InterruptActionCleanUp()
    {
        ObjectInfo.WorldObject.CurrentObjectOwner = null;
        ThisAgent.ResetAgentState();
        ThisAgent.updateAgentMov = true;
   

        return false;
    }

    public override void ActionRunning()
    {

    

        if(blocking == false)
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
        if(blocking == true)
        {

         
            if(ThisAgent.RestNeed > 0f)
            {
               ThisAgent.RestNeed -= (ThisAgent.timeKeeper.counter / 60) * 0.009f;
            }
                 
                
            

            


        }
     
        if (ThisAgent.BlackBoard.myAwareness != AIAwareness.Idle)
        {
            ThisAgent.CancelCurrentGoal();
        }

        

    }
}
