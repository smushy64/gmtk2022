using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float Health;
    [SerializeField] private GameObject DieparticleOrExplosion;

    private EnemyNavMesh navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<EnemyNavMesh>();
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Instantiate(DieparticleOrExplosion, this.transform.position, Quaternion.identity);
            EnemyActions.OnEnemyKilled?.Invoke(navMeshAgent);

            if (navMeshAgent.IsInAttackRange)
                EnemyActions.RemoveEnemyAttacking?.Invoke(navMeshAgent);

            Destroy(this.gameObject);
        }
    }
}
