using UnityEngine;

public class Rocket : MonoBehaviour
{

    [SerializeField]
    float speed = 10f, LifeTime = 5f;

    float gravityScale = 0f;

    Vector3 direction;
    Rigidbody r3d;
    SoundEffectPlayer sfx;

    public GameObject RocketExplosionPrefab;

    private void Awake()
    {
        r3d = GetComponent<Rigidbody>();
        sfx = transform.GetComponentInChildren<SoundEffectPlayer>();
    }

    private void Start() {
        direction = transform.forward;
        Invoke("DestroyMe", LifeTime);
    }

    void DestroyMe()
    {
        Instantiate(RocketExplosionPrefab, this.transform.position, transform.rotation);
        Destroy(gameObject);
    }

    float gravityScaleT = 0f;
    private void FixedUpdate() {
        r3d.AddForce((direction + (Vector3.down * gravityScale)) * speed, ForceMode.Acceleration);
    }
    private void Update()
    {
        gravityScale = Mathf.Lerp(0f, 1f, gravityScaleT);
        gravityScaleT += Time.deltaTime;   
    }
    private void OnCollisionEnter(Collision other) {
        sfx.transform.parent = null;
        sfx.destroyOnFinish = true;
        sfx.Play();
        Instantiate(RocketExplosionPrefab, this.transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
