using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "InvestigatePotentialPreyAction", menuName = "AgentActions/InvestigatePotentialPreyAction")]
public class InvestigatePotentialPrey : GAction
{

    GameObject closestPrey;
    GAgent TargetAgent;
    bool investigating;
    float alert = 1f;
    float increaseAlert = 0;
    float distance;
   
    private void OnEnable()
    {
      
        preconditions.Add(AgentFact.PotentialPrey.ToString(), true);     
        preconditions.Add(AgentFact.StimuliInvestigated.ToString(), false);
        effects.Add(AgentTask.InvestigateStimuli.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {
        ThisAgent.ResetAgentState();
        increaseAlert = 0;
        investigating = false;
        if (ThisAgent.AgentFacts.HasState(AgentFact.CanEat.ToString(), true) )
        {
          
            ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;
            ThisAgent.UpdateAwareness();
            return false;
        }
        if (ThisAgent.BlackBoard.CurrentStimuli.Count == 0)
        {
            Debug.Log("No threats");
            return false;
        }


   



        closestPrey = ThisAgent.BlackBoard.AgentMemory.FindClosestStimuli();
        TargetAgent = closestPrey.GetComponent<GAgent>();



        target = closestPrey;
        if(target == null)
        {
            Debug.Log("No Target");
            return false;
        }

        ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = false;
        ThisAgent.gameObject.transform.Find("InvestigationEye").gameObject.SetActive(true);
        Debug.Log("TRYING TO INVESTIGATE");
        return true;
    }



    public override bool ActionEffect()
    {



        WMFact check = new WMFact();
        check.Fact = AgentFact.StimuliInvestigated;
        check.SourceObject = closestPrey;


        ThisAgent.ResetAgentState();
        if (increaseAlert >= alert && TargetAgent)
        {
          
            if (ThisAgent.AgentMemory.FindMemoryFact(check) != null)
            {
                check = ThisAgent.AgentMemory.FindMemoryFact(check);
                
                check.FactState = true;
            //    Debug.Log("Fact Fixed " + check.Fact.ToString() + " GameObject: " + check.sourceObject + " State: " + check.factState);
            }

            ThreatLevel targetThreat = TargetAgent.threatLevel;

            if (ThisAgent.AgentFacts.HasState(AgentFact.Hungry.ToString(), true) == false)
            {
                Debug.Log("Not Hungry");
                ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;
                ThisAgent.UpdateAwareness();
                return true;
            }






            if (ThisAgent.AgentFacts.HasState(AgentFact.Hungry.ToString(), true))
            {
             
                   

                



                if (ThisAgent.threatLevel == ThreatLevel.LowThreat)
                {
                    switch (targetThreat)
                    {
                        case ThreatLevel.NoThreat:
                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                            ThisAgent.BlackBoard.CurrentEnemy = closestPrey;

                            ThisAgent.UpdateAwareness();
                            break;
                        //my threat lvl
                        case ThreatLevel.LowThreat:

                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                            ThisAgent.BlackBoard.CurrentEnemy = closestPrey;

                            ThisAgent.UpdateAwareness();



                            break;

                        //special case for one lvl up 
                        case ThreatLevel.MediumThreat:
                            
                                ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;
                                

                                ThisAgent.UpdateAwareness();


                            
                           
                            break;

                        case ThreatLevel.HighThreat:
                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;
                            

                            ThisAgent.UpdateAwareness();

                            break;
                        default:
                            break;
                    }

                    return true;
                }

                if (ThisAgent.threatLevel == ThreatLevel.MediumThreat)
                {

                    switch (targetThreat)
                    {
                        case ThreatLevel.NoThreat:
                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                            ThisAgent.BlackBoard.CurrentEnemy = closestPrey;

                            ThisAgent.UpdateAwareness();
                            break;

                        case ThreatLevel.LowThreat:
                           
                                ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                                ThisAgent.BlackBoard.CurrentEnemy = closestPrey;

                                ThisAgent.UpdateAwareness();


                            
                           
                            break;

                        case ThreatLevel.MediumThreat:

                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                            ThisAgent.BlackBoard.CurrentEnemy = closestPrey;

                            ThisAgent.UpdateAwareness();



                            break;

                        case ThreatLevel.HighThreat:
                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;
                           

                            ThisAgent.UpdateAwareness();

                            break;
                        default:
                            break;
                    }

                    return true;

                }
                if (ThisAgent.threatLevel == ThreatLevel.HighThreat)
                {
                    switch (targetThreat)
                    {
                        case ThreatLevel.NoThreat:
                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                            ThisAgent.BlackBoard.CurrentEnemy = closestPrey;

                            ThisAgent.UpdateAwareness();
                            break;
                        case ThreatLevel.LowThreat:
                        
                                ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                                ThisAgent.BlackBoard.CurrentEnemy = closestPrey;

                                ThisAgent.UpdateAwareness();


                            
                          
                            break;

                        //special case for one lvl up 
                        case ThreatLevel.MediumThreat:
                            
                                ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                                ThisAgent.BlackBoard.CurrentEnemy = closestPrey;

                                ThisAgent.UpdateAwareness();


                         
                                ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;
                                ThisAgent.UpdateAwareness();

                            
                            break;

                        case ThreatLevel.HighThreat:

                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;
                            

                            ThisAgent.UpdateAwareness();




                            break;
                        default:
                            break;
                    }

                    return true;

                }





            }
        }


       
        if (ThisAgent.BlackBoard.myAwareness != AIAwareness.Combat && ThisAgent.BlackBoard.myAwareness != AIAwareness.Fleeing)
        {

            ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;
            ThisAgent.BlackBoard.Agent.UpdateAwareness();
        }

        return false;
    }
    public override bool InterruptActionCleanUp()
    {

        WMFact check = new WMFact();
        check.Fact = AgentFact.StimuliInvestigated;
        check.SourceObject = closestPrey;




       
            check = ThisAgent.AgentMemory.FindMemoryFact(check);
            check.FactState = true;
        

       
        // thisAgent.BlackBoard.CurrentEnemy = null;

        return false;
    }

    public override void ActionRunning()
    {


        ThisAgent.LookAtTarget(closestPrey);
        if (ThisAgent.BlackBoard.CurrentStimuli.Contains(closestPrey))
        {
           
            distance = Vector3.Distance(ThisAgent.transform.position, closestPrey.transform.position);
            duration = distance;
      



            
            increaseAlert += Time.deltaTime * ThisAgent.AgentData.AgentAwarenessSensitivity / distance ;
            if(distance < 6f && investigating == false)
            {
                investigating = true;
                ThisAgent.ChangeAgentState(ThisAgent.DefaultAgentData.InvestigateState);
                ThisAgent.myCharacter.characterLocomotion.Stop();
            }
            if (increaseAlert >= alert)
            {

                ThisAgent.currentAction.ActionEffect();
            }
        }
        else
        {
            ThisAgent.currentAction.ActionEffect();
        }
        

        

      
        


    }
}
