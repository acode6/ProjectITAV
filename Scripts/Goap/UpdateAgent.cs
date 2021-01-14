using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UpdateAgent : MonoBehaviour
{
    public Text agentstates;

    public GAgent thisAgent;
    // Update is called once per frame
    private void Start()
    {
        thisAgent = gameObject.GetComponentInParent<GAgent>();
    }
    void LateUpdate()
    {


        
       
        Dictionary<string, bool> agentWorldstates = Gworld.Instance.GetWorld().GetStates();
        agentstates.text = "";
        agentstates.text += "Agent: " + thisAgent.name + "\n";
        agentstates.text += "Current Action: " + thisAgent.currentAction + "\n";
        agentstates.text += "Target: " + thisAgent.currentAction.target.name + "\n";
        agentstates.text += "==================BELIEFS==================== "  + "\n";
        foreach (KeyValuePair<string, bool> s in thisAgent.AgentFacts.GetStates())
        {
          
            agentstates.text +=  s.Key + " | " + " Amount: " + s.Value + "\n"; //the state plus the number of it in the world 
        }
        agentstates.text += "==================INVENTORY==================== " +  "\n";
        foreach (GameObject i in thisAgent.inventory.items)
        {

            agentstates.text += i.name +  "\n"; //the state plus the number of it in the world 
        }
        //update local UI

    }
}
