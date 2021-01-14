using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biome : MonoBehaviour
{
    public float LastTimeVisited { get; set; }
    public AgentFact BiomeFact;
    List<Biomes> SpeciesBiomes;


    private void OnTriggerStay(Collider Other)
    {

        if (Other.gameObject.GetComponent<GAgent>() != null)
        {

            GAgent agent = Other.gameObject.GetComponent<GAgent>();
            SpeciesBiomes = EnvironmentManager.SpeciesBiomes[agent.BlackBoard.Agent.species];
            WMFact checkFact = agent.BlackBoard.AgentMemory.FindMemoryFact(CreateWMFact(BiomeFact,this.gameObject,  agent.BlackBoard.Agent));

            if (checkFact != null)
            {



                checkFact.factConfidence = 1f;


            }

            if (checkFact == null)
            {
                checkFact = CreateWMFact(BiomeFact, this.gameObject, agent.BlackBoard.Agent);
                agent.BlackBoard.AgentMemory.AddMemoryFact(checkFact);

            }
            foreach (Biomes b in SpeciesBiomes)
            {
                string placeholder = "";
                switch (b.ToString())
                {
                    case "1":
                        placeholder = "ForestBiome";
                        break;
                    default:
                        break;
                }
                if (BiomeFact.ToString().Equals(placeholder) )
                {

                    WMFact InSpeciesBiomeFact = agent.BlackBoard.AgentMemory.FindMemoryFact(CreateWMFact(AgentFact.InSpeciesBiome, this.gameObject, agent.BlackBoard.Agent));

                    if (InSpeciesBiomeFact != null)
                    {



                        InSpeciesBiomeFact.factConfidence = 1f;


                    }

                    if (InSpeciesBiomeFact == null)
                    {
                        InSpeciesBiomeFact = CreateWMFact(AgentFact.InSpeciesBiome, this.gameObject, agent.BlackBoard.Agent);
                        agent.BlackBoard.AgentMemory.AddMemoryFact(InSpeciesBiomeFact);

                    }


                }


            }
         




        }



    }


    WMFact CreateWMFact(AgentFact factToAdd ,GameObject Object, GAgent agent)
    {
       

        WMFact LMFact = new WMFact();
        LMFact.Fact = factToAdd;
        LMFact.FactState = true;
        LMFact.SourceObject = Object;
        LMFact.factConfidence = .5f;
      

        return LMFact;


    }

}
