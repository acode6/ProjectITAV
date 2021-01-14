using GameCreator.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCreator.Variables;
using GameCreator.Core;
using GameCreator.Melee;





public class StatsSensor : MonoBehaviour, ISensor
{

  
    


    [SerializeField]
    [Tooltip("How often should we update our knowledge about the agents stats.")]
    private float _tickRate = 1f;

    public float TickRate => _tickRate;
    public float NextTickTime { get; set; }
    public Stats myStats;
    
    public object damageDealer;
    public float Health;
    public float Stamina;
    public float Mana;
    public float AgentBlock;
   

    void Start()
    {

        
        myStats = gameObject.GetComponent<Stats>();
        Health = myStats.GetAttrValue("health", myStats);
        Stamina = myStats.GetAttrValue("stamina", myStats);
        Mana = myStats.GetAttrValue("mana", myStats);
    

    }



    public void Tick(AIBlackBoard BlackBoard)
    {

        HandleHealth(BlackBoard);
       // HandleStamina(BlackBoard);
       // HandleMana(BlackBoard);
     
     
        
       
    }

    void HandleHealth(AIBlackBoard BlackBoard)
    {
        
       
        float curHealth = myStats.GetAttrValue("health", myStats);


        if (curHealth <= BlackBoard.Agent.DefaultAgentData.LowHealthThreshold)
        {
          
            if(BlackBoard.myAwareness != AIAwareness.Fleeing)
            {
                BlackBoard.myAwareness = AIAwareness.Fleeing;
                BlackBoard.Agent.UpdateAwareness();
            }
            
            WMFact HealthFact = BlackBoard.AgentMemory.FindMemoryFact(AddStatsFact(this.gameObject, AgentFact.LowHealth, BlackBoard.Agent));
            if (HealthFact != null)
            {


                HealthFact.factConfidence = 1f;

            }

            if (HealthFact == null)
            {
                HealthFact = AddStatsFact(this.gameObject, AgentFact.LowHealth, BlackBoard.Agent);
                BlackBoard.AgentMemory.AddMemoryFact(HealthFact);

            }
        }


        if (curHealth < Health)
        {
            Health = curHealth;
            damageDealer = VariablesManager.GetLocal(this.gameObject, "Attacker", true);

            WMFact damageTakenFact = BlackBoard.AgentMemory.FindMemoryFact(AddStatsFact(damageDealer as GameObject, AgentFact.DamageTaken, BlackBoard.Agent));
            if (damageTakenFact != null)
            {
                //Add this damager to your enemies list if theyre not in it(out of view most likely)
                if (!BlackBoard.PotentialEnemies.Contains(damageTakenFact.SourceObject))
                {
                    Debug.Log("Adding Damage enemy");
                    BlackBoard.PotentialEnemies.Add(damageTakenFact.SourceObject);
                }
                   //Debug.Log("Damage Dealer " + damageTakenFact.sourceObject);
                damageTakenFact.factConfidence = 1f; 

            }

            if (damageTakenFact == null)
            {
                damageTakenFact = AddStatsFact(damageDealer as GameObject, AgentFact.DamageTaken, BlackBoard.Agent);
                BlackBoard.AgentMemory.AddMemoryFact(damageTakenFact);
                if (!BlackBoard.PotentialEnemies.Contains(damageTakenFact.SourceObject))
                {
                    Debug.Log("Adding Damage enemy");
                    BlackBoard.PotentialEnemies.Add(damageTakenFact.SourceObject);
                }

            }

        }
        if (curHealth < BlackBoard.Agent.AgentData.AgentHealth/2)
        {

              
            WMFact NeedHealthFact = BlackBoard.AgentMemory.FindMemoryFact(AddStatsFact(BlackBoard.Agent.gameObject, AgentFact.NeedHealth, BlackBoard.Agent));
            if (NeedHealthFact != null)
            {


                NeedHealthFact.factConfidence = 1f;

            }

            if (NeedHealthFact == null)
            {

                NeedHealthFact = AddStatsFact(BlackBoard.Agent.gameObject, AgentFact.NeedHealth, BlackBoard.Agent);
                BlackBoard.AgentMemory.AddMemoryFact(NeedHealthFact);

            }


        }


       

        BlackBoard.AgentHealth = curHealth;


    }
    
    void HandleStamina(AIBlackBoard BlackBoard)
    {


        float curStamina = myStats.GetAttrValue("stamina", myStats);
       

        if (curStamina > 10f)
        {
            Stamina = curStamina;
         

            WMFact StaminaFact = BlackBoard.AgentMemory.FindMemoryFact(AddStatsFact(this.gameObject, AgentFact.HasStamina, BlackBoard.Agent));
            if (StaminaFact != null)
            {

                
                StaminaFact.factConfidence = 1f;

            }

            if (StaminaFact == null)
            {
                StaminaFact = AddStatsFact(this.gameObject, AgentFact.HasStamina, BlackBoard.Agent);
                BlackBoard.AgentMemory.AddMemoryFact(StaminaFact);

            }

        }

        if(curStamina < 10f)
        {
            WMFact LowStaminaFact = BlackBoard.AgentMemory.FindMemoryFact(AddStatsFact(this.gameObject, AgentFact.LowStamina, BlackBoard.Agent));
            if (LowStaminaFact != null)
            {


                LowStaminaFact.factConfidence = 1f;

            }

            if (LowStaminaFact == null)
            {
                LowStaminaFact = AddStatsFact(this.gameObject, AgentFact.LowStamina, BlackBoard.Agent);
                BlackBoard.AgentMemory.AddMemoryFact(LowStaminaFact);

            }

        }

        BlackBoard.AgentStamina = curStamina;
    }
    void HandleMana(AIBlackBoard BlackBoard)
    {


        float curMana = myStats.GetAttrValue("mana", myStats);
       

        if (curMana > 0f)
        {
            Mana = curMana;
         

            WMFact ManaFact = BlackBoard.AgentMemory.FindMemoryFact(AddStatsFact(this.gameObject, AgentFact.HasMana, BlackBoard.Agent));
            if (ManaFact != null)
            {

                
                ManaFact.factConfidence = 1f;

            }

            if (ManaFact == null)
            {
                ManaFact = AddStatsFact(this.gameObject, AgentFact.HasMana, BlackBoard.Agent);
                BlackBoard.AgentMemory.AddMemoryFact(ManaFact);

            }

        }

        if(curMana <= 0f)
        {
            WMFact LowManaFact = BlackBoard.AgentMemory.FindMemoryFact(AddStatsFact(this.gameObject, AgentFact.LowMana, BlackBoard.Agent));
            if (LowManaFact != null)
            {


                LowManaFact.factConfidence = 1f;

            }

            if (LowManaFact == null)
            {
                LowManaFact = AddStatsFact(this.gameObject, AgentFact.LowMana, BlackBoard.Agent);
                BlackBoard.AgentMemory.AddMemoryFact(LowManaFact);

            }

        }

        BlackBoard.AgentMana = curMana;
    }


    WMFact AddStatsFact( GameObject FactObject, AgentFact fact, GAgent agent)
    {

        WMFact statFact = new WMFact();
        statFact.Fact = fact;
        statFact.FactState = true;
        statFact.SourceObject = FactObject;
        statFact.factConfidence = 1f;
      

        return statFact;


    }

    public void DrawGizmos(AIBlackBoard BlackBoard)
    {



      
    }
}

