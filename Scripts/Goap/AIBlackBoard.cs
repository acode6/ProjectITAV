using GameCreator.Characters;
using SensorToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public struct TargetInfo
{
   public GameObject targetObject;
   public ThreatLevel targetThreatLvl;

}
//class that will hold the current info / states of your agent 
public class AIBlackBoard
{
    //Agent Character
    public Character AgentCharacter;
    public CharacterAnimator AgentAnimator;
    public GworldSettlement AgentSettlement { get; set; }

    //Planner stuff
    public GAction Action { get; }
    public GAgent Agent { get; } //agent
    public AISenses Senses { get; } // //awareness
    public Transform Head { get; } //head
    public Animator Animator { get; } //animator
    public WorkingMemory AgentMemory { get; set; }                                                               
    public GInventory AgentInventory{get;}
    public WorldStates WorldStates { get; set; }
    public AISQUAD CurrentSquad = null;
    public AgentsStates AgentStates { get; }
    //Stat Variables
    public float AgentHealth { get; set; }
    public float AgentStamina { get; set; }
    public float AgentMana { get; set; }


    public Vector3 Position => Agent.transform.position;
    public Vector3 Forward => Agent.transform.forward;
    public float Time { get; set; }
    public float DeltaTime { get; set; }
    public float GenericTimer { get; set; }
    public List<Resource> KnownResources = new List<Resource>();
    public Resource CurrentFoodTarget { get; set; }
    public Resource CurrentWaterTarget { get; set; }
    public Weapon MainWeapon { get; set; }
    public Weapon SecondaryWeapon { get; set; }
  


    public List<Weapon> KnownWeapons = new List<Weapon>();
    public List<GameObject> PotentialPrey = new List<GameObject>();
    public List<GameObject> PotentialEnemies = new List<GameObject>();
    public List<GameObject> CurrentStimuli = new List<GameObject>();
  
    //Human agents specific actions
    public GworldBuilding AgentHome { set; get; }


    //For animals
    public List<GAgent> potentialMates = new List<GAgent>();
    public GAgent currentMate { get; set; }
    public GAgent mateGenes { get; set; }
    public GAgent myMother { get; set; }
    public GAgent myFather { get; set; }
    public GAgent currentPrey { get; set; }
    public GameObject CurrentEnemy { get; set; }
    public GameObject CurrentTarget { get; set; }
    public bool AgentEating { get; set; }
    //landmarks
    public List<LandMark> KnownLandMarks = new List<LandMark>();
    public List<GameObject> VisitedLandMarks = new List<GameObject>();
    public LandMark CurrentLandMark { get; set; }

    //Agent untility
    public MobileAgentSensor SensorEyes { get; set; }
    public bool canSense { get; set;  }
    public AIAwareness myAwareness { get; set; }
    public bool Defensive = false;

    public AIBlackBoard(GAgent agent, AISenses senses, Transform head, GInventory inventory, Character character, CharacterAnimator characterAnimator, AgentsStates agentStates)
    {
        
        
        Agent = agent;
        Senses = senses;
        Senses.agentLocal = Agent.transform;
        Head = head;           
        AgentInventory = inventory;
        AgentCharacter = character;
        AgentStates = agentStates;
        AgentAnimator = characterAnimator;
        myAwareness = AIAwareness.Idle;
        canSense = true;
        CurrentEnemy = null;
        WorldStates = Gworld.Instance.GetWorld();
    }
}
