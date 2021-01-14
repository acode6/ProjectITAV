using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCreator.Melee;

public enum WeaponType
{
    meleeWeapon,
    rangeWeapon, 
    magicWeapon,
    CreatureWeapon
}

public class Weapon : MonoBehaviour
{
    public WeaponType WeaponType;
    public MeleeWeapon GCWeapon;
    public GameCreator.Shooter.Weapon GCRWeapon;


    public float meleeAttackDistance;
    public float lungeAttackDistance;
    public float projectileRangeDistance;
    public float WepDamage;

    public AnimationClip WeaponShootAnim;
    public AvatarMask WeaponAnimShootMask;
    public AvatarMask WeaponAnimMask;
    public GameObject WandAura;
    public GameObject WandHealPrefab;
    public GameObject WandDashTeleport;

    //Values
    public bool canBlockBreak;
    public bool canLungeAttack;

    public AgentSpell BasicSpell;
    public AgentSpell ChargedBasicSpell;
    public AgentSpell CrowdControllSpell;
    public AgentSpell UltimateSpell;
    public AgentSpell SummoningSpell;

}
