using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameCreator.Melee;
using GameCreator.Shooter;
using GameCreator.Variables;
using static GameCreator.Melee.CharacterMelee;
using GameCreator.Core;
using Pathfinding;








public class AgentCombatManager
{
    #region Variables
    //Battle circle
    private Vector3 destination;
    private Vector3 moveVec;
    public float attackDistance = 1.0f;
    public float dangerDistance = 2.0f;
    public float trackSpeed = 0.1f;
    public float attackRate = 10.0f;
    public float attackRateFluctuation = 0.0f;
    //For melee agents with range capabilities
    public float projectTileattackRate = 10.0f;
    public float projectTileattackRateFluctuation = 0.0f;
    private float lastAttackTime = 0.0f;

    private bool disabled = false;
    private float lastThought = 0.0f;
    private float lastReact = 0.0f;
    private float actualAttackRate = 0.0f;



    private float thinkPeriod = 1.5f;
    private float reactPeriod = 0.4f;
    public GameObject preyObject;

    private Vector3 distVec;
    private Vector3 avoidVec = Vector3.zero;
    private float sqrDistance;
    private float sqrAttackDistance;
    private float sqrprojectileAttackDistance;
    private float sqrDangerDistance;
    private bool engagePrey = false;
    private float strafeDir = 1.0f;
    private float strafeCooldown = 0f;
    private float strafeRate = 3.0f;

    private Avoider avoider;

    public GAgent ThisAgent { get; set; }
    public AIBlackBoard AgentsBB { get; set; }
    public CharacterMelee MyMeleeComponent { get; set; }
    public CharacterShooter MyShooterComponent { get; set; }

    public GameObject LastEnemy;
    public float enemyHealth = 0f;
    public GAgent EnemyAgent { get; set; }
    public PlayerCharacter EnemyPlayer { get; set; }
    public AimingAtTarget aimingTarget;
    TargetGameObject targetx = new TargetGameObject(TargetGameObject.Target.GameObject);

    public CharacterMelee EnemyMeleeComponent = new CharacterMelee();
    public CharacterShooter EnemyShooterComponent = new CharacterShooter();
    public Weapon MyCurrentWeapon;
    public Weapon MyCurrentSubWeapon;
    public Weapon EnemyCurrentWeapon;

    float enemyMeleeDistance;
    public float EnemySqrMeleeRange => enemyMeleeDistance * enemyMeleeDistance;
    float MaxConfidence = 1f;
    public float AgentConfidence = 0f;
    float AConfindenceIncreaseRate = 0f;
    float AConfidenceDecreaseRate = 0f;
    float AgentStartingConfidence { set; get; }

    public GameObject ChargeAura;
    public bool AuraChargedState = false;

    public WMFact AttackingFact;
    public WMFact CanAttackFact;
    public WMFact CanHeavyFact;
    public WMFact CanMeleeSpecialAttack;
    public WMFact BlockingFact;
    public WMFact StaggeredFact;
    public WMFact CanLungeAttack;
    public WMFact CanBreakBlock;
    public WMFact CanDodge;
    public WMFact CanCounter;
    public WMFact InEnemyMeleeRange;
    public WMFact CanChargeAura;
    public WMFact AuraCharged;
    public WMFact CanUseUltimate;
    public WMFact EnemyAttacking;
    public WMFact EnemyDefending;
    public WMFact EnemyAiming;
    public WMFact EnemyChargingShot;
    public WMFact EnemyExecutedShot;
    StatsSensor MyStatsSensor { set; get; }
    StatsSensor EnemyStatsSensor;
    #endregion

    private float attackCooldown
    {
        get
        {
            return Mathf.Max(actualAttackRate - (Time.fixedTime - lastAttackTime), 0f);
        }
    }

    #region GoapCombatManager

    // void CombatConsiderations

