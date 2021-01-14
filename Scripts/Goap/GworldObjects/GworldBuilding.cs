using GameCreator.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GWorldObjects", menuName = "WorldObjectBuilding")]
public class GworldBuilding : GworldObject
{

    public GameObject AttachedAgent;
    public GworldSettlement Settlement;
    public override void InitOwnerObj()
    {

     

    }


    public override void AgentObjectUseState()
    {
      
    }

    public override void AgentObjectUseAnimation()
    {
       


    }

    public override void ObjectUseLogic()
    {
        if (CurrentObjectOwner == null && AttachedAgent!= null)
        {
            GameObject owner = Instantiate(AttachedAgent, ActionDestination.transform.position, Quaternion.identity);
            CurrentObjectOwner = owner.GetComponent<GAgent>();
            
            if (Settlement.SettlementResidents.Contains(CurrentObjectOwner.gameObject) == false)
            {
                Settlement.SettlementResidents.Add(CurrentObjectOwner.gameObject);
            }
           

        }
    }

  

}
