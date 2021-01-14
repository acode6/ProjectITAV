using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCreator.Variables;
public class ChaseAction : GAction
{
    public override bool ContextPreConditions()
    {

        ThisAgent.randomExploration = false;
        target = null;

        int randint = Random.Range(1, 1000000000);

     //   VariablesManager.SetLocal(this.gameObject, "ChaseTarget", randint, true);


        return true;
    }

    public override bool ActionEffect()
    {

        
        // if(!thisAgent.beliefs.HasState("HasEnemyInSight"))
        // thisAgent.randomExploration = true;

        return true;

       

    }
    public override bool InterruptActionCleanUp()
    {
        return false;
    }
    public override void ActionRunning() { }
}
