using GameCreator.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class GworldObject : ScriptableObject
{

    public AgentFact ObjectPrimaryFact;
    public WorldObject WObject;
    public bool ObjectFactState;
    public GAgent CurrentObjectOwner;
    public float ObjectPriority;
    public float ObjectUseTime;
    public GameObject ActionDestination;
    [Header("Object Use Visuals")]
    public AnimationClip AgentAnim;
    public AvatarMask AgentAnimMask;
    public CharacterState AgentState;

    public GObjectInfo OwnerObj;


    public abstract void InitOwnerObj();
    public abstract void ObjectUseLogic();
    public abstract void AgentObjectUseState();


    public abstract void AgentObjectUseAnimation();
    

    
    


}
