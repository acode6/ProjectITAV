using GameCreator.Characters;
using GameCreator.Melee;
using GameCreator.Shooter;
using GameCreator.Stats;
using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class States
{
    public bool onGround;
    public bool isRunning;
    public bool isDodging;
    public bool isLockedOn;
    public bool WeaponDrawn;
    public bool isMoveEnabled;
    public bool isAttacking;
    public bool isBlocking;
    public bool isAiming;
    public bool isUsingItem;
    public bool isAbleToBeParried;
    public bool isParryOn;
    public bool isInteracting;
    public bool isInvincible;
    public bool IsGettingHit;
    public bool isCrouching;
    public bool inMainMenu;
    public bool inAdminMenu;

}

[System.Serializable]
public class InputVariables
{
    public float vertical;
    public float horizontal;
    
    //Maps to mouse keys
    public bool fire1;
    public bool fire2;
    public bool fire3;
    public bool fire4;

    //Maps to input key
    public bool interacting;

}

public enum ArmedState
{
    Tool, MeleeWeapon, RangeWeapon
}

public class PlayerCharacter : MonoBehaviour
{
    //Player States
    public States states;
    public InputVariables playerInputs;
    public Transform mTransform;
    public ListVariables lockOnTargets;
    //Player inputs
    bool rightmouseClick;
    bool leftmouseClick;
    
    public Species PlayerSpecies;
    public ThreatLevel PlayerThreatLvl;
    public GameObject EquippedWeapon;
    public float attackRange;
    public GameObject Attacker; //Agent attacks you

    private List<GameObject> attackers; //all agents about to attack
    public int simultaneousAttackers = 1;
    public float currentTotalDamage = 0f;
    public float currentTotalDefense = 0f;
    private float baseDamage = 10f;
    private float baseDefense = 5f;
    float timeSinceLastHit;
    float timeSinceLastAttack;

    //Stats
    public float playerSpeed=3f;
    public float playerSprintSpeed=3f;
    public float playerHealth;
    public float playerStamina;
    public float bodyTemperature;
    public float movementSpeed;
    public  float playerMovemnentMod;
    public  float hunger;
    public  float thirst;
    public float maxCarryWeight;
    public float hungerDecreaseRate = .01f;
    public float thirstDecreaseRate = .01f;
    public float sprintStaminaDrainRate = .01f;
    public float AttackDrainRate = .01f;
    public float dodgeCost = 10f;

    public Stats playerStats;
    public TimeKeeper timeKeeper;

    public CharacterMelee PlayerMeleeComponenent;
    public CharacterShooter PlayerShooterComponent;
    public  Character playerCharacterGC;
    public CharacterAnimator PlayerCharacterAnimator;

    bool registerAttack;
    private void Awake()
    {
        attackers = new List<GameObject>();
    }
    // Start is called before the first frame update
    void Start()
    {
      //  timeKeeper = GameObject.FindGameObjectWithTag("TimeSystem").GetComponent<TimeKeeper>();
        playerStats = gameObject.GetComponent<Stats>();
        playerHealth = playerStats.GetAttrValue("health", playerStats);
        playerStamina = playerStats.GetAttrValue("stamina", playerStats);
        bodyTemperature = playerStats.GetAttrValue("bodyTemperature", playerStats);
        hunger = playerStats.GetAttrValue("hunger", playerStats);
        thirst = playerStats.GetAttrValue("thirst", playerStats);
        PlayerMeleeComponenent = gameObject.GetComponent<CharacterMelee>();
        PlayerShooterComponent = gameObject.GetComponent<CharacterShooter>();
        playerCharacterGC = gameObject.GetComponent<Character>();
        PlayerCharacterAnimator = gameObject.GetComponent<CharacterAnimator>();
        mTransform = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerStates();
        UpdatePlayerStats();

       
        CheckForInteractionInput();
        if (states.isRunning)
        {
            playerCharacterGC.characterLocomotion.runSpeed = playerSprintSpeed;
        }
        else
        {
            playerCharacterGC.characterLocomotion.runSpeed = playerSpeed;

        }

        if (states.isAttacking || states.isBlocking)
        {
            playerCharacterGC.characterLocomotion.faceDirection = CharacterLocomotion.FACE_DIRECTION.CameraDirection;
        }
        else
        {

            playerCharacterGC.characterLocomotion.faceDirection = CharacterLocomotion.FACE_DIRECTION.MovementDirection;
        }
    }
    private void FixedUpdate()
    {


    }

