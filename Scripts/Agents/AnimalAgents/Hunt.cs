using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunt : GAction
{
    public override bool ContextPreConditions()
    {

            
        return true;
    }

    public override bool ActionEffect()
    {
        

        ThisAgent.BlackBoard.Agent.randomExploration = true;
        return true;

       

    }
    public override bool InterruptActionCleanUp()
    {
        return false;
    }
    public override void ActionRunning() { }
}
