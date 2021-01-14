using UnityEngine;
using System.Collections;

public class Avoider : MonoBehaviour
{
    public string packTag = "Agent";
    public Transform avoidEnemy;
    public GAgent aGagent;
    public void Start()
    {
        aGagent = transform.parent.gameObject.GetComponent<GAgent>();
    }
    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag(packTag) && collider.transform != transform.parent)
        {

            if (collider.gameObject != aGagent.BlackBoard.CurrentEnemy)
            {
                avoidEnemy = collider.transform; //Avoid(collider.transform.position - transform.position);
            }

        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject == avoidEnemy.gameObject)
        {
            avoidEnemy = null;
        }
    }
}