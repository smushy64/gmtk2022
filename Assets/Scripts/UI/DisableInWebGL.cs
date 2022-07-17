using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableInWebGL : MonoBehaviour
{
    private void Awake()
    {
        if( Application.platform == RuntimePlatform.WebGLPlayer ) {
            Destroy(this.gameObject);
        }
    }
}
