using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Docien.Springs;

public class WeaponAnimator : MonoBehaviour
{
    [SerializeField] private float reloadAngle;
    [SerializeField] private float damping = 0.5f;
    [SerializeField] private float frequency = 10f;

    private float target = 0f;
    private float vel = 0f;

    private void Update()
    {
        float currentRotationX = transform.localEulerAngles.x;
        if (currentRotationX > 180f)
            currentRotationX -= 360f;

        SpringMotion.CalcDampedSimpleHarmonicMotion(ref currentRotationX, ref vel, target, Time.deltaTime, frequency, damping);
        UpdateRotation(currentRotationX);
    }

    public void StartReload()
    {
        target = 50f;
    }

    public void EndReload()
    {
        target = 0f;
    }

    public void QuitReload()
    {
        target = 0f;
        vel = 0f;
        UpdateRotation(target);
    }

    private void UpdateRotation(float newX)
    {
        transform.localEulerAngles = new Vector3(newX, transform.localRotation.y, transform.localRotation.z);
    }
}
