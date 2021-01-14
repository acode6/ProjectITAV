using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DashTeleportAction", menuName = "AgentActions/DashTeleportAction")]
public class DashTeleportAction : GAction
{
   
    GameObject currentTarget;
    bool Dodge = false;
    public void OnEnable()
    {

        preconditions.Add(AgentFact.CanDodge.ToString(), true);
        preconditions.Add(AgentFact.MagicWeapon.ToString(), true);                
        effects.Add(AgentTask.AvoidDamage.ToString(), true);

 
    }

    public override bool ContextPreConditions()
    {
        if(ThisAgent.combatStates.dodging == true)
        {
            Debug.Log("Already doding");
            return false;
        }


        Dodge = false;
        currentTarget = ThisAgent.BlackBoard.CurrentEnemy.gameObject;


        GameObject instance = Instantiate(ThisAgent.BlackBoard.MainWeapon.WandDashTeleport, ThisAgent.BlackBoard.Position, ThisAgent.BlackBoard.MainWeapon.WandDashTeleport.transform.rotation);

        target = null;

        if (ThisAgent.agentCombatManager.EnemyMeleeComponent.HasFocusTarget == true)
        {
            ThisAgent.agentCombatManager.EnemyMeleeComponent.ReleaseTargetFocus();
        }



        return true;




    }



    public override bool ActionEffect()
    {




      //  ThisAgent.agentCombatManager.AgentConfidence = ThisAgent.AgentDataCopy.AgentStartingConfidence;


        return true;
    }
    public override bool InterruptActionCleanUp()
    {
      
        return false;
    }

    public override void ActionRunning()
    {
       

        if (Dodge == false)
        {
            int random = Random.Range(1, 4);

            switch (random)
            {
                     
                case 1:
                    Dodge = true;
                    if (ThisAgent.myCharacter.characterLocomotion.currentLocomotionSystem.isDashing == false)
                    {
                        ThisAgent.pathAi.destination = Vector3.left.normalized;

                        ThisAgent.pathAi.Teleport(ThisAgent.transform.position + Vector3.left * 5);
                        GameObject instance = Instantiate(ThisAgent.BlackBoard.MainWeapon.WandDashTeleport, ThisAgent.BlackBoard.Position, ThisAgent.BlackBoard.MainWeapon.WandDashTeleport.transform.rotation);
                        Debug.Log("Dodging left");
                    }

                    break;
                case 2:
                    Dodge = true;
                    if (ThisAgent.myCharacter.characterLocomotion.currentLocomotionSystem.isDashing == false)
                    {
                       
                        ThisAgent.pathAi.destination = Vector3.right.normalized;

                        ThisAgent.pathAi.Teleport(ThisAgent.transform.position + Vector3.right * 5 );
                        GameObject instance = Instantiate(ThisAgent.BlackBoard.MainWeapon.WandDashTeleport, ThisAgent.BlackBoard.Position, ThisAgent.BlackBoard.MainWeapon.WandDashTeleport.transform.rotation);
                        Debug.Log("Dodging Right");
                    }
                    break;
                case 3:
                    Dodge = true;
                    if (ThisAgent.myCharacter.characterLocomotion.currentLocomotionSystem.isDashing == false)
                    {

                        ThisAgent.pathAi.destination = Vector3.back.normalized;

                        ThisAgent.pathAi.Teleport(ThisAgent.transform.position + Vector3.back * 5);
                        GameObject instance = Instantiate(ThisAgent.BlackBoard.MainWeapon.WandDashTeleport, ThisAgent.BlackBoard.Position, ThisAgent.BlackBoard.MainWeapon.WandDashTeleport.transform.rotation);
                        Debug.Log("Dodging back");
                    }
                    break;
                case 4:
                    Dodge = true;
                    if (ThisAgent.myCharacter.characterLocomotion.currentLocomotionSystem.isDashing == false)
                    {

                        ThisAgent.pathAi.destination = Vector3.forward.normalized;

                        ThisAgent.pathAi.Teleport(ThisAgent.transform.position + Vector3.back * 5);
                        GameObject instance = Instantiate(ThisAgent.BlackBoard.MainWeapon.WandDashTeleport, ThisAgent.BlackBoard.Position, ThisAgent.BlackBoard.MainWeapon.WandDashTeleport.transform.rotation);
                        Debug.Log("Dodging back");
                    }
                    break;
                   
                default:
                    break;
            }



        }





        if (ThisAgent.myCharacter.characterLocomotion.currentLocomotionSystem.isDashing == true)
        {


            Dodge = true;
            duration = .5f;

            ThisAgent.MeleeComponent.SetInvincibility(duration);

            if (ThisAgent.myCharacter.characterLocomotion.currentLocomotionSystem.isDashing == false)
            {



                ThisAgent.currentAction.ActionEffect();
            }
        }



       
               
        
     













    }
}
