using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpWeapon : GAction
{
    public GameObject WeaponTarget;
    bool pickingUp;
    private void Start()
    {
        preconditions.Add(AIWorldState.Armed.ToString(), false);
        preconditions.Add(AgentFact.WeaponObject.ToString(), true);
        effects.Add(AgentTask.PickUpWeapon.ToString(), true);
    }

    public override bool ContextPreConditions()
    {

        pickingUp = false;
        WeaponTarget = ThisAgent.BlackBoard.AgentMemory.FindClosestWeapon();
   
        target = WeaponTarget;
      //  VariablesManager.SetLocal(this.gameObject, "LockOn", false, true);
        if (target == null)
        {
            Debug.Log("No weapon target");
            return false;
        }

        ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = true;
        return true;
    }

   

    public override bool ActionEffect()
    {
        HumanoidAgent humanoid = ThisAgent.BlackBoard.Agent as HumanoidAgent;
        GameObject add = new GameObject();
        add = WeaponTarget;
        ThisAgent.BlackBoard.AgentInventory.AddItem(add);

        humanoid.UpdateWeapon();     
        ThisAgent.currentAction.running = false;
      //  VariablesManager.SetLocal(this.gameObject, "PickUp", false, true);
        return true;
    }
    public override bool InterruptActionCleanUp()
    {
        return false;
    }

    public override void ActionRunning() {

        if(WeaponTarget == null)
        {
            ThisAgent.CancelCurrentGoal();
        }
        if(pickingUp == false)
        {
            if (Vector3.Distance(ThisAgent.transform.position, target.transform.position) < 2.5f)
            {
                pickingUp = true;

                if (pickingUp == true)
                {

             //       VariablesManager.SetLocal(this.gameObject, "PickUp", true, true);
                  
                }
            }





        }
       

      
      



    }
}
