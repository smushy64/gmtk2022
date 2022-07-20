using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [SerializeField]
    GameObject rocketPrefab;

    Queue<Rocket> rockets;
    const int MAX_ROCKETS = 5;

    private void Awake() {
        rockets = new Queue<Rocket>();
        for (int i = 0; i < MAX_ROCKETS; i++) {
            rockets.Enqueue(
                Instantiate(
                    rocketPrefab,
                    Vector3.zero,
                    Quaternion.identity
                ).GetComponent<Rocket>()
            );
        }
    }

    public void Fire(float damage) {
        Rocket rocket = rockets.Dequeue();
        rocket.Activate(
            transform.position + (transform.forward) * 0.2f,
            transform.rotation,
            damage
        );
        rockets.Enqueue(rocket);
    }

}
