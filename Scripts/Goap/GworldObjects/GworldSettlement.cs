using GameCreator.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SensorToolkit;


public abstract class GworldSettlement : MonoBehaviour
{
    bool settleLightsOn = false;
    public List<GameObject> SettlementObjectsAtStart;
    public List<GameObject> SettlementBuildings;
    public List<GameObject> SettlementResidents;
    
   public Dictionary<string, List<GameObject> > SettlementResources = new Dictionary<string, List<GameObject>>();
    public TriggerSensor settlementTriggerSensor;

    bool updateRes = false;
    private void Awake()
    {
        settlementTriggerSensor = transform.Find("SettlementSensor").GetComponent<TriggerSensor>();
    }
 

    IEnumerator UpdateResidents()
    {

        while (updateRes == true)
        {
           yield return new WaitForSeconds(1f);
          UpdateResidentKnowledge();
         
        }
          
        
    }
    public void RegisterGworldObject()
    {

        for (int i = 0; i < settlementTriggerSensor.DetectedObjects.Count; i++)
        {


            if (settlementTriggerSensor.DetectedObjects[i].GetComponent<GObjectInfo>() != null)
            {
                if (SettlementObjectsAtStart.Contains(settlementTriggerSensor.DetectedObjects[i].gameObject) == false)
                {
                    SettlementObjectsAtStart.Add(settlementTriggerSensor.DetectedObjects[i].gameObject);
                    
                }
               
            }
            //We'll use this to agents that visit the village

            //if (settlementTriggerSensor.DetectedObjects[i].GetComponent<GAgent>() != null)
            //{
            //    if (SettlementResidents.Contains(settlementTriggerSensor.DetectedObjects[i]) == false)
            //    {
            //        SettlementResidents.Add(settlementTriggerSensor.DetectedObjects[i]);
            //    }

            //}
          
            
        }
        foreach(GameObject building in SettlementObjectsAtStart)
        {
            GObjectInfo b = building.GetComponent<GObjectInfo>();
            if (b.WorldObject.WObject == WorldObject.Building)
            {
                GworldBuilding build = b.WorldObject as GworldBuilding;
                build.Settlement = this;
                if(SettlementBuildings.Contains(building) == false)
                {
                 SettlementBuildings.Add(building);
                }
                
                building.GetComponent<GObjectInfo>().UseObjectLogic();
            }
        }
        updateRes = true;
      //  StartCoroutine("UpdateResidents");
       

   }

    //TODO: Fix dup WMFact
    public void UpdateResidentKnowledge()
    {

        foreach (var item in SettlementResidents)
        {


            GAgent Agent = item.GetComponent<GAgent>();
          
            foreach(GameObject fact in SettlementObjectsAtStart)
            {
                GObjectInfo currentObjInfo = fact.GetComponent<GObjectInfo>();
                WMFact WorldObjFact = Agent.BlackBoard.AgentMemory.FindMemoryFact(AddAgentFact(currentObjInfo.WorldObject.WObject.ToString(), currentObjInfo.gameObject, Agent, currentObjInfo.WorldObject.ObjectFactState));
                if (WorldObjFact != null)
                {

                    if (WorldObjFact.StringFact == currentObjInfo.WorldObject.WObject.ToString())
                    {
                        WorldObjFact.factConfidence = 1f;

                    }
                    else
                    {
                        WorldObjFact.factConfidence = 0f;
                    }


                  //  WorldObjFact.factState = WorldObj.ObjectFactState;


                }

                if (WorldObjFact == null)
                {
                    WorldObjFact = AddAgentFact(currentObjInfo.WorldObject.WObject.ToString(), currentObjInfo.gameObject, Agent, currentObjInfo.WorldObject.ObjectFactState);
                    Agent.BlackBoard.AgentMemory.AddMemoryFact(WorldObjFact);

                }
               

            }
            if (Agent.BlackBoard.AgentSettlement == null)
            {
                Agent.BlackBoard.AgentSettlement = this;
            }

        }



    }

    public void UpdateSettlement()
    { 


        ToggleVillageLights(true);
    }

    public void ToggleVillageLights(bool LightsOn)
    {
        this.settleLightsOn = LightsOn;

        
        // Toggle Village Lights on/off
        if (LightsOn)
        {
            GetComponent<Animation>().Play("Open");
        }
        else
        {
            GetComponent<Animation>().Play("Close");
        }
    }

    public WMFact AddAgentFact(string fact, GameObject FactObject, GAgent agent, bool state)
{

    WMFact statFact = new WMFact();
    
     statFact.StringFact = fact;
     statFact.FactState = state;
    statFact.SourceObject = FactObject;
    statFact.factConfidence = 1f;


    return statFact;


}

}
