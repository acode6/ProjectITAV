using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class CunsumeAction : GAction
{

    public override bool ContextPreConditions()
    {

        //if you have a dead animal or plant in your sight go to it and eat 
        //otherwise return false
   

        return false;
    }

    public override bool ActionEffect()
    {
       
        //if food is good then we should be good 
        
        return true;

       

    }
    public override bool InterruptActionCleanUp()
    {
        return false;
    }

    public override void ActionRunning() { }

}
