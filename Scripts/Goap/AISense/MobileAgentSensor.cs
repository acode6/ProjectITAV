using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileAgentSensor : MonoBehaviour, ISensor
{

    List<Species> enemies;
    List<Species> prey;



    WMFact InMeleeRangeFact;
    WMFact InLungeRangeFact;

    bool updateview = false;
    public bool IsTargetPresent { get; set; }
    public bool IsTargetWithinFov { get; set; }
    public bool IsTargetShootable { get; set; }
    public bool IsTargetFacingyou { get; set; }
    Vector3 GetLastRecordedPosition { get; set; }
    float GetTimeTargetHasBeenVisible { get; set; }
    float GetTimeTargetHasBeenOutOfView { get; set; }


    [SerializeField]
    [Tooltip("How often should we update our knowledge about the whereabouts of Agents.")]
    private float _tickRate = 1f;

    public float TickRate => _tickRate;
    public float NextTickTime { get; set; }
   
  
    public void Tick(AIBlackBoard BlackBoard)
    {
        if (BlackBoard != null)
        {


            prey = EnvironmentManager.preyBySpecies[BlackBoard.Agent.species]; //getting a list of my prey
            enemies = EnvironmentManager.predatorsBySpecies[BlackBoard.Agent.species]; // a list of my predetors



            BlackBoard.PotentialPrey.Clear();
            BlackBoard.PotentialEnemies.Clear();
            BlackBoard.potentialMates.Clear();
            BlackBoard.AgentMemory.FoodSources.Clear();




            if (BlackBoard.Agent.SensorEyes.DetectedObjects.Count > 0)
            {
               
                for (var i = 0; i < BlackBoard.Agent.SensorEyes.DetectedObjects.Count; i++)
                {

                    var mobileAgent = BlackBoard.Agent.SensorEyes.DetectedObjects[i].GetComponent<GAgent>();



                    if (mobileAgent != null && mobileAgent != this.gameObject.GetComponent<GAgent>())
                    {


                        //add to food sources if the detected agent is in prey list 
                        if (prey.Contains(mobileAgent.species))
                        {
                            if (mobileAgent.BlackBoard.AgentHealth > 0)
                            {
                                //Debug.Log("Mobile Agent Game Object" + mobileAgent.gameObject);
                                BlackBoard.PotentialPrey.Add(mobileAgent.gameObject);
                            }

                            //if (mobileAgent.BlackBoard.AgentHealth <= 0)
                            //{
                            //    WMFact CanEatFact = BlackBoard.AgentMemory.FindMemoryFact(CreateWMFact(AgentFact.CanEat, mobileAgent.gameObject, BlackBoard.Agent));
                            //    if (CanEatFact != null)
                            //    {

                            //        if (BlackBoard.AgentMemory.FoodSources.Contains(mobileAgent.gameObject) == false)
                            //        {
                            //            //  Debug.Log("Added Food Source");
                            //            BlackBoard.AgentMemory.FoodSources.Add(mobileAgent.gameObject);

                            //        }

                            //        CanEatFact.factConfidence = 1;
                            //        CanEatFact.factState = true;



                            //    }

                            //    if (CanEatFact == null)
                            //    {
                            //        CanEatFact = CreateWMFact(AgentFact.CanEat, mobileAgent.gameObject, BlackBoard.Agent);
                            //        BlackBoard.AgentMemory.AddMemoryFact(CanEatFact);
                            //        BlackBoard.AgentMemory.FoodSources.Add(mobileAgent.gameObject);

                            //    }
                            //}


                        


                        }

                        //add to enemies
                        if (enemies.Contains(mobileAgent.species) && mobileAgent.BlackBoard.AgentHealth > 0)
                        {

                            BlackBoard.PotentialEnemies.Add(mobileAgent.gameObject);

                            

                        }
                        //if i need a mate to breed 
                        if (BlackBoard.Agent.AgentFacts.HasState(AgentFact.NeedMate.ToString(), true))
                        {

                            //if its not you and its a member of your species and they desire a mate
                            if (mobileAgent != BlackBoard.Agent && mobileAgent.species == BlackBoard.Agent.species && mobileAgent.BlackBoard.Agent.AgentFacts.HasState(AgentFact.NeedMate.ToString(), true))
                            {
                                AnimalAgent potentialMate = (AnimalAgent)mobileAgent.BlackBoard.Agent;
                                AnimalAgent me = (AnimalAgent)BlackBoard.Agent;


                                if (me.gender != potentialMate.gender)
                                {

                                    BlackBoard.potentialMates.Add(mobileAgent);


                                }

                            }
                            foreach (GAgent a in BlackBoard.potentialMates)
                            {

                                WMFact checkFact = BlackBoard.AgentMemory.FindMemoryFact(CreateWMFact(AgentFact.PotentialMateInSight, a.gameObject, BlackBoard.Agent));
                                if (checkFact != null)
                                {



                                    checkFact.factConfidence = 1;


                                }

                                if (checkFact == null)
                                {
                                    checkFact = CreateWMFact(AgentFact.PotentialMateInSight, a.gameObject, BlackBoard.Agent);
                                    BlackBoard.AgentMemory.AddMemoryFact(checkFact);

                                }









                            }

                        }





                    }

                    if (BlackBoard.Agent.SensorEyes.DetectedObjects[i].tag == "Player")
                    {

                        GameObject player = BlackBoard.Agent.SensorEyes.DetectedObjects[i];
                        PlayerCharacter playerInfo = player.transform.GetComponent<PlayerCharacter>();



                        if (prey.Contains(playerInfo.PlayerSpecies))
                        {
                            if (playerInfo.playerHealth > 0)
                            {
                                BlackBoard.PotentialPrey.Add(player);
                            }


                         


                        }

                        //add to enemies
                        if (enemies.Contains(playerInfo.PlayerSpecies) && playerInfo.playerHealth > 0)
                        {

                            BlackBoard.PotentialEnemies.Add(player);

                       




                        }
















                    }
                }






            }
            foreach (GameObject a in BlackBoard.PotentialEnemies)
            {
                if(BlackBoard.myAwareness == AIAwareness.Idle || BlackBoard.myAwareness == AIAwareness.Searching)
                {
                    WMFact checkFact = BlackBoard.AgentMemory.FindMemoryFact(CreateWMFact(AgentFact.PotentialEnemy, a.gameObject, BlackBoard.Agent));
                    if (checkFact != null)
                    {



                        checkFact.factConfidence = 1;


                    }

                    if (checkFact == null)
                    {
                        checkFact = CreateWMFact(AgentFact.PotentialEnemy, a.gameObject, BlackBoard.Agent);
                        BlackBoard.AgentMemory.AddMemoryFact(checkFact);

                    }
                }
               

            }
            foreach (GameObject a in BlackBoard.PotentialPrey)
            {
                if(BlackBoard.myAwareness == AIAwareness.Idle || BlackBoard.myAwareness == AIAwareness.Searching)
                {
                    WMFact checkFact = BlackBoard.AgentMemory.FindMemoryFact(CreateWMFact(AgentFact.PotentialPrey, a, BlackBoard.Agent));
                    if (checkFact != null)
                    {



                        checkFact.factConfidence = 1;


                    }

                    if (checkFact == null)
                    {
                        checkFact = CreateWMFact(AgentFact.PotentialPrey, a, BlackBoard.Agent);
                        BlackBoard.AgentMemory.AddMemoryFact(checkFact);

                    }
                }
              










            }
            UpdateInView(BlackBoard);
        }
    }
    public void UpdateInView(AIBlackBoard BlackBoard)
    {
       // Debug.Log("Updating in view");
        for (var i = 0; i < BlackBoard.Agent.SensorEyes.DetectedObjects.Count; i++)
        {




            if (BlackBoard.Agent.currentEnemy != null && BlackBoard.Agent.SensorEyes.DetectedObjects.Contains(BlackBoard.Agent.currentEnemy))
            {
                BlackBoard.SensorEyes.IsTargetWithinFov = true;

                GameObject TargetObj = BlackBoard.Agent.SensorEyes.DetectedObjects[i];

                if (IsFacingMe(TargetObj) == false)
                {
                    // Debug.Log("Looking At me");
                    BlackBoard.SensorEyes.IsTargetFacingyou = true;

                }
                else
                {
                    BlackBoard.SensorEyes.IsTargetFacingyou = false;

                    //    Debug.Log("Looking Away");
                }












            }
        }
    }

    bool IsFacingMe(GameObject other)
    {
        // Check if the gaze is looking at the front side of the object
        Vector3 forward = other.transform.forward;
        Vector3 toOther = (other.transform.position - transform.position).normalized;

        if (Vector3.Dot(forward, toOther) <= 0.7f)
        {
            //  Debug.Log("Target Facing me");
            return false;
        }
        // Debug.Log("Target not facing me");

        return true;
    }

    WMFact CreateWMFact(AgentFact factToAdd, GameObject mobileAgent, GAgent agent)
    {

        WMFact seeAgentFact = new WMFact();
        seeAgentFact.Fact = factToAdd;
        seeAgentFact.FactState = true;
        seeAgentFact.FactState = true;
        seeAgentFact.SourceObject = mobileAgent;
        seeAgentFact.factConfidence = TickRate;
        //agent.BlackBoard.AgentMemory.AddMemoryFact(seeWeaponFact);

        return seeAgentFact;


    }

    public void DrawGizmos(AIBlackBoard BlackBox)
    {

        foreach (var enemy in BlackBox.PotentialEnemies)
        {
            DrawnEnemyGizmo(BlackBox, enemy);
        }

        foreach (var potentialMate in BlackBox.potentialMates)
        {
        //    DrawMateGizmo(BlackBox, potentialMate);
        }
        foreach (var potentialPrey in BlackBox.PotentialPrey)
        {
       //     DrawPreyGizmo(BlackBox, potentialPrey.gameObject);
        }

    }

    private void DrawnEnemyGizmo(AIBlackBoard BlackBoard, GameObject enemy)
    {
     
      
            Gizmos.color = Color.yellow;
        
        Gizmos.DrawLine(BlackBoard.Head.position, enemy.transform.position + Vector3.up * 2f);
        Gizmos.DrawSphere(enemy.transform.position + Vector3.up * 1.5f, 0.25f);
    }


    private void DrawPreyGizmo(AIBlackBoard BlackBoard, GameObject prey)
    {
        if (prey == BlackBoard.AgentMemory.FindClosestPrey().GetComponent<GAgent>())
        {
            Gizmos.color = Color.green;
        }
        else
        {
          Gizmos.color = Color.black;
        }
        
        Gizmos.DrawLine(BlackBoard.Head.position, prey.transform.position + Vector3.up * 2f);
        Gizmos.DrawSphere(prey.transform.position + Vector3.up * 1.5f, 0.25f);
    }
    private void DrawMateGizmo(AIBlackBoard BlackBoard, GameObject mates)
    {
        if(BlackBoard.currentMate != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(BlackBoard.Head.position, BlackBoard.currentMate.transform.position + Vector3.up * 2f);
            Gizmos.DrawSphere(BlackBoard.currentMate.transform.position + Vector3.up * 1.5f, 0.25f);
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(BlackBoard.Head.position, mates.transform.position + Vector3.up * 2f);
        Gizmos.DrawSphere(mates.transform.position + Vector3.up * 1.5f, 0.25f);
    }
}


