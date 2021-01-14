using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSensor : MonoBehaviour, ISensor
{
    private static Collider[] _hits = new Collider[128];

    [SerializeField]
    [Tooltip("How often should we update our knowledge about the whereabouts of resources.")]
    private float _tickRate = 1f;

    public float TickRate => _tickRate;
    public float NextTickTime { get; set; }
    public Resource closest = null;
    public Resource closestFood = null;//closest food
    public Resource closestWater = null;//closest water
    public void Tick(AIBlackBoard BlackBox)
    {
      
        BlackBox.KnownResources.Clear(); //on each tick clear the list of known resources to update it 
        
       
      
        var hitCount = Physics.OverlapSphereNonAlloc(BlackBox.Position, BlackBox.Senses.DetectRadius, _hits);
        if (hitCount > 0)
        {
            for (var i = 0; i < hitCount; i++)
            {
                var resource = _hits[i].GetComponent<Resource>();
                if (resource != null)
                {
                    //add gameobjects of known resources to list
                    BlackBox.KnownResources.Add(resource);
                   
                    
                }
            }
           // if (BlackBox.CurrentWaterTarget == null || BlackBox.CurrentFoodTarget == null)
            //{

                float closetResource = Mathf.Infinity;
                Vector3 curLocation = BlackBox.Position;
            int amount = 0;
            int amount2 = 0;
            foreach (Resource r in BlackBox.KnownResources)
            {
                if (r.gameObject.tag == "Water")
                {
                    Vector3 directionToTarget = r.transform.position - BlackBox.Position;
                    float disSqrToTarget = directionToTarget.sqrMagnitude;
                    if (disSqrToTarget < closetResource)
                    {
                        closetResource = disSqrToTarget;
                        closestWater = r;
                        BlackBox.CurrentWaterTarget = closestWater;


                    }
                    
                    amount++;
                 

                }
               
            }
            foreach (Resource r in BlackBox.KnownResources)
            {

                if (r.gameObject.tag == "Food")
                {
                    Vector3 directionToTarget = r.transform.position - BlackBox.Position;
                    float disSqrToTarget = directionToTarget.sqrMagnitude;
                    if (disSqrToTarget < closetResource)
                    {
                        closetResource = disSqrToTarget;
                        closestFood = r;
                        BlackBox.CurrentFoodTarget = closestFood;


                    }
                    amount2++;
                   
                }
                

            }
         
                
         

            }

           

        }
    
    public void DrawGizmos(AIBlackBoard BlackBox)
    {
        foreach (var resource in BlackBox.KnownResources)
        {
            if (resource == BlackBox.CurrentFoodTarget)
            {
                Gizmos.color = Color.red;
            }
            if (resource == BlackBox.CurrentWaterTarget)
            {
                Gizmos.color = Color.blue;
            }
            else
            {
                Gizmos.color = new Color(0f, 1f, 1f, 0.15f);
            }
            Gizmos.DrawLine(BlackBox.Head.position, resource.transform.position + Vector3.up * 2f);
            Gizmos.DrawSphere(resource.transform.position + Vector3.up * 2f, 0.25f);
          
        }
    }
}
