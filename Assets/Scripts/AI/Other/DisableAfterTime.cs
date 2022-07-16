using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterTime : MonoBehaviour
{

    public float time;

    private void OnEnable()
    {
        Invoke("Disable", time);
    }

    void Disable()
    {
        this.gameObject.SetActive(false);
    }
}
