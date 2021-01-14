using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//aka the local states of the agent
    public enum AIWorldState
    {
        
     Armed,
     ArmedRange,
     ArmedMelee,
     Unarmed,
     Hungry,
     Tired,
    HasEnemyInMeleeRange,
    SeeMeleeWeapon,
    SeeRangeWeapon

    }

    public enum AIDestinationTarget
    {
        None,
       Resource,
        Enemy,
        weapon,
    }


public enum CauseOfDeath
{
    Hunger,
    Thirst,
    Age,
    Eaten,
    Killed
}