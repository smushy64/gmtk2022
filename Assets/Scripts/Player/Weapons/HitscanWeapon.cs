using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class HitscanWeapon : MonoBehaviour
{
    private float fireRate = 1f;
    private float timeSinceLastShot = 0f;

    public bool CanFire => timeSinceLastShot >= 1f / fireRate;

    private void OnEnable()
    {
        // Bypass firerate cooldown if we're just pulling this weapon out.
        timeSinceLastShot = 1f / fireRate;
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;
    }

    public void Fire(Vector3 origin, Vector3 direction)
    {
        timeSinceLastShot = 0f;

        Ray ray = new Ray(origin, direction);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Debug.DrawLine(origin, hit.point, Color.blue, 3f);
        }
        else
        {
            Debug.DrawRay(origin, direction * 100f, Color.red, 3f);
        }
    }
}
