using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using GameCreator.Core;
using GameCreator.Variables;
using GameCreator.Stats;
using GameCreator.Characters;
using GameCreator.Melee;
using Pathfinding;
using GameCreator.Shooter;
using Pathfinding.RVO;
using SensorToolkit;
using static System.Math;

#if UNITY_EDITOR
using UnityEditor;
#endif

//class for subgoals agents has to accommplish
public class SubGoal
{



    public Dictionary<string, bool> goal; //a dictionary of subgoals and their priority
    public string GoalString;
    public bool removableGoal;

    //constructor of a subgoal 
    public SubGoal(string s, bool i, bool r) 
    {
        GoalString = s;
        goal = new Dictionary<string, bool>
        {
            { s, i } //add
        }; 

        removableGoal = r;
    }

   
}


public enum Species
{
    Undefined = (1 << 0),
    Plant = (1 << 1),
    Rabbit = (1 << 2),
    Wolf = (1 << 3),
    Deer = (1 << 4),
    Elf = (1 << 5),
    Humans = (1 << 6),
    Orcs = (1 << 7),
    TreeCreature = (1 << 8),
    ElementalGolems = (1 << 9),
    Elementals = (1 << 10),
    KingSnake = (1 << 11),
    GiantSpider = (1 << 12),
    Player = (1 << 13),
    LivingChest = (1 << 14),
    Undead = (1 << 15)


}

public enum ThreatLevel
{
    NoThreat,
    LowThreat,
    MediumThreat,
    HighThreat
}


public enum AgentNeeds
{
    HungerNeed,
    WorkNeed,
    RestNeed

}

[System.Serializable]
public class AgentsStates
{
    public bool working;
    public bool eating;
    public bool resting;
    public bool socalizing;
    public bool interacting;
    public bool sleeping;
}

public class AgentCombatState
{
    public bool dodging = false;
    public bool dodgingRecovery = false;
    public bool blocking = false;
    public bool attacking = false;
    public bool attackingRecovery = false;
    public bool stunned = false;
    public bool running = false;
    public bool unbalanced = false;
    public bool strafing = false;
    public void Reset()
    {
        this.dodging = false;
        this.dodgingRecovery = false;
        this.blocking = false;
        this.attacking = false;
        this.attackingRecovery = false;
        this.stunned = false;
        this.running = false;
        this.unbalanced = false;
    }
}

public abstract class GAgent : MonoBehaviour
{
    public GameObject locotionTester;
    #region Agent Variables

    //Planner / actions
    [Header("Planner Info")]

    GPlanner planner; //planner class
    public List<GAction> actions = new List<GAction>(); //declare a list of actions
    public List<string> actionHistory = new List<string>(); //list of action history
    public Dictionary<SubGoal, float> goals = new Dictionary<SubGoal, float>(); //list of goals
    Queue<GAction> currentPlan;
    public GAction currentAction;
    public SubGoal currentGoal;
    bool invoked = false;

    public AgentsStates AgentStates = new AgentsStates();
    //Memory
    public AIBlackBoard BlackBoard;
    public WorkingMemory AgentMemory;
    public WorldStates AgentFacts = new WorldStates(); //beliefs are the agent's own local world states 
    public AgentCombatManager agentCombatManager;

    //Agent Inventory
    public GInventory inventory = new GInventory(); //giving an inventory
    private List<GameObject> attackers; //all agents about to attack


    [Header("Agent Data Info")]
  //  public AgentType AgentType;
    public AgentData DefaultAgentData;
    public AgentData AgentData;
    public List<AgentGoal> IdleGoals;
    public List<AgentGoal> SuspiciousGoals;
    public List<AgentGoal> AlertGoals;
    public List<AgentGoal> IndangerGoals;

    public SubGoal WorkGoal;
    public SubGoal EatGoal;
    public SubGoal RestGoal;
    public SubGoal AttackGoal;
    public SubGoal DefendGoal;
  

    [Header("AgentSpecies")]
    public Species species;
    public ThreatLevel threatLevel;
    public bool offspring = false;
    [EnumFlags]
    public Species AgentFoes;
    [EnumFlags]
    public Biomes AgentHabitatBiomes;


    [Header("Agent Locomotion")]
    public float moveRadius = 20;
    public bool canMove;
    public float characterWalkSpeed;
    public float characterRunSpeed;

    [Header("Combat Info")]
    public AIAwareness myLastAwareness;
    public GameObject currentEnemy;
    public float CurrentTotalDamage;
    public float CurrentTotalDefense;
    private float baseAttack = 10;
    private float baseDefense = 1;

    public float AttackConfidence = 0;
    public float MeleeDistance = 0;
    public float AttackDistance = 0;

    public GameObject baseAbilities;
    public float maxStamina = 30.0f;
    public float followUpWindow = 0.2f;
    public bool telegraphAttacks = false;
    public bool cannotBeStunned = false;
    public float stunResistance = 0.0f;
    public float telegraphRate = 0.6f;
    public AgentCombatState combatStates = new AgentCombatState();
    private float followUpTimer = 0.0f;
    private float attackCooldown = 0.0f;
    private float attackDelay = 0.0f; // disabled 'cause it kinda sucks
    private float stamina;
    private float attackCancelWindow = 0.25f;
    private const float attackAnimationRatio = 1.0f; // magic number, shouldn't change= 
    private const float defaultStun = 2.0f;
    private float blockStartTime = 0.0f;
    private bool disabled = false;
    private bool cancelable = false; // in a state where actions can cancel into others
   

    public int team = 0;
    public bool blocking = false;

    static readonly System.Random prng = new System.Random();



    [Header("Agent Weapons")]
    public GameObject DefaultWeapon;
    public GameObject MainWeapon;
    public GameObject SecondaryWeapon;
    public Weapon CurrentWeapon;
    public Weapon CurrentSecondaryWeapon;

    GameObject mainWeaponModel;
    GameObject secondaryWeaponModel;
    [HideInInspector]
    public EnvironmentManager eManger;

    public bool dead = false;
    public bool weaponDrawn = false;
 

    [Header("Agent Sensor Settings")]
    public SensorySystem sensory;
    public AISenses senses;
    public SensorToolkit.RangeSensor SensorDectect;
    public TriggerSensor SensorEyes;


    Vector3 actionDestination = Vector3.zero; //destination for the agent to go to 
    public Vector3 myLocation = Vector3.zero;
    public bool AtActionLocation = false;
    [HideInInspector]
    public Transform head = null;
   

    private float timer;

    [HideInInspector]
    public Character myCharacter;
    [HideInInspector]
    public CharacterAnimator myCharacterAnimator;
    [HideInInspector]
    public Stats myCharacterStats;
    public bool randomExploration = false; //Activate random exploration of an area in your line of sight
    [HideInInspector]
    public RichAI pathAi;

    [HideInInspector]
    public TimeKeeper timeKeeper;
    [HideInInspector]
    public bool updateAgentMov = true;
    [HideInInspector]
    public CharacterMelee MeleeComponent;
    [HideInInspector]
    public CharacterShooter ShooterComponent;

    [Header("Combat Needs")] 
    public float AttackNeed = 0f;
    public float DefendNeed = 0f;



