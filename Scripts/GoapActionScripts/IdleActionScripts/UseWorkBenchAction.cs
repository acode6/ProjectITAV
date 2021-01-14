using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UseWorkbenchAction", menuName = "AgentActions/IdleActions/UseWorkBenchAction")]
public class UseWorkBenchAction : GAction
{

    GameObject currentTarget;
    bool blocking;
    GObjectInfo ObjectInfo = null;
    public void OnEnable()
    {


        preconditions.Add(AgentNeeds.WorkNeed.ToString(), true);
        effects.Add(AgentTask.UseGrindStone.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {


        ThisAgent.randomExploration = false;
        blocking = false;
        if (ThisAgent.BlackBoard.myAwareness != AIAwareness.Idle || ThisAgent.timeKeeper.currentTOD == TimeOfDay.Night)
        {
            Debug.Log("Idle or night");
            if(ThisAgent.timeKeeper.currentTOD == TimeOfDay.Night)
            {
                //maybe insert belief to shut off forge here
            }
            return false;
        }

       
        target = Gworld.Instance.GetQueue(WorldObject.WorkBench.ToString()).FindResourceClosestToMe(ThisAgent.BlackBoard.Position);
        Debug.Log(" Workbench Target  " + target);

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

        ThisAgent.BlackBoard.AgentStates.working = false;
        ObjectInfo.WorldObject.CurrentObjectOwner = null;
        ThisAgent.ResetAgentState();
        ThisAgent.updateAgentMov = true;
        return true;
    }
    public override bool InterruptActionCleanUp()
    {
        ThisAgent.BlackBoard.AgentStates.working = false;
        ObjectInfo.WorldObject.CurrentObjectOwner = null;
        ThisAgent.randomExploration = false;
        ThisAgent.ResetAgentState();
        ThisAgent.updateAgentMov = true;
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

                Debug.Log("Use Object");
            }




        }
        if (blocking == true)
        {
            ThisAgent.BlackBoard.AgentStates.working = true;

          

            


        }
       

        if (ThisAgent.BlackBoard.myAwareness != AIAwareness.Idle)
        {
            ThisAgent.CancelCurrentGoal();
        }

        

    }
}
