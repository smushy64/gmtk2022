using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ShakeCamera : MonoBehaviour
{
    CinemachineImpulseSource CIS;

    private void Start()
    {
        CIS = GetComponent<CinemachineImpulseSource>();
        CIS.GenerateImpulse();
    }
}
