using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ManningBarAction", menuName = "AgentActions/IdleActions/ManningBarAction")]
public class ManningBarAction : GAction
{

    GameObject currentTarget;
    bool blocking;
    GObjectInfo ObjectInfo = null;
    public void OnEnable()
    {

     
       
        effects.Add(AgentTask.Work.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {


        ThisAgent.randomExploration = false;
        blocking = false;
        if (ThisAgent.BlackBoard.myAwareness != AIAwareness.Idle  )
        {
            return false;
        }


        target = Gworld.Instance.GetQueue(WorldObject.Bar.ToString()).FindResourceClosestToMe(ThisAgent.BlackBoard.Position);
        //   Debug.Log(" Workbench Target  " + target);

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
       
        
        ObjectInfo.WorldObject.CurrentObjectOwner = null;
        ThisAgent.ResetAgentState();
        ThisAgent.updateAgentMov = true;
        return true;
    }
    public override bool InterruptActionCleanUp()
    {
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
