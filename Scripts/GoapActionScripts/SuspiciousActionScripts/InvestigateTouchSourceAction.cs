using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "InvestigateTouchSourceAction", menuName = "AgentActions/InvestigateTouchSourceAction")]
public class InvestigateTouchSourceAction : GAction
{

    GameObject TouchSource;
    GAgent TargetAgent;
    PlayerCharacter pInfo;
    bool investigating;
    float alert = 1f;
    float alertIncreasePercent = 0;
    float distance;
    private void OnEnable()
    {
      
        preconditions.Add(AgentFact.TouchedStimulus.ToString(), true);
        preconditions.Add(AgentFact.StimuliInvestigated.ToString(), false);
        effects.Add(AgentTask.InvestigateStimuli.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {

        alertIncreasePercent = 0;
        investigating = false;
        TouchSource = null;
        if(ThisAgent.BlackBoard.CurrentStimuli.Count == 0)
        {
            Debug.Log("No threats");
            return false;
        }


        ThisAgent.BlackBoard.CurrentEnemy = null;



        TouchSource = ThisAgent.BlackBoard.AgentMemory.FindClosestStimuli();
        if (TouchSource.GetComponent<GAgent>() != null)
        {

            TargetAgent = TouchSource.GetComponent<GAgent>();

        }

        if (TouchSource.GetComponent<PlayerCharacter>())
        {
            pInfo = TouchSource.GetComponent<PlayerCharacter>();
          //  Debug.Log("closest threat" + pInfo);
        }
       // Debug.Log("Damage Source" + DamageSource);
        target = null;
      
        ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = false;

        ThisAgent.gameObject.transform.Find("InvestigationEye").gameObject.SetActive(true);
       
       
        return true;
    }



    public override bool ActionEffect()
    {

       

       // Debug.Log("Damage Investigated");
        WMFact check = new WMFact();
        check.Fact = AgentFact.StimuliInvestigated;
        check.SourceObject = TouchSource;
        if ( TargetAgent)
        {
      
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
                ThisAgent.BlackBoard.CurrentEnemy = TouchSource;

                ThisAgent.UpdateAwareness();
                return true;
            }








            if (ThisAgent.threatLevel == ThreatLevel.NoThreat)
            {
                ThisAgent.BlackBoard.myAwareness = AIAwareness.Fleeing;
                ThisAgent.BlackBoard.CurrentEnemy = TouchSource;

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
                        ThisAgent.BlackBoard.CurrentEnemy = TouchSource;

                        ThisAgent.UpdateAwareness();



                        break;

                    //special case for one lvl up 
                    case ThreatLevel.MediumThreat:
                        if (TargetAgent.BlackBoard.CurrentEnemy == ThisAgent)
                        {
                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                            ThisAgent.BlackBoard.CurrentEnemy = TouchSource;

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
                        ThisAgent.BlackBoard.CurrentEnemy = TouchSource;

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
                            ThisAgent.BlackBoard.CurrentEnemy = TouchSource;

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
                        ThisAgent.BlackBoard.CurrentEnemy = TouchSource;

                        ThisAgent.UpdateAwareness();



                        break;

                    case ThreatLevel.HighThreat:
                        ThisAgent.BlackBoard.myAwareness = AIAwareness.Fleeing;
                        ThisAgent.BlackBoard.CurrentEnemy = TouchSource;

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
                            ThisAgent.BlackBoard.CurrentEnemy = TouchSource;

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
                            ThisAgent.BlackBoard.CurrentEnemy = TouchSource;

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
                        ThisAgent.BlackBoard.CurrentEnemy = TouchSource;

                        ThisAgent.UpdateAwareness();




                        break;
                    default:
                        break;
                }

                return true;

            }





        }
        if (pInfo)
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
                ThisAgent.BlackBoard.CurrentEnemy = TouchSource;

                ThisAgent.UpdateAwareness();
                return true;
            }








            if (ThisAgent.threatLevel == ThreatLevel.NoThreat)
            {
                ThisAgent.BlackBoard.myAwareness = AIAwareness.Fleeing;
                ThisAgent.BlackBoard.CurrentEnemy = TouchSource;

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
                        ThisAgent.BlackBoard.CurrentEnemy = TouchSource;

                        ThisAgent.UpdateAwareness();



                        break;

                    //special case for one lvl up 
                    case ThreatLevel.MediumThreat:

                        ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                        ThisAgent.BlackBoard.CurrentEnemy = TouchSource;

                        ThisAgent.UpdateAwareness();




                        break;

                    case ThreatLevel.HighThreat:
                        ThisAgent.BlackBoard.myAwareness = AIAwareness.Fleeing;
                        ThisAgent.BlackBoard.CurrentEnemy = TouchSource;

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
                        ThisAgent.BlackBoard.CurrentEnemy = TouchSource;

                        ThisAgent.UpdateAwareness();




                        break;

                    case ThreatLevel.MediumThreat:

                        ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                        ThisAgent.BlackBoard.CurrentEnemy = TouchSource;

                        ThisAgent.UpdateAwareness();



                        break;

                    case ThreatLevel.HighThreat:
                        ThisAgent.BlackBoard.myAwareness = AIAwareness.Fleeing;
                        ThisAgent.BlackBoard.CurrentEnemy = TouchSource;

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
                        ThisAgent.BlackBoard.CurrentEnemy = TouchSource;

                        ThisAgent.UpdateAwareness();




                        break;

                    //special case for one lvl up 
                    case ThreatLevel.MediumThreat:

                        ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                        ThisAgent.BlackBoard.CurrentEnemy = TouchSource;

                        ThisAgent.UpdateAwareness();




                        break;

                    case ThreatLevel.HighThreat:

                        ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                        ThisAgent.BlackBoard.CurrentEnemy = TouchSource;

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

        ThisAgent.UpdateAwareness();
     
        return false;


      
    }

    public override bool InterruptActionCleanUp()
    {
        ThisAgent.gameObject.transform.Find("InvestigationEye").gameObject.SetActive(false);
        WMFact check = new WMFact();
        check.Fact = AgentFact.StimuliInvestigated;
        check.SourceObject = TouchSource;




      

        return false;
    }

    public override void ActionRunning()
    {
     

          //  distance = Vector3.Distance(ThisAgent.transform.position, TouchSource.transform.position);
         //   duration = distance;
           
            ThisAgent.LookAtTarget(TouchSource);
            
            
             

















    }
}
