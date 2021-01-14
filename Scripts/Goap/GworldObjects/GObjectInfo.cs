using GameCreator.Characters;
using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GObjectInfo : MonoBehaviour
{
    public GworldObject WorldObject;


    private void Awake()
    {
   
        //create a new instance of this object on start so the data is not modified across all objects
        if(WorldObject != null)
        {

            WorldObject.OwnerObj = this;

            WorldObject.ActionDestination = transform.Find("Destination").gameObject;

            WorldObject.InitOwnerObj();

            //Debug.Log(transform.name + " WorldObject.ActionDestination " + WorldObject.ActionDestination);
            if(WorldObject.ActionDestination == null)
            {
                Debug.Log("Problem destination" + gameObject.name);
            }
            if (WorldObject.ObjectPrimaryFact != AgentFact.Fact_InvalidType)
            {
                if (Gworld.Instance.GetWorldResources().ContainsKey(WorldObject.ObjectPrimaryFact.ToString()))
                {
                    // Debug.Log("QUEUE IN WORLD" + gameObject.name);
                    Gworld.Instance.GetQueue(WorldObject.ObjectPrimaryFact.ToString()).AddResource(WorldObject.ActionDestination);
                }

                else
                {

                    // Debug.Log("QUEUE Not IN WORLD" + gameObject.name);

                    ResourceQueue newResource = new ResourceQueue("", WorldObject.ObjectPrimaryFact.ToString(), Gworld.Instance.GetWorld());

                    newResource.que.Add(gameObject);
                    Gworld.Instance.GetWorldResources().Add(WorldObject.ObjectPrimaryFact.ToString(), newResource);
                }
            }
            else
            {

                if (Gworld.Instance.GetWorldResources().ContainsKey(WorldObject.WObject.ToString()))
                {
                   //  Debug.Log("QUEUE IN WORLD" + gameObject.name);
                    Gworld.Instance.GetQueue(WorldObject.WObject.ToString()).AddResource(WorldObject.ActionDestination);
                }

                else
                {

                    // Debug.Log("QUEUE Not IN WORLD" + gameObject.name);

                    ResourceQueue newResource = new ResourceQueue("", WorldObject.WObject.ToString(), Gworld.Instance.GetWorld());

                    newResource.que.Add(gameObject);
                    Gworld.Instance.GetWorldResources().Add(WorldObject.WObject.ToString(), newResource);
                }



            }
        }
        else
        {
            Debug.Log("Check World Object " + gameObject.name);
        }
    
    }
    public void SwitchObjectFactState()
    {

        if (WorldObject.ObjectFactState == true)
        {
            WorldObject.ObjectFactState = false;
        }

        else
        {
            WorldObject.ObjectFactState = true;
        }


    }
    public void ModifyObjectFact()
    {

     //   GworldObject Holder = WorldObject;
     //   WorldObject = SecondaryObject;
     //   SecondaryObject = Holder;

      

    }
    public void UseObjectLogic()
    {

        WorldObject.ObjectUseLogic();
    }
    
}
