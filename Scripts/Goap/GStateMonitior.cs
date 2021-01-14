//Keeps track of a state 
using GameCreator.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GStateMonitior : MonoBehaviour
{
    public string state;
    public float statesStrength;
    public float stateDecayRate;
    public WorldStates beliefs;
    public GameObject resourcePrefab;
    public string worldState;
    public GAction action; // action that satisfies the state

    bool stateFound = false;
    float intialStrength;
    GAgent thisAgent;
  
    EnvironmentManager eManager;
    void Awake()
    {

        thisAgent = this.GetComponent<GAgent>();
      
        beliefs = this.GetComponent<GAgent>().AgentFacts;
        intialStrength = statesStrength;
        GameObject eman = GameObject.FindGameObjectWithTag("EManager");
        eManager = eman.GetComponent<EnvironmentManager>();

    }

   
    void LateUpdate()
    {

       


        if (action.running)
        {
            stateFound = false;
            statesStrength = intialStrength;
        }
     

      
        if (!stateFound && beliefs.HasState(state,true))
            stateFound = true;

        if (stateFound)
        {
           // Debug.Log("state found");
            statesStrength -= stateDecayRate * Time.deltaTime;
            if(statesStrength <= 0)
            {
                
                
                GameObject p = Instantiate(resourcePrefab);
                CharacterAnimatorEvents oldComponent = p.GetComponentInChildren<CharacterAnimatorEvents>();
                CharacterAttachments oldcompent2 = p.GetComponentInChildren<CharacterAttachments>();
               Destroy(oldComponent);
                Destroy(oldcompent2);

             //   p.transform.localScale = Vector3.one * .5f;

             ///   p.GetComponent<AnimalAgent>().offSpring = true;
               
             //   p.GetComponent<AnimalAgent>().genes=Genes.InheritedGenes(thisAnimal.genes, thisAnimal.genes);
          
        
               // p.GetComponent<AnimalAgent>().Init(p.GetComponent<GAgent>().BlackBoard);
            
     




                stateFound = false;
                statesStrength = intialStrength;
                beliefs.RemoveState(state);
                //adding resource into the world 
              
                   
            }
        }
    }
}
