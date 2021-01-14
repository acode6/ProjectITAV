using GameCreator.Core;
using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DodgeAction", menuName = "AgentActions/DodgeAction")]
public class DodgeAction : GAction
{

    GameObject currentTarget;
    bool Dodge = false;
    
    Vector3 moveDirection;
    public void OnEnable()
    {

        preconditions.Add(AgentFact.CanDodge.ToString(), true);
        preconditions.Add(AgentFact.MeleeWeapon.ToString(), true);


        effects.Add(AgentTask.AvoidDamage.ToString(), true);


    }

    public override bool ContextPreConditions()
    {
        Debug.Log("Dodge pre");
        moveDirection = Vector3.zero;

        if ( ThisAgent.myCharacter.characterLocomotion.currentLocomotionSystem.isDashing == true )
        {
            Debug.Log("Already dashing");
            return false;
        }


        currentTarget = ThisAgent.BlackBoard.CurrentEnemy.gameObject;


        Dodge = false;

        if (Dodge == false && currentTarget != null)
        {
            int random = Random.Range(1, 3);

            switch (random)
            {

                case 1:
                    if (ThisAgent.myCharacter.characterLocomotion.currentLocomotionSystem.isDashing == false)
                    {
                        if(ThisAgent.DodgeAttack(ThisAgent.transform.position + Vector3.left.normalized) == true)
                        {
                            Debug.Log("Left dodge");
                            return true;
                        }

                       
                    }

                    break;
                case 2:
                    if (ThisAgent.myCharacter.characterLocomotion.currentLocomotionSystem.isDashing == false)
                    {
                        if (ThisAgent.DodgeAttack(ThisAgent.transform.position + Vector3.right.normalized) == true)
                        {
                            Debug.Log("right dodge");
                            return true;
                        }

                        
                    }
                    break;
                case 3:
                    if (ThisAgent.myCharacter.characterLocomotion.currentLocomotionSystem.isDashing == false)
                    {

                        Debug.Log("back dodge");
                        if (ThisAgent.DodgeAttack(ThisAgent.transform.position + Vector3.back.normalized) == true)
                        {
                            return true;
                        }

                        
                    }
                    break;

                default:
                    break;
            }



        }



        target = null;

        Debug.Log("Dodge failed");
        return false;













    }



    public override bool ActionEffect()
    {


        if (ThisAgent.agentCombatManager.AgentConfidence < 0f)
        {
            
            ThisAgent.agentCombatManager.AgentConfidence = ThisAgent.AgentData.AgentStartingConfidence;
        }

        ThisAgent.currentAction.running = false;


        return true;
    }
    public override bool InterruptActionCleanUp()
    {
        Debug.Log("Dodge Cancelled");
        return false;
    }

    public override void ActionRunning()
    {
       
      

     


        if (ThisAgent.myCharacter.characterLocomotion.currentLocomotionSystem.isDashing == true)
        {

          
            Dodge = true;
            duration = .5f;

            ThisAgent.MeleeComponent.SetInvincibility(.35f);


        }
       
       














    }
}
