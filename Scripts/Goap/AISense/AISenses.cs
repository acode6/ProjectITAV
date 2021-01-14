using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[Serializable]
public class AISenses
{
    public Transform agentLocal;
    public float viewRadius;

    [Range(0,360)]
    public float viewAngle ;

    public float DetectRadius = 15f;
    public float SqrEyeSight => DetectRadius * DetectRadius;

    public float MeleeRange;
    public float LungeRange;
    public float ProjectTileRange;
    public float SqrMeleeRange => MeleeRange * MeleeRange;
    public float SqrLungeRange => LungeRange * LungeRange;
    public float SqrProjectileRange => ProjectTileRange * ProjectTileRange;

    public void DrawGizmos(AIBlackBoard BlackBox)
    {
        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);
        Gizmos.color = new Color(1f, 0f, 0f, 0.125f);
        Gizmos.DrawSphere(BlackBox.Position, DetectRadius);

        Gizmos.color = Color.black;
        Gizmos.DrawLine(BlackBox.Head.transform.position, BlackBox.Position + viewAngleA * viewRadius);
        Gizmos.DrawLine(BlackBox.Head.transform.position, BlackBox.Position + viewAngleB * viewRadius);
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += agentLocal.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }




}

