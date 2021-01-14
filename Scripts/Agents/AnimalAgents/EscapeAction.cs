using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCreator.Variables;
using GameCreator.Characters;
public class EscapeAction : GAction
{
    bool hiding ;
    float runspeed = 1.2f;
    public override bool ContextPreConditions()
    {
        hiding = false;
        GameObject bestHidingSpot;
        GameObject burrow;
   /*     if (thisAgent.AgentFacts.HasState("inDanger"))
        {

            target = null;
         


            bestHidingSpot = Gworld.Instance.GetQueue("hidingspots").FindResourceClosestToMe(thisAgent.BlackBoard.Position);
            burrow = Gworld.Instance.GetQueue("burrows").FindResourceClosestToMe(thisAgent.BlackBoard.Position);
            if (bestHidingSpot  && Vector3.Distance(thisAgent.BlackBoard.Position, bestHidingSpot.transform.position) < 15f)
            {
                target = bestHidingSpot;
                thisAgent.myCharacter.characterLocomotion.runSpeed = thisAgent.myCharacter.characterLocomotion.runSpeed * runspeed;
                thisAgent.randomExploration = false;
                return true;
            }

           else if (bestHidingSpot && Vector3.Distance(thisAgent.BlackBoard.Position, burrow.transform.position) < 20f)
            {
                target = burrow;
                thisAgent.myCharacter.characterLocomotion.runSpeed = thisAgent.myCharacter.characterLocomotion.runSpeed * runspeed;
                thisAgent.randomExploration = false;
                return true;
                
            }

            else 
            {
               
                thisAgent.myCharacter.characterLocomotion.runSpeed = 4;
                thisAgent.randomExploration = true;
                thisAgent.myCharacter.characterLocomotion.canRun = true;
               
                return false;
            }


         




        }

    */
        return false;

        
                   
       
    }
    public override void ActionRunning() {

        Debug.Log("HIDINGsS");
        if (Vector3.Distance(ThisAgent.BlackBoard.Position, target.transform.position) < 1.2f)
        {
            if (!hiding)
            {
                Debug.Log("HIDING");
               // thisAgent.myCharacter.characterLocomotion.runSpeed = thisAgent.BlackBoard.Agent.baseSpeed; 
             //   VariablesManager.SetLocal(this.gameObject, "hide", true, true);
                hiding = true;
               
            //    if(hiding)
          //       Invoke("HideMe" ,1f);
            }
          
                
        }

    }

    void HideMe()
    {
        ThisAgent.gameObject.SetActive(false);
    }
    void UnHideMe()
    {
        ThisAgent.gameObject.SetActive(true);
    }
    public override bool ActionEffect()
    {
     //   Invoke("UnHideMe",0f);
        ThisAgent.AgentFacts.RemoveState("inDanger");
        
     //   VariablesManager.SetLocal(this.gameObject, "hide", false, true);
        ThisAgent.randomExploration = false;
      



        return true;

       

    }

    public override bool InterruptActionCleanUp()
    {
        return false;
    }
}
