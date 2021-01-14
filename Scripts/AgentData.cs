using GameCreator.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AgentData", menuName = "Agent Data")]
public class AgentData : ScriptableObject
{
    [Header("Agent Visuals")]
    public string AgentName;
    public GameObject DefaultWeapon;
    public GameObject MainWeapon;
    public GameObject SecondaryWeapon;
    public GameObject AgentModel;
    public List<GameObject> AgentModels;
    public GameObject BabyModel;
    public Material AgentMaterial;
    public CharacterState LocomotionState;
    public SkinnedMeshRenderer AgentRenderer;

    [Header("Agent States")]
    public CharacterState InvestigateState;
    public CharacterState BreedingState;
    public CharacterState HidingState;
    public CharacterState RestingState;
    public CharacterState SleepingState;
    public CharacterState DeathState;
    public List<CharacterState> DeathStates;
    public CharacterState EatingState;
    public CharacterState DrinkingState;
    public CharacterState ChargingState;

    [Header("Agent Anims")]
    public AnimationClip AuraChargeAnim;
    public AnimationClip UltimateAttackAnim;
    //Refactor to scriptable object later or attach to weapons
    public AnimationClip DodgeForward;
    public AnimationClip DodgeBack;
    public AnimationClip DodgeLeft;
    public AnimationClip DodgeRight;

    [EnumFlags]
    public Biomes Biomes;

    [EnumFlags]
    public Species AgentFoes;

    [Header("General Agent Settings")]
    public GAgent Agent;
    public float AgentHealth;
    public float AgentStamina;
    public float AgentMana;
    public float Agentwalkspeed;
    public float Agentrunspeed;
    public float AgentStrafespeed;
    public float AgentAwarenessSensitivity;
    public float DetectRadius;
    public float EyeDetectAngle;
    public Species AgentSpecies;
    public ThreatLevel threatLevel;
    public AgentFact Home;


    [Header(" Agent Combat Settings")]
    //backStabAngle
    public float attackDistance = 1.0f;
    public float dangerDistance = 2.0f;
    public float trackSpeed = 0.1f;
    public float attackRate = 10.0f;
    public float attackRateFluctuation = 0.0f;
    public float seperation = 0f;
    public float AgentStartingConfidence;
    public float AgentConfidenceIncreaseRate;
    public float ConfidenceDecreaseRate;
    public float AgentBaseDamage = 0f;
    public float AgentBaseDefense = 0f;
    public float AgentEvadeChance;
    public float AgentCounterChance;
    public float AgentSkillChance;
    public float AgentLungeChance;
    public float LowHealthThreshold;
    public bool CanLockOn = false;
    public bool CanDodge = false;
    public bool AvoidOthers = true;
    public bool RagdollOnDeath = false;

    [Header(" Animal Agent Settings")]
    public float timeBetweenActionChoices = 1;
    public int DaysToDeathByHunger = 1;
    public int DaysToDeathByThirst = 1;
    public int DaysToReproduceAgain = 1;
    public float AnimalNeedsCriticalPercent = 0.7f;
    public float minBreedChance = 0.3f;
    public float GestationPeriod = 10f;
    public int maxBabies = 3;
    public float growthRate = 1;
    public float drinkDuration = 6;
    public float eatDuration = 3;
    public float criticalPercent = 0.7f;
    public bool CanHide;
    public bool nocturnalAnimal;
    public float HideTime = 5f;
    public List<Material> maleMaterial;
    public List<Material> femaleMaterial;

    [Header(" Agent Goals/Actions")]
    public AgentGoal[] AgentGoals = new AgentGoal[0];
    public List<GAction> AgentActions = new List<GAction>();


    [Header("Reoccuring Goals")]
    public AgentGoal WorkGoal;
    public AgentGoal EatGoal;
    public AgentGoal RestGoal;
    public AgentGoal AttackGoal;
    public AgentGoal DefendGoal;

    [Header("Human need rates")]
    public float HungerNeedIncrease = 0;
    public float WorkNeedIncrease = 0;
    public float RestNeedIncrease = 0;

}
