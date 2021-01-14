using GameCreator.Characters;
using GameCreator.Stats;
using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;

/*
public class WolfAgent : AnimalAgent
{
    float delta;
    public IEnumerator gestateCoroutine;


    Stats myStats;

    float currrentHealth;


    public override void Init()
    {
        myCharacter = this.gameObject.GetComponent<Character>();
        myCharacterAnimator = this.gameObject.GetComponent<CharacterAnimator>();
        myStats = this.gameObject.GetComponent<Stats>();



        health = myStats.GetAttrValue("health", myStats);

        if (actions.Count > 0)
        {
            actions.Clear();
        }
        GAction[] acts = this.GetComponents<GAction>(); //get the actions of this agent

        foreach (GAction a in acts)
            actions.Add(a); //add them to your list of actions


        sensory = new SensorySystem(this);
        BlackBoard = new AIBlackBoard(this, senses, head, this.gameObject.GetComponentInChildren<Animator>(), inventory);
        int randFur = Random.Range(1, 3);
        myrender = this.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        myLastAwareness = BlackBoard.myAwareness;

        if (!offSpring)
        {
            genes = Genes.RandomGenes(1);
            gender += genes.gender;
            if (genes.gender.Equals("male"))
            {
                myrender.material = maleMaterial[randFur];
                desirable = genes.desirability;

            }
            else if (genes.gender.Equals("female"))
            {
                myrender.material = femaleMaterial[randFur];
            }

            myCharacter.characterLocomotion.runSpeed += genes.speed;
            reproductiveUrge = genes.reproductionUrge;

        }
        else
        {
            hunger = 0;
            thirst = 0;
            gender = " ";
            gender += genes.gender;
            if (genes.gender.Equals("male"))
            {
                myrender.material = maleMaterial[randFur];
                desirable = genes.desirability;

            }
            else if (genes.gender.Equals("female"))
            {
                myrender.material = femaleMaterial[randFur];
            }
            myCharacter.characterLocomotion.runSpeed = 3;
            myCharacter.characterLocomotion.runSpeed += genes.speed;


            Debug.Log("This is offspring " + this.gameObject.name);
             Debug.Log(" Current Gender of " + this.gameObject.name + " " + gender);
              Debug.Log("Inherited genes of " + this.gameObject.name +" " + genes);
              Debug.Log("Inherited gender of " + this.gameObject.name + " " + genes.gender);
              Debug.Log("Inherited Desiriability of "+ this.gameObject.name + " " +genes.desirability);
              Debug.Log("Inherited Sex drive of" + this.gameObject.name + " " + genes.reproductionUrge);
            
            gestating = false;
            growUp = true;
        }
        //base subgoals
        if (this.BlackBoard != null)
        {
            this.BlackBoard.Agent.AgentFacts.states.Clear();

            SubGoal s1 = new SubGoal("exploreHabitat", true, false);
            this.BlackBoard.Agent.goals.Add(s1, 1);


            SubGoal s3 = new SubGoal("notHungry", true, false);
            this.BlackBoard.Agent.goals.Add(s3, 3);

            SubGoal s4 = new SubGoal("notThirsty", true, false);
            this.BlackBoard.Agent.goals.Add(s4, 3);

        }

        baseSpeed = myCharacter.characterLocomotion.runSpeed;


    }



    //Updates the status of the agent 
    IEnumerator UpdateAgentStatus()
    {
        yield return new WaitForSeconds(1f);
        AgentStatus();


    }
    void AgentStatus()
    {
      




    }

    /*
    void ResetGoals(SubGoal[] newGoals)
    {

        if (this.BlackBoard.Agent.goals.Count > 0)
        {
            this.BlackBoard.Agent.goals.Clear();

            for (int i = 0; i < newGoals.Length; i++)
            {

                this.BlackBoard.Agent.goals.Add(newGoals[i] , newGoals[i].sgoals.));


            }
        }



    }

    public override void SpawnBabies()
    {
        List<Transform> near = new List<Transform>();
        foreach (Transform spawnPoint in transform)
        {
            if (Vector3.Distance(this.transform.position, spawnPoint.position) < 4f)
            {
                near.Add(spawnPoint);
            }


        }

        int randSpawn = Random.Range(1, maxBabies);
        AnimalAgent mateGenes = this.BlackBoard.mateGenes as AnimalAgent;
        GameObject p = null;
        VariablesManager.SetLocal(this.gameObject, "birth", true, true);
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

                p.GetComponent<AnimalAgent>().offSpring = true;
                p.GetComponent<AnimalAgent>().myMother = this;
                p.GetComponent<AnimalAgent>().myFather = mateGenes;
                p.GetComponent<AnimalAgent>().genes = Genes.InheritedGenes(this.genes, mateGenes.genes);

                p.GetComponent<AnimalAgent>().Init();


            }
            VariablesManager.SetLocal(this.gameObject, "birth", false, true);

        }
        BlackBoard.Agent.randomExploration = true;
        gestating = false;
        this.BlackBoard.Agent.AgentFacts.RemoveState("isPregnant");
        this.BlackBoard.mateGenes = null;








    }

    public override void GrowUp()
    {
        if (this.BlackBoard != null)
        {
            this.BlackBoard.Agent.AgentFacts.SetState("juvenile", true);
            this.BlackBoard.myMother = myMother;
            this.BlackBoard.myFather = myFather;

        }
        if (this.transform.localScale != Vector3.one)
        {
            transform.localScale += Vector3.one * Time.deltaTime * growthRate;
            reproductiveUrge = 0;

            if (this.transform.localScale.y >= 1)
            {
                transform.localScale = Vector3.one;
                reproductiveUrge = genes.reproductionUrge;
                growUp = false;
                this.BlackBoard.Agent.AgentFacts.RemoveState("juvenile");


                return;
            }



        }


    }

    new void LateUpdate()
    {
        //Starting coroutine to update agent's status
        base.LateUpdate();
        StartCoroutine("UpdateAgentStatus");

        if (growUp)
        {

            GrowUp();
        }


        // Increase hunger and thirst over time
        // hunger += Time.deltaTime * 1 /  timeToDeathByHunger;
       
        //  thirst += Time.deltaTime * 1 / timeToDeathByThirst;
      //  reproductiveUrge += Time.deltaTime * 1 / timeToReproduceAgain;
       
    

        float timeSinceLastActionChoice = Time.time - lastActionChooseTime;
        if (timeSinceLastActionChoice > timeBetweenActionChoices)
        {
            ChooseNextAction();
        }


        if (hunger >= 1)
        {
            Die(CauseOfDeath.Hunger);
        }
        else if (thirst >= 1)
        {
            Die(CauseOfDeath.Thirst);
        }
        else if (health <= 0)
        {

            VariablesManager.SetLocal(this.gameObject, "death", true, true);
            Die(CauseOfDeath.Killed);
        }
        //reproductive urge
    }
    //Animals set triggers to planner for next actions
    protected virtual void ChooseNextAction()
    {


        lastActionChooseTime = Time.time;

        bool currentlyEating = false;
        bool currentlyMating = false;
        // Decide next action:
        // Eat if (more hungry than thirsty) or (currently eating and not critically thirsty)
        if (BlackBoard.Agent.currentAction != null && BlackBoard.Agent.currentAction.actionName == "Eating")
        {

            currentlyEating = true;
        }
        if (BlackBoard.Agent.currentAction != null && BlackBoard.Agent.currentAction.actionName == "Breeding")
        {

            currentlyMating = true;
        }

        //if your hunger is greater than your thirst or ur currently eating and ur thirst is less that the critcal percent
        if (hunger > 0.6f || currentlyEating && thirst < criticalPercent)
        {
            FindFood();
        }
        //if your reproductive urge is greater than ur hunger annd thirst and
        //and thirst and hunger are below critical percents then reproduce
        if (reproductiveUrge > 2f && reproductiveUrge > hunger && reproductiveUrge > thirst && hunger < criticalPercent && thirst < criticalPercent)
        {
            timeToReproduceAgain = 40;
            FindMate();
        }
        // More thirsty than hungry
        if (thirst > 0.4f)
        {
            FindWater();
        }



    }




    public override bool RequestMate(AnimalAgent male)
    {

        float chance = Mathf.Lerp(minBreedChance, 1, male.genes.desirability);
        if (Random.value > chance)
        {
            //   Debug.Log("mating Denied");
            return false;
        }
        //  Debug.Log("mating Accepted");
        return true;

    }



    public override bool PotentialMateFound(AnimalAgent female)
    {

        bool accepted = female.RequestMate(this);
        return accepted;
    }


    public override void FindFood()
    {
        BlackBoard.Agent.AgentFacts.SetState("isHungry", true);


    }

    public override void FindWater()
    {
        BlackBoard.Agent.AgentFacts.SetState("isThirsty", true);


    }

    public override void FindMate()
    {
        if (!BlackBoard.Agent.AgentFacts.HasState("needMate", true))
        {
            BlackBoard.Agent.AgentFacts.SetState("needMate", true);
        }





    }
    public override void UpdateAgentGoals()
    {

    }
}*/



