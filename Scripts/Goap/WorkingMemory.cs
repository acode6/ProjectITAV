using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Working memory stores the AI's observations about the world 
/// </summary>



//Goals
public enum AgentTask
{
    AttackEnemy,
    AttackTarget,
    AvoidDamage,
    InvestigateStimuli,
    DrawWeapon,
    FindOpening,
    ExploreArea,
    InSpeciesBiome,
    SearchForTarget,
    PickUpWeapon,
    StaySafe,
    FindMate,
    Breed,
    SatisfyNeed,
    OpenPath,
    Work,
    Eat,
    Rest,
    Harvest,
    UseGrindStone,
    GetShot,
    GoToTarget
}









public class WMFact
{


    public AgentFact Fact { get; set; } 
    public string StringFact { get; set; } //incase the fact is off another enum --placeholder--
    public GameObject SourceObject { get; set; } //object attached to fact
    Vector3 factPosition;
    Vector3 factDirection;
    public bool FactState { get; set; } //state of the fact
    float factTime { get; set; } //game time the fact was set 
    public float factConfidence { get; set; } //agents confidence in fact

    //Working memory default constructor
    public WMFact()
    {
        //default fact for now
        Fact = AgentFact.Fact_InvalidType;
        StringFact = null;
        SourceObject = null;
        factTime = 0;
        factConfidence = 0f;
    }


    public WMFact(AgentFact NewFact, GameObject Source)
    {
        Fact = NewFact;
        StringFact = null;
        SourceObject = Source;
        factPosition = SourceObject.transform.position;
        factTime = 0;
        factConfidence = 0f;
    }









}

public class WorkingMemory
{
    public WorldStates AgentFacts { get;} 
    public AIBlackBoard AgentsBlackBoard {get;}
    //List of current/ old facts 
    List<WMFact> CurrentWorkingMemoryFacts = new List<WMFact>();
    public List<WMFact> OldWorkingMemoryFacts = new List<WMFact>();
    //current stimuli for agents to investigate
    public List<GameObject> CurrentStimuli = new List<GameObject>();
    //stores evaulated threats for later use
    public List<GameObject> EvaluatedThreats = new List<GameObject>();
    public List<GameObject> FoodSources = new List<GameObject>();//for animal agents
    public AISQUAD currentSquad = null;

    //constructor
    public WorkingMemory(WorldStates agentfacts, AIBlackBoard blackBoard)
    {
        AgentFacts = agentfacts;
        AgentsBlackBoard = blackBoard;
    }