    public AgentCombatManager(CharacterMelee AgentMelee, CharacterShooter AgentShooter, GAgent Agent)
    {
        MyMeleeComponent = AgentMelee;
        MyShooterComponent = AgentShooter;
        ThisAgent = Agent;
        AgentsBB = Agent.BlackBoard;
        avoider = ThisAgent.GetComponentInChildren<Avoider>();

        attackDistance = ThisAgent.AgentData.attackDistance;
        dangerDistance = ThisAgent.AgentData.dangerDistance;
        attackRate = ThisAgent.AgentData.attackRate;
        trackSpeed = ThisAgent.AgentData.trackSpeed;
        attackRateFluctuation = ThisAgent.AgentData.attackRateFluctuation;
        actualAttackRate = attackRate + (Random.value - 0.5f) * attackRateFluctuation;
        lastAttackTime = -actualAttackRate;

        if (avoider != null)
        {
            avoider.gameObject.GetComponent<SphereCollider>().radius = ThisAgent.AgentData.seperation;

        }

        sqrAttackDistance = Mathf.Pow(attackDistance, 2);

        sqrDangerDistance = Mathf.Pow(dangerDistance, 2);


        // offset the start of the think ticks to spread the load out a little
        lastThought += thinkPeriod * Random.value;
        lastReact += reactPeriod * Random.value;


        if (ThisAgent != null)
        {
            AttackingFact = new WMFact(AgentFact.Attacking, ThisAgent.gameObject);
            CanAttackFact = new WMFact(AgentFact.CanAttack, ThisAgent.gameObject);
            BlockingFact = new WMFact(AgentFact.Blocking, ThisAgent.gameObject);

            StaggeredFact = new WMFact(AgentFact.Staggered, ThisAgent.gameObject);
            CanLungeAttack = new WMFact(AgentFact.CanLungeAttack, ThisAgent.gameObject);
            CanHeavyFact = new WMFact(AgentFact.CanHeavyAttack, ThisAgent.gameObject);
            CanMeleeSpecialAttack = new WMFact(AgentFact.CanSpecialAttack, ThisAgent.gameObject);
            CanBreakBlock = new WMFact(AgentFact.CanBreakBlock, ThisAgent.gameObject);
            CanDodge = new WMFact(AgentFact.CanDodge, ThisAgent.gameObject);
            CanCounter = new WMFact(AgentFact.CanCounter, ThisAgent.gameObject);
            InEnemyMeleeRange = new WMFact(AgentFact.InEnemyMeleeRange, ThisAgent.gameObject);
            CanChargeAura = new WMFact(AgentFact.CanChargeAura, ThisAgent.gameObject);
            AuraCharged = new WMFact(AgentFact.AuraCharged, ThisAgent.gameObject);
            CanUseUltimate = new WMFact(AgentFact.CanUseUltimate, ThisAgent.gameObject);

            EnemyAttacking = new WMFact(AgentFact.EnemyAttacking, ThisAgent.gameObject);
            EnemyDefending = new WMFact(AgentFact.EnemyDefending, ThisAgent.gameObject);
            EnemyAiming = new WMFact(AgentFact.EnemyAiming, ThisAgent.gameObject);
            EnemyChargingShot = new WMFact(AgentFact.EnemyChargingShot, ThisAgent.gameObject);
            EnemyExecutedShot = new WMFact(AgentFact.EnemyExecutedShot, ThisAgent.gameObject);

            AgentStartingConfidence = ThisAgent.AgentData.AgentStartingConfidence;
            MyStatsSensor = ThisAgent.gameObject.GetComponent<StatsSensor>();
        }
    }
    public void UpdateAgentCombat()
    {
        BattleCircleAI();
        //Setting Up Current Enemy
        if ((LastEnemy == null || preyObject != LastEnemy) && preyObject != null)
        {
            if (ThisAgent.AgentData.MainWeapon != null)
            {
                Weapon thisweapon = ThisAgent.AgentData.MainWeapon.GetComponent<Weapon>();
                if(thisweapon.WeaponType == WeaponType.meleeWeapon)
                {
                    attackDistance = thisweapon.meleeAttackDistance;
                    ThisAgent.MeleeDistance = attackDistance;
                    sqrAttackDistance = Mathf.Pow(attackDistance, 2);
                }
                else
                {
                    attackDistance = thisweapon.projectileRangeDistance;
                    ThisAgent.MeleeDistance = attackDistance;
                    sqrAttackDistance = Mathf.Pow(attackDistance, 2);
                }
              
            }

            LastEnemy = preyObject;

            EnemyMeleeComponent = LastEnemy.GetComponent<CharacterMelee>();
            EnemyShooterComponent = LastEnemy.GetComponent<CharacterShooter>();
            if (preyObject.GetComponent<GAgent>() != null)
            {

                EnemyAgent = LastEnemy.GetComponent<GAgent>();
                EnemyStatsSensor = LastEnemy.GetComponent<StatsSensor>();
                EnemyCurrentWeapon = EnemyAgent.BlackBoard.MainWeapon;
            }
            else
            {
                EnemyAgent = null;
                EnemyStatsSensor = null;
            }
            if (preyObject.GetComponent<PlayerCharacter>() != null)
            {

                EnemyPlayer = LastEnemy.GetComponent<PlayerCharacter>();

            }
            else
            {
                EnemyPlayer = null;
            }
            MyCurrentWeapon = ThisAgent.BlackBoard.MainWeapon;
            MyCurrentSubWeapon = ThisAgent.BlackBoard.SecondaryWeapon;
           // Debug.Log("Current subweapon " + MyCurrentSubWeapon);
           // Debug.Log("Current sweapon " + MyCurrentWeapon);
            if (MyCurrentWeapon.WeaponType == WeaponType.rangeWeapon || MyCurrentWeapon.WeaponType == WeaponType.magicWeapon ||
               MyCurrentSubWeapon != null && MyCurrentSubWeapon.WeaponType == WeaponType.rangeWeapon || MyCurrentSubWeapon != null && MyCurrentSubWeapon.WeaponType == WeaponType.magicWeapon)
            {
               
                aimingTarget = new AimingAtTarget(MyShooterComponent);
                targetx.gameObject = preyObject;
                aimingTarget.Setup(targetx);
               
            }

            //Cleaner****
            if(MyCurrentSubWeapon != null && (MyCurrentSubWeapon.WeaponType == WeaponType.magicWeapon || MyCurrentSubWeapon.WeaponType == WeaponType.rangeWeapon))
            {
               
                sqrprojectileAttackDistance = Mathf.Pow(MyCurrentSubWeapon.projectileRangeDistance, 2);
                Debug.Log("Square projectile attack distance" + sqrprojectileAttackDistance);
            }

            //Get their attack range from their weapons
            if (EnemyAgent != null)
            {
                enemyMeleeDistance = 3;
            }
            else if (EnemyPlayer != null)
            {
                enemyMeleeDistance = EnemyPlayer.attackRange;

            }





            AgentConfidence = AgentStartingConfidence;

        }

        //Checks to see if enemy is dead or not
        if (EnemyAgent != null)
        {
            if (EnemyAgent.BlackBoard.AgentHealth <= 0 && AgentsBB.PotentialEnemies.Count == 0)
            {
                if (ThisAgent.ShooterComponent.isAiming == true)
                {
                    ThisAgent.ShooterComponent.StopAiming();
                }
                if (ThisAgent.BlackBoard.AgentMemory.EvaluatedThreats.Contains(ThisAgent.BlackBoard.CurrentEnemy))
                {
                    ThisAgent.BlackBoard.AgentMemory.EvaluatedThreats.Remove(ThisAgent.BlackBoard.CurrentEnemy);
                    ThisAgent.BlackBoard.CurrentEnemy = null;
                }
                ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;
                ThisAgent.UpdateAwareness();


            }

        }
        else if (EnemyPlayer != null)
        {
            if (EnemyPlayer.playerHealth <= 0 && AgentsBB.PotentialEnemies.Count == 0)
            {

                ThisAgent.ShooterComponent.StopAiming();

                if (ThisAgent.BlackBoard.AgentMemory.EvaluatedThreats.Contains(ThisAgent.BlackBoard.CurrentEnemy))
                {
                    ThisAgent.BlackBoard.AgentMemory.EvaluatedThreats.Remove(ThisAgent.BlackBoard.CurrentEnemy);
                    ThisAgent.BlackBoard.CurrentEnemy = null;
                }

                ThisAgent.BlackBoard.myAwareness = AIAwareness.Idle;
                ThisAgent.UpdateAwareness();


            }
        }

        ThisAgent.goals[ThisAgent.AttackGoal] = CalculateAttackWeight();
        ThisAgent.goals[ThisAgent.DefendGoal] = CalculateDefendWeight();


        if (ThisAgent.MeleeComponent.IsStaggered)
        {
            if (ThisAgent.AgentMemory.FindMemoryFact(StaggeredFact) == null)
            {
                ThisAgent.AgentMemory.AddMemoryFact(StaggeredFact);
            }
            else
            {
                StaggeredFact.factConfidence = 1;
            }
            StaggeredFact.FactState = true;
            ThisAgent.combatStates.stunned = true;

        }
        else
        {
            StaggeredFact.FactState = false;
            ThisAgent.combatStates.stunned = false;
        }
        if(AgentsBB.AgentCharacter.characterLocomotion.currentLocomotionSystem.isDashing == true)
        {
            if (MyMeleeComponent.HasFocusTarget)
            {
                MyMeleeComponent.ReleaseTargetFocus();
            }
            ThisAgent.TrackTarget(preyObject);
            ThisAgent.combatStates.dodging = true;
            CanDodge.FactState = false;
            ThisAgent.updateAgentMov = false;
        }
        else
        {
            ThisAgent.combatStates.dodging = false;
            ThisAgent.updateAgentMov = true;
        }
        if (preyObject != null)
        {
            var distance = Vector3.SqrMagnitude(preyObject.transform.position - ThisAgent.BlackBoard.Position);
            if (distance <= EnemySqrMeleeRange)
            {
                if (ThisAgent.AgentMemory.FindMemoryFact(InEnemyMeleeRange) == null)
                {
                    ThisAgent.AgentMemory.AddMemoryFact(InEnemyMeleeRange);
                    InEnemyMeleeRange.factConfidence = 1;
                }
                else
                {
                    InEnemyMeleeRange.factConfidence = 1;
                }
                InEnemyMeleeRange.FactState = true;
            }
            else
            {
                InEnemyMeleeRange.FactState = false;
            }

            if (EnemyMeleeComponent.IsAttacking == true)
            {

                if (ThisAgent.AgentMemory.FindMemoryFact(EnemyAttacking) == null)
                {
                    ThisAgent.AgentMemory.AddMemoryFact(EnemyAttacking);
                    EnemyAttacking.factConfidence = 1;
                }
                else
                {
                    EnemyAttacking.FactState = true;
                    EnemyAttacking.factConfidence = 1;
                }


            }
            else
            {
                EnemyAttacking.FactState = false;
            }

            if (EnemyMeleeComponent.IsBlocking == true)
            {
                if (ThisAgent.AgentMemory.FindMemoryFact(EnemyDefending) == null)
                {
                    ThisAgent.AgentMemory.AddMemoryFact(EnemyDefending);
                    EnemyDefending.factConfidence = 1;
                }
                else
                {
                    EnemyDefending.factConfidence = 1;
                }

                EnemyDefending.FactState = true;
            }
            else
            {
                EnemyDefending.FactState = false;
            }
            if (EnemyShooterComponent.isAiming == true)
            {
                EnemyAiming.FactState = true;
            }
            else
            {
                EnemyAiming.FactState = false;
            }
            if (EnemyShooterComponent.isChargingShot == true)
            {
                EnemyChargingShot.FactState = true;



            }
            else
            {
                EnemyChargingShot.FactState = false;
            }
        }

        if (ThisAgent.MeleeComponent.IsAttacking)
        {
            if (MyMeleeComponent.HasFocusTarget)
            {
                MyMeleeComponent.ReleaseTargetFocus();
            }

            ThisAgent.TrackTarget(preyObject);



        }

        //Attacking Fact --Aiming and charging shot or just regular melee attack
        if (MyShooterComponent.isChargingShot && MyShooterComponent.isAiming || ThisAgent.MeleeComponent.IsAttacking)
        {
          

         

            if (ThisAgent.AgentMemory.FindMemoryFact(AttackingFact) == null)
            {
                ThisAgent.AgentMemory.AddMemoryFact(AttackingFact);
            }
            else
            {
                AttackingFact.factConfidence = 1;
            }
            AttackingFact.FactState = true;
            ThisAgent.combatStates.attacking = true;


        }
        else
        {
            AttackingFact.FactState = false;
            ThisAgent.combatStates.attacking = false;
        }




    

        if (ThisAgent.MeleeComponent.IsBlocking)
        {
            ThisAgent.combatStates.blocking = true;
        }
        else
        {
            ThisAgent.combatStates.blocking = false;
        }

        if (ThisAgent.AgentFacts.HasState(AgentFact.AttackLanded.ToString(), true))
        {
            AgentConfidence += Time.deltaTime * ThisAgent.AgentData.AgentConfidenceIncreaseRate;


        }

        if (ThisAgent.AgentFacts.HasState(AgentFact.DamageTaken.ToString(), true))
        {



            AgentConfidence -= Time.deltaTime * ThisAgent.AgentData.ConfidenceDecreaseRate;






        }






        //TODO: Check to see what type of weapon the enemy has draw and draw appropriate weapon based on available weapons

        

      
      

        ThisAgent.AttackConfidence = AgentConfidence;

    }
    void UpdateMeleeCombat()
    {


       

        if (ThisAgent.AgentData.CanDodge == true && InEnemyMeleeRange.FactState == true && MyMeleeComponent.IsAttacking == false && MyMeleeComponent.IsStaggered == false)
        {

            if (EnemyMeleeComponent.IsAttacking == true)
            {

                if (ThisAgent.RequestDodge() == true && ThisAgent.combatStates.dodging == false)
                {
                    if (ThisAgent.AgentMemory.FindMemoryFact(CanDodge) == null)
                    {
                        ThisAgent.AgentMemory.AddMemoryFact(CanDodge);
                        CanDodge.factConfidence = 1;
                    }
                    else
                    {
                        CanDodge.factConfidence = 1;
                    }
                    CanDodge.FactState = true;




                }

            }
           







        }
        else
        {
            CanDodge.factConfidence = 0;
            CanDodge.FactState = false;
        }

        if (ThisAgent.AgentFacts.HasState(AgentFact.InLungeRange.ToString(), true))
        {


            if (ThisAgent.RequestSkill(ThisAgent.AgentData.AgentLungeChance) == true && avoidVec == Vector3.zero && MyCurrentWeapon.canLungeAttack)
            {
                if (ThisAgent.AgentMemory.FindMemoryFact(CanLungeAttack) == null)
                {
                    ThisAgent.AgentMemory.AddMemoryFact(CanLungeAttack);
                    CanLungeAttack.factConfidence = 1;
                }
                else
                {
                    CanLungeAttack.factConfidence = 1;
                }

                CanLungeAttack.FactState = true;

            }












        }


        if (InEnemyMeleeRange.FactState == true && EnemyMeleeComponent.IsAttacking && ThisAgent.BlackBoard.SensorEyes.IsTargetFacingyou == true && AgentConfidence <= 0)
        {
            if(MyCurrentWeapon.WeaponType == WeaponType.meleeWeapon)
            {
                if (MyCurrentWeapon.GCWeapon.defaultShield != null)
                {

                    if (ThisAgent.AgentMemory.FindMemoryFact(BlockingFact) == null)
                    {
                        ThisAgent.AgentMemory.AddMemoryFact(BlockingFact);
                        BlockingFact.factConfidence = 1;
                        BlockingFact.FactState = true;
                    }
                }
                else
                {
                    BlockingFact.factConfidence = 1;
                }
            }
            else
            {

                if (MyCurrentSubWeapon.GCWeapon.defaultShield != null)
                {

                    if (ThisAgent.AgentMemory.FindMemoryFact(BlockingFact) == null)
                    {
                        ThisAgent.AgentMemory.AddMemoryFact(BlockingFact);
                        BlockingFact.factConfidence = 1;
                        BlockingFact.FactState = true;
                    }
                }
                else
                {
                    BlockingFact.factConfidence = 1;
                }


            }
           

        }



        if (EnemyMeleeComponent.IsAttacking && InEnemyMeleeRange.FactState == true && ThisAgent.BlackBoard.SensorEyes.IsTargetFacingyou == true)
        {

            if (AgentConfidence < 0)
            {
                AgentConfidence -= Time.deltaTime * ThisAgent.AgentData.ConfidenceDecreaseRate;
            }

        }
        else
        {
            //   AgentConfidence += Time.deltaTime * ThisAgent.AgentDataCopy.ConfidenceDecreaseRate;
        }








        





        if (ThisAgent.AgentFacts.HasState(AgentFact.CanAttack.ToString(), true))
        {

            if (ThisAgent.AgentFacts.HasState(AgentFact.CounteredAttack.ToString(), false))
            {


                AgentConfidence += ThisAgent.AgentData.ConfidenceDecreaseRate;
            }











            if (AgentConfidence >= 1f)
            {
                if (ThisAgent.RequestSkill(ThisAgent.AgentData.AgentSkillChance) == true)
                {
                    if (ThisAgent.AgentMemory.FindMemoryFact(CanMeleeSpecialAttack) == null)
                    {
                        ThisAgent.AgentMemory.AddMemoryFact(CanMeleeSpecialAttack);
                    }
                    else
                    {
                        CanMeleeSpecialAttack.factConfidence = 1;
                    }


                    CanMeleeSpecialAttack.FactState = true;

                }
                else
                {
                    CanMeleeSpecialAttack.FactState = false;

                }




            }

















        }


     
        
       



        



    }
    void UpdateRangeCombat()
    {

        if (AgentConfidence < 0f)
        {




            if (EnemyMeleeComponent.IsAttacking && InEnemyMeleeRange.FactState == true)
            {

                if (AgentConfidence < 0)
                {
                    AgentConfidence -= Time.deltaTime * ThisAgent.AgentData.ConfidenceDecreaseRate;
                }

            }
            else
            {
                AgentConfidence += Time.deltaTime * ThisAgent.AgentData.ConfidenceDecreaseRate;
            }


            MyShooterComponent.StopAiming();
            if (MyShooterComponent.isChargingShot == true)
                MyShooterComponent.currentAmmo.StopCharge(MyShooterComponent);


        }





        CanAttackFact.factConfidence = 1;
        AttackingFact.factConfidence = 1;











    }
    void UpdateMagicCombat()
    {

        //some time of timer after aura is charged to reset it 

     
        if (ThisAgent.AgentMemory.FindMemoryFact(CanChargeAura) == null)
        {
            ThisAgent.AgentMemory.AddMemoryFact(CanChargeAura);
            CanChargeAura.factConfidence = 1;
        }
        if (AgentConfidence >= 1 && AuraCharged.FactState == false)
        {
            CanChargeAura.FactState = true;
            CanChargeAura.factConfidence = 1;
        }
        else
        {
            CanChargeAura.FactState = false;
        }


        if (AuraChargedState == true)
        {
            if (ThisAgent.AgentMemory.FindMemoryFact(AuraCharged) == null)
            {
                ThisAgent.AgentMemory.AddMemoryFact(AuraCharged);
            }

            AuraCharged.FactState = true;
            AuraCharged.factConfidence = 1;
            if (ThisAgent.ShooterComponent.currentAmmo != ThisAgent.BlackBoard.MainWeapon.ChargedBasicSpell.SpellAmmo)
            {
                ThisAgent.ShooterComponent.ChangeAmmo(ThisAgent.BlackBoard.MainWeapon.ChargedBasicSpell.SpellAmmo);
            }
        }

        //if (AgentConfidence < 0f)
        //{




        //    if (EnemyMeleeComponent.IsAttacking && InEnemyMeleeRange.factState == true)
        //    {

        //        if (AgentConfidence < 0)
        //        {
        //            AgentConfidence -= Time.deltaTime * ThisAgent.AgentDataCopy.ConfidenceDecreaseRate;
        //        }

        //    }
        //    else
        //    {
        //        AgentConfidence += Time.deltaTime * ThisAgent.AgentDataCopy.ConfidenceDecreaseRate;
        //    }


        //    MyShooterComponent.StopAiming();
        //    if (MyShooterComponent.isChargingShot == true)
        //        MyShooterComponent.currentAmmo.StopCharge(MyShooterComponent);


        //}












      






        





      





    }

