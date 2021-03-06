﻿using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoreCropsAction", menuName = "AgentActions/IdleActions/StoreCropsAction")]
public class StoreCropsAction : GAction
{

    GameObject currentTarget;
    bool blocking;
    GObjectInfo ObjectInfo;
    public void OnEnable()
    {

     
        preconditions.Add(AgentFact.Storage.ToString(), true);
        preconditions.Add(AgentTask.Harvest.ToString(), true);
        effects.Add(AgentTask.Work.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {



        blocking = false;
        if (ThisAgent.BlackBoard.myAwareness != AIAwareness.Idle  )
        {
            return false;
        }

      
        target = Gworld.Instance.GetQueue(AgentFact.Storage.ToString()).FindResourceClosestToMe(ThisAgent.BlackBoard.Position);
        Debug.Log(" Store Target  " + target);
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

  

        return true;
    }
    public override bool InterruptActionCleanUp()
    {
        ThisAgent.randomExploration = false;
      
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

                duration = ObjectInfo.WorldObject.ObjectUseTime;

                Debug.Log("Use Object");
            }




        }
        if (blocking == true)
        {

            Debug.Log("updating work");
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
