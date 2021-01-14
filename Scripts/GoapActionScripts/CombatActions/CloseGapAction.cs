using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CloseGapAction", menuName = "AgentActions/CloseAction")]
public class CloseGapAction : GAction
{

    GameObject currentTarget;
    bool blocking;

    public void OnEnable()
    {
       

      
        preconditions.Add(AgentFact.MeleeWeapon.ToString(), true);
        preconditions.Add(AgentFact.CanAttack.ToString(), true);

        effects.Add(AgentFact.InMeleeRange.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {
      
     
        blocking = false;

        if (ThisAgent.AgentFacts.HasState(AgentFact.MeleeWeapon.ToString(), true))
        {

                currentTarget = ThisAgent.BlackBoard.CurrentEnemy.gameObject;

                target = currentTarget;

                if (currentTarget == null)
                {
                Debug.Log("CLOSE GAP FALSE");
                    return false;
                }
            duration = Vector3.Distance(ThisAgent.transform.position, currentTarget.transform.position);
            return true;

            
        }

      
        
      
        return false;
    }



    public override bool ActionEffect()
    {
        
     
        ThisAgent.currentAction.running = false;
        ThisAgent.BlackBoard.Defensive = true;
        return true;
    }

    public override bool InterruptActionCleanUp()
    {
        ThisAgent.BlackBoard.Defensive = true;
        return false;
    }

    public override void ActionRunning()
    {



         ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = true;

        if (Vector3.Distance(ThisAgent.transform.position, currentTarget.transform.position) > ThisAgent.agentCombatManager.EnemySqrMeleeRange)
        {
            
         
            
            
               ThisAgent.FollowTarget(currentTarget);
            ThisAgent.BlackBoard.Defensive = false;


        }

       if (Vector3.Distance(ThisAgent.transform.position, currentTarget.transform.position) < ThisAgent.agentCombatManager.EnemySqrMeleeRange)
        {
     
            ThisAgent.BlackBoard.Defensive = true;
            Vector3 targetDir = (currentTarget.transform.position - ThisAgent.gameObject.transform.position);
            float angle = Vector3.Angle(targetDir, ThisAgent.BlackBoard.Forward);

            if (angle != 0)
            {

               
                ThisAgent.StrafeTowardTarget(ThisAgent.agentCombatManager.preyObject.transform.position);


            }
           


            //    thisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = false;
        }

        if (ThisAgent.AgentFacts.HasState(AgentFact.MeleeWeapon.ToString(), true))
        {


            if (ThisAgent.AgentFacts.HasState(AgentFact.InMeleeRange.ToString(), true) || ThisAgent.AgentFacts.HasState(AgentFact.InLungeRange.ToString(), true) && ThisAgent.AgentFacts.HasState(AgentFact.CanLungeAttack.ToString(), true))
            {

                ThisAgent.currentAction.ActionEffect();

            }

        }





    }
}
