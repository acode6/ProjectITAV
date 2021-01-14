using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabLungeAttackAction", menuName = "AgentActions/PrefabLungeAttackAction")]
public class PrefabLungeAttackAction : GAction
{

    GameObject currentTarget;
    public GameObject AttackPrefab;
    GameObject spawnedAttack;
    public void OnEnable()
    {


        preconditions.Add(AgentFact.CanLungeAttack.ToString(), true);
        preconditions.Add(AgentFact.InLungeRange.ToString(), true);

        effects.Add(AgentFact.InMeleeRange.ToString(), true);
        effects.Add(AgentFact.CanAttack.ToString(), true);


    }

    public override bool ContextPreConditions()
    {

        Debug.Log("Lunge Pre");
        currentTarget = ThisAgent.BlackBoard.CurrentEnemy.gameObject;


        if (currentTarget != null)
        {


            target = ThisAgent.gameObject;

            ThisAgent.MeleeComponent.Execute(GameCreator.Melee.CharacterMelee.ActionKey.D);

            return true;
        }


        return false;
    }



    public override bool ActionEffect()
    {

        spawnedAttack = Instantiate(AttackPrefab, new Vector3(ThisAgent.transform.position.x, ThisAgent.transform.position.y, ThisAgent.transform.position.z), ThisAgent.transform.rotation);
        spawnedAttack.GetComponent<ShooterAgent>().SetShooter(ThisAgent.gameObject);
        
      
        return false;

    }
    public override bool InterruptActionCleanUp()
    {
        ThisAgent.MeleeComponent.StopAttack();
        return false;
    }

    public override void ActionRunning()
    {



        if (ThisAgent.MeleeComponent.IsAttacking == true)
        {
            //   ThisAgent.BlackBoard.AgentCharacter.characterLocomotion.canRun = true;
            //   thisAgent.FollowTarget(currentTarget);


           // Debug.Log("Duration " + ThisAgent.MeleeComponent.ReturnCurrentMeleeClip().animationClip.length);
            duration = ThisAgent.MeleeComponent.ReturnCurrentMeleeClip().animationClip.length; 
        }
        else
        {
         //   ThisAgent.currentAction.ActionEffect();
        }








    }
}
