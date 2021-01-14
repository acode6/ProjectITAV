using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SqaudObservation
{
    PotentialEnemy,
    NeedHealth

}
public enum SquadIntention
{
    
    CastingUltimateSpell,
    CastingAOESpell


}



public class AISQUAD
{

    public int SquadId;
    //List of new Squad Members
    public GAgent thisAgent;
    public List<GAgent> squadMembers = new List<GAgent>();
    //World states that the squad will share
    List<WMFact> SquadFacts = new List<WMFact>();
    public void Think()
    {

        Observe();
        ProcessTeamObservations();
        ProcessTeamIntentions();
        
    }

    

    void Observe()
    {
     //   Debug.Log("Squad Facts # " + SquadFacts.Count);
        //foreach (var item in thisAgent.AgentMemory.WorkingMemoryFacts)
        //{
            
        //    if (item.Fact == AgentFact.PotentialEnemy || item.Fact == AgentFact.NeedHealth)
        //    {
        //        WMFact fact = new WMFact();
        //        Debug.Log("Item Fact " + item.Fact.ToString());
        //        fact.Fact = item.Fact;
        //        fact.factState = item.factState;
        //        fact.sourceObject = item.sourceObject;
        //        fact.factConfidence = 5f;

        //        if(SquadFacts.Count == 0)
        //        {
        //            SquadFacts.Add(fact);
        //        }
        //        else
        //        {
        //            //want to check all the facts already in the list and see if it doesnt exist
        //            if (NewSquadFactCheck(fact.Fact.ToString()) == true)
        //            {
        //                SquadFacts.Add(fact);
        //            }


        //            foreach (WMFact f in SquadFacts)
        //            {
                       
        //                //If we have the same fact but not on the same object -- new fact then add it
        //                if(fact.Fact == f.Fact && fact.sourceObject != f.sourceObject)
        //                {
        //                    SquadFacts.Add(fact);
        //                }

        //            }
        //        }
                
        //    }
        //}

    }
    //Checks sqaud facts to see if this fact matches any and if not then its a new fact
    bool NewSquadFactCheck(string fact)
    {

        for (int i = 0; i < SquadFacts.Count; i++)
        {

            if (SquadFacts[i].Fact.ToString() == fact)
            {
                return false;
            }


        }

        return true;


    }
    //UpdateSquadMembers
    void ProcessTeamObservations()
    {

        foreach (var item in thisAgent.AgentMemory.GetWMFacts())
        {

            if (item.Fact == AgentFact.PotentialEnemy || item.Fact == AgentFact.NeedHealth)
            {
                WMFact fact = new WMFact();
               // Debug.Log("Item Fact " + item.Fact.ToString());
                fact.Fact = item.Fact;
                fact.FactState = item.FactState;
                fact.SourceObject = item.SourceObject;
                fact.factConfidence = item.factConfidence;

                foreach (GAgent sMember in squadMembers)
                {

                    WMFact newFact = sMember.AgentMemory.FindMemoryFact(fact);
                    if (newFact == null)
                    {

                        sMember.AgentMemory.AddMemoryFact(fact);
                    }


                }

            }
        }

     

            
        

    }
    void ProcessTeamIntentions()
    {

    }

    GAgent FindAgent()
    {


        return null;
    }


    //Get Agents withing a set distance and add them to the list
    void GetAgentsWithinDistance()
    {



    }

}

public class SquadManager : MonoBehaviour
{
    public List<GAgent> AllAgent;
    public List<AISQUAD> CurrentAISquads;

    //Based on paramaters create a new AI squad and add the Agents that are needed to be added 
    //Squad prefab this will make it instantiate only when it's needed
    //Also should allow to attached a debugger to see each generated sqauds individual intentions and beliefs




    void Start()
    {   //Right now find tag of agents and add them to a new sqaud list 

        GameObject[] AgentsAtStart = GameObject.FindGameObjectsWithTag("Agent");

        foreach (var item in AgentsAtStart)
        {
            if (item.GetComponent<GAgent>() != null)
            {
                AllAgent.Add(item.GetComponent<GAgent>());
            }
        }

        AISQUAD testSquad = new AISQUAD();
        foreach (var agent in AllAgent)
        {

            testSquad.squadMembers.Add(agent);
            agent.currentSquad = testSquad;
        }
        //  CurrentAISquads.Add(testSquad);

        //foreach agent thats 
    }


    void DeleteAllSquads() { }



}