    //Updates the agents facts
    public void UpdateWorkingMemory()
    {
        //if (AgentsBlackBoard.CurrentSquad != null)
        //{
        //    currentSquad = AgentsBlackBoard.CurrentSquad;
        //}

        //stores fact that need to be deleted
        List<WMFact> DeleteFact = new List<WMFact>(); 

        //iterates throught working memory facts and decreases the confidence attached to fact 
        foreach (WMFact fact in CurrentWorkingMemoryFacts)
        {


            fact.factConfidence -= AgentsBlackBoard.DeltaTime /2f; //check this

            //Updates the fact for the agent
            if (fact.StringFact == null && fact.Fact != AgentFact.Fact_InvalidType)
            {
                AgentFacts.SetState(fact.Fact.ToString(), fact.FactState);
            }
            else
            {
                AgentFacts.SetState(fact.StringFact, fact.FactState);
            }


            //Marks fact for deletion if confidence is too low
            if (fact.factConfidence <= 0)
            {
                DeleteFact.Add(fact);
            }

        }

        //Deletes working memory fact and logs it to old facts
        if (DeleteFact.Count > 0)
        {
            foreach (WMFact fact in DeleteFact)
            {
                OldWorkingMemoryFacts.Add(fact);
                CurrentWorkingMemoryFacts.Remove(fact);
                AgentFacts.RemoveState(fact.Fact.ToString());

            }
        }

        //updates stimuli the agents need to look out for
        AssignCurrentStimuli();

        //Updates current enemy fact when not null
        if (AgentsBlackBoard.CurrentEnemy != null)
        {
            AgentsBlackBoard.Agent.currentEnemy = AgentsBlackBoard.CurrentEnemy;
            WMFact check = AgentsBlackBoard.AgentMemory.FindMemoryFact(CreateWMFact(AgentFact.currentEnemy, false, AgentsBlackBoard.CurrentEnemy.gameObject, AgentsBlackBoard.Agent));
            if (check != null)
            {



                check.factConfidence = 1f;


            }

            if (check == null)
            {
                check = CreateWMFact(AgentFact.currentEnemy, true, AgentsBlackBoard.CurrentEnemy.gameObject, AgentsBlackBoard.Agent);
                AgentsBlackBoard.AgentMemory.AddMemoryFact(check);

            }

        }







    }
    public void AssignCurrentStimuli()
    {


        CurrentStimuli.Clear();
        foreach (WMFact fact in CurrentWorkingMemoryFacts)
        {

            if (fact.Fact == AgentFact.PotentialEnemy && !EvaluatedThreats.Contains(fact.SourceObject) ||
                fact.Fact == AgentFact.PotentialPrey && !EvaluatedThreats.Contains(fact.SourceObject) ||
                fact.Fact == AgentFact.DamageTaken && !EvaluatedThreats.Contains(fact.SourceObject) || fact.Fact == AgentFact.LostTarget && !EvaluatedThreats.Contains(fact.SourceObject)
                || fact.Fact == AgentFact.TouchedStimulus && !EvaluatedThreats.Contains(fact.SourceObject))
            {

                WMFact check = AgentsBlackBoard.AgentMemory.FindMemoryFact(CreateWMFact(AgentFact.StimuliInvestigated, false, fact.SourceObject, AgentsBlackBoard.Agent));
                if (check != null)
                {


                    if (AgentsBlackBoard.myAwareness == AIAwareness.Searching)
                    {
                        check.factConfidence = 1f;
                    }




                    if (check.FactState == true)
                    {

                        if (!EvaluatedThreats.Contains(fact.SourceObject))
                        {

                            //  EvaluatedThreats.Add(fact.sourceObject);
                        }
                    }
                    else
                    {

                        if (AgentsBlackBoard.myAwareness == AIAwareness.Idle)
                        {
                            CurrentStimuli.Add(check.SourceObject);
                            AgentsBlackBoard.myAwareness = AIAwareness.Searching;
                            AgentsBlackBoard.Agent.UpdateAwareness();
                        }
                        else
                        {
                            if (!CurrentStimuli.Contains(check.SourceObject))
                            {
                                CurrentStimuli.Add(check.SourceObject);
                            }

                        }

                    }

                }

                if (check == null)
                {
                    if (AgentsBlackBoard.myAwareness != AIAwareness.Combat)
                    {
                        check = CreateWMFact(AgentFact.StimuliInvestigated, false, fact.SourceObject, AgentsBlackBoard.Agent);
                        AgentsBlackBoard.AgentMemory.AddMemoryFact(check);
                        CurrentStimuli.Add(check.SourceObject);

                    }

                }



            }


        }
        if (CurrentStimuli.Count == 0)
        {

            if (AgentsBlackBoard.myAwareness != AIAwareness.Combat && AgentsBlackBoard.myAwareness != AIAwareness.Fleeing)
            {

                AgentsBlackBoard.myAwareness = AIAwareness.Idle;
                AgentsBlackBoard.Agent.UpdateAwareness();
            }
        }

        AgentsBlackBoard.CurrentStimuli.Clear();
        for (int i = 0; i < CurrentStimuli.Count; i++)
        {
            AgentsBlackBoard.CurrentStimuli.Add(CurrentStimuli[i]);
        }


    }



    //Add fact to current working memory
    public void AddMemoryFact(WMFact fact)
    {
        if (!CurrentWorkingMemoryFacts.Contains(fact))
        {

            CurrentWorkingMemoryFacts.Add(fact);

        }



    }

    //Delete WMFact
    public void DeleteMemoryFact(WMFact fact)
    {

        //not needed but just in case
        if (CurrentWorkingMemoryFacts.Count == 0)
        {
            return;
        }

        if (CurrentWorkingMemoryFacts.Count > 0)
        {
            if (CurrentWorkingMemoryFacts.Contains(fact))
            {

                CurrentWorkingMemoryFacts.Remove(fact);
                AgentFacts.RemoveState(fact.Fact.ToString());

            }
        }
    }
    //Create a new WMFact
    WMFact CreateWMFact(AgentFact factToAdd, bool factState, GameObject mobileAgent, GAgent agent)
    {

        WMFact Fact = new WMFact();
        Fact.Fact = factToAdd;
        Fact.FactState = factState;
        Fact.SourceObject = mobileAgent;
        Fact.factConfidence = 1f;


        return Fact;


    }

    //Queries Current wmfacts to find a specific fact
    public WMFact FindMemoryFact(WMFact fact)
    {
        WMFact foundFact;
        foreach (WMFact f in CurrentWorkingMemoryFacts)
        {
            if (f.StringFact == null)
            {
                if (f.Fact == fact.Fact && f.SourceObject == fact.SourceObject)
                {
                    foundFact = f;
                    //  Debug.Log("Fact match found");
                    return foundFact;
                }
            }
            else
            {
                if (f.StringFact == fact.StringFact && f.SourceObject == fact.SourceObject)
                {
                    foundFact = f;
                    //      Debug.Log("string Fact match found");
                    return foundFact;
                }
            }


        }

        return null;
    }
    //Queries Current wmfacts to find a specific fact along with a state
    public WMFact FindMemoryFact(WMFact fact, bool State)
    {
        WMFact foundFact;
        foreach (WMFact f in CurrentWorkingMemoryFacts)
        {

            if (f.Fact == fact.Fact && f.SourceObject == fact.SourceObject && f.FactState == State)
            {
                foundFact = f;
                //  Debug.Log("Fact match found");
                return foundFact;
            }



        }

        return null;
    }

