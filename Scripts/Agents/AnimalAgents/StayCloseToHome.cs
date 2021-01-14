using GameCreator.Characters;
using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StayCloseToHome", menuName = "AgentActions / StayCloseToHome")]
public class StayCloseToHome : GAction
{
    bool resting;
    bool restingState;
    bool sleepingState;
    float sleep = 1f;
    float increaseSleep = 0;
    AnimalAgent AnimalAgent;
    GameObject homebase;
    private void OnEnable()
    {



        preconditions.Add(AgentFact.Tired.ToString(), true);
        effects.Add(AgentTask.StaySafe.ToString(), true);


    }

    public override bool ContextPreConditions()
    {
        

        if (increaseSleep > 50)
        {
            increaseSleep = 0;
        }
        resting = false;
        restingState = false;
        sleepingState = false;
        AnimalAgent = ThisAgent as AnimalAgent;
        homebase = null;
        if (ThisAgent.AgentFacts.HasState(AgentFact.Tired.ToString(), false) || ThisAgent.AgentFacts.HasState(AgentFact.CanEat.ToString(), true) && ThisAgent.AgentFacts.HasState(AgentFact.Hungry.ToString(), true))
        {
            return false;
        }

        VariablesManager.SetLocal(ThisAgent.gameObject, "LockOn", false, true);

        if (!ThisAgent.AgentFacts.HasState(ThisAgent.DefaultAgentData.Home.ToString(), true))
        {




            ThisAgent.randomExploration = false;

            target = Gworld.Instance.GetQueue(ThisAgent.AgentData.Home.ToString()).FindResourceClosestToMe(ThisAgent.BlackBoard.Position);
            homebase = target;

            return true;




        }
        else if (ThisAgent.AgentFacts.HasState(ThisAgent.DefaultAgentData.Home.ToString(), true))
        {

            target = null;
            ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = false;
            ThisAgent.randomExploration = true;
            return true;




        }

        if (ThisAgent.AgentFacts.HasState(AgentFact.Juvenile.ToString(), true) && !ThisAgent.AgentFacts.HasState(ThisAgent.DefaultAgentData.Home.ToString(), true)
            || ThisAgent.AgentFacts.HasState(ThisAgent.DefaultAgentData.Home.ToString(), true)
            && ThisAgent.AgentFacts.HasState(ThisAgent.DefaultAgentData.Home.ToString(), true))
        {




            if (ThisAgent.BlackBoard.myMother != null && !ThisAgent.BlackBoard.myMother.BlackBoard.Agent.AgentFacts.HasState(ThisAgent.DefaultAgentData.Home.ToString(), true))
            {

                ThisAgent.randomExploration = false;
                target = null;
                return true;
            }

            if (ThisAgent.BlackBoard.myMother == null)
            {
                ThisAgent.randomExploration = false;
                target = Gworld.Instance.GetQueue(ThisAgent.AgentData.Home.ToString()).FindResourceClosestToMe(ThisAgent.BlackBoard.Position);
                return true;

            }
        }

        return false;
    }

    public override bool ActionEffect()
    {
        if (resting == true)
        {
            ThisAgent.ResetAgentState();
        }
        if (ThisAgent.AgentFacts.HasState(AgentFact.Tired.ToString(), false))
        {
            increaseSleep = 0;
        }
        return true;



    }
    public override bool InterruptActionCleanUp()
    {
        if (resting == true)
        {
            ThisAgent.ResetAgentState();
        }
        if (ThisAgent.AgentFacts.HasState(AgentFact.Tired.ToString(), false))
        {
            increaseSleep = 0;
        }
        return false;
    }
    public override void ActionRunning()
    {

        if (homebase != null)
        {
            if (ThisAgent.AgentFacts.HasState(ThisAgent.DefaultAgentData.Home.ToString(), true))
            {

                ThisAgent.CancelCurrentGoal();

            }

        }

        if (ThisAgent.AgentFacts.HasState(AgentFact.Juvenile.ToString(), true))
        {
            if (ThisAgent.AgentFacts.HasState(ThisAgent.DefaultAgentData.Home.ToString(), true))
            {

                ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = false;

                if (ThisAgent.AgentFacts.HasState(AgentFact.Tired.ToString(), true))
                {

                    increaseSleep += Time.deltaTime / 2 * AnimalAgent.metabolismGene;
                
                    if (increaseSleep > 0.5f)
                    {
                        resting = true;
                    }
                }
                if (resting == true)
                {


                    duration += 0.2f;
                    increaseSleep += Time.deltaTime / 5 * AnimalAgent.metabolismGene;

                    if (restingState == false)
                    {

                        ThisAgent.pathAi.destination = ThisAgent.transform.position;
                        ThisAgent.canMove = false;
                        ThisAgent.ChangeAgentState(ThisAgent.AgentData.RestingState);
                        restingState = true;
                    }
                    if (increaseSleep > 3f && sleepingState == false)
                    {
                        ThisAgent.ChangeAgentState(ThisAgent.AgentData.SleepingState);
                        sleepingState = true;
                    }

                }
                if (resting == false)
                {
                    ThisAgent.randomExploration = true;
                }
                if (resting == true)
                {
                    ThisAgent.randomExploration = false;
                }

                else
                {
                    if (Vector3.Distance(ThisAgent.BlackBoard.Position, ThisAgent.BlackBoard.myMother.BlackBoard.Position) > 6f)
                    {
                        ThisAgent.randomExploration = false;
                        ThisAgent.FollowTarget(ThisAgent.BlackBoard.myMother.gameObject);
                        ThisAgent.LookAtTarget(ThisAgent.BlackBoard.myMother.gameObject);
                        ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = true;
                    }
                    if (Vector3.Distance(ThisAgent.BlackBoard.Position, ThisAgent.BlackBoard.myMother.BlackBoard.Position) <= 5.8f)
                    {
                        ThisAgent.randomExploration = true;
                        ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = false;
                    }
                }
            }
        }
        else
        {
            ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = true;


            if (ThisAgent.AgentFacts.HasState(ThisAgent.DefaultAgentData.Home.ToString(), true))
            {

                ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = false;

                if (ThisAgent.AgentFacts.HasState(AgentFact.Tired.ToString(), true))
                {

                    increaseSleep += Time.deltaTime / 15 * AnimalAgent.metabolismGene;
                   
                    if (increaseSleep > 0.5f)
                    {
                        resting = true;
                    }
                }
                if (resting == true)
                {


                    duration += 0.2f;
                    increaseSleep += Time.deltaTime / 5 * AnimalAgent.metabolismGene;

                    if (restingState == false)
                    {

                        ThisAgent.pathAi.destination = ThisAgent.transform.position;
                        ThisAgent.canMove = false;
                        ThisAgent.ChangeAgentState(ThisAgent.AgentData.RestingState);
                        restingState = true;
                    }
                    if (increaseSleep > 3f && sleepingState == false)
                    {
                        ThisAgent.ChangeAgentState(ThisAgent.AgentData.SleepingState);
                        sleepingState = true;
                    }

                }
                if (resting == false)
                {
                    ThisAgent.randomExploration = true;
                }
                else
                {
                    ThisAgent.randomExploration = false;
                }

            }

        }

            if (ThisAgent.AgentFacts.HasState(AgentFact.Tired.ToString(), false) || ThisAgent.BlackBoard.myAwareness != AIAwareness.Idle || ThisAgent.AgentFacts.HasState(AgentFact.CanEat.ToString(), true) && ThisAgent.AgentFacts.HasState(AgentFact.Hungry.ToString(), true))
            {
                ThisAgent.CancelCurrentGoal();
            }
        }
    }

