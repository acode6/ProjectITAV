using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UpdateWorld : MonoBehaviour
{

    public Text states;

   
    // Update is called once per frame
    void LateUpdate()
    {


        //update world UI
        
        Dictionary<string, bool> worldstates = Gworld.Instance.GetWorld().GetStates();
        states.text = "";
        foreach (KeyValuePair<string, bool> s in worldstates)
        {
            states.text += "World State: "+ s.Key + " | "+" State: " + s.Value + "\n"; //the state plus the number of it in the world 
        }
        //update local UI

    }
}
