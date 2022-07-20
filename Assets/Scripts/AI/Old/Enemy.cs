using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator anim;

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

        if (anim != null)
        {
            anim.SetTrigger("TakeDamage");
        }

        if (!isDead && Health <= 0)
        {
            isDead = true;
            Instantiate(DieparticleOrExplosion, this.transform.position, Quaternion.identity);
            EnemyActions.OnEnemyKilled?.Invoke(navMeshAgent);

            if (navMeshAgent.IsInAttackRange)
                EnemyActions.RemoveEnemyAttacking?.Invoke(navMeshAgent);

            DeathSound.Play();

            if (anim != null)
            {
                this.gameObject.GetComponent<EnemyNavMesh>().Speed = 0;
                anim.SetBool("IsDead", true);
                Destroy(this.gameObject, 1f);
            }
            else
                Destroy(this.gameObject, .1f);
        }
        else
            EnemyHitSound.Play();
    }

}
