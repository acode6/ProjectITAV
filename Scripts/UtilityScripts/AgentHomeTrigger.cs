using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentHomeTrigger : MonoBehaviour
{
    public float LastTimeVisited { get; set; }
    public GworldBuilding Building;
    public GAgent homeOwner;
    private void Start()
    {
        if (gameObject.GetComponent<GObjectInfo>() != null)
        {
            Building = gameObject.GetComponent<GObjectInfo>().WorldObject as GworldBuilding;
            homeOwner = Building.CurrentObjectOwner;

        }
        else
        {
            Debug.Log("Check object info " + gameObject.name);
        }

    }
    private void OnTriggerEnter(Collider Other)
    {
       
        if(homeOwner == null)
        {
            homeOwner = Building.CurrentObjectOwner;

        }
        if(homeOwner.BlackBoard != null && homeOwner.BlackBoard.AgentHome == null)
        {
            homeOwner.BlackBoard.AgentHome = Building;
        }
        if (Other.gameObject.GetComponent<GAgent>() != null && Other.gameObject.GetComponent<GAgent>() == homeOwner)
        {


            homeOwner.AgentFacts.SetState(AgentFact.AtHome.ToString(), true);



        }

    }

    private void OnTriggerExit(Collider Other)
    {
        if (homeOwner.BlackBoard != null && homeOwner.BlackBoard.AgentHome == null)
        {
            homeOwner.BlackBoard.AgentHome = Building;
        }
        if (Other.gameObject.GetComponent<GAgent>() != null && Other.gameObject.GetComponent<GAgent>() == homeOwner)
        {


            homeOwner.AgentFacts.SetState(AgentFact.AtHome.ToString(), false);



        }

    }

    private void OnTriggerStay(Collider Other)
    {

        


    }


    

}
