using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMark : MonoBehaviour
{
    public float LastTimeVisited { get; set; }
    public AgentFact LMarkFact;

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerStay(Collider Other)
    {

        if (Other.gameObject.GetComponent<GAgent>() != null)
        {

            GAgent agent = Other.gameObject.GetComponent<GAgent>();

            WMFact checkFact = agent.BlackBoard.AgentMemory.FindMemoryFact(CreateWMFact(LMarkFact,this.gameObject,  agent.BlackBoard.Agent));
            if (checkFact != null)
            {



                checkFact.factConfidence = 1f;


            }

            if (checkFact == null)
            {
                checkFact = CreateWMFact(LMarkFact, this.gameObject, agent.BlackBoard.Agent);
                agent.BlackBoard.AgentMemory.AddMemoryFact(checkFact);

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
