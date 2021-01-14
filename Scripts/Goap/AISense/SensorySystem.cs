
using System.Collections.Generic;
using UnityEngine;

public class SensorySystem
{
    private ISensor[] _sensors;

    public SensorySystem(GAgent agent)
    {
        _sensors = agent.transform.GetComponents<ISensor>();
    }

    public void Tick(AIBlackBoard BlackBox)
    {
        foreach (var sensor in _sensors)
        {
            if (BlackBox.Time >= sensor.NextTickTime)
            {
                sensor.NextTickTime = BlackBox.Time + sensor.TickRate;
                sensor.Tick(BlackBox);
            }
        }
    }

    public void DrawGizmos(AIBlackBoard BlackBox)
    {
        foreach (var sensor in _sensors)
        {
            sensor.DrawGizmos(BlackBox);
        }
    }
}