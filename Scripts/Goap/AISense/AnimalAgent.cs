using GameCreator.Characters;
using GameCreator.Core;
using GameCreator.Melee;
using GameCreator.Shooter;
using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAgent : GAgent
{


    [Header("Genetics")]
    public GAgent myMother;
    public GAgent myFather;
    public GAgent LastMateGenes;
    public Genes genes;
    public bool AnimalOffSpring = false;
    public bool growUp;


    [Header("Genes")]
    public string gender = " ";
    public float healthGene;
    public float attackDamageGene;
    public float desirabilityGene;
    public float gestationPeriodGenes;
    public float speedGene;
    public float staminaGene;
    public float reproductionUrgeGenes;
    public float metabolismGene;




    public SkinnedMeshRenderer myrender;

    // State:
    [Header("Animal Needs")]
    public float hunger ;
    public float thirst ;
    public float reproduce;

    public float reproductionUrge;

    float moveTime;
    float moveSpeedFactor;


    // Other
    public float lastActionChooseTime;
    const float sqrtTwo = 1.4142f;
    const float oneOverSqrtTwo = 1 / sqrtTwo;
    public bool gestating;
    public float gestationPeriod = 0f;
  

    CharacterMelee meleeComponent;
    CharacterShooter shooterComponent;


    public override void Init()
    {
        myrender = DefaultAgentData.AgentRenderer;
        int randFur;


        if (!AnimalOffSpring)
        {
            genes = Genes.RandomGenes(8);
            gender += genes.gender;
            healthGene = genes.health;
            attackDamageGene = genes.attackDamage;

            gestationPeriodGenes = genes.gestationPeriod;
            speedGene = genes.speed;
            staminaGene = genes.stamina;
            reproductionUrge = genes.reproductionUrge;
            reproductionUrgeGenes = genes.reproductionUrge;
            metabolismGene = genes.metabolism;
            if (genes.gender.Equals("male"))
            {
                randFur = Random.Range(0, DefaultAgentData.maleMaterial.Count);
                myrender.material = DefaultAgentData.maleMaterial[randFur];
                desirabilityGene = genes.desirability;

            }
            else if (genes.gender.Equals("female"))
            {
                randFur = Random.Range(0, DefaultAgentData.femaleMaterial.Count);
                myrender.material = DefaultAgentData.femaleMaterial[randFur];
            }

            myCharacter.characterLocomotion.runSpeed += genes.speed;
      
         

        }
        if (AnimalOffSpring == true)
        {
            LastMateGenes = null;

           
            gender = " ";
            gender += genes.gender;
            healthGene = genes.health;
            attackDamageGene = genes.attackDamage;

            gestationPeriodGenes = genes.gestationPeriod;
            speedGene = genes.speed;
            staminaGene = genes.stamina;
            reproductionUrge = genes.reproductionUrge;
            reproductionUrgeGenes = genes.reproductionUrge;
            metabolismGene = genes.metabolism;
            if (genes.gender.Equals("male"))
            {
                randFur = Random.Range(0, DefaultAgentData.maleMaterial.Count);
                myrender.material = DefaultAgentData.maleMaterial[randFur];
                desirabilityGene = genes.desirability;

            }
            else if (genes.gender.Equals("female"))
            {
                randFur = Random.Range(0, DefaultAgentData.femaleMaterial.Count);
                myrender.material = DefaultAgentData.femaleMaterial[randFur];
            }




            Debug.Log("This is offspring " + this.gameObject.name);


            gestating = false;
            gestationPeriod = 0f;
            growUp = true;
        }
        characterRunSpeed = AgentData.Agentrunspeed + speedGene;
        characterWalkSpeed = AgentData.Agentwalkspeed + speedGene;
       
        hunger = 0;
        thirst = 0;

        InitStartWeapon();
        UpdateAgentGoals();
     

    }

    public override void LateUpdate()
    {

        base.LateUpdate();
        if (growUp == true)
        {

            GrowUp();
        }
   
        UpdateAnimalNeeds();

     
    }

    public override void UpdateAgentNeed()
    {
      
    }
    //public void InitStartWeapon()
    //{
    //    meleeComponent = this.gameObject.GetComponent<CharacterMelee>();
    //    shooterComponent = this.gameObject.GetComponent<CharacterShooter>();

    //    if (AgentDataCopy.DefaultWeapon != null)
    //    {
    //        Weapon thisWeapon = AgentDataCopy.DefaultWeapon.GetComponent<Weapon>();
    //        if (thisWeapon.WeaponType == WeaponType.meleeWeapon)
    //        {

    //            CoroutinesManager.Instance.StartCoroutine(meleeComponent.Draw(thisWeapon.GCWeapon, thisWeapon.GCWeapon.defaultShield));
    //            BlackBoard.Senses.MeleeRange = thisWeapon.meleeRange;
    //            BlackBoard.Senses.LungeRange = thisWeapon.lungeRange;
    //        }

    //        if (AgentData.MainWeapon != null)
    //        {
    //            MainWeapon = AgentData.MainWeapon;
    //        }
    //        if (AgentData.SecondaryWeapon != null)
    //        {
    //            SecondaryWeapon = AgentData.SecondaryWeapon;
    //        }

    //        if (MainWeapon != null || SecondaryWeapon != null)
    //        {
    //            BlackBoard.Agent.AgentFacts.SetState(AgentGoals.Armed.ToString(), true);
    //        }
    //    }
    //    else
    //    {
    //        BlackBoard.Agent.AgentFacts.SetState(AIWorldState.Armed.ToString(), false);
    //    }


    //}
    //public override void DrawWeapon()
    //{
    //    Weapon thisWeapon = null;
    //    if (MainWeapon == null && SecondaryWeapon == null)
    //    {
    //        Debug.Log("No Weapon Equipped");
    //        return;
    //    }
    //    if (MainWeapon != null)
    //    {
    //        thisWeapon = MainWeapon.GetComponent<Weapon>();
    //    }
    //    if (weaponDrawn == false)
    //    {
    //        if (MainWeapon != null)
    //        {


    //            if (thisWeapon.WeaponType == WeaponType.meleeWeapon)
    //            {
    //                MeleeWeapon mWep = Instantiate(thisWeapon.GCWeapon);
    //                MeleeShield wShield = null;
    //                if (thisWeapon.GCWeapon.defaultShield!= null)
    //                {
    //                  wShield = Instantiate(thisWeapon.GCWeapon.defaultShield);
    //                    wShield.perfectBlockWindow = AgentDataCopy.AgentCounterChance;
    //                }


    //                CoroutinesManager.Instance.StartCoroutine(meleeComponent.Draw(mWep, wShield));

    //                BlackBoard.CurrentWeapon = thisWeapon;
    //                BlackBoard.Agent.AgentFacts.SetState(AgentFact.MeleeWeapon.ToString(), true);
    //                BlackBoard.Senses.MeleeRange = thisWeapon.meleeRange;
    //                BlackBoard.Senses.LungeRange = thisWeapon.lungeRange;
    //                thisWeapon = BlackBoard.CurrentWeapon;
    //                weaponDrawn = true;
    //            }
    //            if (thisWeapon.WeaponType == WeaponType.rangeWeapon)
    //            {
    //                GameCreator.Shooter.Weapon wWep = Instantiate(thisWeapon.GCRWeapon);

    //                shooterComponent.currentWeapon = (wWep);
    //                CoroutinesManager.Instance.StartCoroutine(shooterComponent.ChangeWeapon(wWep, Instantiate(thisWeapon.GCRWeapon.defaultAmmo)));
    //                shooterComponent.currentAmmo.prefabProjectile.GetComponent<ShooterAgent>().shooter = gameObject;
    //                BlackBoard.Agent.AgentFacts.SetState(AgentFact.RangeWeapon.ToString(), true);
    //                BlackBoard.CurrentWeapon = thisWeapon;

    //                weaponDrawn = true;
    //            }
    //            if (thisWeapon.WeaponType == WeaponType.magicWeapon)
    //            {
    //                GameCreator.Shooter.Weapon wWep = Instantiate(thisWeapon.GCRWeapon);

    //                shooterComponent.currentWeapon = (wWep);
    //                CoroutinesManager.Instance.StartCoroutine(shooterComponent.ChangeWeapon(wWep, Instantiate(thisWeapon.GCRWeapon.defaultAmmo)));

    //                BlackBoard.Agent.AgentFacts.SetState(AgentFact.MagicWeapon.ToString(), true);
    //                BlackBoard.CurrentWeapon = thisWeapon;

    //                weaponDrawn = true;
    //            }
    //        }

    //    }
    //    else
    //    {
    //        Debug.Log("Weapon already drawn");


    //        if (thisWeapon.WeaponType == WeaponType.meleeWeapon)
    //        {
    //            meleeComponent.Sheathe();
    //        }

    //        if (thisWeapon.WeaponType == WeaponType.magicWeapon || thisWeapon.WeaponType == WeaponType.rangeWeapon)
    //        {
    //            shooterComponent.ChangeWeapon(null);
    //        }

    //        weaponDrawn = false;
    //    }

    //}
    //public void UpdateWeapon()
    //{



    //    if (MainWeapon != null)
    //    {
    //        Weapon thisWeapon = MainWeapon.GetComponent<Weapon>();


    //        if (thisWeapon.WeaponType == WeaponType.meleeWeapon)
    //        {
    //            meleeComponent.previousWeapon = thisWeapon.GCWeapon;
    //            meleeComponent.previousShield = thisWeapon.GCWeapon.defaultShield;
    //            meleeComponent.previousShield.perfectBlockWindow = AgentData.AgentCounterChance;
    //            BlackBoard.Agent.AgentFacts.SetState(AIWorldState.ArmedMelee.ToString(), true);
    //            BlackBoard.Senses.MeleeRange = thisWeapon.meleeRange;
    //            BlackBoard.Senses.LungeRange = thisWeapon.lungeRange;
    //        }

    //        if (thisWeapon.WeaponType == WeaponType.rangeWeapon)
    //        {

    //            shooterComponent.ChangeWeapon(thisWeapon.GCRWeapon, thisWeapon.GCRWeapon.defaultAmmo);
    //            BlackBoard.Agent.AgentFacts.SetState(AgentFact.RangeWeapon.ToString(), true);

    //        }

    //        if (BlackBoard.myAwareness == AIAwareness.Alert)
    //        {
    //            if (thisWeapon.WeaponType == WeaponType.meleeWeapon)
    //            {
    //                VariablesManager.SetLocal(this.gameObject, "DrawWeapon", true, true);
    //                BlackBoard.Agent.AgentFacts.SetState(AgentFact.MeleeWeapon.ToString(), true);

    //            }

    //            else if (thisWeapon.WeaponType == WeaponType.rangeWeapon)
    //            {

    //                //   VariablesManager.SetLocal(this.gameObject, "DrawRange", true, true);
    //                //  shooterComponent.ChangeWeapon(shooterComponent.currentWeapon, shooterComponent.currentWeapon.defaultAmmo);
    //                shooterComponent.ChangeWeapon(thisWeapon.GCRWeapon, thisWeapon.GCRWeapon.defaultAmmo);
    //                BlackBoard.Agent.AgentFacts.SetState(AgentFact.MeleeWeapon.ToString(), true);

    //            }



    //        }


    //    }
    //    else
    //    {
    //        BlackBoard.Agent.AgentFacts.SetState(AIWorldState.Armed.ToString(), false);
    //        BlackBoard.Agent.AgentFacts.SetState(AgentFact.MeleeWeapon.ToString(), false);
    //    }



    //}

    //public override void UpdateAgentGoals()
    //{

    //    if (BlackBoard.Agent.goals.Count > 0)
    //    {
    //        BlackBoard.Agent.goals.Clear();
    //    }



    //    switch (BlackBoard.myAwareness)
    //    {
    //        case AIAwareness.Idle:
    //            if (BlackBoard.Agent.currentAction != null && BlackBoard.Agent.currentAction.running == true)
    //            {
    //                BlackBoard.Agent.CancelCurrentGoal();
    //            }
    //            if (weaponDrawn == true)
    //            {
    //                DrawWeapon();
    //            }
    //            foreach (AgentGoal G in IdleGoals)
    //            {
    //                SubGoal goal = new SubGoal(G.Satisfies.ToString(), G.state, G.constantGoal);
    //                this.BlackBoard.Agent.goals.Add(goal, G.GoalPriority);
    //            }




    //            break;
    //        case AIAwareness.Suspicious:
    //            if (BlackBoard.Agent.currentAction != null && BlackBoard.Agent.currentAction.running == true)
    //            {
    //                BlackBoard.Agent.CancelCurrentGoal();
    //            }
    //            foreach (AgentGoal G in SuspiciousGoals)
    //            {
    //                SubGoal goal = new SubGoal(G.Satisfies.ToString(), G.state, G.constantGoal);
    //                this.BlackBoard.Agent.goals.Add(goal, G.GoalPriority);
    //            }

    //            break;
    //        case AIAwareness.Alert:
    //            if (BlackBoard.Agent.currentAction != null && BlackBoard.Agent.currentAction.running == true)
    //            {


    //                BlackBoard.Agent.CancelCurrentGoal();
    //            }
    //            if (weaponDrawn == false)
    //            {
    //                DrawWeapon();
    //            }

    //            foreach (AgentGoal G in AlertGoals)
    //            {
    //                SubGoal goal = new SubGoal(G.Satisfies.ToString(), G.state, G.constantGoal);
    //                this.BlackBoard.Agent.goals.Add(goal, G.GoalPriority);
    //            }





    //            break;
    //        case AIAwareness.InDanger:
    //            if (BlackBoard.Agent.currentAction.running == true)
    //            {
    //                BlackBoard.Agent.CancelCurrentGoal();
    //            }


    //            SubGoal RunAway = new SubGoal(AgentTask.StaySafe.ToString(), true, false);
    //            this.BlackBoard.Agent.goals.Add(RunAway, 2);

    //            break;
    //        default:
    //            break;
    //    }


    //}


    //public void ShotLanded()
    //{
    //    Debug.Log("Called");
    //    WMFact checkFact = BlackBoard.AgentMemory.FindMemoryFact(AddAgentFact(AgentFact.AttackLanded, gameObject, BlackBoard.Agent));
    //    if (checkFact != null)
    //    {



    //        checkFact.factConfidence = .5f;


    //    }

    //    if (checkFact == null)
    //    {
    //        checkFact = AddAgentFact(AgentFact.AttackLanded, gameObject, BlackBoard.Agent);
    //        BlackBoard.AgentMemory.AddMemoryFact(checkFact);

    //    }







    //}

    float increaseHunger = 0;
   
    public void UpdateAnimalNeeds()
    {





       // hunger = (timeKeeper.counter / 60) * metabolismGene / (AgentDataCopy.DaysToDeathByHunger * 1440);






      //  thirst = (timeKeeper.counter / 60) * metabolismGene / (AgentDataCopy.DaysToDeathByThirst * 1440);

        //   reproduce = (timeKeeper.counter / 60) * reproductionUrge / (AgentDataCopy.DaysToReproduceAgain * 1440);

        WMFact CheckNeedsFact = BlackBoard.AgentMemory.FindMemoryFact(CreateWMFact(AgentFact.HasNeed, this.gameObject, true, BlackBoard.Agent));
        if (CheckNeedsFact != null)
        {



            if (AgentFacts.HasState(AgentFact.Hungry.ToString(), true) || AgentFacts.HasState(AgentFact.Thirsty.ToString(), true) )
            {
                AgentMemory.EvaluatedThreats.Clear();
                CheckNeedsFact.FactState = true;



            }
            else
            {
                CheckNeedsFact.FactState = false;
            }


            CheckNeedsFact.factConfidence = 1f;
        }

        if (CheckNeedsFact == null)
        {
            CheckNeedsFact = CreateWMFact(AgentFact.HasNeed, this.gameObject, false, BlackBoard.Agent);
            BlackBoard.AgentMemory.AddMemoryFact(CheckNeedsFact);

        }

        WMFact TiredFact = BlackBoard.AgentMemory.FindMemoryFact(CreateWMFact(AgentFact.Tired, this.gameObject, false, BlackBoard.Agent));
        if (TiredFact != null)
        {




            if (AgentFacts.HasState(AgentFact.Pregnant.ToString(), true) || AgentFacts.HasState(AgentFact.Juvenile.ToString(), true) || BlackBoard.WorldStates.HasState(TimeOfDay.Evening.ToString(), true) || BlackBoard.WorldStates.HasState(TimeOfDay.Night.ToString(), true))
            {

                TiredFact.FactState = true;



            }
            else
            {
                TiredFact.FactState = false;
            }

            TiredFact.factConfidence = 1f;
        }

        if (TiredFact == null)
        {
            TiredFact = CreateWMFact(AgentFact.Tired, this.gameObject, false, BlackBoard.Agent);
            BlackBoard.AgentMemory.AddMemoryFact(TiredFact);

        }


        if (hunger > 0.01f  && thirst < AgentData.AnimalNeedsCriticalPercent )
        {

            WMFact HungryFact = BlackBoard.AgentMemory.FindMemoryFact(CreateWMFact(AgentFact.Hungry, this.gameObject, true, BlackBoard.Agent));
            if (HungryFact != null)
            {




                HungryFact.FactState = true;
                
                HungryFact.factConfidence = 1f;
            }

            if (HungryFact == null)
            {
                HungryFact = CreateWMFact(AgentFact.Hungry, this.gameObject, false, BlackBoard.Agent);
                BlackBoard.AgentMemory.AddMemoryFact(HungryFact);

            }



        }
        
      
        // More thirsty than hungry
        if (thirst > 0.5f && hunger < AgentData.AnimalNeedsCriticalPercent)
        {

            WMFact ThirstyFact = BlackBoard.AgentMemory.FindMemoryFact(CreateWMFact(AgentFact.Thirsty, this.gameObject, true, BlackBoard.Agent));
            if (ThirstyFact != null)
            {



                ThirstyFact.FactState = true;
              
                ThirstyFact.factConfidence = 1f;
            }

            if (ThirstyFact == null)
            {
                ThirstyFact = CreateWMFact(AgentFact.Thirsty, this.gameObject, false, BlackBoard.Agent);
                BlackBoard.AgentMemory.AddMemoryFact(ThirstyFact);

            }





        }

       
        if (AgentFacts.HasState(AgentFact.Pregnant.ToString(), true))
        {




            gestationPeriod += Time.deltaTime;

            if (gestationPeriod >= DefaultAgentData.GestationPeriod)
            {
                gestating = true;
                if (gestating == true)
                {
                    gestating = false;
                    //    StartCoroutine("SpawnBabies");

                    AgentFacts.RemoveState(AgentFact.Pregnant.ToString());

                }




            }

          

        }



        if (hunger >= AgentData.DaysToDeathByHunger * 1440)
        {
            Die(CauseOfDeath.Hunger);
        }
        else if (thirst >= 1)
        {
            Die(CauseOfDeath.Thirst);
        }

      


    }

    public IEnumerator AnimalIsPregnant(float gestatePeriod)
    {
        gestating = true;
        yield return new WaitForSeconds(gestatePeriod);
        SpawnBabies();

        gestating = false;
    }

    public IEnumerator SpawnBabies()
    {
        List<Transform> near = new List<Transform>();
        foreach (Transform spawnPoint in transform)
        {
            if (Vector3.Distance(this.transform.position, spawnPoint.position) < 10f)
            {
                near.Add(spawnPoint);
            }


        }

        int randSpawn = Random.Range(1, DefaultAgentData.maxBabies);
        AnimalAgent mateGenes = this.BlackBoard.mateGenes as AnimalAgent;
        GameObject p = null;
        // VariablesManager.SetLocal(this.gameObject, "birth", true, true);
        BlackBoard.Agent.randomExploration = false;
        for (int i = 0; i < randSpawn; i++)
        {
            if (near.Count > 0)
            {
                int index = Random.Range(0, near.Count);
                Transform spawnAt = near[index];


                p = Instantiate(this.gameObject, spawnAt.position, Quaternion.identity);
                CharacterAnimatorEvents oldComponent = p.GetComponentInChildren<CharacterAnimatorEvents>();
                CharacterAttachments oldcompent2 = p.GetComponentInChildren<CharacterAttachments>();
                Destroy(oldComponent);
                Destroy(oldcompent2);

                p.transform.localScale = Vector3.one * .5f;

                p.GetComponent<AnimalAgent>().AnimalOffSpring = true;
                p.GetComponent<AnimalAgent>().myMother = this;
                p.GetComponent<AnimalAgent>().myFather = mateGenes;
                p.GetComponent<AnimalAgent>().genes = Genes.InheritedGenes(this.genes, mateGenes.genes);
                p.GetComponent<AnimalAgent>().offspring = true;
                p.GetComponent<GAgent>().actions.Clear();
                p.GetComponent<GAgent>().goals.Clear();
                p.GetComponent<GAgent>().AgentFacts.states.Clear();
                p.GetComponent<GAgent>().offspring = true;

                p.GetComponent<GAgent>().currentAction = null;
                p.GetComponent<AnimalAgent>().growUp = true;


            }
            //   VariablesManager.SetLocal(this.gameObject, "birth", false, true);

        }
        BlackBoard.Agent.randomExploration = true;
        gestating = false;
        gestationPeriod = 0;
        AgentFacts.RemoveState(AgentFact.Pregnant.ToString());
        this.BlackBoard.mateGenes = null;
        yield return null;


    }

    public void GrowUp()
    {
        if (this.BlackBoard != null)
        {

            this.BlackBoard.myMother = myMother;
            this.BlackBoard.myFather = myFather;
            this.BlackBoard.Agent.AgentFacts.SetState(AgentFact.Juvenile.ToString(), true);
        }
        if (this.transform.localScale != Vector3.one)
        {
            transform.localScale += Vector3.one * Time.deltaTime * DefaultAgentData.growthRate;
            reproductionUrge = 0;

            if (this.transform.localScale.y >= 1)
            {
                transform.localScale = Vector3.one;
                reproductionUrge = genes.reproductionUrge;
                // growUp = false;
                this.BlackBoard.Agent.AgentFacts.RemoveState(AgentFact.Juvenile.ToString());


                return;
            }



        }
    }

    public bool PotentialMateFound(AnimalAgent female)
    {


        bool accepted = female.RequestMate(this);
        return accepted;




    }

    public bool RequestMate(AnimalAgent male)
    {
        float chance = Mathf.Lerp(DefaultAgentData.minBreedChance, 1, male.genes.desirability);
        if (Random.value > chance)
        {

            return false;
        }

        return true;
    }

    WMFact CreateWMFact(AgentFact factToAdd, GameObject Object, bool FactState, GAgent agent)
    {


        WMFact AgentFact = new WMFact();
        AgentFact.Fact = factToAdd;
        AgentFact.FactState = FactState;
        AgentFact.SourceObject = Object;
        AgentFact.factConfidence = 1f;


        return AgentFact;


    }
}




