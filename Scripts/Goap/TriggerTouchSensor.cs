using UnityEngine;
using System.Collections;

public class TriggerTouchSensor : MonoBehaviour
{
    
    public GameObject TouchLocation;
    public GAgent aGagent;
    WMFact TouchedFact;
    public void Start()
    {
        aGagent = transform.parent.gameObject.GetComponent<GAgent>();
        TouchedFact = new WMFact(AgentFact.TouchedStimulus, gameObject);
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.root != transform.parent.root && aGagent.BlackBoard.myAwareness == AIAwareness.Idle)
        {

            if (collider.gameObject != transform.parent.gameObject )
            {
                TouchLocation = collider.transform.root.gameObject;

            }

            if (aGagent.BlackBoard.AgentMemory.FindMemoryFact(TouchedFact) == null)
            {
                aGagent.BlackBoard.AgentMemory.AddMemoryFact(TouchedFact);
                TouchedFact.SourceObject = TouchLocation;
                TouchedFact.FactState = true;
                TouchedFact.factConfidence = 3;
            }
            else
            {
                TouchedFact.factConfidence = 3;
            }

        }
    }


    public void OnTriggerExit(Collider collider)
    {

        TouchLocation = null;

    }
}