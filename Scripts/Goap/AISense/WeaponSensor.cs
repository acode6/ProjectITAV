using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSensor : MonoBehaviour, ISensor
{
    private static Collider[] _hits = new Collider[128];

    [SerializeField]
    [Tooltip("How often should we update our knowledge about the whereabouts of weapons.")]
    private float _tickRate = 1f;

    public float TickRate => _tickRate;
    public float NextTickTime { get; set; }
    public Weapon closestWeapon = null;
   
    public void Tick(AIBlackBoard BlackBoard)
    {
      
        BlackBoard.KnownWeapons.Clear(); //on each tick clear the list of known resources to update it 


      
        var hitCount = Physics.OverlapSphereNonAlloc(BlackBoard.Position, BlackBoard.Senses.DetectRadius, _hits);
        if (hitCount > 0)
        {
            for (var i = 0; i < hitCount; i++)
            {
                var weapon = _hits[i].GetComponentInParent<Weapon>();
                if (weapon != null)
                {
                    //add gameobjects of known resources to list
                    BlackBoard.KnownWeapons.Add(weapon);
                   
                    
                }
            }
         

               
            foreach (Weapon w in BlackBoard.KnownWeapons)
            {

                WMFact checkFact = BlackBoard.AgentMemory.FindMemoryFact(CreateWMFact(w.gameObject, BlackBoard.Agent));
                if (checkFact != null) {



                    checkFact.factConfidence = 1f;


                }

                if(checkFact == null)
                {
                   checkFact = CreateWMFact(w.gameObject, BlackBoard.Agent);
                   BlackBoard.AgentMemory.AddMemoryFact(checkFact);

                }
                
                    
                
               

               
                  
                    

            }
            
         
                
         

            }

           

        }

    WMFact CreateWMFact(GameObject weapon , GAgent agent) {

        WMFact seeWeaponFact = new WMFact();
        seeWeaponFact.Fact = AgentFact.WeaponObject;
        seeWeaponFact.FactState = true;
        seeWeaponFact.SourceObject = weapon;
        seeWeaponFact.factConfidence = 1f;
        //agent.BlackBoard.AgentMemory.AddMemoryFact(seeWeaponFact);

        return seeWeaponFact;


    }
    
    public void DrawGizmos(AIBlackBoard BlackBox)
    {
        foreach (var weapon in BlackBox.KnownWeapons)
        {
            if (weapon == BlackBox.AgentMemory.FindClosestWeapon().GetComponent<Weapon>())
            {
                Gizmos.color = Color.red;
            }        
            else
            {
                Gizmos.color = new Color(0f, 1f, 1f, 0.15f);
            }
            Gizmos.DrawLine(BlackBox.Head.position, weapon.transform.position + Vector3.up * 2f);
            Gizmos.DrawSphere(weapon.transform.position + Vector3.up * 2f, 0.25f);
          
        }
    }
}
