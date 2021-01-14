using GameCreator.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GWorldObjects", menuName = "WorldObjectItem")]
public class GworldItem : GworldObject
{
    public override void InitOwnerObj()
    {


    }



    public override void AgentObjectUseState()
    {
        CurrentObjectOwner.ChangeAgentState(AgentState);
    }

    public override void AgentObjectUseAnimation()
    {
        if (AgentAnimMask != null)
        {

            CurrentObjectOwner.PlayAnimClip(AgentAnim, AgentAnimMask);
        }
        else
        {
            CurrentObjectOwner.PlayAnimClip(AgentAnim);
        }


    }

    public override void ObjectUseLogic()
    {

        //VariablesManager.SetLocal(gameObject, "UseObject", true, true);


        //need to make sure that the agent is in the correct position before changing anims
        //account for this in the use time 

      //  CurrentObjectOwner.pathAi.Teleport(ActionDestination.transform.position);

        ILocomotionSystem.TargetRotation cRotation = new ILocomotionSystem.TargetRotation(true, ActionDestination.transform.forward);
        CurrentObjectOwner.myCharacter.characterLocomotion.SetTarget(CurrentObjectOwner.pathAi.steeringTarget, cRotation, CurrentObjectOwner.pathAi.endReachedDistance);
        CurrentObjectOwner.myCharacter.characterLocomotion.angularSpeed = 40;




        //make sure that the agent position matchs
        //then play anim if possible
        if (AgentAnim != null)
        {
          AgentObjectUseAnimation();
        }

        else if (AgentState != null)
        {
          AgentObjectUseState();
        }


    }


}
