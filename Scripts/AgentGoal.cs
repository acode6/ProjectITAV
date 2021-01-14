using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AgentGoal", menuName = "Agent Goals")]
public class AgentGoal : ScriptableObject
{
    public GAgent ThisAgent;
    public AIAwareness AwarenessType;
    public AgentTask Satisfies;
    public bool state;
    public int GoalPriority;
    public bool RemovableGoal;
    
    //calculate goal relevancy and make goals scriptale objs!!!!!!!



}