    //Finds a fact on a game object
    bool FindMemoryFactonObject(AgentFact FacttoFind, GameObject obj)
    {

        foreach (WMFact f in CurrentWorkingMemoryFacts)
        {

            if (f.Fact == FacttoFind && f.SourceObject == obj)
            {

                Debug.Log("Fact match found");
                return true;
            }



        }

        return false;
    }


  //Find Closest stimulus to address
    public GameObject FindClosestStimuli()
    {

        GameObject closestThreat = null;
        float closetResource = Mathf.Infinity;
        foreach (GameObject threat in CurrentStimuli)
        {


            Vector3 directionToTarget = threat.transform.position - AgentsBlackBoard.Position;
            float disSqrToTarget = directionToTarget.sqrMagnitude;
            if (disSqrToTarget < closetResource)
            {
                closetResource = disSqrToTarget;

                closestThreat = threat;

            }



        }
        return closestThreat;
    }

    //For animal agents
    public GameObject FindClosestFoodSource()
    {





        GameObject ClosestFood = null;
        float closetResource = Mathf.Infinity;
        foreach (GameObject Food in FoodSources)
        {


            Vector3 directionToTarget = Food.transform.position - AgentsBlackBoard.Position;
            float disSqrToTarget = directionToTarget.sqrMagnitude;
            if (disSqrToTarget < closetResource)
            {
                closetResource = disSqrToTarget;

                ClosestFood = Food;

            }



        }
        Debug.Log("Returning Food SOurce" + ClosestFood);
        return ClosestFood;
    }

    //Find the closest object that matches the given fact
    public GameObject FindClosestObjectMatchingFact(AgentFact factToFind, bool state)
    {

        GameObject matchingObject = null;
        float closetResource = Mathf.Infinity;

        foreach (WMFact fact in CurrentWorkingMemoryFacts)
        {

            if (fact.Fact == factToFind && fact.FactState == state)
            {
                Vector3 directionToTarget = fact.SourceObject.transform.position - AgentsBlackBoard.Position;
                float disSqrToTarget = directionToTarget.sqrMagnitude;
                if (disSqrToTarget < closetResource)
                {
                    closetResource = disSqrToTarget;

                    matchingObject = fact.SourceObject;

                }

            }

        }



        return matchingObject;



    }
 

    //Find the closet weapon
    public GameObject FindClosestWeapon()
    {

        GameObject closestWeapon = null;
        float closetResource = Mathf.Infinity;
        foreach (WMFact fact in CurrentWorkingMemoryFacts)
        {

            if (fact.Fact == AgentFact.WeaponObject)
            {
                Vector3 directionToTarget = fact.SourceObject.transform.position - AgentsBlackBoard.Position;
                float disSqrToTarget = directionToTarget.sqrMagnitude;
                if (disSqrToTarget < closetResource)
                {
                    closetResource = disSqrToTarget;

                    closestWeapon = fact.SourceObject;

                }

            }

        }
        return closestWeapon;
    }

    //Find the closest potential enemy
    public GameObject FindClosestEnemy()
    {

        GameObject closestEnemy = null;
        float closetResource = Mathf.Infinity;
        foreach (WMFact fact in CurrentWorkingMemoryFacts)
        {

            if (fact.Fact == AgentFact.PotentialEnemy)
            {
                Vector3 directionToTarget = fact.SourceObject.transform.position - AgentsBlackBoard.Position;
                float disSqrToTarget = directionToTarget.sqrMagnitude;
                if (disSqrToTarget < closetResource)
                {
                    closetResource = disSqrToTarget;

                    closestEnemy = fact.SourceObject;

                }

            }

        }
        return closestEnemy;
    }

    //Find the closest potential enemy
    public GameObject FindClosestPrey()
    {

        GameObject closestPrey = null;
        float closetResource = Mathf.Infinity;
        foreach (WMFact fact in CurrentWorkingMemoryFacts)
        {

            if (fact.Fact == AgentFact.PotentialPrey)
            {
                Vector3 directionToTarget = fact.SourceObject.transform.position - AgentsBlackBoard.Position;
                float disSqrToTarget = directionToTarget.sqrMagnitude;
                if (disSqrToTarget < closetResource)
                {
                    closetResource = disSqrToTarget;

                    closestPrey = fact.SourceObject;

                }

            }

        }
        return closestPrey;
    }

    //Returns wmlist of current facts
    public List<WMFact> GetWMFacts()
    {

        return CurrentWorkingMemoryFacts;

    }
}
