using GameCreator.Variables;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCreator.Melee;
using GameCreator.Shooter;
using GameCreator.Core;

public class CreatureAgent : GAgent
{






  

    public override void Init()
    {


        InitStartWeapon();
        UpdateAgentGoals();


    }


    public override void UpdateAgentNeed()
    {
        
    }

}