using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RepositionAction", menuName = "AgentActions/RepositionAction")]
public class DefensiveRepositionAction : GAction
{

    GameObject currentTarget = null;
    HumanoidAgent hAgent;
    public void OnEnable()
    {
        hAgent = ThisAgent as HumanoidAgent;

        preconditions.Add(AgentFact.MeleeWeapon.ToString(), true);
        preconditions.Add(AgentFact.Repositioning.ToString(), true);
        

        effects.Add(AgentTask.AvoidDamage.ToString(), true);


    }

    public override bool ContextPreConditions()
    {

        if (ThisAgent.MeleeComponent.IsStaggered == true || ThisAgent.agentCombatManager.AgentConfidence > 0f)
        {
            return false;
        }


        currentTarget = ThisAgent.agentCombatManager.preyObject;

        if (currentTarget == null)
        {
            return false;
        }

        target = null;



        return true;




    }



    public override bool ActionEffect()
    {



        ThisAgent.BlackBoard.Defensive = false;


        return true;
    }
    public override bool InterruptActionCleanUp()
    {
        ThisAgent.MeleeComponent.StopBlocking();
        return false;
    }

    public override void ActionRunning()
    {


        if (currentTarget == null)
        {
            ThisAgent.CancelCurrentGoal();
        }

        if (currentTarget != null)
        {


            if (Vector3.Distance(ThisAgent.transform.position, ThisAgent.agentCombatManager.preyObject.transform.position) > ThisAgent.agentCombatManager.EnemySqrMeleeRange * 1.5)
            {
                ThisAgent.FollowTarget(ThisAgent.agentCombatManager.preyObject);
            }
            if (Vector3.Distance(ThisAgent.transform.position, ThisAgent.agentCombatManager.preyObject.transform.position) < 2)
            {
                if (ThisAgent.AgentData.CanLockOn == true)
                {

                    ThisAgent.LookAtTarget(ThisAgent.agentCombatManager.preyObject);
                }
                ThisAgent.FleeFromTarget(ThisAgent.agentCombatManager.preyObject);

            }
            if (Vector3.Distance(ThisAgent.transform.position, currentTarget.transform.position) < ThisAgent.agentCombatManager.EnemySqrMeleeRange)
            {
                if (ThisAgent.AgentData.CanLockOn == true)
                {

                    ThisAgent.LookAtTarget(ThisAgent.agentCombatManager.preyObject);
                }
                ThisAgent.BlackBoard.Defensive = true;
                Vector3 targetDir = (currentTarget.transform.position - ThisAgent.gameObject.transform.position);
                float angle = Vector3.Angle(targetDir, ThisAgent.BlackBoard.Forward);

                if (angle != 0)
                {


                    ThisAgent.StrafeTowardTarget(ThisAgent.agentCombatManager.preyObject.transform.position);


                }



                //    thisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = false;
            }



        }










        if (ThisAgent.agentCombatManager.AgentConfidence < 0f && ThisAgent.agentCombatManager.BlockingFact.FactState == false)
        {
            duration = 1f;
        }
        if (ThisAgent.agentCombatManager.AgentConfidence < 0f && ThisAgent.agentCombatManager.InEnemyMeleeRange.FactState == false)
        {
            if (ThisAgent.agentCombatManager.AgentConfidence < 0.1)
            {
                ThisAgent.agentCombatManager.AgentConfidence  += Time.deltaTime * ThisAgent.AgentData.AgentConfidenceIncreaseRate;
            }
        }
        else
        {
            ThisAgent.CancelCurrentGoal();
        }

    }










}
