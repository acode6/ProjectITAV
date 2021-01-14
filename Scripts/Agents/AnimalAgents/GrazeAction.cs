using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrazeAction : GAction
{
    LandMark bestTarget;
    GameObject closestLandMark;
    public override bool ContextPreConditions()
    {
        
    /*    target = null;
        List<GameObject> currentLandMarks = Gworld.Instance.GetQueue("landmarks").ReturnResourceQueue();
       int randomLocal = Random.Range(1, currentLandMarks.Count);
        if (thisAgent.BlackBoard.KnownLandMarks.Count == 0)
        {
           foreach(var lmark in currentLandMarks)
            {
              
                thisAgent.BlackBoard.KnownLandMarks.Add(lmark.gameObject.GetComponent<LandMark>());
            }
        }

       closestLandMark = Gworld.Instance.GetQueue("landmarks").FindResourceClosestToMe(thisAgent.BlackBoard.Position);


        if (thisAgent.AgentFacts.HasState("isHungry") && !thisAgent.AgentFacts.HasState("hasFoodInSight") )
        {

            if (!thisAgent.AgentFacts.HasState("GrazeLocation"))
            {
                thisAgent.randomExploration = false;

                if (!thisAgent.AgentFacts.HasState("GrazeLocation"))
                {
                    
                    thisAgent.randomExploration = false;

                    target = currentLandMarks[randomLocal];


                    return true;


                }



            }
            
        }

        if (thisAgent.AgentFacts.HasState("needMate") && thisAgent.BlackBoard.currentMate == null)
        {
       
            if (!thisAgent.AgentFacts.HasState("GrazeLocation"))
            {
                thisAgent.randomExploration = false;
                target = currentLandMarks[randomLocal];


                return true;
                    
                
               
            }
        }
        */
        return false;
    }

    public override bool ActionEffect()
    {
       
       
        return true;

       

    }
    public override bool InterruptActionCleanUp()
    {
        return false;
    }
    public override void ActionRunning() {
       /* if (thisAgent.AgentFacts.HasState("needMate") && thisAgent.AgentFacts.HasState("hasMateInSight"))
        {
            thisAgent.CancelCurrentGoal();

        }
        return;*/
    }
}
