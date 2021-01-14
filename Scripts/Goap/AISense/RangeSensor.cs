using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RangeSensor : MonoBehaviour, ISensor
{
    [SerializeField]
    [Tooltip("How often should we update our knowledge about the whereabouts of mobiles.")]
    private float _tickRate = 1f;

    public float TickRate => _tickRate;
    public float NextTickTime { get; set; }

    public void Tick(AIBlackBoard BlackBoard)
    {
       
        if (BlackBoard.Agent.BlackBoard.CurrentEnemy != null)
        {
            WMFact checkFact = BlackBoard.AgentMemory.FindMemoryFact(CreateWMFact(BlackBoard.Agent.BlackBoard.CurrentEnemy.gameObject, BlackBoard.Agent));
            if (checkFact != null)
            {



                checkFact.factConfidence = 1f;
                var distance = Vector3.Distance(BlackBoard.CurrentEnemy.transform.position , BlackBoard.Position);
              
                if (distance >= 8 && distance <=18)
                {
                    checkFact.FactState = true;
                }
                else
                {
                    checkFact.FactState = false;
                }

            }

            if (checkFact == null)
            {
                checkFact = CreateWMFact(BlackBoard.Agent.BlackBoard.CurrentEnemy.gameObject, BlackBoard.Agent);
                BlackBoard.AgentMemory.AddMemoryFact(checkFact);

            }
          
            
        }
       
    }

    WMFact CreateWMFact( GameObject inRange, GAgent agent)
    {

        WMFact inMeleeRangeFact = new WMFact();
        inMeleeRangeFact.Fact = AgentFact.InLungeRange;
        inMeleeRangeFact.FactState = false;
        inMeleeRangeFact.SourceObject = inRange;
        inMeleeRangeFact.factConfidence = .5f;
      

        return inMeleeRangeFact;


    }

    public void DrawGizmos(AIBlackBoard BlackBoard)
    {



        if (BlackBoard.Agent.AgentFacts.HasState(AgentFact.InLungeRange.ToString(),true))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(BlackBoard.Head.position, BlackBoard.CurrentEnemy.transform.position);
        }
    }
}

