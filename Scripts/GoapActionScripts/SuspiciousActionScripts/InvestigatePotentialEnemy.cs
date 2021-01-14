using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "InvestigatePotentialEnemyAction", menuName = "AgentActions/InvestigatePotentialEnemyAction")]
public class InvestigatePotentialEnemy : GAction
{

    GameObject closestThreat;
    GAgent TargetAgent;
    PlayerCharacter pInfo;
    bool investigating;
    float alert = 1f;
    float alertIncreasePercent = 0;
    float distance;
    private void OnEnable()
    {
      
        preconditions.Add(AgentFact.PotentialEnemy.ToString(), true);
        preconditions.Add(AgentFact.StimuliInvestigated.ToString(), false);
        effects.Add(AgentTask.InvestigateStimuli.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {
        
        alertIncreasePercent = 0;
        investigating = false;

        if(ThisAgent.BlackBoard.CurrentStimuli.Count == 0)
        {
            Debug.Log("No threats");
            return false;
        }

        //a reset just in case
        ThisAgent.BlackBoard.CurrentEnemy = null;


   
        closestThreat = ThisAgent.BlackBoard.AgentMemory.FindClosestStimuli();

        if (closestThreat.GetComponent<GAgent>() != null)
        {
         
            TargetAgent = closestThreat.GetComponent<GAgent>();

        }

        if (closestThreat.GetComponent<PlayerCharacter>())
        {
            pInfo = closestThreat.GetComponent<PlayerCharacter>();
         
        }

  
        


        target = closestThreat;

        if(target == null)
        {
            Debug.Log("Error check target");
            return false;
        }


        //ThisAgent.ChangeAgentState(ThisAgent.AgentDataCopy.InvestigateState);

        ThisAgent.walk = true;

        ThisAgent.gameObject.transform.Find("InvestigationEye").gameObject.SetActive(true);
        return true;
    }



    public override bool ActionEffect()
    {

        if(ThisAgent.weaponDrawn == false)
        {
            ThisAgent.ResetAgentState();
        }
       


        WMFact check = new WMFact();
        check.Fact = AgentFact.StimuliInvestigated;
        check.SourceObject = closestThreat;


     //   Debug.Log("Gagent Target effect before if" + closestThreat + "" + thisAgent.name);
        if (alertIncreasePercent >= alert && TargetAgent)
        {
          //  Debug.Log("Gagent Target Effect  " + closestThreat + "  " + thisAgent.name);
          //  Debug.Log("Action effect" + thisAgent.name);
            ThisAgent.gameObject.transform.Find("InvestigationEye").gameObject.SetActive(false);
            ThisAgent.gameObject.transform.Find("AlertEye").gameObject.SetActive(true);
            if (ThisAgent.AgentMemory.FindMemoryFact(check) != null)
            {
                check = ThisAgent.AgentMemory.FindMemoryFact(check);
                check.FactState = true;
           
            }

            ThreatLevel targetThreat = TargetAgent.threatLevel;

            if (ThisAgent.AgentFacts.HasState(AIWorldState.Armed.ToString(), false))
            {
                ThisAgent.BlackBoard.myAwareness = AIAwareness.Fleeing;
                ThisAgent.BlackBoard.CurrentEnemy = closestThreat;

                ThisAgent.UpdateAwareness();
                return true;
            }






            

            if(ThisAgent.threatLevel == ThreatLevel.NoThreat)
            {
                ThisAgent.BlackBoard.myAwareness = AIAwareness.Fleeing;
                ThisAgent.BlackBoard.CurrentEnemy = closestThreat;

                ThisAgent.UpdateAwareness();
                return true;

            }

            if (ThisAgent.threatLevel == ThreatLevel.LowThreat)
            {
                switch (targetThreat)
                {
                  //my threat lvl
                    case ThreatLevel.LowThreat:
                       
                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                            ThisAgent.BlackBoard.CurrentEnemy = closestThreat;

                            ThisAgent.UpdateAwareness();
                          

                       
                        break;

                        //special case for one lvl up 
                    case ThreatLevel.MediumThreat:
                        if (TargetAgent.BlackBoard.CurrentEnemy == ThisAgent)
                        {
                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                            ThisAgent.BlackBoard.CurrentEnemy = closestThreat;

                            ThisAgent.UpdateAwareness();


                        }
                        else
                        {
                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;
                            ThisAgent.UpdateAwareness();

                        }
                        break;
                        
                    case ThreatLevel.HighThreat:
                        ThisAgent.BlackBoard.myAwareness = AIAwareness.Fleeing;
                        ThisAgent.BlackBoard.CurrentEnemy = closestThreat;

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
                    // lower lvl
                    case ThreatLevel.LowThreat:
                        if (TargetAgent.BlackBoard.CurrentEnemy == ThisAgent)
                        {
                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                            ThisAgent.BlackBoard.CurrentEnemy = closestThreat;

                            ThisAgent.UpdateAwareness();


                        }
                        else
                        {
                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;
                            ThisAgent.UpdateAwareness();

                        }
                        break;

                    case ThreatLevel.MediumThreat:

                        ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                        ThisAgent.BlackBoard.CurrentEnemy = closestThreat;

                        ThisAgent.UpdateAwareness();



                        break;

                    case ThreatLevel.HighThreat:
                        ThisAgent.BlackBoard.myAwareness = AIAwareness.Fleeing;
                        ThisAgent.BlackBoard.CurrentEnemy = closestThreat;

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
                    // lower lvl
                    case ThreatLevel.LowThreat:
                        if (TargetAgent.BlackBoard.CurrentEnemy == ThisAgent)
                        {
                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                            ThisAgent.BlackBoard.CurrentEnemy = closestThreat;

                            ThisAgent.UpdateAwareness();


                        }
                        else
                        {
                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;
                            ThisAgent.UpdateAwareness();

                        }
                        break;

                    //special case for one lvl up 
                    case ThreatLevel.MediumThreat:
                        if (TargetAgent.BlackBoard.CurrentEnemy == ThisAgent)
                        {
                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                            ThisAgent.BlackBoard.CurrentEnemy = closestThreat;

                            ThisAgent.UpdateAwareness();


                        }
                        else
                        {
                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;
                            ThisAgent.UpdateAwareness();

                        }
                        break;

                    case ThreatLevel.HighThreat:
                       
                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                            ThisAgent.BlackBoard.CurrentEnemy = closestThreat;

                            ThisAgent.UpdateAwareness();


                       
                       
                        break;
                    default:
                        break;
                }

                return true;

            }

           

           

        }
        if (alertIncreasePercent >= alert && pInfo)
        {
            
            ThisAgent.gameObject.transform.Find("InvestigationEye").gameObject.SetActive(false);
            ThisAgent.gameObject.transform.Find("AlertEye").gameObject.SetActive(true);
            if (ThisAgent.AgentMemory.FindMemoryFact(check) != null)
            {
                check = ThisAgent.AgentMemory.FindMemoryFact(check);
                check.FactState = true;

            }

            ThreatLevel targetThreat = pInfo.PlayerThreatLvl;

            if (ThisAgent.AgentFacts.HasState(AIWorldState.Armed.ToString(), false))
            {
                ThisAgent.BlackBoard.myAwareness = AIAwareness.Fleeing;
                ThisAgent.BlackBoard.CurrentEnemy = closestThreat;

                ThisAgent.UpdateAwareness();
                return true;
            }








            if (ThisAgent.threatLevel == ThreatLevel.NoThreat)
            {
                ThisAgent.BlackBoard.myAwareness = AIAwareness.Fleeing;
                ThisAgent.BlackBoard.CurrentEnemy = closestThreat;

                ThisAgent.UpdateAwareness();
                return true;

            }

            if (ThisAgent.threatLevel == ThreatLevel.LowThreat)
            {
                switch (targetThreat)
                {
                    //my threat lvl
                    case ThreatLevel.LowThreat:

                        ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                        ThisAgent.BlackBoard.CurrentEnemy = closestThreat;

                        ThisAgent.UpdateAwareness();



                        break;

                    //special case for one lvl up 
                    case ThreatLevel.MediumThreat:
                       
                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                            ThisAgent.BlackBoard.CurrentEnemy = closestThreat;

                            ThisAgent.UpdateAwareness();


                        
                       
                        break;

                    case ThreatLevel.HighThreat:
                        ThisAgent.BlackBoard.myAwareness = AIAwareness.Fleeing;
                        ThisAgent.BlackBoard.CurrentEnemy = closestThreat;

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
                    // lower lvl
                    case ThreatLevel.LowThreat:
                      
                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                            ThisAgent.BlackBoard.CurrentEnemy = closestThreat;

                            ThisAgent.UpdateAwareness();


                        
                       
                        break;

                    case ThreatLevel.MediumThreat:

                        ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                        ThisAgent.BlackBoard.CurrentEnemy = closestThreat;

                        ThisAgent.UpdateAwareness();



                        break;

                    case ThreatLevel.HighThreat:
                        ThisAgent.BlackBoard.myAwareness = AIAwareness.Fleeing;
                        ThisAgent.BlackBoard.CurrentEnemy = closestThreat;

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
                    // lower lvl
                    case ThreatLevel.LowThreat:
                      
                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                            ThisAgent.BlackBoard.CurrentEnemy = closestThreat;

                            ThisAgent.UpdateAwareness();


                        
                      
                        break;

                    //special case for one lvl up 
                    case ThreatLevel.MediumThreat:
                       
                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                            ThisAgent.BlackBoard.CurrentEnemy = closestThreat;

                            ThisAgent.UpdateAwareness();


                        
                     
                        break;

                    case ThreatLevel.HighThreat:

                        ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                        ThisAgent.BlackBoard.CurrentEnemy = closestThreat;

                        ThisAgent.UpdateAwareness();




                        break;
                    default:
                        break;
                }

                return true;

            }





        }
        ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;

        ThisAgent.gameObject.transform.Find("InvestigationEye").gameObject.SetActive(false);
        ThisAgent.UpdateAwareness();
        Debug.Log(ThisAgent.name + "  Returning false here");
        return false;
    }
    public override bool InterruptActionCleanUp()
    {

        ThisAgent.gameObject.transform.Find("InvestigationEye").gameObject.SetActive(false);
        ThisAgent.ResetAgentState();
        WMFact check = new WMFact();
        check.Fact = AgentFact.StimuliInvestigated;
        check.SourceObject = closestThreat;




        if (ThisAgent.AgentMemory.FindMemoryFact(check) != null && ThisAgent.BlackBoard.CurrentEnemy != null)
        {
            check = ThisAgent.AgentMemory.FindMemoryFact(check);
            check.FactState = true;
           // Debug.Log("Interrupted but checked");

        }
        else
        {
            check = ThisAgent.AgentMemory.FindMemoryFact(check);
            check.FactState = false;
          //  Debug.Log("Interrupted");
        }
       // thisAgent.BlackBoard.CurrentEnemy = null;

        return false;
    }

    public override void ActionRunning()
    {

    
        ThisAgent.TrackTarget(closestThreat);
        if (ThisAgent.BlackBoard.CurrentStimuli.Contains(closestThreat))
        {
        
            distance = Vector3.Distance(ThisAgent.transform.position, closestThreat.transform.position);
            duration = distance;


        



            alertIncreasePercent += Time.deltaTime * ThisAgent.AgentData.AgentAwarenessSensitivity / distance ;
            if(distance < 6f && investigating == false)
            {
                investigating = true;
                ThisAgent.ChangeAgentState(ThisAgent.DefaultAgentData.InvestigateState);
           
            }
            if (alertIncreasePercent >= alert)
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
