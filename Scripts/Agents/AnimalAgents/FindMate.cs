using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FindMateAction", menuName = "AgentActions/FindMateAction")]
public class FindMate : GAction
{
    AnimalAgent currentMate;
    AnimalAgent thisAnimal;

    public void OnEnable()
    {


        preconditions.Add(AgentFact.NeedMate.ToString(), true);
        preconditions.Add(AgentFact.PotentialMateInSight.ToString(), true);
   
    

        effects.Add(AgentTask.FindMate.ToString(), true);


    }
    public override bool ContextPreConditions()
    {
        
         thisAnimal = (AnimalAgent)ThisAgent.BlackBoard.Agent;

        if (ThisAgent.BlackBoard.currentMate != null)
        {
            currentMate = ThisAgent.BlackBoard.currentMate as AnimalAgent;
            ThisAgent.BlackBoard.currentMate.BlackBoard.currentMate = ThisAgent;



            target = null;

            return true;





        }



        if ( ThisAgent.BlackBoard.currentMate ==null)
        {
         
          
            if(thisAnimal.gender.Equals(" male") && ThisAgent.BlackBoard.currentMate == null)
            {
             
                for (int i = 0; i < ThisAgent.BlackBoard.potentialMates.Count; i++)
                {
                   
                    if (thisAnimal.PotentialMateFound((AnimalAgent)ThisAgent.BlackBoard.potentialMates[i]))
                    {
                        currentMate = (AnimalAgent)ThisAgent.BlackBoard.potentialMates[i];
                       
                       
                        if (currentMate.BlackBoard.currentMate == null)
                        {
                            ThisAgent.BlackBoard.currentMate = currentMate;
                            currentMate.BlackBoard.currentMate = thisAnimal;


                            return true;

                        }
                        //they have a mate already
                        else 
                        {


                            ThisAgent.BlackBoard.currentMate = null;

                       

                        }


                    
                       
                    }
                }

            }
           



        }






        return false;
    }

    public override bool ActionEffect()
    {
       
        VariablesManager.SetLocal(ThisAgent.gameObject, "LockOn", false, true);

       
        ThisAgent.currentAction.running = false;
        return true;
        
        
       

       

    }
    public override bool InterruptActionCleanUp()
    {
        if(ThisAgent.BlackBoard.currentMate != null)
        {
            ThisAgent.BlackBoard.currentMate = null;
        }

        return false;
    }

    public override void ActionRunning() {


        if (ThisAgent.BlackBoard.currentMate != null)
        {


            Debug.Log("Mait Distance :" + Vector3.Distance(ThisAgent.BlackBoard.Position, ThisAgent.BlackBoard.currentMate.BlackBoard.Position));

            if (Vector3.Distance(ThisAgent.BlackBoard.Position, ThisAgent.BlackBoard.currentMate.BlackBoard.Position) <= 2.5f )
            {
                ThisAgent.LookAtTarget(currentMate.gameObject);
                duration = 0;
                ThisAgent.currentAction.ActionEffect();
            }

            else 
            {
                duration = Vector3.Distance(ThisAgent.BlackBoard.Position, ThisAgent.BlackBoard.currentMate.BlackBoard.Position);

                ThisAgent.FollowTarget(ThisAgent.BlackBoard.currentMate.gameObject);
                currentMate.LookAtTarget(ThisAgent.gameObject);
             
               
            }
        }

      
     
     
       
    }
}
