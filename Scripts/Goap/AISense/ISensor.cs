using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public interface ISensor
    {
        float TickRate { get; }
        float NextTickTime { get; set; }
        void Tick(AIBlackBoard BlackBox);
        void DrawGizmos(AIBlackBoard BlackBox);
    }

