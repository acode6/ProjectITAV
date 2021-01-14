using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FindBiomeAction", menuName = "AgentActions/FindBiomeAction")]
public class FindBiomeAction : GAction
{

    GameObject currentTarget;
    bool blocking;
    List<Biomes> Biomes;
    public void OnEnable()
    {

        preconditions.Add(AgentFact.Tired.ToString(), false);
        effects.Add(AgentFact.InSpeciesBiome.ToString(), true);

    }

    public override bool ContextPreConditions()
    {

        if (ThisAgent.AgentFacts.HasState(AgentFact.Tired.ToString(), true) || ThisAgent.AgentFacts.HasState(AgentFact.InSpeciesBiome.ToString(), true))
        {

            return false;

        }
        Biomes = EnvironmentManager.SpeciesBiomes[ThisAgent.BlackBoard.Agent.species];


        foreach (Biomes b in Biomes)
        {
            if (ThisAgent.AgentFacts.HasState(b.ToString(), true))
            {

                return true;


            }


        }
        //not at habitat
        foreach (Biomes b in Biomes)
        {

            Debug.Log("FINDING BIOME");
            target = Gworld.Instance.GetQueue(b.ToString()).FindResourceClosestToMe(ThisAgent.BlackBoard.Position);
            duration = Vector3.Distance(ThisAgent.BlackBoard.Position, target.transform.position);
            return true;



        }
        //check to find closest biome and go to it


        return false;





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
        ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = true;
        if (ThisAgent.AgentFacts.HasState(AgentFact.Tired.ToString(), true) || ThisAgent.AgentFacts.HasState(AgentFact.InSpeciesBiome.ToString(), true))
        {

            ThisAgent.CancelCurrentGoal();

        }




    }
}
