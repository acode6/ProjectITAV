using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static MapGrid;

public enum Biomes
{
  
    ForestBiome = (1 << 1),
    Desert = (1 << 2),
    Swamps = (1 << 3),
    Volcano = (1 << 4)
 
}
public class ResourceQueue
{
   

    public List<GameObject> que = new List<GameObject>();
    public string tag;
    public string modState; //how this resource modifies the world
    
    public ResourceQueue(string t, string ms, WorldStates w)
    {
       
        tag = t;
        modState = ms;
        if(tag != "")
        {
            GameObject[] resources = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject r in resources)
                que.Add(r);
        }
        //if mod state is not empty modify the world state along with how much is in the queue
        if(modState != "")
        {
          //  w.ModifyState(modState, que.);
          
        }
    }

    public void AddResource(GameObject r)
    {
        que.Add(r);
    }

  /*  public GameObject RemoveResource()
    {
        if (que.Count == 0)
            return null;
        return que.Dequeue();
    }*/

    public List<GameObject> ReturnResourceQueue()
    {
        return que;
    }
    //overloaded remove resource
    //This command loops thru queue and copies it into another while skipping selected resource
    public GameObject RemoveResource(GameObject r)
    {
       // que = new Queue<GameObject>(que.Where(p => p != r));
        if (que.Count == 0)
        {

            Debug.Log("Returning Null Here");
            return null;
        }
      
        else
        {
            que.Remove(r);

        }
        return r;
    }

    //search thru resources to find and return a specific resource in the queue and delete if need be 
    public GameObject FindResource(GameObject g)
    {

        if (que.Contains(g))
            return g;
        else
            return null;
    }

  //  Get all the resources in the game world and then cycle through the gameobjects in the queue to find the resource closet to you
    public GameObject FindResourceClosestToMe(Vector3 currentLocation)
    {
        GameObject closest = null;
        float closetResource = Mathf.Infinity;
        Vector3 curLocation = currentLocation;
        foreach (GameObject r in que)
        {
            Vector3 directionToTarget = r.transform.position - currentLocation;
            float disSqrToTarget = directionToTarget.sqrMagnitude;
            if(disSqrToTarget < closetResource)
            {
                closetResource = disSqrToTarget;
                closest = r;
            }
        }
      //  Debug.Log("Closest Resource :" + closest.name);
        return closest;
    }
   
}

public sealed class Gworld 
{
 
    private static readonly Gworld instance = new Gworld();
    private static WorldStates world; // dictionary holding all our states

    private static ResourceQueue hidingSpots;
    private static ResourceQueue grazeLocal;
    private static ResourceQueue waterLocation;
    private static ResourceQueue rabbitBurrows;
    private static ResourceQueue wolfDens;
    private static ResourceQueue LandMarks;
    private static ResourceQueue ForestBiomes;
    private static ResourceQueue DesertBiomes;
    private static ResourceQueue SwampBiomes;

    //Game Dictionaries 
    private static Dictionary<string, ResourceQueue> resources = new Dictionary<string, ResourceQueue>();
 
    
    //static so theirs only one game world
    static Gworld()
    {
        world = new WorldStates(); // create a new world space
      //  patients = new ResourceQueue("","",world);// intialize patients
        //resources.Add("patients", patients);
        
        //create a resource and its tag and state it adds to the world and send the world state
       hidingSpots = new ResourceQueue("HidingSpot", "availableHidingSpot ", world); //refactor later
       resources.Add("hidingspots", hidingSpots); //add this to the resources queue along with the resource queue name

       //grazeLocal = new ResourceQueue("GrazeLocal", "grazeBiomes", world); //refactor later
       //resources.Add("grazeLocals", grazeLocal);

       //waterLocation = new ResourceQueue("WaterLocal", "availableWater", world); //refactor later
       //resources.Add("waterLocals", waterLocation);

       //rabbitBurrows = new ResourceQueue("RabbitBurrow", "rabbitHomes", world); //refactor later
       //resources.Add("RabbitBurrow", rabbitBurrows);

       // wolfDens = new ResourceQueue("WolfDen", "wolfdens", world); //refactor later
       // resources.Add("WolfDen", wolfDens);

       // LandMarks = new ResourceQueue("LandMark", "landMarks", world); //refactor later
       //resources.Add("landmarks", LandMarks);

       // ForestBiomes= new ResourceQueue("ForestBiome", "forestBiomes", world); //refactor later
       // resources.Add("1", ForestBiomes);
        //  Time.timeScale = 2f;

    }

    //this returns the resource queue of string entered 
    public ResourceQueue GetQueue(string type)
    {
        return resources[type]; //returns a resource queue of the type entered
    }
    public Dictionary<string,ResourceQueue> GetWorldResources()
    {
        return resources;
    }
    public void AddToResources(string r, ResourceQueue q)
    {
        
        resources.Add(r, q);
    }

    private Gworld()
    {

    }
    
       
    //instance of the world state
    public static Gworld Instance
    {
        get { return instance; }
    }

    //get the world state 
    public WorldStates GetWorld()
    {
        return world;
    }
 

   
}
