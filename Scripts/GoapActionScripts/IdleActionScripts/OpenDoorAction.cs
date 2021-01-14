using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OpenDoorAction", menuName = "AgentActions/IdleActions/OpenDoorAction")]
public class OpenDoorAction : GAction
{

    GameObject currentTarget;
    bool blocking;
    GObjectInfo ObjectInfo;
    public void OnEnable()
    {


        preconditions.Add(AgentFact.OpenDoor.ToString(), false);
        preconditions.Add(AgentFact.NeedPath.ToString(), true);

        effects.Add(AgentTask.OpenPath.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {

      //  Debug.Log("Using door Action");

        blocking = false;

        Debug.Log("Door pre");


        target = Gworld.Instance.GetQueue(AgentFact.OpenDoor.ToString()).FindResourceClosestToMe(ThisAgent.BlackBoard.Position);
      //  Debug.Log("Queue Count " + Gworld.Instance.GetQueue(AgentFact.OpenDoor.ToString()).que.Count);
        if(target == null)
        {
            Debug.Log("No Door");
            return false;  

        }
        else
        {
          //  Debug.Log("Door " + target);
          //  Debug.Log("Door Parent" + target.transform.parent);
            ObjectInfo = target.GetComponent<GObjectInfo>();
            target = ObjectInfo.WorldObject.ActionDestination;
            ObjectInfo.WorldObject.CurrentObjectOwner = ThisAgent;
        }

        ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = false;
    
     
        return true;

   
    }



    public override bool ActionEffect()
    {
        //Debug.Log("Using door Action end");
        ObjectInfo.WorldObject.CurrentObjectOwner = null;
        ThisAgent.AgentFacts.RemoveState(AgentFact.NeedPath.ToString());
        return true;
    }
    public override bool InterruptActionCleanUp()
    {
        ThisAgent.randomExploration = false;
      
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
              
               
                duration = ObjectInfo.WorldObject.ObjectUseTime;
              //  Debug.Log("Use Object");
            }




        }

       
        

    }
}