    //checks for interaction inputs such as attacks or activating an item
    //bool so that some inputs don't overlap 
    bool CheckForInteractionInput()
    {
        if (playerInputs.fire1)
        {
            if (Time.realtimeSinceStartup - timeSinceLastAttack > 0.5f)
            {
                registerAttack = false;
            }
           
          
            HandleAction();
            return true;
        }
        if (playerInputs.fire2)
        {

            return true;
        }
        if (playerInputs.fire3)
        {

            return true;
        }
        if (playerInputs.fire4)
        {

            return true;
        }
        return false;
    }

    //Will check input and handle it based on what is equipped in hands

    void HandleAction() //getting scriptable object action
    {
        //switch based on the action type
        if(playerStamina > 5 && !states.isDodging)
        {
            PlayerMeleeComponenent.Execute(CharacterMelee.ActionKey.A);
        }
        
        


        


    }

    //Updates the player's current stats
    void UpdatePlayerStats()
    {
        playerHealth = playerStats.GetAttrValue("health", playerStats);
        playerStamina = playerStats.GetAttrValue("stamina", playerStats);
        if (states.isRunning)
        {
            playerStats.SetAttrValue("stamina", playerStamina - (Time.deltaTime * sprintStaminaDrainRate), true);
        }
        if (states.isAttacking)
        {
            if(registerAttack == false)
            {
                timeSinceLastAttack = Time.realtimeSinceStartup;

                registerAttack = true;
                playerStats.SetAttrValue("stamina", playerStamina - AttackDrainRate, true);
            }

            

        }
       
        // 
        //hunger -= (Time.deltaTime * timeKeeper.timeScale) * hungerDecreaseRate;
        //playerStats.SetAttrValue("hunger", hunger,true);
        //thirst -= (Time.deltaTime * timeKeeper.timeScale) * thirstDecreaseRate;
        //playerStats.SetAttrValue("thirst", thirst, true);
    }

   
    //updates the current states of the player character
    void UpdatePlayerStates()
    {
        states.onGround = playerCharacterGC.IsGrounded();
        VariablesManager.SetGlobal("OnAir", !states.onGround);
        if (states.IsGettingHit)
        {
            //setting an invincibility frame after getting hit
            if (Time.realtimeSinceStartup - timeSinceLastHit > 0.2f)
            {
                states.IsGettingHit = false;
            }
        }

        states.isDodging = playerCharacterGC.characterLocomotion.currentLocomotionSystem.isDashing;
        states.isAttacking = PlayerMeleeComponenent.IsAttacking;
        states.isBlocking = PlayerMeleeComponenent.IsBlocking;
        states.isAiming = PlayerShooterComponent.isAiming;

        //if (states.isRunning)
        //  //spring speed
        //  //change state
        //else
        //  //  regular speed
        //  //change state back

        //Debug.Log("Is Grounded" + states.onGround);
        //Debug.Log("Is gettiing hit" + states.IsGettingHit);
        //Debug.Log("Is dodging" + states.isDodging);
        //Debug.Log("Is attacking" + states.isAttacking);
        //Debug.Log("Is blocking" + states.isBlocking);
    }


    //AI uses this to request attack 
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

   
    //notify damage to the player and the agent that hit you
    public void NotifyDamage()
    {


        Attacker =(GameObject) VariablesManager.GetLocal(gameObject, "Attacker", true);


        if(Attacker.gameObject.GetComponent<GAgent>() != null)
        {

            GAgent targetAgent = Attacker.gameObject.GetComponent<GAgent>();
            WMFact checkFact = targetAgent.BlackBoard.AgentMemory.FindMemoryFact(AddAgentFact(AgentFact.AttackLanded, gameObject, targetAgent.BlackBoard.Agent));
            if (checkFact != null)
            {



                checkFact.factConfidence = .5f;


            }

            if (checkFact == null)
            {
                checkFact = AddAgentFact(AgentFact.AttackLanded, gameObject, targetAgent.BlackBoard.Agent);
                targetAgent.BlackBoard.AgentMemory.AddMemoryFact(checkFact);

            }

            playerStats.SetAttrValue("health", playerHealth - CalculateDamage(targetAgent.CurrentTotalDamage), true);
        }

        if (!states.IsGettingHit)
        {
            
            states.IsGettingHit = true;
            timeSinceLastHit = Time.realtimeSinceStartup;
        }




    }

    public float CalculateDamage(float damage)
    {
        float total = 0;

        if (damage > 0)
        {
            // damage formula
            
            total = Mathf.Max(1, damage - Mathf.Max(0, currentTotalDefense));
        }

        return total;
    }

    //Adds fact to agent 
    public WMFact AddAgentFact(AgentFact fact, GameObject FactObject, GAgent agent)
    {

        WMFact statFact = new WMFact();
        statFact.Fact = fact;
        statFact.FactState = true;
        statFact.SourceObject = FactObject;
        statFact.factConfidence = 1f;


        return statFact;


    }
}
