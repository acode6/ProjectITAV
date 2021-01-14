using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantCropsAction", menuName = "AgentActions/IdleActions/PlantCropsAction")]
public class PlantCropsAction : GAction
{

    GameObject currentTarget;
    bool blocking;
    GObjectInfo ObjectInfo = null;
    public void OnEnable()
    {

     
        preconditions.Add(WorldObject.PlantingPlot.ToString(), true);
        effects.Add(AgentTask.Work.ToString(), true);
       

 
    }

    public override bool ContextPreConditions()
    {


        ThisAgent.randomExploration = false;
        blocking = false;
        if (ThisAgent.BlackBoard.myAwareness != AIAwareness.Idle || Gworld.Instance.GetQueue(WorldObject.PlantingPlot.ToString().ToString()).que.Count <= 0 )
        {
            Debug.Log("Empty queue");
            return false;
        }


        target = Gworld.Instance.GetQueue(WorldObject.PlantingPlot.ToString()).FindResourceClosestToMe(ThisAgent.BlackBoard.Position);
        Gworld.Instance.GetQueue(WorldObject.PlantingPlot.ToString()).RemoveResource(target);
         Debug.Log(" plant Target  " + target);
        //   Debug.Log(" plant Target Parent " + target.transform.parent);
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

        ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = false;



        return true;

   
    }



    public override bool ActionEffect()
    {
        ObjectInfo.SwitchObjectFactState();
        
        ObjectInfo.WorldObject.CurrentObjectOwner = null;
        ThisAgent.ResetAgentState();
        ThisAgent.updateAgentMov = true;
        return true;
    }
    public override bool InterruptActionCleanUp()
    {
        Gworld.Instance.GetQueue(WorldObject.PlantingPlot.ToString()).AddResource(target);
        ObjectInfo.WorldObject.CurrentObjectOwner = null;
        ThisAgent.randomExploration = false;
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

                Debug.Log("Use Object");
            }




        }
        if (blocking == true)
        {

           
            if (ThisAgent.WorkNeed > 0f)
            {
                ThisAgent.WorkNeed -= (ThisAgent.timeKeeper.counter / 60) * 0.009f;
            }


            


        }
       

        if (ThisAgent.BlackBoard.myAwareness != AIAwareness.Idle)
        {
            ThisAgent.CancelCurrentGoal();
        }

        

    }
}
