using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeleeRangeSensor : MonoBehaviour, ISensor
{
    [SerializeField]
    [Tooltip("How often should we update our knowledge about the range of the current enemy.")]
    private float _tickRate = 1f;

    public float TickRate => _tickRate;
    public float NextTickTime { get; set; }
    WMFact InMeleeRangeFact;
    WMFact InLungeRangeFact;
    void Start()
    {


        InMeleeRangeFact = new WMFact(AgentFact.InMeleeRange, gameObject);
        InLungeRangeFact = new WMFact(AgentFact.InLungeRange, gameObject);

    }
    public void Tick(AIBlackBoard BlackBoard)
    {

        
      
        if (BlackBoard.Agent.BlackBoard.CurrentEnemy != null && BlackBoard.myAwareness == AIAwareness.Combat)
        {
          
         





           
            var distance = Vector3.SqrMagnitude(BlackBoard.CurrentEnemy.transform.position - BlackBoard.Position);
           
            if (distance <= BlackBoard.Senses.SqrMeleeRange)
            {
                if (BlackBoard.AgentMemory.FindMemoryFact(InMeleeRangeFact) == null)
                {
                    BlackBoard.AgentMemory.AddMemoryFact(InMeleeRangeFact);
                    InMeleeRangeFact.FactState = true;

                }
              
              
                InMeleeRangeFact.factConfidence = TickRate;
            }
         
           


            if (distance <= BlackBoard.Senses.SqrLungeRange && distance >= BlackBoard.Senses.SqrLungeRange - 5)
            {
                if (BlackBoard.AgentMemory.FindMemoryFact(InLungeRangeFact) == null)
                {
                    BlackBoard.AgentMemory.AddMemoryFact(InLungeRangeFact);
                    InLungeRangeFact.FactState = true;
                }
               

               
                InLungeRangeFact.factConfidence = TickRate;
            }
           
        }

    }



    public void DrawGizmos(AIBlackBoard BlackBoard)
    {



        if (BlackBoard.Agent.AgentFacts.HasState(AgentFact.InMeleeRange.ToString(),true))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(BlackBoard.Head.position, BlackBoard.MainWeapon.meleeAttackDistance);
        }
        if (BlackBoard.Agent.AgentFacts.HasState(AgentFact.InLungeRange.ToString(), true))
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(BlackBoard.Head.position, BlackBoard.MainWeapon.lungeAttackDistance);
          //  Gizmos.DrawLine(BlackBoard.Head.position, BlackBoard.CurrentEnemy.transform.position + Vector3.up * 2f);
           // Gizmos.DrawSphere(BlackBoard.CurrentEnemy.transform.position + Vector3.up * 1.5f, 0.25f);
        }
    }
}