    [Header("Human Needs")]
    public float BaseMood = 10;
    public float Happiness = 0f;
    public float HungerNeed = 0f;
    public float WorkNeed = 0f;
    public float RestNeed = 0f;



    public AISQUAD currentSquad;
   

    bool previousTarget = false;
    public RVOController RvoController;
    Seeker seeker;
    public bool walk = true;
  
    private int simultaneousAttackers = 1;
    #endregion

    private void Awake()
    {
        attackers = new List<GameObject>();
    }
    #region Agent Init

    public void Start()
    {
        //Incase it's an offspring 
        IdleGoals.Clear();
        IndangerGoals.Clear();
        SuspiciousGoals.Clear();
        AlertGoals.Clear();

        //Instantiate and assigning components 
        AgentData = Instantiate(DefaultAgentData);
        AgentData.Agent = this;
        myCharacterAnimator = GetComponent<CharacterAnimator>();
        myCharacter = GetComponent<Character>();
        myCharacterStats = GetComponent<Stats>();
        pathAi = GetComponent<RichAI>();
        timeKeeper = GameObject.FindGameObjectWithTag("TimeSystem").GetComponent<TimeKeeper>();
        MeleeComponent = GetComponent<CharacterMelee>();
        ShooterComponent = GetComponent<CharacterShooter>();
        RvoController = GetComponent<RVOController>();
        this.gameObject.name = AgentData.AgentName;

        //Assigning character model to gamecreator character  
        if (myCharacterAnimator.animator != null)
        {
            if (offspring == true)
            {
                myCharacterAnimator.ChangeModel(AgentData.BabyModel);
            }
            else if(AgentData.AgentModels.Count == 0)
            {
                myCharacterAnimator.ChangeModel(AgentData.AgentModel);
            }
            else
            {
                int randomModel = UnityEngine.Random.Range(0, AgentData.AgentModels.Count);
              
                myCharacterAnimator.ChangeModel(AgentData.AgentModels[randomModel]);
            }

          
        }

        //Assigning the locomotion state of the agent 
        if (AgentData.LocomotionState != null)
        {
            myCharacterAnimator.ResetControllerTopology(AgentData.LocomotionState.GetRuntimeAnimatorController());
        }

        //Finding the head transform on the game object to assign the head. Important for sensor
        Transform[] children = transform.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            if (child.name == "Head")
            {
                head = child;

            }

            DefaultAgentData.AgentRenderer = child.gameObject.GetComponent<SkinnedMeshRenderer>();
        }

        //Setting basic Agent Info based on Agent data
        
        characterWalkSpeed = AgentData.Agentwalkspeed + RandomValue();
        characterRunSpeed = AgentData.Agentrunspeed;
        myCharacterStats.SetAttrValue("health", AgentData.AgentHealth);
        
        myCharacterStats.SetAttrValue("stamina", AgentData.AgentStamina);
        myCharacterStats.SetStatBase("wisdom", AgentData.AgentMana, true);
      
        //get actions from agent data
        //for each action set this agent as this
        //Instantiated so that actions dont get changed by other agents
        foreach (GAction AgentAction in AgentData.AgentActions)
        {
            GAction Newaction = Instantiate(AgentAction);
            Newaction.ThisAgent = AgentData.Agent;
            actions.Add(Newaction);
        }
      

      
        //Assigning sensors and the blackboard here
        SensorDectect = transform.Find("SensorDetection").GetComponent<SensorToolkit.RangeSensor>();
        SensorEyes = transform.Find("SensorEyes").GetComponent<TriggerSensor>();
       
        sensory = new SensorySystem(this);
        BlackBoard = new AIBlackBoard(this, senses, head, inventory, myCharacter, myCharacterAnimator, AgentStates);
        AgentMemory = new WorkingMemory(this.AgentFacts, BlackBoard);
        agentCombatManager = new AgentCombatManager(MeleeComponent, ShooterComponent, this);
        BlackBoard.AgentMemory = AgentMemory;
        myLastAwareness = BlackBoard.myAwareness;
        species = AgentData.AgentSpecies;
        AgentFoes = AgentData.AgentFoes;
        AgentHabitatBiomes = AgentData.Biomes;
        threatLevel = AgentData.threatLevel;
        senses.DetectRadius = AgentData.DetectRadius;
        senses.viewAngle = AgentData.EyeDetectAngle;

          SensorDectect.SensorRange = AgentData.DetectRadius;
        transform.Find("SensorEyes").GetComponent<FOVCollider>().FOVAngle = AgentData.EyeDetectAngle;
        transform.Find("SensorEyes").GetComponent<FOVCollider>().Length = AgentData.DetectRadius;
        transform.Find("SensorEyes").GetComponent<FOVCollider>().CreateCollider();
        if (SensorEyes != null)
        {
            BlackBoard.SensorEyes = GetComponent<MobileAgentSensor>();
        }
        //Copying goals from agent data
        foreach (AgentGoal g in AgentData.AgentGoals)
        {
            AgentGoal instanitiateGoal = Instantiate(g);

            if (g.AwarenessType == AIAwareness.Idle)
            {
                IdleGoals.Add(instanitiateGoal);
            }
            if (g.AwarenessType == AIAwareness.Searching)
            {
                SuspiciousGoals.Add(instanitiateGoal);
            }
            if (g.AwarenessType == AIAwareness.Combat)
            {
                AlertGoals.Add(instanitiateGoal);
            }
            if (g.AwarenessType == AIAwareness.Fleeing)
            {
                IndangerGoals.Add(instanitiateGoal);
            }


        }

       
        eManger = GameObject.FindGameObjectWithTag("EManager").GetComponent<EnvironmentManager>();
        eManger.Register(this);
        seeker  = GetComponent<Seeker>();

