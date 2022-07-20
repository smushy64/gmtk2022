using UnityEngine;
using System.Collections.Generic;

public class Hitscan : MonoBehaviour
{
    [SerializeField]
        GameObject hitParticlesPrefab;
    [SerializeField]
        GameObject wallHitParticlesPrefab;

    Queue<ParticleSystem> enemyHitParticles;
    Queue<ParticleSystem> wallHitParticles;
    enum BulletParticleType {
        EnemyHit,
        WallHit
    }
    const int MAX_PARTICLES = 12;

    private void Awake() {
        enemyHitParticles = new Queue<ParticleSystem>();
        wallHitParticles = new Queue<ParticleSystem>();
        for (int i = 0; i < MAX_PARTICLES; ++i) {
            enemyHitParticles.Enqueue(
                Instantiate(
                    hitParticlesPrefab,
                    Vector3.zero,
                    Quaternion.identity
                ).GetComponent<ParticleSystem>()
            );
            wallHitParticles.Enqueue(
                Instantiate(
                    wallHitParticlesPrefab,
                    Vector3.zero,
                    Quaternion.identity
                ).GetComponent<ParticleSystem>()
            );
        }
    }        
    
    [SerializeField]
    float spread = 5f;
    [SerializeField]
    LayerMask hitscanLayer;

    float multiplier = 1f;
    public void SetSpreadMultiplier(float multiplier) => this.multiplier = multiplier;

    void RayCast( Vector3 direction, float damage ) {
        Ray ray = new Ray(transform.position, direction);

        if( Physics.Raycast(
            ray, out RaycastHit hit,
            Mathf.Infinity, hitscanLayer,
            QueryTriggerInteraction.Ignore
        )) {
            BulletParticleType particlesType = BulletParticleType.EnemyHit;
            
            int enemyLayer = 8;
            if( hit.transform.gameObject.layer == enemyLayer ) {
                hit.transform.GetComponentInChildren<Health>().Damage(damage);
            }
            else {
                particlesType = BulletParticleType.WallHit;
            }

            PlayHitParticle(particlesType, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }
    
    void PlayHitParticle( BulletParticleType type, Vector3 atPosition, Quaternion withRotation ) {
        ParticleSystem particles;
        switch( type ) {
            case BulletParticleType.WallHit:
                particles = wallHitParticles.Dequeue();
                break;
            default: case BulletParticleType.EnemyHit:
                particles = enemyHitParticles.Dequeue();
                break;
        }

        particles.transform.position = atPosition;
        particles.transform.rotation = withRotation;
        particles.Play();

        switch( type ) {
            case BulletParticleType.WallHit:
                wallHitParticles.Enqueue(particles);
                break;
            default: case BulletParticleType.EnemyHit:
                enemyHitParticles.Enqueue(particles);
                break;
        }
    }

    public void Fire(float damage) {
        RayCast(CalculateSpread(this.multiplier), damage);
    }
    public void Fire( int pelletCount, float damage ) {
        for (int x = 0; x < pelletCount; ++x) {
            RayCast( CalculateSpread( this.multiplier * 1.2f ), damage );
        }
    }

    private Vector3 CalculateSpread( float multiplier )
    {
        Vector3 rotatedDir = Quaternion.AngleAxis(Random.Range(0f, (spread * multiplier)), Vector3.up) * transform.forward;
        return Quaternion.AngleAxis(Random.Range(0f, 360f), transform.forward) * transform.forward;
    }
}
