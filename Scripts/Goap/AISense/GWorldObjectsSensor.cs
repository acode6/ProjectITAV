using GameCreator.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GWorldObjectsSensor : MonoBehaviour, ISensor
{

    private static Collider[] _hits = new Collider[500];

    [SerializeField]
    [Tooltip("How often should we update our knowledge about the whereabouts of World Objects.")]
    private float _tickRate = 1f;

    public float TickRate => _tickRate;
    public float NextTickTime { get; set; }

    public List<GameObject> visibleWorldObjects = new List<GameObject>();
    public void Tick(AIBlackBoard BlackBoard)
    {
        if (BlackBoard != null)
        {
            visibleWorldObjects.Clear();
            var hitCount = Physics.OverlapSphereNonAlloc(BlackBoard.Head.position, BlackBoard.Senses.DetectRadius, _hits);
            if (hitCount > 0)
            {
                for (var i = 0; i < hitCount; i++)
                {

                    var WorldObjectInfo = _hits[i].GetComponent<GObjectInfo>();
            


                    if (WorldObjectInfo != null && !visibleWorldObjects.Contains(_hits[i].gameObject) )
                    {
                        visibleWorldObjects.Add(_hits[i].gameObject);
                        var WorldObj = WorldObjectInfo.WorldObject;

                        WMFact WorldObjFact = BlackBoard.AgentMemory.FindMemoryFact(CreateWMFact(WorldObj.ObjectPrimaryFact, WorldObjectInfo.gameObject, BlackBoard.Agent, WorldObj.ObjectFactState));
                        if (WorldObjFact != null )
                        {

                            if(WorldObjFact.Fact == WorldObjectInfo.WorldObject.ObjectPrimaryFact)
                            {
                                WorldObjFact.factConfidence = 1f;

                            }
                            else
                            {
                                WorldObjFact.factConfidence = 0f;
                            }

                          
                            WorldObjFact.FactState = WorldObj.ObjectFactState;


                        }

                        if (WorldObjFact == null)
                        {
                            WorldObjFact = CreateWMFact(WorldObj.ObjectPrimaryFact, WorldObjectInfo.gameObject, BlackBoard.Agent, WorldObj.ObjectFactState);
                            if(WorldObj.ObjectPrimaryFact == AgentFact.Fact_InvalidType)
                            {
                                WorldObjFact.StringFact = WorldObj.WObject.ToString();

                            }
                            BlackBoard.AgentMemory.AddMemoryFact(WorldObjFact);

                        }





                    }



                }
            }
        }
    }


    WMFact CreateWMFact(AgentFact factToAdd, GameObject mobileAgent, GAgent agent, bool state)
    {

        WMFact NewFact = new WMFact();
        NewFact.Fact = factToAdd;
        NewFact.FactState = state;
        NewFact.SourceObject = mobileAgent;
        NewFact.factConfidence = 1f;


        return NewFact;


    }

    public void DrawGizmos(AIBlackBoard BlackBoard)
    {
        foreach(GameObject obj in visibleWorldObjects)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(BlackBoard.Head.position, obj.transform.position + Vector3.up * 2f);
            Gizmos.DrawSphere(obj.transform.position + Vector3.up * 1.5f, 0.25f);

        }

    }
    
}


