using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private SoundEffectPlayer DeathSound, EnemyHitSound;

    [SerializeField] private float Health;
    [SerializeField] private GameObject DieparticleOrExplosion;
    private bool isDead = false;

    public float publicHealth
    {
        get
        {
            return Health;
        }
        set
        {
            //we can change maybe color of the enemy or make it a bit bigger if it got increased health
            Health = value;
        }
    }

    private EnemyNavMesh navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<EnemyNavMesh>();
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;

        if (!isDead && Health <= 0)
        {
            isDead = true;
            Instantiate(DieparticleOrExplosion, this.transform.position, Quaternion.identity);
            EnemyActions.OnEnemyKilled?.Invoke(navMeshAgent);

            if (navMeshAgent.IsInAttackRange)
                EnemyActions.RemoveEnemyAttacking?.Invoke(navMeshAgent);

            DeathSound.Play();

            Destroy(this.gameObject, .1f);
        }
        else
            EnemyHitSound.Play();
    }
}
