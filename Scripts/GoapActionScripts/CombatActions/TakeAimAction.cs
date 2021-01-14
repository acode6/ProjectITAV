using GameCreator.Core;
using GameCreator.Shooter;
using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameCreator.Core.TargetCharacter;

[CreateAssetMenu(fileName = "TakeAimAction", menuName = "AgentActions/TakeAimAction")]
public class TakeAimAction : GAction
{

    GameObject currentTarget;

    private void OnEnable()
    {
     
        preconditions.Add(AgentFact.CanAttack.ToString(), true);     
        preconditions.Add(AgentFact.RangeWeapon.ToString(), true);         
        effects.Add(AgentTask.GetShot.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {


        target = null;
       

        return true;
    }



    public override bool ActionEffect()
    {

     
      
        
      

     
        return true;
    }
    public override bool InterruptActionCleanUp()
    {
        return false;
    }

    public override void ActionRunning()
    {

        //duration of this will depend on if you can shoot or not so if you can't hold aim longer
        //distance might factor into this too 
        //should start charging shot here too

       
          
        if (ThisAgent.ShooterComponent.isChargingShot == false)
        {
            ThisAgent.ShooterComponent.StartChargedShot();
        }









    }
}
