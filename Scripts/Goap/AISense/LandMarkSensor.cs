using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMarkSensor : MonoBehaviour, ISensor
{
    private static Collider[] _hits = new Collider[128];

    [SerializeField]
    [Tooltip("How often should we update our knowledge about the whereabouts of our current location.")]
    private float _tickRate = 1f;

    public float TickRate => _tickRate;
    public float NextTickTime { get; set; }
    public LandMark currentLandMark = null;
    
   
    public void Tick(AIBlackBoard BlackBox)
    {


        if(BlackBox.CurrentLandMark != null)
        {

            BlackBox.Agent.AgentFacts.RemoveState(BlackBox.CurrentLandMark.LMarkFact.ToString());
            BlackBox.CurrentLandMark = null;
          //  BlackBox.KnownLandMarks.Clear(); //on each tick clear the list of known LandMarks to update it 
        }

      




        
        var hitCount = Physics.OverlapSphereNonAlloc(BlackBox.Position, BlackBox.Senses.DetectRadius, _hits);
        if (hitCount > 0)
        {
            for (var i = 0; i < hitCount; i++)
            {
                var landmark = _hits[i].GetComponent<LandMark>();
                if (landmark != null)
                {
                    //add gameobjects of known resources to list
                    //  BlackBox.KnownLandMarks.Add(landmark);
                    
                    BlackBox.CurrentLandMark = landmark;
                    if(BlackBox.CurrentLandMark != null)
                    {
                        BlackBox.CurrentLandMark.LastTimeVisited = BlackBox.Time;
                       // Debug.Log("Last Visited + " + BlackBox.CurrentLandMark + " : " + BlackBox.CurrentLandMark.LastTimeVisited);
                      
                    }
                    BlackBox.Agent.AgentFacts.SetState(BlackBox.CurrentLandMark.LMarkFact.ToString(), true);
                }
            }
                                 
         

       }

       



    }
    
    public void DrawGizmos(AIBlackBoard BlackBox)
    {
        foreach (var landmark in BlackBox.KnownLandMarks)
        {


            Gizmos.color = Color.gray;
            
            Gizmos.DrawLine(BlackBox.Head.position, landmark.transform.position + Vector3.up * 2f);
            Gizmos.DrawSphere(landmark.transform.position + Vector3.up * 2f, 0.25f);
          
        }
    }
}
