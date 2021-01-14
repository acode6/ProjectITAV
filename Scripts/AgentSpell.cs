using GameCreator.Characters;
using GameCreator.Shooter;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpellType
{
    ProjectileSpell,
    AdvancedProjectileSpell,
    CrowdControlSpell,
    UltimateSpell,
    HealingSpell,
    Summoning

}


[CreateAssetMenu(fileName = "AgentSpellData", menuName = "AgentSpellData")]
public class AgentSpell : ScriptableObject
{
    public SpellType SpellType;
    public Ammo SpellAmmo;
    public GameObject SpellPreFab;
    public CharacterState CastingState;

   



}