        Init();

    }
    public void InitStartWeapon()
    {
        Weapon mainCheck = null;
        //Because gamecreator is giving problems when trying to run both the melee draw and range draw coroutines
        //Checking here if the main weapon is a range weapon and if it is them the sub weapon must be a melee weapon 
        if (AgentData.MainWeapon != null)
        {
           mainCheck = AgentData.MainWeapon.GetComponent<Weapon>();
        }
      

        if (AgentData.DefaultWeapon != null )
        {
            if(mainCheck == null || mainCheck != null && mainCheck.WeaponType == WeaponType.meleeWeapon)
            {
                Weapon thisWeapon = AgentData.DefaultWeapon.GetComponent<Weapon>();
                if (thisWeapon.WeaponType == WeaponType.meleeWeapon)
                {

                    CoroutinesManager.Instance.StartCoroutine(MeleeComponent.Draw(thisWeapon.GCWeapon, thisWeapon.GCWeapon.defaultShield));
                    BlackBoard.Senses.MeleeRange = thisWeapon.meleeAttackDistance;
                    BlackBoard.Senses.LungeRange = thisWeapon.lungeAttackDistance;
                }

                if (DefaultAgentData.MainWeapon != null)
                {
                    MainWeapon = DefaultAgentData.MainWeapon;
                    BlackBoard.MainWeapon = MainWeapon.GetComponent<Weapon>();

                }
                if (DefaultAgentData.SecondaryWeapon != null)
                {
                    SecondaryWeapon = DefaultAgentData.SecondaryWeapon;
                    BlackBoard.SecondaryWeapon = SecondaryWeapon.GetComponent<Weapon>();

                }

                if (MainWeapon != null || SecondaryWeapon != null)
                {
                    BlackBoard.Agent.AgentFacts.SetState(AgentGoals.Armed.ToString(), true);
                }
                else
                {
                    BlackBoard.Agent.AgentFacts.SetState(AIWorldState.Armed.ToString(), false);
                }
            }
            else if(mainCheck != null && mainCheck.WeaponType != WeaponType.meleeWeapon)
            {
                Weapon thisWeapon = null;
             
                if(AgentData.SecondaryWeapon != null)
                {
                    thisWeapon = AgentData.SecondaryWeapon.GetComponent<Weapon>();
                }
               
                if(thisWeapon != null)
                {
                    Debug.Log("Wanna be here1");
                    if (thisWeapon.WeaponType == WeaponType.meleeWeapon)
                    {

                        CoroutinesManager.Instance.StartCoroutine(MeleeComponent.Draw(thisWeapon.GCWeapon, thisWeapon.GCWeapon.defaultShield));
                        BlackBoard.Senses.MeleeRange = thisWeapon.meleeAttackDistance;
                        BlackBoard.Senses.LungeRange = thisWeapon.lungeAttackDistance;
                    }

                    if (DefaultAgentData.MainWeapon != null)
                    {
                        MainWeapon = DefaultAgentData.MainWeapon;

                    }
                    if (DefaultAgentData.SecondaryWeapon != null)
                    {
                        SecondaryWeapon = DefaultAgentData.SecondaryWeapon;
                    }

                    if (MainWeapon != null || SecondaryWeapon != null)
                    {
                        BlackBoard.Agent.AgentFacts.SetState(AgentGoals.Armed.ToString(), true);
                    }
                    else
                    {
                        BlackBoard.Agent.AgentFacts.SetState(AIWorldState.Armed.ToString(), false);
                    }
                }
                else
                {
                    Debug.Log("Wanna be here");
                    thisWeapon = AgentData.DefaultWeapon.GetComponent<Weapon>();
                    if (thisWeapon.WeaponType == WeaponType.meleeWeapon)
                    {

                        CoroutinesManager.Instance.StartCoroutine(MeleeComponent.Draw(thisWeapon.GCWeapon, thisWeapon.GCWeapon.defaultShield));
                        BlackBoard.Senses.MeleeRange = thisWeapon.meleeAttackDistance;
                        BlackBoard.Senses.LungeRange = thisWeapon.lungeAttackDistance;
                    }

                    if (DefaultAgentData.MainWeapon != null)
                    {
                        MainWeapon = DefaultAgentData.MainWeapon;

                    }
                    if (DefaultAgentData.SecondaryWeapon != null)
                    {
                        SecondaryWeapon = DefaultAgentData.SecondaryWeapon;
                    }

                    if (MainWeapon != null || SecondaryWeapon != null)
                    {
                        BlackBoard.Agent.AgentFacts.SetState(AgentGoals.Armed.ToString(), true);
                    }
                    else
                    {
                        BlackBoard.Agent.AgentFacts.SetState(AIWorldState.Armed.ToString(), false);
                    }
                }
              


            }
            CurrentTotalDamage = baseAttack + AgentData.AgentBaseDamage;
            CurrentTotalDefense = baseDefense + AgentData.AgentBaseDefense;
        }

       

        else
        {
            Debug.Log("Error Check default wep");
        }


    }
    public void UpdateAgentGoals()
    {
        if (BlackBoard.Agent.goals.Count > 0)
        {
            BlackBoard.Agent.goals.Clear();
           
        }



        switch (BlackBoard.myAwareness)
        {
            case AIAwareness.Idle:
                if (BlackBoard.Agent.currentAction != null && BlackBoard.Agent.currentAction.running == true)
                {
                    BlackBoard.Agent.CancelCurrentGoal();
                }
                if (weaponDrawn == true && !dead)
                {
                    DrawWeapon();
                }

                foreach (AgentGoal G in IdleGoals)
                {
                    SubGoal goal = new SubGoal(G.Satisfies.ToString(), G.state, G.RemovableGoal);
                    this.BlackBoard.Agent.goals.Add(goal, G.GoalPriority);
                }

                if (AgentData.WorkGoal != null)
                {
                   
                    WorkGoal= new SubGoal(AgentData.WorkGoal.Satisfies.ToString(), AgentData.WorkGoal.state, AgentData.WorkGoal.RemovableGoal);
                    BlackBoard.Agent.goals.Add(WorkGoal, AgentData.WorkGoal.GoalPriority);
                }
                if (AgentData.EatGoal != null)
                {
                    EatGoal = new SubGoal(AgentData.EatGoal.Satisfies.ToString(), AgentData.EatGoal.state, AgentData.EatGoal.RemovableGoal);
                    BlackBoard.Agent.goals.Add(EatGoal, AgentData.EatGoal.GoalPriority);
                }
                if (AgentData.RestGoal != null)
                {
                    RestGoal = new SubGoal(AgentData.RestGoal.Satisfies.ToString(), AgentData.RestGoal.state, AgentData.RestGoal.RemovableGoal);
                    BlackBoard.Agent.goals.Add(RestGoal, AgentData.RestGoal.GoalPriority);
                }

               
                
                



                break;
            case AIAwareness.Searching:
                if (BlackBoard.Agent.currentAction != null && BlackBoard.Agent.currentAction.running == true)
                {
                    BlackBoard.Agent.CancelCurrentGoal();
                }
                foreach (AgentGoal G in SuspiciousGoals)
                {
                    SubGoal goal = new SubGoal(G.Satisfies.ToString(), G.state, G.RemovableGoal);
                    this.BlackBoard.Agent.goals.Add(goal, G.GoalPriority);
                }

                break;
            case AIAwareness.Combat:
                if (BlackBoard.Agent.currentAction != null && BlackBoard.Agent.currentAction.running == true)
                {


                    BlackBoard.Agent.CancelCurrentGoal();
                }
                if (weaponDrawn == false)
                {
                    DrawWeapon();
                }

                foreach (AgentGoal G in AlertGoals)
                {
                    SubGoal goal = new SubGoal(G.Satisfies.ToString(), G.state, G.RemovableGoal);
                    this.BlackBoard.Agent.goals.Add(goal, G.GoalPriority);
                }
                if (AgentData.AttackGoal != null)
                {
                    AttackGoal = new SubGoal(AgentData.AttackGoal.Satisfies.ToString(), AgentData.AttackGoal.state, AgentData.AttackGoal.RemovableGoal);
                    BlackBoard.Agent.goals.Add(AttackGoal, AgentData.AttackGoal.GoalPriority);
                }
                 if (AgentData.DefendGoal != null)
                {
                    DefendGoal = new SubGoal(AgentData.DefendGoal.Satisfies.ToString(), AgentData.DefendGoal.state, AgentData.DefendGoal.RemovableGoal);
                    BlackBoard.Agent.goals.Add(DefendGoal, AgentData.DefendGoal.GoalPriority);
                }




                break;
            case AIAwareness.Fleeing:
                if (BlackBoard.Agent.currentAction != null && BlackBoard.Agent.currentAction.running == true)
                {


                    BlackBoard.Agent.CancelCurrentGoal();
                }
                foreach (AgentGoal G in IndangerGoals)
                {
                    SubGoal goal = new SubGoal(G.Satisfies.ToString(), G.state, G.RemovableGoal);
                    this.BlackBoard.Agent.goals.Add(goal, G.GoalPriority);
                }



                break;
            default:
                break;
        }


    }

    #endregion

    #region Goap Scripts

    void CompleteAction()
    {
        
        currentAction.running = false;
        currentAction.remainingduration = 0;
        currentAction.ActionEffect();
       actionHistory.Add(currentAction.name);
        currentAction = null;
        invoked = false;


    }
    void CancelAction()
    {
        currentAction.running = false;
        currentAction.InterruptActionCleanUp();
       
        invoked = false;
    }
    public void CancelCurrentGoal()
    {


        // Cancel the CompleteAction method as this has a timer on it which we don't want to run
        CancelInvoke("CompleteAction");

        // Use CancelAction instead of CompleteAction
        CancelAction();
        actionHistory.Add(currentAction.name);
        // Remove the current action and queue
        currentAction = null;
     //   Debug.Log("Action Queue" + actionQueue.Count);
        if(currentPlan == null)
        {
            return;
        }
        if (currentPlan.Count > 0 && currentPlan!=null)
            currentPlan.Clear();
    }

    #endregion

    #region Belief Mods

    public void ShotLanded()
    {
        Debug.Log("Called");
        WMFact checkFact = BlackBoard.AgentMemory.FindMemoryFact(AddAgentFact(AgentFact.AttackLanded, gameObject, BlackBoard.Agent));
        if (checkFact != null)
        {



            checkFact.factConfidence = 1f;


        }

        if (checkFact == null)
        {
            checkFact = AddAgentFact(AgentFact.AttackLanded, gameObject, BlackBoard.Agent);
            BlackBoard.AgentMemory.AddMemoryFact(checkFact);

        }

        




    }
    public void BlockedAttack()
    {
        Debug.Log("Block attack Called");
        WMFact checkFact = BlackBoard.AgentMemory.FindMemoryFact(AddAgentFact(AgentFact.BlockedAttack, gameObject, BlackBoard.Agent));
        if (checkFact != null)
        {



            checkFact.factConfidence = 1f;


        }

        if (checkFact == null)
        {
            checkFact = AddAgentFact(AgentFact.BlockedAttack, gameObject, BlackBoard.Agent);
            BlackBoard.AgentMemory.AddMemoryFact(checkFact);

        }







    }
    public void CounteredAttack()
    {
       
        WMFact checkFact = BlackBoard.AgentMemory.FindMemoryFact(AddAgentFact(AgentFact.CounteredAttack, gameObject, BlackBoard.Agent));
        if (checkFact != null)
        {



            checkFact.factConfidence = 1f;
            

        }

        if (checkFact == null)
        {
          
            checkFact = AddAgentFact(AgentFact.CounteredAttack, gameObject, BlackBoard.Agent);
            checkFact.FactState = true;
            BlackBoard.AgentMemory.AddMemoryFact(checkFact);

        }







    }

    public bool RequestCounter()
    {
        float chance = AgentData.AgentCounterChance;
        //Debug.Log("Counter request");
        if (UnityEngine.Random.value > chance)
        {
       //     Debug.Log("Counter request fail");

            return false;
        }
     //w   Debug.Log("Counter request sucess");

        return true;
    }

    public bool RequestDodge()
    {
        float chance = AgentData.AgentEvadeChance;
        if (UnityEngine.Random.value > chance)
        {

            return false;
        }

        return true;
    }
    public bool RequestSkill(float skillChance)
    {
        float chance = skillChance;
        if (UnityEngine.Random.value > chance)
        {

            return false;
        }

        return true;
    }

    public void NotifyDamage()
    {


        GameObject Attacker = (GameObject)VariablesManager.GetLocal(gameObject, "Attacker", true);


        if (Attacker.gameObject.GetComponent<GAgent>() != null)
        {

            GAgent targetAgent = Attacker.gameObject.GetComponent<GAgent>();
            WMFact checkFact = targetAgent.BlackBoard.AgentMemory.FindMemoryFact(AddAgentFact(AgentFact.AttackLanded, gameObject, targetAgent.BlackBoard.Agent));
            if (checkFact != null)
            {



                checkFact.factConfidence = 1f;


            }

            if (checkFact == null)
            {
                checkFact = AddAgentFact(AgentFact.AttackLanded, gameObject, targetAgent.BlackBoard.Agent);
                targetAgent.BlackBoard.AgentMemory.AddMemoryFact(checkFact);

            }
            //put in a script when damage calculations get more complex


           myCharacterStats.SetAttrValue("health", BlackBoard.AgentHealth - CalculateDamage(targetAgent.CurrentTotalDamage), true);
        }
        if (Attacker.gameObject.GetComponent<PlayerCharacter>() != null)
        {

            PlayerCharacter player = Attacker.gameObject.GetComponent<PlayerCharacter>();
            myCharacterStats.SetAttrValue("health", BlackBoard.AgentHealth - CalculateDamage(player.currentTotalDamage), true);


        }
   



    }

    public float CalculateDamage(float damage)
    {
        float total = 0;

        if (damage > 0)
        {
            // potential damage formula
            //var totaldamage = amount - Mathf.Max(0, armor - penetration);
            total = Mathf.Max(1, damage - Mathf.Max(0, CurrentTotalDefense));
        }

        return total;
    }

    public WMFact AddAgentFact(AgentFact fact, GameObject FactObject, GAgent agent)
    {

        WMFact statFact = new WMFact();
        statFact.Fact = fact;

        statFact.SourceObject = FactObject;
        statFact.factConfidence = 1f;


        return statFact;


    }

    #endregion

    #region Agent Utilities
    //void AttachObject()
    //{
    //    HumanBodyBones bone = HumanBodyBones.RightHand;
    //    TargetGameObject instance = new TargetGameObject();
    //    Space space = Space.Self;
    //    Vector3 position = Vector3.zero;
    //    Vector3 rotation = Vector3.zero;


    //}
    IEnumerator SheathWeapon()
    {


        yield return new WaitUntil(NotAttackingCheck);
        CoroutinesManager.Instance.StartCoroutine(MeleeComponent.Sheathe());

    }
    bool NotAttackingCheck()
    {
        if(MeleeComponent.IsAttacking == true)
        {
            return false;
        }
        else
        {
            return true;
        }
 
    }
    public void DrawWeapon()
    {
        Weapon thisWeapon = null;
        Weapon thisSecondWeapon = null;
        if (MainWeapon == null && SecondaryWeapon == null)
        {
            Debug.Log("No Weapon Equipped");
            return;
        }
        if (MainWeapon != null)
        {
            thisWeapon = MainWeapon.GetComponent<Weapon>();
        }
        if (SecondaryWeapon != null)
        {
            thisSecondWeapon = SecondaryWeapon.GetComponent<Weapon>();
        }

      
        if (weaponDrawn == false)
        {
            if (SecondaryWeapon != null)
            {


                //melee secondary should have already been drawn at start
                if (thisSecondWeapon.WeaponType == WeaponType.meleeWeapon)
                {

                    BlackBoard.SecondaryWeapon = thisSecondWeapon;



                }
                if (thisSecondWeapon.WeaponType == WeaponType.rangeWeapon)
                {
                    GameCreator.Shooter.Weapon wWep = Instantiate(thisSecondWeapon.GCRWeapon);

                    //Current work around to drawing both weapons at the same time
                    ShooterComponent.currentWeapon = (wWep);
                    ShooterComponent.currentAmmo = (Instantiate(thisSecondWeapon.GCRWeapon.defaultAmmo));
                    // CoroutinesManager.Instance.StartCoroutine(ShooterComponent.ChangeWeapon(wWep, ));
                    BlackBoard.Agent.AgentFacts.SetState(AgentFact.RangeWeapon.ToString(), true);
                    BlackBoard.SecondaryWeapon = thisSecondWeapon;



                }
                if (thisSecondWeapon.WeaponType == WeaponType.magicWeapon)
                {
                    GameCreator.Shooter.Weapon wWep = Instantiate(thisSecondWeapon.GCRWeapon);


                    CoroutinesManager.Instance.StartCoroutine(ShooterComponent.ChangeWeapon(wWep, Instantiate(thisSecondWeapon.GCRWeapon.defaultAmmo)));




                    BlackBoard.Agent.AgentFacts.SetState(AgentFact.MagicWeapon.ToString(), true);
                    BlackBoard.SecondaryWeapon = thisSecondWeapon;



                }
            }

            if (MainWeapon != null )
            {
               

                if (thisWeapon.WeaponType == WeaponType.meleeWeapon)
                {
                    MeleeWeapon mWep = Instantiate(thisWeapon.GCWeapon);
                    MeleeShield wShield = null;
                    if (thisWeapon.GCWeapon.defaultShield != null)
                    {
                        wShield = Instantiate(thisWeapon.GCWeapon.defaultShield);
                        wShield.perfectBlockWindow = AgentData.AgentCounterChance;
                    }


                    CoroutinesManager.Instance.StartCoroutine(MeleeComponent.Draw(mWep, wShield));

                    BlackBoard.MainWeapon = thisWeapon;
                    BlackBoard.Agent.AgentFacts.SetState(AgentFact.MeleeWeapon.ToString(), true);
                    BlackBoard.Senses.MeleeRange = thisWeapon.meleeAttackDistance;
                    BlackBoard.Senses.LungeRange = thisWeapon.lungeAttackDistance;
                   
                }
                if (thisWeapon.WeaponType == WeaponType.rangeWeapon)
                {
                    GameCreator.Shooter.Weapon wWep = Instantiate(thisWeapon.GCRWeapon);

                    ShooterComponent.currentWeapon = (wWep);
                    CoroutinesManager.Instance.StartCoroutine(ShooterComponent.ChangeWeapon(wWep, Instantiate(thisWeapon.GCRWeapon.defaultAmmo)));
                  
                    BlackBoard.Agent.AgentFacts.SetState(AgentFact.RangeWeapon.ToString(), true);
                    BlackBoard.MainWeapon = thisWeapon;

                    
                }
                if (thisWeapon.WeaponType == WeaponType.magicWeapon)
                {
                    GameCreator.Shooter.Weapon wWep = Instantiate(thisWeapon.GCRWeapon);

                    ShooterComponent.currentWeapon = (wWep);

                    CoroutinesManager.Instance.StartCoroutine(ShooterComponent.ChangeWeapon(wWep, Instantiate(thisWeapon.GCRWeapon.defaultAmmo)));

                    BlackBoard.Agent.AgentFacts.SetState(AgentFact.MagicWeapon.ToString(), true);
                    BlackBoard.MainWeapon = thisWeapon;
                   

                }
            }

          
          //  Debug.Log("At Draw  Wep " + BlackBoard.CurrentWeapon);

         //   Debug.Log("At Draw Secondary Wep " + BlackBoard.CurrentSecondaryWeapon);

            weaponDrawn = true;
        }
        else
        {
            Debug.Log("Weapon already drawn");


            if (BlackBoard.MainWeapon.WeaponType == WeaponType.meleeWeapon)
            {


                StartCoroutine(SheathWeapon());

            }

            //if (thisWeapon.WeaponType == WeaponType.magicWeapon || thisWeapon.WeaponType == WeaponType.rangeWeapon)
            //{
            //    ShooterComponent.ChangeWeapon(null);
            //}
            
            weaponDrawn = false;
        }



    }
    public void UpdateWeapon()
    {



        if (MainWeapon != null)
        {
            Weapon thisWeapon = MainWeapon.GetComponent<Weapon>();


            if (thisWeapon.WeaponType == WeaponType.meleeWeapon)
            {
                MeleeComponent.previousWeapon = thisWeapon.GCWeapon;
                MeleeComponent.previousShield = thisWeapon.GCWeapon.defaultShield;
                MeleeComponent.previousShield.perfectBlockWindow = DefaultAgentData.AgentCounterChance;
                BlackBoard.Agent.AgentFacts.SetState(AIWorldState.ArmedMelee.ToString(), true);
                BlackBoard.Senses.MeleeRange = thisWeapon.meleeAttackDistance;
                BlackBoard.Senses.LungeRange = thisWeapon.lungeAttackDistance;
            }

            if (thisWeapon.WeaponType == WeaponType.rangeWeapon)
            {

                ShooterComponent.ChangeWeapon(thisWeapon.GCRWeapon, thisWeapon.GCRWeapon.defaultAmmo);
                BlackBoard.Agent.AgentFacts.SetState(AgentFact.RangeWeapon.ToString(), true);

            }

            if (BlackBoard.myAwareness == AIAwareness.Combat)
            {
                if (thisWeapon.WeaponType == WeaponType.meleeWeapon)
                {
                    VariablesManager.SetLocal(this.gameObject, "DrawWeapon", true, true);
                    BlackBoard.Agent.AgentFacts.SetState(AgentFact.MeleeWeapon.ToString(), true);

                }

                else if (thisWeapon.WeaponType == WeaponType.rangeWeapon)
                {

                    //   VariablesManager.SetLocal(this.gameObject, "DrawRange", true, true);
                    //  shooterComponent.ChangeWeapon(shooterComponent.currentWeapon, shooterComponent.currentWeapon.defaultAmmo);
                    ShooterComponent.ChangeWeapon(thisWeapon.GCRWeapon, thisWeapon.GCRWeapon.defaultAmmo);
                    BlackBoard.Agent.AgentFacts.SetState(AgentFact.MeleeWeapon.ToString(), true);

                }



            }


        }
        else
        {
            BlackBoard.Agent.AgentFacts.SetState(AIWorldState.Armed.ToString(), false);
            BlackBoard.Agent.AgentFacts.SetState(AgentFact.MeleeWeapon.ToString(), false);
        }



    }
    public void LookAtTarget(GameObject target)
    {
     
        MeleeComponent.SetTargetFocus(target.GetComponent<TargetMelee>());
        
    }
    public void TrackTarget(GameObject target)
    {

         var targetPoint = target.transform.position;
         targetPoint.y = transform.position.y;
         var targetRotation = Quaternion.LookRotation(targetPoint - transform.position, Vector3.up.normalized);
         transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, AgentData.trackSpeed);
    }
    public void TrackTarget(GameObject target, float speed)
    {
        
         var targetPoint = target.transform.position;
         var targetRotation = Quaternion.LookRotation(targetPoint - transform.position, Vector3.up);
         transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed);
    }
    public void UpdateAwareness()
    {
        AIAwareness currentAwareness = BlackBoard.myAwareness;
        if (myLastAwareness == currentAwareness)
        {
            return;
        }

        if (myLastAwareness == AIAwareness.Combat || currentAwareness == AIAwareness.Combat)
        {

            if (currentAwareness == AIAwareness.Combat)
            {

                BlackBoard.Agent.UpdateAgentGoals();

                //update agent
            }
        }
        if (myLastAwareness == AIAwareness.Fleeing || currentAwareness == AIAwareness.Fleeing)
        {

            if (currentAwareness == AIAwareness.Fleeing)
            {

                BlackBoard.Agent.UpdateAgentGoals();

                //update agent
            }
        }
        BlackBoard.Agent.UpdateAgentGoals();
        myLastAwareness = currentAwareness;

    }
    public void Look(Vector3 lookVec, float speed)
    {
        lookVec.y = 0.0f;
        if (transform.forward == lookVec || lookVec == Vector3.zero) return;

        if (speed > 0.0f)
        {
            // HACK : commented out for now, doesn't work on deployed builds
            lookVec = Vector3.RotateTowards(transform.forward, lookVec, speed, 0.0f);
        }
        //iTween.Stop(gameObject, "LookTo");
        //iTween.LookTo(gameObject, transform.position + moveVec, 0.5f);
        //iTween.RotateTo(gameObject, 
        //transform.LookAt(transform.position + moveVec);
        transform.rotation = Quaternion.LookRotation(lookVec, Vector3.up);
    }
    public bool DodgeAttack(Vector3 Direction)
    {
        //Todo: raycast into the dash direction
        Vector3 PLANE = new Vector3(1, 0, 1);
        Vector3 moveDirection = Vector3.zero;
        moveDirection = Direction - myCharacter.transform.position;
        moveDirection.Scale(PLANE);


        Vector3 charDirection = Vector3.Scale(
               myCharacter.transform.TransformDirection(Vector3.forward),
               PLANE
           );

        float angle = Vector3.SignedAngle(moveDirection, charDirection, Vector3.up);
        AnimationClip clip = null;

        if (angle <= 45f && angle >= -45f) clip = AgentData.DodgeForward;
        else if (angle < 135f && angle > 45f) clip = AgentData.DodgeLeft;
        else if (angle > -135f && angle < -45f) clip = AgentData.DodgeRight;
        else clip = AgentData.DodgeBack;

        bool isDashing = myCharacter.Dash( moveDirection.normalized,  1.2f,  0.4f,  6f );

        if (isDashing && clip != null && myCharacterAnimator != null)
        {
            myCharacterAnimator.CrossFadeGesture(clip, 1f, null, 0.05f, 0.5f);
        }


        return true;
    }
    #endregion

    #region Agent Movement

    public void UpdateAgentMovement()
    {
        if (myCharacter.characterLocomotion.currentLocomotionSystem.isRootMoving == false && dead == false && myCharacter.characterLocomotion.currentLocomotionSystem.isDashing == false && MeleeComponent.IsStaggered == false)
        {
            ILocomotionSystem.TargetRotation cRotation = new ILocomotionSystem.TargetRotation(true, pathAi.transform.forward); 
            myCharacter.characterLocomotion.SetTarget(pathAi.steeringTarget, cRotation, pathAi.endReachedDistance);
            myCharacter.characterLocomotion.angularSpeed = pathAi.rotationSpeed;
        }
        if (myCharacter.characterLocomotion.currentLocomotionSystem.isRootMoving == true)
        {
            pathAi.destination = transform.position;
        }

    }
    public void FollowTarget(GameObject Target)
    {
        pathAi.SearchPath();
        pathAi.destination = Target.transform.position;

    }
    public void FleeFromTarget(GameObject Target)
    {
        Vector3 thePointToFleeFrom = Vector3.zero;

        thePointToFleeFrom = Target.transform.position;


        int theGScoreToStopAt = 10000;


        FleePath path = FleePath.Construct(transform.position, thePointToFleeFrom, theGScoreToStopAt);


        path.aimStrength = .8f;



        path.spread = 20;


        pathAi.SetPath(path);
















    }
    public void StrafeAroundTarget(Vector3 position)
    {


        var perpendicularVec = Vector3.Cross(Vector3.up, position);

        pathAi.destination = perpendicularVec * 1f;


    }
    public void StrafeTowardTarget(Vector3 position)
    {




        pathAi.destination = ((position - transform.position) + transform.position);


    }
    public void RandomExploration()
    {






        if (!pathAi.pathPending && (pathAi.reachedEndOfPath || !pathAi.hasPath) || previousTarget == true)
        {

            pathAi.SearchPath();
            pathAi.destination = PickRandomPoint();



        }









    }
    Vector3 PickRandomPoint()
    {
        var point = UnityEngine.Random.insideUnitSphere * moveRadius;

        point.y = 0;
        point += pathAi.position;
        return point;
    }
    IEnumerator WaitAlittle()
    {

        
        yield return new WaitForSeconds(1f);
        randomExploration = true;
        RandomExploration();
    }
  
    public abstract void UpdateAgentNeed();
    #endregion

    #region Agent Animations

    public void ChangeAgentState(CharacterState characterState)
    {
        if (characterState == null)
        {
            return;
        }

        CharacterAnimation.Layer layer = CharacterAnimation.Layer.Layer1;
        AvatarMask avatarMask = null;


        float weight = 1.0f;
        float transitionTime = 0.1f;
        NumberProperty speed = new NumberProperty(1.0f);

        myCharacterAnimator.SetState(
                        characterState,
                        avatarMask,
                        weight,
                        transitionTime,
                        speed.GetValue(this.gameObject),
                        layer
                    );

    }
    public void ResetAgentState()
    {
        float transitionTime = 0.15f;
        CharacterAnimation.Layer layer = CharacterAnimation.Layer.Layer1;
        myCharacterAnimator.ResetState(transitionTime, layer);

    }
    public void PlayAnimClip(AnimationClip clip)
    {
        NumberProperty speed = new NumberProperty(1.0f);

        if (clip != null && myCharacterAnimator != null)
        {

            myCharacterAnimator.CrossFadeGesture(
            clip, speed.GetValue(this.gameObject), null,
            0.1f, 0.1f
        );
        }

    }
    public void PlayAnimClip(AnimationClip clip, AvatarMask mask)
    {
        NumberProperty speed = new NumberProperty(1.0f);

        if (clip != null && myCharacterAnimator != null)
        {

            myCharacterAnimator.CrossFadeGesture(
            clip, speed.GetValue(this.gameObject), mask,
            0.1f, 0.1f
        );
        }

    }
    public void StopAnimClip()
    {
        myCharacterAnimator.StopGesture(0.1f);

    }

    #endregion

    #region AgentCombatScripts

    void OnRequestAttack(GameObject requestor)
    {
        attackers.RemoveAll(item => item == null);

        if (attackers.Count < simultaneousAttackers)
        {
            if (!attackers.Contains(requestor))
                attackers.Add(requestor);
            //Debug.Log("Requestor " + requestor);
            requestor.GetComponent<GAgent>().agentCombatManager.OnAllowAttack(requestor);
            //   requestor.SendMessage("OnAllowAttack", gameObject);
            // Debug.Log("Attack accepted, current attackers: " + attackers.Count);
        }
        else
        {
            // Debug.Log("Attack REJECTED, current attackers: " + attackers.Count);
        }
    }

    void OnCancelAttack(GameObject requestor)
    {
        // Debug.Log("Requestor " + requestor);
        attackers.Remove(requestor);
    }

    public void OnBash()
    {
        /*
		if(disabled || status.dodging || status.attacking) return;
		
		OnBlockEnd();
		StartCoroutine( "doBash" );
		*/
    }

    public bool OnAttack()
    {
      //  Debug.Log("On Attack");
        // actually just see if I'm disabled or not
        if (blocking || combatStates.stunned || attackCooldown > 0.0f) return false;

       
      

        return true;
    }

    public void OnBlock()
    {
        //if(status.attacking || status.dodging || status.running || mainWeapon.blockPower <= 0) return;

       

        combatStates.blocking = true;
        blockStartTime = Time.time;
        //start blocking
       
        blocking = true;
    }

    public void OnBlockEnd()
    {
        if (combatStates.dodging || combatStates.running || combatStates.attacking) return;

        blocking = false;
        blockStartTime = 0.0f;
       
        //stop blocking

       
    }


   

    public void OnStopAttack()
    {
     
        //Stop Attack

        combatStates.attacking = false;
        combatStates.unbalanced = false;
    }

    public void OnInterrupt()
    {
        gameObject.SendMessage("OnStun", -0.3f);
    }

    public IEnumerator OnStun(float stunDuration)
    {
        if (combatStates.stunned || cannotBeStunned) yield break;

        if (stunDuration == 0.0f)
            stunDuration = defaultStun;

        // HACK: stunDuration of < 0 means "force a stun of this amount"
        if (stunDuration < 0.0f)
        {
            stunDuration *= -1.0f;
        }
        else
        {
            stunDuration -= stunResistance;
            if (stunDuration <= 0.0f)
                stunDuration = 0.5f; //yield break;

        }

       

        if (stunDuration >= 1.0f)
        {
            //change to a stunned state
 
       
        }

        // stop all active processes
        gameObject.BroadcastMessage("OnDisable");
  
      

        this.combatStates.stunned = true;
        yield return new WaitForSeconds(stunDuration);
     

        if (stunDuration >= 1.0f)
        {
            // get up
          //Reset state

            yield return new WaitForSeconds(0.5f);
        }

        gameObject.BroadcastMessage("OnEnable");
        this.combatStates.stunned = false;

        // have to reset other statuses as well
        // I really shouldn't have to do this...
        this.combatStates.Reset();
    }




    public void OnEnable()
    {
        disabled = false;
    }

    public void OnDisable()
    {
        disabled = true;
    }

    public bool isDisabled
    {
        get { return disabled; }
    }
    #endregion


    private void FixedUpdate()
    {
        if (BlackBoard == null)
        {
            return;
        }
        if (BlackBoard != null)
        {
            if (dead == false)
            {
                randomExploration = false;

            }
        }
       

  //      if (status.dodging && !status.dodgingRecovery)
  //      {
  //          //var finalMove = transform.forward  * dodgeSpeed;
  //          //dude.RawMovement(finalMove, false);
  //      }

  //      if (followUpTimer > 0.0f)
  //          followUpTimer -= Time.fixedDeltaTime;

  //      if (attackCooldown > 0.0f)
  //          attackCooldown -= Time.fixedDeltaTime;

  //      var staminaRecovery = 1.0f;
  //      if (stamina < maxStamina)
  //      {
  //          stamina += Time.fixedDeltaTime * staminaRecovery;
  //          if (stamina > maxStamina)
  //              stamina = maxStamina;
  //      }
  //      /*
		

       

    }
    private void Update()
    {
        if(BlackBoard == null)
        {
            return;
        }
        if(BlackBoard != null)
        {
            if (dead == false)
            {

                if (BlackBoard.myAwareness != AIAwareness.Combat)
                {

                    UpdateAgentNeed();

                }

                if(combatStates.strafing == true)
                {
                    myCharacter.characterLocomotion.runSpeed = AgentData.AgentStrafespeed;
                }
                else
                {
                    myCharacter.characterLocomotion.runSpeed = characterWalkSpeed;
                }

                if (walk == true)
                {
                    BlackBoard.AgentCharacter.characterLocomotion.canRun = false;
                }
                else
                {
                    BlackBoard.AgentCharacter.characterLocomotion.canRun = true;
                }

                if (updateAgentMov == true)
                {
                    UpdateAgentMovement();
                }
                if (BlackBoard.myAwareness == AIAwareness.Combat)
                {
                    if (currentSquad != null)
                    {
                        currentSquad.thisAgent = this;
                        BlackBoard.CurrentSquad = currentSquad;

                    }

                    //  agentCombatManager.preyObject = BlackBoard.CurrentEnemy;
                    agentCombatManager.UpdateAgentCombat();
                }
            }

           
        }
      
     
    }
    public virtual void LateUpdate()
    {
        timer += Time.deltaTime;
     
      

        if (BlackBoard == null || sensory == null)
        {

            Debug.Log("Context or sensory problem");
            return;
        }

        BlackBoard.Time = Time.time;
        BlackBoard.DeltaTime = Time.deltaTime;

       
        if (BlackBoard.canSense)
        {
            sensory.Tick(BlackBoard);
            
        }
        if (BlackBoard.AgentHealth <= 0)
        {
            MeleeComponent.SetInvincibility(timer);
            BlackBoard.canSense = false;

            if (BlackBoard.myAwareness != AIAwareness.Idle)
            {
                BlackBoard.myAwareness = AIAwareness.Idle;
                UpdateAwareness();
            }

            if (MeleeComponent.HasFocusTarget)
            {
                MeleeComponent.ReleaseTargetFocus();
            }

            //pathAi.destination = transform.position;
            //pathAi.canMove = false;
            //pathAi.canSearch = false;
            //ILocomotionSystem.TargetRotation cRotation = new ILocomotionSystem.TargetRotation(true, pathAi.transform.forward); ;
            //myCharacter.characterLocomotion.SetTarget(transform.position, cRotation, pathAi.endReachedDistance);
            //myCharacter.characterLocomotion.angularSpeed = pathAi.rotationSpeed;
            if (currentAction != null && currentAction.running == true)
            {
                CancelCurrentGoal();

            }


            Die(CauseOfDeath.Killed);


            return;
        }


        if (CurrentWeapon != null)
        {
            CurrentWeapon = BlackBoard.MainWeapon;
        }
        if (CurrentSecondaryWeapon != null)
        {
            CurrentSecondaryWeapon = BlackBoard.SecondaryWeapon;
        }

        if (myCharacter.characterLocomotion.canRun == false)
        {
            myCharacter.characterLocomotion.runSpeed = characterWalkSpeed;
        }
        if (myCharacter.characterLocomotion.canRun == true)
        {
            if (BlackBoard.Defensive == false)
            {
                myCharacter.characterLocomotion.runSpeed = characterRunSpeed;
            }
            else
            {
                myCharacter.characterLocomotion.runSpeed = characterWalkSpeed;
            }

        }

        AgentMemory.UpdateWorkingMemory();

       




        myLocation = this.transform.position; 

        //if you have no actions and you're idle then stay still
        if(currentAction == null && BlackBoard.myAwareness == AIAwareness.Idle)
        {
            pathAi.destination = myLocation;
        }




        
        //Takes care of action running
        if (currentAction != null && currentAction.running)
        {
            randomExploration = false;


            AtActionLocation = false;
            if (currentAction.target == null)
            {

                currentAction.ActionRunning();
               
                if (!invoked)
                {
                    currentAction.remainingduration = currentAction.duration;
                    if(currentAction != null)
                    {
                        
                        Invoke("CompleteAction", currentAction.duration);
                        invoked = true;
                    }

                }
                else
                {
                    currentAction.remainingduration -= Time.deltaTime;
                }

            }

            if(currentAction == null)
            {
                return;
            }
         
            if (currentAction.target != null && currentAction != null)
            {



           //     Debug.Log("Current action target" + currentAction.target);
              
                
                if (currentAction != null)
                {
                    currentAction.ActionRunning();
                }


                float distanceToTarget = Vector3.Distance(actionDestination, this.transform.position);
                if(distanceToTarget > 1 )
                {
                    


                    if (pathAi.reachedDestination == false && pathAi.hasPath == false)
                    {
                       // Debug.Log("We Lost path");
                        GenerateGoalsFromPath(currentAction.target.transform.position);
                    }
                }

               
          
                if (pathAi.reachedDestination == true)
                {
                  //  Debug.Log("We Are at the action destination");
                    AtActionLocation = true;
                    if (!invoked)
                    {
                        currentAction.remainingduration = currentAction.duration;
                        if (currentAction != null)
                        {
                            Invoke("CompleteAction", currentAction.duration);
                            invoked = true;
                        }


                    }
                    else
                    {
                        currentAction.remainingduration -= Time.deltaTime;
                    }
                }
               
                   

                


            }

            return;
        }




        if (planner == null || currentPlan == null)
        {
            planner = new GPlanner();

            //decision maker goes here
            var sortedGoals = from entry in goals orderby entry.Value descending select entry; //orders goals

            foreach (KeyValuePair<SubGoal, float> sg in sortedGoals)
            {

                currentPlan = planner.plan(actions, sg.Key.goal, AgentFacts);
                if (currentPlan != null)
                {
                   
                    currentGoal = sg.Key;

                    break;
                }
            }
        }

        //pops off last remaining goal and nulls planner -- safety
        if (currentPlan != null && currentPlan.Count == 0)
        {
            
            if (currentGoal.removableGoal)
            {
                goals.Remove(currentGoal);
            }
            //Debug.Log("Nulled planner");
            planner = null;
        }




        if (currentPlan != null && currentPlan.Count > 0)
        {
            currentAction = currentPlan.Dequeue();

           
            if (currentAction.ContextPreConditions())
            {
                if (currentAction.target == null && currentAction.targetTag != "")
                    currentAction.target = GameObject.FindWithTag(currentAction.targetTag);

                if (currentAction.target != null )
                {
                   
              

                    currentAction.running = true;
                    actionDestination = currentAction.target.transform.position;
                    if (currentAction.target.transform.Find("Destination") != null)
                    {
                        actionDestination = currentAction.target.transform.Find("Destination").position;
                    }

                    pathAi.destination = actionDestination;
               
                    previousTarget = true;



                }

                if (currentAction.target == null)
                {

                    previousTarget = false;             
                    currentAction.running = true;










                }


            }
            else
            {

                currentPlan = null;

            }
        }

        if (randomExploration == true)
        {

            RandomExploration();

        }

    }

    void GenerateGoalsFromPath(Vector3 Destination)
    {
        //ToDo: Get path info to destination and see which path is needed and then use this to trigger specific actions 
        // use door , use lever etc
        //Use path finder tags to see which it specifically needs
       // Debug.Log("Destination " + Destination);
        pathAi.destination = Destination;

        if(pathAi.hasPath == false)
        {
            AgentFacts.SetState(AgentFact.NeedPath.ToString(), true);
            SubGoal MoveToGoal = new SubGoal(AgentTask.OpenPath.ToString(), true, true);
            goals.Add(MoveToGoal, 100);
           
           //Todo: get node info and see if it matches the door/ ofmesh link
         //   Debug.Log("Doesn't have path");
            CancelCurrentGoal();
           
          
        }
        
        


    }


    protected virtual void Die(CauseOfDeath cause)
    {
        if (dead == false)
        {
            dead = true;
            if(AgentData.RagdollOnDeath == true)
            {
                myCharacter.SetRagdoll(true, false);

            }

            StartCoroutine("Death");
            Debug.Log(this.gameObject.name + "Died of " + cause.ToString());

        }
    }
    IEnumerator Death()
    {
        //for head rolling bug temp fix
        if (myCharacter.IsRagdoll())
        {
            head.gameObject.GetComponent<Rigidbody>().useGravity = false;
            head.gameObject.GetComponent<Rigidbody>().isKinematic = false;
          

        }
        if(AgentData.RagdollOnDeath == false)
        {
            if (AgentData.DeathStates.Count == 0)
            {
                ChangeAgentState(AgentData.DeathState);

            }
            else
            {
                int randomAnim = UnityEngine.Random.Range(0, AgentData.DeathStates.Count);
                CharacterState DeathAnimclip = AgentData.DeathStates[randomAnim];
                ChangeAgentState(AgentData.DeathStates[randomAnim]);

            }


        }

        yield return new WaitForSeconds(100f);
        Destroy(gameObject);
    }

    static float RandomValue()
    {
        return (float)prng.NextDouble();
    }

    public abstract void Init();


    private void OnDrawGizmos()
    {
        if (BlackBoard == null)
            return;

         senses?.DrawGizmos(BlackBoard);
         sensory?.DrawGizmos(BlackBoard);
        //if (BlackBoard.myAwareness == AIAwareness.Combat)
        //{
        //   Gizmos.color = Color.blue;
        //    Gizmos.DrawWireSphere(transform.position, agentCombatManager.dangerDistance);

        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(transform.position, agentCombatManager.attackDistance);
        //}

#if UNITY_EDITOR
        var task = currentAction;

        Handles.color = Color.cyan;
        Handles.Label(BlackBoard.Head.transform.position + Vector3.up * 1.5f, "Current Awareness: " + BlackBoard.myAwareness.ToString());
        if (BlackBoard.myAwareness == AIAwareness.Combat)
        {
            Handles.Label(BlackBoard.Head.transform.position + Vector3.up * 2f, "Agent Confidence: " + AttackConfidence);
        }


        if (task != null && currentAction.running)
        {
            Handles.Label(BlackBoard.Head.transform.position + Vector3.up, "Current Action: " + task.actionName);

        }
        else if (task != null && !currentAction.running || task == null)
        {
            Handles.Label(BlackBoard.Head.transform.position + Vector3.up, "No Valid Actions");
        }

#endif
    }







}
