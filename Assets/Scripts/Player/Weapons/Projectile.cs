using UnityEngine;

public class Projectile : MonoBehaviour
{

    [SerializeField]
    GameObject rocketPrefab;

    public void Fire() {
        GameObject rocket = Instantiate(
            rocketPrefab,
            transform.position + (transform.forward * 0.2f) + (transform.up * 0.1f),
            transform.rotation
        );
    }

}
