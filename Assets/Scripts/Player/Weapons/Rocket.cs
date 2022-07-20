using System.Collections;
using UnityEngine;
using Docien.FPSMovement;

public class Rocket : MonoBehaviour
{

    [SerializeField]
    float speed = 10f, maxGravityScale = 0.4f;
    [SerializeField]
    float explosionRadius = 1f, impulseForce = 10f;
    [SerializeField]
    LayerMask enemyLayer, blockingLayers;
    [SerializeField]
    AnimationCurve damageFalloff;

    float damage = 1f;
    int playerLayer = 6;
    bool active = false;

    GameObject mesh;
    CapsuleCollider rocketCollider;
    Rigidbody r3d;
    ParticleSystem explosionParticles;
    SoundEffectPlayer sfxPlayer;
    static PlayerMovement playerMovement;
    static PlayerHealth playerHealth;
    void Awake() {
        r3d = GetComponent<Rigidbody>();
        rocketCollider = GetComponent<CapsuleCollider>();
        mesh = transform.Find("rocket").gameObject;
        explosionParticles = transform.Find("ExplosionParticle").GetComponent<ParticleSystem>();
        mesh.SetActive(false);
        rocketCollider.enabled = false;
        sfxPlayer = GetComponentInChildren<SoundEffectPlayer>();
        if( playerMovement == null ) {
            playerMovement = FindObjectOfType<PlayerMovement>();
            playerHealth = FindObjectOfType<PlayerHealth>();
        }
    }

    public void Activate( Vector3 atPosition, Quaternion withRotation, float damage ) {
        transform.position = atPosition;
        transform.rotation = withRotation;
        gravityScale = 0f;
        gravityScaleT = 0f;
        r3d.velocity = Vector3.zero;
        this.damage = damage;
        active = true;
        mesh.SetActive(true);
        rocketCollider.enabled = true;
    }

    private void FixedUpdate() {
        if(active)
            r3d.AddForce((transform.forward + (Vector3.down * gravityScale)) * speed, ForceMode.Acceleration);
    }
    float gravityScaleT = 0f;
    float gravityScale = 0f;
    private void Update()
    {
        if(active) {
            gravityScale = Mathf.Lerp(0f, maxGravityScale, gravityScaleT);
            gravityScaleT += Time.deltaTime;   
        }
    }

    void Explode() {
        // sphere cast check for enemies
        // ray cast to each enemy to see if there an obstacle in the way
        // if not, apply impulse force and damage
        int maxColliders = 10;
        Collider[] hitColliders = new Collider[maxColliders];
        int colliderCount = Physics.OverlapSphereNonAlloc(
            transform.position, explosionRadius,
            hitColliders, enemyLayer, QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < colliderCount; i++) {
            Collider hit = hitColliders[i];
            Vector3 direction = (-hit.transform.position + transform.position).normalized;

            if( !Physics.Raycast( transform.position, direction, explosionRadius, blockingLayers ) ) {
                float falloff = damageFalloff.Evaluate(
                    Vector3.Distance( transform.position, hit.transform.position ) / explosionRadius
                );
                hit.transform.GetComponent<Health>()?.Damage(damage * falloff);
                float impulseScaled = impulseForce * Mathf.Clamp01(falloff * 2f);

                Vector3 impulse = (-direction + (Vector3.up * 2f)) * impulseScaled;

                hit.transform.GetComponent<IImpulse>().Impulse(impulse);
            }
        }
        Vector3 playerDirection = (playerMovement.transform.position - transform.position ).normalized;
        // cast a ray to the player and if there's nothing blocking, apply impulse force at player
        if (!Physics.Raycast(transform.position, playerDirection, explosionRadius, blockingLayers)) {
            float falloff = damageFalloff.Evaluate(
                Vector3.Distance( transform.position, playerMovement.transform.position ) / explosionRadius
            );
            float impulseScaled = impulseForce * Mathf.Clamp01(falloff * 2f);
            Vector3 impulse = (playerDirection + (Vector3.up * 2f)) * impulseScaled;
            playerMovement.Impulse(impulse);
            playerHealth.Damage((damage * falloff) / 2f);
        }
    }

    IEnumerator DeactivateAfterExplosion() {
        yield return new WaitForSeconds(1.2f);
        active = false;
    }
    private void OnCollisionEnter(Collision other) {
        if( other.gameObject.layer == playerLayer )
            return;
        active = false;
        mesh.SetActive(false);
        rocketCollider.enabled = false;
        r3d.velocity = Vector3.zero;
        r3d.angularVelocity = Vector3.zero;
        Explode();
        sfxPlayer.Play();
        explosionParticles.Play();
        this.StartCoroutine(DeactivateAfterExplosion());
    }

    void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
