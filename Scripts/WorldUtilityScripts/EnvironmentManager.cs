using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    [System.Serializable]
    public struct Population
    {
        public GAgent prefab;
        public int count;

    }



    [Header("Populations")]
    public  Population[] initialPopulations;
    public static Dictionary<Species, List<Species>> preyBySpecies;
    public static Dictionary<Species, List<Species>> predatorsBySpecies;
    public static Dictionary<Species, List<Biomes>> SpeciesBiomes;
    static System.Random prng;

    public GAgent Agent;
    List<GAgent> agentInfo = new List<GAgent>();
    private void Start()
    {
        
    }
    public void Register(GAgent agent)
    {
        prng = new System.Random();
        Agent = agent;

        if (agentInfo.Count <= 0)
        {
            agentInfo.Add(Agent);
        }
        else
        {

            foreach (GAgent a in agentInfo)
            {

                if(a.species == agent.species)
                {
                    return;
                }


            }
            agentInfo.Add(agent);
        }
     

        Init();
       // SpawnInitialPopulations();

    }

    void Init()
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();

        int numSpecies = System.Enum.GetNames(typeof(Species)).Length;
        int numBiomes = System.Enum.GetNames(typeof(Biomes)).Length;
        preyBySpecies = new Dictionary<Species, List<Species>>();
        predatorsBySpecies = new Dictionary<Species, List<Species>>();
        SpeciesBiomes = new Dictionary<Species, List<Biomes>>();
        // Init species maps

        for (int i = 0; i < numSpecies; i++)
        {
            Species species = (Species)(1 << i);
          

            preyBySpecies.Add(species, new List<Species>());
            predatorsBySpecies.Add(species, new List<Species>());
        }

        for (int i = 0; i < numBiomes; i++)
        {
            Species species = (Species)(1 << i);


            SpeciesBiomes.Add(species, new List<Biomes>());
            
        }

        // Store predator/prey relationships for all species
        for (int i = 0; i < agentInfo.Count; i++)
        {


            GAgent animal = agentInfo[i];
            Species foes = animal.AgentFoes;
            Biomes agentBiomes = animal.AgentHabitatBiomes;
                for (int huntedSpeciesIndex = 0; huntedSpeciesIndex < numSpecies; huntedSpeciesIndex++)
                {
                    int bit = ((int)foes >> huntedSpeciesIndex) & 1;
                    // this bit of diet mask set (i.e. the hunter eats this species)
                    if (bit == 1)
                    {
                        int huntedSpecies = 1 << huntedSpeciesIndex;
                      
                        preyBySpecies[animal.species].Add((Species)huntedSpecies);
                        predatorsBySpecies[(Species)huntedSpecies].Add(animal.species);
                    }
                }

            for (int k = 0; k < numBiomes; k++)
            {

                int bit = ((int)agentBiomes >> k) & 1;

                if (bit == 1)
                {
                    int b = 1 << k;
                    SpeciesBiomes[animal.species].Add((Biomes)k);
                }

            }

        }

      // LogPredatorPreyRelationships ();


       // Debug.Log("Init time: " + sw.ElapsedMilliseconds);
    }


    void LogPredatorPreyRelationships()
    {
        int numSpecies = System.Enum.GetNames(typeof(Species)).Length;
        for (int i = 0; i < numSpecies; i++)
        {
            string s = "(" + System.Enum.GetNames(typeof(Species))[i] + ") ";
            int enumVal = 1 << i;
            var prey = preyBySpecies[(Species)enumVal];
            var predators = predatorsBySpecies[(Species)enumVal];

            s += "Prey: " + ((prey.Count == 0) ? "None" : "");
            for (int j = 0; j < prey.Count; j++)
            {
                s += prey[j];
                if (j != prey.Count - 1)
                {
                    s += ", ";
                }
            }

            s += " | Predators: " + ((predators.Count == 0) ? "None" : "");
            for (int j = 0; j < predators.Count; j++)
            {
                s += predators[j];
                if (j != predators.Count - 1)
                {
                    s += ", ";
                }
            }
            print(s);
        }
    }
}
