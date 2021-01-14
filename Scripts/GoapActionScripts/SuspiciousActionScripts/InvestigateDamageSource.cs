using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "InvestigateDamageSource", menuName = "AgentActions/InvestigateDamageSource")]
public class InvestigateDamageSource : GAction
{

    GameObject DamageSource;
    GAgent TargetAgent;
    PlayerCharacter pInfo;
    bool investigating;
    float alert = 1f;
    float alertIncreasePercent = 0;
    float distance;
    private void OnEnable()
    {
      
        preconditions.Add(AgentFact.DamageTaken.ToString(), true);
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


        ThisAgent.BlackBoard.CurrentEnemy = null;



        DamageSource = ThisAgent.BlackBoard.AgentMemory.FindClosestStimuli();
        if (DamageSource.GetComponent<GAgent>() != null)
        {

            TargetAgent = DamageSource.GetComponent<GAgent>();

        }

        if (DamageSource.GetComponent<PlayerCharacter>())
        {
            pInfo = DamageSource.GetComponent<PlayerCharacter>();
          //  Debug.Log("closest threat" + pInfo);
        }
       // Debug.Log("Damage Source" + DamageSource);
        target = null;
        ThisAgent.pathAi.destination = DamageSource.transform.position;
        ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = false;

        ThisAgent.gameObject.transform.Find("InvestigationEye").gameObject.SetActive(true);
        if(ThisAgent.weaponDrawn == false)
        {
        //    thisAgent.DrawWeapon();
        }
       
        return true;
    }



    public override bool ActionEffect()
    {

       

       // Debug.Log("Damage Investigated");
        WMFact check = new WMFact();
        check.Fact = AgentFact.StimuliInvestigated;
        check.SourceObject = DamageSource;
        if ( TargetAgent)
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
                ThisAgent.BlackBoard.CurrentEnemy = DamageSource;

                ThisAgent.UpdateAwareness();
                return true;
            }








            if (ThisAgent.threatLevel == ThreatLevel.NoThreat)
            {
                ThisAgent.BlackBoard.myAwareness = AIAwareness.Fleeing;
                ThisAgent.BlackBoard.CurrentEnemy = DamageSource;

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
                        ThisAgent.BlackBoard.CurrentEnemy = DamageSource;

                        ThisAgent.UpdateAwareness();



                        break;

                    //special case for one lvl up 
                    case ThreatLevel.MediumThreat:
                        if (TargetAgent.BlackBoard.CurrentEnemy == ThisAgent)
                        {
                            ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                            ThisAgent.BlackBoard.CurrentEnemy = DamageSource;

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
                        ThisAgent.BlackBoard.CurrentEnemy = DamageSource;

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
                            ThisAgent.BlackBoard.CurrentEnemy = DamageSource;

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
                        ThisAgent.BlackBoard.CurrentEnemy = DamageSource;

                        ThisAgent.UpdateAwareness();



                        break;

                    case ThreatLevel.HighThreat:
                        ThisAgent.BlackBoard.myAwareness = AIAwareness.Fleeing;
                        ThisAgent.BlackBoard.CurrentEnemy = DamageSource;

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
                            ThisAgent.BlackBoard.CurrentEnemy = DamageSource;

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
                            ThisAgent.BlackBoard.CurrentEnemy = DamageSource;

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
                        ThisAgent.BlackBoard.CurrentEnemy = DamageSource;

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
                ThisAgent.BlackBoard.CurrentEnemy = DamageSource;

                ThisAgent.UpdateAwareness();
                return true;
            }








            if (ThisAgent.threatLevel == ThreatLevel.NoThreat)
            {
                ThisAgent.BlackBoard.myAwareness = AIAwareness.Fleeing;
                ThisAgent.BlackBoard.CurrentEnemy = DamageSource;

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
                        ThisAgent.BlackBoard.CurrentEnemy = DamageSource;

                        ThisAgent.UpdateAwareness();



                        break;

                    //special case for one lvl up 
                    case ThreatLevel.MediumThreat:

                        ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                        ThisAgent.BlackBoard.CurrentEnemy = DamageSource;

                        ThisAgent.UpdateAwareness();




                        break;

                    case ThreatLevel.HighThreat:
                        ThisAgent.BlackBoard.myAwareness = AIAwareness.Fleeing;
                        ThisAgent.BlackBoard.CurrentEnemy = DamageSource;

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
                        ThisAgent.BlackBoard.CurrentEnemy = DamageSource;

                        ThisAgent.UpdateAwareness();




                        break;

                    case ThreatLevel.MediumThreat:

                        ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                        ThisAgent.BlackBoard.CurrentEnemy = DamageSource;

                        ThisAgent.UpdateAwareness();



                        break;

                    case ThreatLevel.HighThreat:
                        ThisAgent.BlackBoard.myAwareness = AIAwareness.Fleeing;
                        ThisAgent.BlackBoard.CurrentEnemy = DamageSource;

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
                        ThisAgent.BlackBoard.CurrentEnemy = DamageSource;

                        ThisAgent.UpdateAwareness();




                        break;

                    //special case for one lvl up 
                    case ThreatLevel.MediumThreat:

                        ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                        ThisAgent.BlackBoard.CurrentEnemy = DamageSource;

                        ThisAgent.UpdateAwareness();




                        break;

                    case ThreatLevel.HighThreat:

                        ThisAgent.BlackBoard.myAwareness = AIAwareness.Combat;
                        ThisAgent.BlackBoard.CurrentEnemy = DamageSource;

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
        Debug.Log(ThisAgent.name + "  Returning false here");
        return false;


      
    }

    public override bool InterruptActionCleanUp()
    {
        ThisAgent.gameObject.transform.Find("InvestigationEye").gameObject.SetActive(false);
        WMFact check = new WMFact();
        check.Fact = AgentFact.StimuliInvestigated;
        check.SourceObject = DamageSource;




      

        return false;
    }

    public override void ActionRunning()
    {
       // if (ThisAgent.BlackBoard.CurrentStimuli.Contains(DamageSource))
        //{

            distance = Vector3.Distance(ThisAgent.transform.position, DamageSource.transform.position);
            duration = distance;


            ThisAgent.TrackTarget(DamageSource);



        
            
      //  }
       

       

      







    }
}
