using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Node
{
    public Node parent;
    public float cost;
    public Dictionary<string, bool> state;
    public GAction action; //action that this node is pointing to

    public Node(Node parent, float cost, Dictionary<string, bool> allstates,GAction action)
    {
        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<string, bool>(allstates); //copyof all states
        this.action = action;


    }

    public Node(Node parent, float cost, Dictionary<string, bool> allstates, Dictionary<string, bool> beliefStates, GAction action)
    {
        this.parent = parent;
        this.cost = cost;
        this.state = new Dictionary<string, bool>(allstates); //copyof all states
        foreach (KeyValuePair<string, bool> b in beliefStates)
        {
            bool bValue = b.Value;
            if (!this.state.TryGetValue(b.Key, out bValue))
                this.state.Add(b.Key, b.Value);

        }
            
        this.action = action;


    }
}



public class GPlanner 
{
    public Queue<GAction> plan(List<GAction> actions, Dictionary<string, bool> goal, WorldStates beliefStates)
    {

        List<GAction> usableActions = new List<GAction>();
        foreach(GAction a in actions)
        {
            if (a.IsAchievable())
                usableActions.Add(a);
        }

        List<Node> leaves = new List<Node>();
        Node start = new Node(null, 0, Gworld.Instance.GetWorld().GetStates(), beliefStates.GetStates(), null);

        bool success = BuildGraph(start, leaves, usableActions, goal);

        if (!success)
        {
          //  Debug.Log("NO PLAN");
            return null;
        }

        Node cheapest = null;
        foreach(Node leaf in leaves)
        {
            if (cheapest ==null)
                cheapest = leaf;
            else
            {
                if (leaf.cost < cheapest.cost)
                    cheapest = leaf;
            }

        }

        List<GAction> result = new List<GAction>();
        Node n = cheapest;
        while(n!= null)
        {
            if(n.action != null)
            {
                result.Insert(0, n.action);
            }
            n = n.parent;
        }

        Queue<GAction> queue = new Queue<GAction>();
        foreach(GAction a in result)
        {
            queue.Enqueue(a);
        }

        //Debug.Log("The Plan is: ");
        foreach(GAction a in queue)
        {
        //   Debug.Log("Q: " + a.actionName);
        }
        return queue;
    }

    private bool BuildGraph(Node parent, List<Node> leaves, List<GAction>usableActions, Dictionary<string, bool> goal)
    {
        bool foundPath = false;
        foreach(GAction action in usableActions)
        {
            if (action.IsAchievableGiven(parent.state))
            {
                Dictionary<string, bool> currentState = new Dictionary<string, bool>(parent.state);
                foreach(KeyValuePair<string, bool> eff in action.effects)
                {
                    bool aValue = eff.Value;
                    if (!currentState.TryGetValue(eff.Key, out aValue))
                        currentState.Add(eff.Key, eff.Value);
                }

                Node node = new Node(parent, parent.cost + action.cost, currentState, action);

                if (GoalAchieved(goal, currentState))
                {
                    leaves.Add(node);
                    foundPath = true;
                }
                else
                {
                    List<GAction> subset = ActionSubset(usableActions, action);
                    bool found = BuildGraph(node, leaves, subset, goal);
                    if (found)
                        foundPath = true;
                }
            }
        }
        return foundPath;
    }

    private bool GoalAchieved(Dictionary<string, bool> goal, Dictionary<string, bool> state)
    {
        foreach(KeyValuePair<string, bool> g in goal)
        {
            bool gValue = g.Value;
         
            if(!state.TryGetValue(g.Key, out gValue))
               return false;
        }
        return true;
    }

    private List<GAction> ActionSubset(List<GAction> actions, GAction removeMe)
    {
        List<GAction> subset = new List<GAction>();
        foreach(GAction a in actions)
        {
            if (!a.Equals(removeMe))
                subset.Add(a);
        }
        return subset;
    }
}
