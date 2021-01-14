using GameCreator.Variables;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCreator.Melee;
using GameCreator.Shooter;
using GameCreator.Core;




public class HumanoidAgent : GAgent
{



    public override void Init()
    {


        InitStartWeapon();
        UpdateAgentGoals();


    }


    public override void UpdateAgentNeed()
    {
        //should increase constantly during the day --maybe stop it when agent is sleeping 
        if (HungerNeed >= 0)
        {
            HungerNeed += Time.deltaTime * AgentData.HungerNeedIncrease;
            //hungryfact?
        }
        if (HungerNeed < 0)
        {
            HungerNeed = 0;
        }



        //should stop increasing work need when agent should be resting
        if (WorkNeed >= 0)
        {
            switch (timeKeeper.currentTOD)
            {
              
                case TimeOfDay.Morning:
                    switch (BlackBoard.AgentStates.working)
                    {
                        case true:
                            if (WorkNeed > 0)
                            {
                                WorkNeed -= Time.deltaTime * (AgentData.WorkNeedIncrease/3);
                            }
                            else
                            {
                                WorkNeed = 0;
                            }

                            break;
                        case false:
                            WorkNeed += Time.deltaTime * AgentData.WorkNeedIncrease;
                            break;
                        default:
                            break;
                    }
                    break;
                case TimeOfDay.Afternoon:
                    switch (BlackBoard.AgentStates.working)
                    {
                        case true:
                            if (WorkNeed > 0)
                            {
                                WorkNeed -= Time.deltaTime * (AgentData.WorkNeedIncrease / 3);
                            }
                            else
                            {
                                WorkNeed = 0;
                            }

                            break;
                        case false:
                            WorkNeed += Time.deltaTime * AgentData.WorkNeedIncrease;
                            break;
                        default:
                            break;
                    }
                    break;
                case TimeOfDay.Evening:
                    switch (BlackBoard.AgentStates.working)
                    {
                        case true:
                            if (WorkNeed > 0)
                            {
                                WorkNeed -= Time.deltaTime * AgentData.WorkNeedIncrease;
                            }
                            else
                            {
                                WorkNeed = 0;
                            }

                            break;
                        case false:
                       
                            break;
                        default:
                            break;
                    }
                    break;
                case TimeOfDay.Night:
                    if (WorkNeed > 0)
                    {
                        WorkNeed -= Time.deltaTime * AgentData.WorkNeedIncrease;
                    }
                   
                    break;
                default:
                    break;
            }



        

            if (WorkNeed > 0.1)
            {
                AgentFacts.SetState(AgentNeeds.WorkNeed.ToString(), true);
            }

            if (WorkNeed < 0.1)
            {
            
                if (AgentFacts.HasState(AgentNeeds.WorkNeed.ToString(), true))
                {
                    AgentFacts.RemoveState(WorkNeed.ToString());
                }
            }

        }
        else
        {
            WorkNeed = 0;
        }
       

        //Should increase when working or at night when its time to sleep
        if (RestNeed >= 0)
        {
            
            switch (BlackBoard.AgentStates.resting)
            {
                case true:
                   
                    if(RestNeed > 0)
                    {
                        RestNeed -= Time.deltaTime * AgentData.RestNeedIncrease;
                    }
                   
                    break;
                case false:
                    
                    switch (timeKeeper.currentTOD)
                    {

                        case TimeOfDay.Morning:
                            switch (BlackBoard.AgentStates.working)
                            {
                                case true:
                                    RestNeed += Time.deltaTime * (AgentData.RestNeedIncrease / 3);
                                    break;
                                case false:

                                    break;
                                default:
                                    break;
                            }
                            break;
                        case TimeOfDay.Afternoon:
                            switch (BlackBoard.AgentStates.working)
                            {
                                case true:
                                    RestNeed += Time.deltaTime * (AgentData.RestNeedIncrease / 3);
                                    break;
                                case false:
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case TimeOfDay.Evening:
                            switch (BlackBoard.AgentStates.working)
                            {
                                case true:
                                    RestNeed += Time.deltaTime * AgentData.RestNeedIncrease;
                                    break;
                                case false:

                                    break;
                                default:
                                    break;
                            }
                            break;
                        case TimeOfDay.Night:

                            RestNeed += Time.deltaTime * AgentData.RestNeedIncrease;
                            break;
                        default:
                            break;
                    }
                    break;

                default:
                    break;
            }



        }
        else
        {
            RestNeed = 0;
        }
       



        AddressNeeds();

        if (Happiness >= 0)
        {
            CalculateTotalHappiness();
        }
        if (Happiness <= 0)
        {
            Happiness = 0;
        }

    }





    float CalculateTotalHappiness()
    {
        Happiness = BaseMood;
        Happiness = Happiness + CalculateNeedImpact(HungerNeed);
        Happiness = Happiness + CalculateNeedImpact(WorkNeed);
        Happiness = Happiness + CalculateNeedImpact(RestNeed);


        return Happiness;

    }


    void AddressNeeds()
    {


        float HungerNeedAfter = Mathf.Max(HungerNeed - 10, 0);
        float HungerhappinessIncrease = CalculateNeedImpact(HungerNeedAfter) - CalculateNeedImpact(HungerNeed);

        if (HungerhappinessIncrease >= 0f && HungerhappinessIncrease <= 9)
        {
            if (EatGoal != null)
            {

                goals[EatGoal] = HungerhappinessIncrease;

            }

        }


        float restNeedAfter = Mathf.Max(RestNeed - 10, 0);
        float RestHappinessIncrease = CalculateNeedImpact(restNeedAfter) - CalculateNeedImpact(RestNeed);

        if (RestHappinessIncrease >= 0f && RestHappinessIncrease <= 9)
        {
            if (RestGoal != null)
            {
                goals[RestGoal] = RestHappinessIncrease;
            }

            //  Debug.Log("Rest Happiness incress " + RestHappinessIncrease);
        }




        float WorkneedAfter = Mathf.Max(WorkNeed - 10, 0);
        float WorkHapinessIncrease = CalculateNeedImpact(WorkneedAfter) - CalculateNeedImpact(WorkNeed);

        if (WorkHapinessIncrease >= 0f && WorkHapinessIncrease <= 9)
        {
            if (WorkGoal != null)
            {
                goals[WorkGoal] = WorkHapinessIncrease;
            }

        }



    }

    float CalculateNeedImpact(float need)
    {

        return (-1 * (Mathf.Pow(need, 2)));
    }
}
