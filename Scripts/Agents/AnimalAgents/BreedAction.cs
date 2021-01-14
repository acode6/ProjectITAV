using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BreedAction", menuName = "AgentActions/BreedAction")]
public class BreedAction : GAction
{
    AnimalAgent me;
    AnimalAgent mate;

    bool breeding;


    public void OnEnable()
    {


        preconditions.Add(AgentTask.FindMate.ToString(), true);

   
        effects.Add(AgentTask.Breed.ToString(), true);


    }

    public override bool ContextPreConditions()
    {

    
        breeding = false;

        Debug.Log("Mates Action " + ThisAgent.BlackBoard.currentMate.currentAction.actionName);
        if (ThisAgent.BlackBoard.currentMate == null )
            return false;


        target = null;



        me = (AnimalAgent)ThisAgent.BlackBoard.Agent;
        mate = (AnimalAgent)ThisAgent.BlackBoard.currentMate;



       


        return true;
    }

    public override bool ActionEffect()
    {

        me.reproductionUrge = me.genes.reproductionUrge;

        ThisAgent.BlackBoard.Agent.AgentFacts.RemoveState(AgentFact.NeedMate.ToString());
        if (me.gender.Equals(" female"))
        {
            ThisAgent.ResetAgentState();
            ThisAgent.BlackBoard.mateGenes = ThisAgent.BlackBoard.currentMate;
            ThisAgent.FleeFromTarget(ThisAgent.BlackBoard.currentMate.gameObject);
            ThisAgent.BlackBoard.currentMate = null;
            me.LastMateGenes = ThisAgent.BlackBoard.mateGenes as AnimalAgent;
        
            ThisAgent.BlackBoard.Agent.AgentFacts.SetState(AgentFact.Hungry.ToString(), true);

           
        }
        if (me.gender.Equals(" male"))
        {

            ThisAgent.ResetAgentState();
            ThisAgent.FleeFromTarget(ThisAgent.BlackBoard.currentMate.gameObject);
            ThisAgent.BlackBoard.currentMate = null;
       
        
        }
       
        return true;

       
     
  


    }

    public override bool InterruptActionCleanUp()
    {



        if (me.gender.Equals(" female"))
        {

            ThisAgent.BlackBoard.mateGenes = null;       
           
            me.LastMateGenes = null;

            ThisAgent.BlackBoard.currentMate = null;
           
        }
        else
        {

          
            ThisAgent.BlackBoard.currentMate = null;

 
        }
        ThisAgent.ResetAgentState();


        return true;
    }

    public override void ActionRunning()
    {
     
        if (ThisAgent.BlackBoard.currentMate != null)
        {
           
            
           

            if (Vector3.Distance(ThisAgent.BlackBoard.Position, ThisAgent.BlackBoard.currentMate.transform.position) <= 2.2f)
            {
                ThisAgent.LookAtTarget(ThisAgent.BlackBoard.currentMate.gameObject);
                if (breeding == false)
                {

                    duration = 3;
                    breeding = true;

                    ThisAgent.ChangeAgentState(ThisAgent.DefaultAgentData.BreedingState);
               

                }


            }
            else
            {
                duration = Vector3.Distance(ThisAgent.BlackBoard.Position, ThisAgent.BlackBoard.currentMate.BlackBoard.Position);

                ThisAgent.FollowTarget(ThisAgent.BlackBoard.currentMate.gameObject);
            }
        }
       



    }
}
