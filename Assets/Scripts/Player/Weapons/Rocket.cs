using UnityEngine;

public class Rocket : MonoBehaviour
{

    [SerializeField]
    float speed = 10f;

    float gravityScale = 0f;

    Vector3 direction;
    Rigidbody r3d;
    SoundEffectPlayer sfx;

    private void Awake()
    {
        r3d = GetComponent<Rigidbody>();
        sfx = transform.GetComponentInChildren<SoundEffectPlayer>();
    }

    private void Start() {
        direction = transform.forward;
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
        Destroy(gameObject);
    }
}
