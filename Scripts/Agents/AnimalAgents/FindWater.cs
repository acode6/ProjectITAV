using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindWater : GAction
{
    bool drinking;

    public override bool ContextPreConditions()
    {
        drinking = false;

   
        target = Gworld.Instance.GetQueue("waterLocals").FindResourceClosestToMe(ThisAgent.BlackBoard.Position);
        if (target == false)
            return false;

        return true;
        

        
    }

    public override bool ActionEffect()
    {

      //  VariablesManager.SetLocal(this.gameObject, "drink", false, true);
        return true;

       

    }
    public override bool InterruptActionCleanUp()
    {
        return false;
    }
    public override void ActionRunning() {

        AnimalAgent me = ThisAgent.BlackBoard.Agent as AnimalAgent;

        if (target == null)
        {
           // VariablesManager.SetLocal(this.gameObject, "drink", false, true);
            ThisAgent.BlackBoard.Agent.CancelCurrentGoal();
            return;
        }

        else if (Vector3.Distance(ThisAgent.BlackBoard.Position, target.transform.position) < 1.2f && target != null)
        {
            if (!drinking)
            {


                drinking = true;
              //  VariablesManager.SetLocal(this.gameObject, "drink", true, true);

            }

            else if (drinking)
            {
                if (target != null)
                {


                    if (me.thirst > 0.1f)
                    {

                   
                       // float drinkAmount = Mathf.Min(me.thirst, Time.deltaTime * 1 / me.drinkDuration);
                   
                       // me.thirst -= drinkAmount;
                        if (me.thirst < 0.1f)
                        {
                           
                            ThisAgent.BlackBoard.Agent.AgentFacts.RemoveState("isThirsty");
                            ThisAgent.BlackBoard.Agent.currentAction.ActionEffect();
                            return;
                        }
                    }

                }


            }





        }

        return;



    }
}
