using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlyinEnemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public LayerMask Layer;
    public float SphereSize = 10;
    RaycastHit hit;

    private float DelayBetweenAddingHeight;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    private void FixedUpdate()
    {
        Collider[] touching = Physics.OverlapSphere(transform.position, SphereSize, Layer, QueryTriggerInteraction.Ignore);
        Collider[] touchingCheck = Physics.OverlapSphere(transform.position, SphereSize + .25f, Layer, QueryTriggerInteraction.Ignore);

        if (touchingCheck.Length == 0 && touching.Length == 0)
        {
            if (DelayBetweenAddingHeight < 0)
            {
                DelayBetweenAddingHeight = .005f;
                agent.baseOffset -= 0.125f;
            }
            else
                DelayBetweenAddingHeight -= 1 * Time.deltaTime;
        }

        if (touching.Length != 0)
        {
            if (DelayBetweenAddingHeight < 0)
            {
                DelayBetweenAddingHeight = .005f;
                agent.baseOffset += 0.125f;
            }
            else
                DelayBetweenAddingHeight -= 1 * Time.deltaTime;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 10f);

    }
}