    //Goal Weights
    float CalculateAttackWeight()
    {
        //enemys health
        //my confidence
        float result = 0;



        if (EnemyPlayer != null)
        {
            result = BoundedLinearCurve(EnemyPlayer.playerHealth / 100);
        }
        else
        {
            result = BoundedLinearCurve(EnemyAgent.BlackBoard.AgentHealth / EnemyAgent.AgentData.AgentHealth);
        }




        return result;
    }

    float CalculateDefendWeight()
    {
        float result;


        result = BoundedLinearCurve(ThisAgent.BlackBoard.AgentHealth / ThisAgent.AgentData.AgentHealth);







        return result;




    }

    //Decision Curves
    float BoundedLinearCurve(float value)
    {

        return Mathf.Max((1 - value), 0f);


    }
    float ExponectialCurve(float value)
    {

        return (-1 * (Mathf.Pow(value, 2)));
    }

    float InverseCurve(float value, float factor, float offset)
    {
        return 1 / (value * factor + offset);
    }

    float AboveZeroCurve(float value)
    {


        if (value > 0)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    float EqualsZeroCurve(float value)
    {
        if (value == 0)
        {
            return 1;
        }
        else
        {
            return 0;

        }
    }

    #endregion



    #region Battle Circle AI Logic

    void UpdateDistance()
    {

        distVec = destination - ThisAgent.transform.position;
        sqrDistance = distVec.sqrMagnitude;

        //   Debug.Log("Square distance " + sqrDistance + "Attack distance"+ sqrAttackDistance);

        if(sqrprojectileAttackDistance == 0)
        {
           
            if (sqrDistance > sqrAttackDistance)// && engagePrey)
            {
                OnAttackComplete();


            }
        }
        else
        {


            if (sqrDistance > sqrprojectileAttackDistance)// && engagePrey)
            {
                OnAttackComplete();


            }


        }

       
    }

    void BattleCircleAI()
    {
        if (engagePrey && !ThisAgent.combatStates.attacking)
        {


            OnAttackComplete();

        }

        // keep looking at the target even if it's disabled
        if (disabled)
        {
           
            if (preyObject != null)
            {
                if (trackSpeed > 0.0f && ThisAgent.combatStates.attacking && !ThisAgent.combatStates.stunned)
                {
                    Vector3 lookVec = preyObject.transform.position - ThisAgent.transform.position;
                    ThisAgent.Look(lookVec, trackSpeed);
                }

                // quick-react, just get the data we need
                lastReact = Time.fixedTime;

                UpdateDistance();
            }
            return;
        }

        if (strafeCooldown > 0.0f)
        {
            strafeCooldown -= Time.fixedDeltaTime;
        }

        // during each thinkperiod,
        if (preyObject == null || (Time.fixedTime - lastThought) > thinkPeriod)
        {
            lastThought = Time.fixedTime;
            Think();

            if (ThisAgent.BlackBoard.CurrentSquad != null)
            {


                ThisAgent.BlackBoard.CurrentSquad.Think();
            }
        }
        if (preyObject == null)
        {
            return;
        }

        if ((Time.fixedTime - lastReact) > reactPeriod)
        {
            React();
        }

        UpdateDistance();



         bool staystill = (ThisAgent.currentAction != null && ThisAgent.currentAction.running && ThisAgent.currentAction.target != null);
         bool shouldAvoid = (avoidVec != Vector3.zero && sqrDistance <= sqrDangerDistance && ThisAgent.AgentData.AvoidOthers);
         bool shouldStrafe = (!shouldAvoid && !engagePrey && sqrDistance <= sqrAttackDistance || ThisAgent.combatStates.blocking == true );
         bool shouldAttack = (engagePrey && sqrDistance <= sqrAttackDistance || engagePrey && sqrDistance > sqrAttackDistance && sqrDistance <= sqrprojectileAttackDistance);
         ThisAgent.combatStates.strafing = shouldStrafe;

  
        if (!staystill)
        {
            if (shouldAvoid)
            {

                ThisAgent.myCharacter.characterLocomotion.canRun = false;
                Avoid(avoidVec);
            }
            else if (shouldAttack)
            {

                // I have permission
                Attack(preyObject.transform.position);
            }
            else if (shouldStrafe)
            {
                // I don't have permission, so I'll just strafe around
                ThisAgent.myCharacter.characterLocomotion.canRun = false;
                Strafe(preyObject.transform.position, strafeDir);

            }
            else
            {

                if (sqrDistance > sqrDangerDistance)
                {
                    ThisAgent.myCharacter.characterLocomotion.canRun = true;
                }
                else
                {
                    ThisAgent.myCharacter.characterLocomotion.canRun = false;

                }




                // I'm outside the danger zone so seek the target
                if (sqrDistance > sqrAttackDistance)
                {
                    Seek(preyObject.transform.position);
                }




            }
        }
    }

    void Seek(Vector3 distVec)
    {
        Seek(distVec, true);
    }

    void Seek(Vector3 distVec, bool align)
    {
        // whenever I decide to move, I am giving up permission to attack
        if (engagePrey)
        {
            OnAttackComplete();
        }

        if (align == true)
        {
            ThisAgent.LookAtTarget(preyObject);
        }


        destination = preyObject.transform.position;
        moveVec = distVec;
      

        ThisAgent.pathAi.destination = moveVec;

    }

    void Avoid(Vector3 distVec)
    {
        if (ThisAgent.AgentData.CanLockOn == true)
        {

            Seek(ThisAgent.BlackBoard.Position - distVec, true);
        }
        else
        {
            Seek(ThisAgent.BlackBoard.Position - distVec, false);
        }

    }

    void Strafe(Vector3 distVec, float direction)
    {


        var perpendicularVec = Vector3.Cross(ThisAgent.transform.position, distVec);
       

        perpendicularVec.Normalize();
        if (ThisAgent.AgentData.CanLockOn == true)
        {
            Seek(perpendicularVec * direction, true);
        }
        else
        {
            Seek(perpendicularVec * direction, false);
        }

    }

    IEnumerator OnWait(float delay)
    {
        this.OnDisable();
        yield return new WaitForSeconds(delay);
        this.OnEnable();
    }

    void Attack(Vector3 target)
    {

        if (!ThisAgent.combatStates.attacking && attackCooldown <= 0.0f)
        {
            //   ThisAgent.Look(preyObject.transform.position -ThisAgent.transform.position, 0f);
            bool success = false;

            success = ThisAgent.OnAttack();

            if (success)
            {
                //Debug.Log("Attack allowed");

                if (ThisAgent.AgentMemory.FindMemoryFact(CanAttackFact) == null)
                {
                    ThisAgent.AgentMemory.AddMemoryFact(CanAttackFact);
                    CanAttackFact.FactState = true;
                    CanAttackFact.factConfidence = 1;
                }
                else
                {
                    CanAttackFact.factConfidence = 1;
                }
                // Debug.Log("Successful attack request");
                if (EnemyDefending.FactState == true && ThisAgent.RequestSkill(ThisAgent.AgentData.AgentSkillChance) == true && MyCurrentWeapon.canBlockBreak)
                {
                    if (ThisAgent.AgentMemory.FindMemoryFact(CanBreakBlock) == null)
                    {
                        ThisAgent.AgentMemory.AddMemoryFact(CanBreakBlock);
                        CanBreakBlock.factConfidence = 1;
                    }
                    else
                    {
                        CanBreakBlock.factConfidence = 1;
                    }

                    CanBreakBlock.FactState = true;
                }
                lastAttackTime = Time.fixedTime;
                actualAttackRate = attackRate + (Random.value - 0.5f) * attackRateFluctuation;

            }
            else
            {

            }

        }


    }

    GameObject GetClosestTarget()
    {

        GameObject target = null;

        var closestDist = Mathf.Infinity;
        if (AgentsBB.PotentialEnemies.Count <= 0)
        {
            return null;
        }
        if (AgentsBB.PotentialEnemies.Count > 0)
        {
            foreach (var p in AgentsBB.PotentialEnemies)
            {


                var dirVec = ThisAgent.transform.position - p.transform.position;
                var d = dirVec.sqrMagnitude;
                if (d < closestDist)
                {
                    target = p;
                    closestDist = d;
                }
            }


        }
        return target;
    }


    void Think()
    {
        preyObject = GetClosestTarget();
        AgentsBB.CurrentEnemy = preyObject;
        // nothing to kill!

       

        if (preyObject == null)
        {
            return;
        }
      
   


        // for enemy pack avoidance
        if (avoider != null && avoider.avoidEnemy != null && avoider.aGagent.BlackBoard.AgentHealth > 0) //&& avoider.avoidEnemy.gameObject != EnemyAgent.gameObject
        {

            avoidVec =  avoider.avoidEnemy.transform.position - ThisAgent.transform.position;
         
            avoidVec.Normalize();
            
        }
        else if(avoider != null && avoider.avoidEnemy != null && avoider.aGagent.BlackBoard.AgentHealth <= 0)
        {

           // avoider.avoidEnemy = null;
            avoidVec = Vector3.zero;
        }
        else
        {
            avoidVec = Vector3.zero;
        }

        // for strafing, if I decide to strafe
        if (!engagePrey && strafeCooldown <= 0f)
        {

            strafeCooldown = strafeRate;
            strafeDir = 1.0f;
            if (Random.value > 0.5f) strafeDir = -1.0f;
        }
    }

    void React()
    {
        lastReact = Time.fixedTime;
      
        if (MyCurrentWeapon.WeaponType == WeaponType.meleeWeapon || MyCurrentSubWeapon != null && MyCurrentSubWeapon.WeaponType == WeaponType.meleeWeapon)
        {
        
            UpdateMeleeCombat();
        }
        if (MyCurrentWeapon.WeaponType == WeaponType.magicWeapon || MyCurrentSubWeapon != null && MyCurrentSubWeapon.WeaponType == WeaponType.magicWeapon)
        {


            MyShooterComponent.currentAmmo.prefabProjectile.GetComponent<ShooterAgent>().shooter = ThisAgent.gameObject;
            if (MyShooterComponent.currentAmmo.animationShoot == null)
            {
                MyShooterComponent.currentAmmo.animationShoot = MyCurrentWeapon.WeaponShootAnim;
                if (MyCurrentWeapon.WeaponAnimMask != null)
                {
                    MyShooterComponent.currentAmmo.maskShoot = MyCurrentWeapon.WeaponAnimMask;

                }
            }
     
            UpdateMagicCombat();


        }
        if (MyCurrentWeapon.WeaponType == WeaponType.rangeWeapon)
        {
            MyShooterComponent.currentAmmo.prefabProjectile.GetComponent<ShooterAgent>().shooter = ThisAgent.gameObject;
            //    Debug.Log("AMMO" + MyShooterComponent.currentAmmo.prefabProjectile.gameObject);
            UpdateRangeCombat();
        }
       
        if (EnemyAttacking.FactState == true && MyMeleeComponent.IsAttacking == false && InEnemyMeleeRange.FactState == true)
        {
            // Debug.Log("In React");



            if (ThisAgent.RequestCounter() == true)
            {
                CanCounter.FactState = true;

            }



        }
        else
        {
            CanCounter.FactState = false;
        }



        sqrDistance = Vector3.SqrMagnitude(preyObject.transform.position - ThisAgent.BlackBoard.Position);

        if(MyCurrentSubWeapon != null)
        {
            if (MyCurrentSubWeapon.WeaponType == WeaponType.rangeWeapon || MyCurrentSubWeapon.WeaponType == WeaponType.magicWeapon)
            {

                if (sqrDistance != 0 && sqrDistance > sqrAttackDistance && sqrDistance <= sqrprojectileAttackDistance)
                {

                    if (!engagePrey)
                    {
                        Debug.Log(" project Tile attack");
                        preyObject.gameObject.SendMessage("OnRequestAttack", ThisAgent.gameObject);

                    }
                }
            }
        }
       


        if (sqrDistance != 0 && sqrDistance <= sqrAttackDistance)
        {
            if (!engagePrey)
            {
                preyObject.gameObject.SendMessage("OnRequestAttack", ThisAgent.gameObject);

            }
        }
    }

    public void OnAllowAttack(GameObject target)
    {

        if (preyObject != null && target == ThisAgent.gameObject)
            engagePrey = true;
       //   Debug.Log("Target " + target + "preyObject " + preyObject);
    }

    void OnAttackComplete()
    {

        // disengage when completing an attack to give other enemies a chance
        // CAVEAT: this currently only happens if I am stunned or am not in range of my prey
        //   and NOT when I complete an attack
        engagePrey = false;

        if (preyObject != null)
            preyObject.gameObject.SendMessage("OnCancelAttack", ThisAgent.gameObject, SendMessageOptions.DontRequireReceiver);
    }

    void OnStun(float d)
    {
        OnAttackComplete();
    }



    void OnEnable()
    {
        disabled = false;
    }

    void OnDisable()
    {
        disabled = true;
    }

    #endregion











































}
