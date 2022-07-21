using System.Collections;
using UnityEngine;

public class MuzzleFlashLight : MonoBehaviour
{
    Light mfLight;

    private void Awake() {
        mfLight = GetComponent<Light>();
        mfLight.enabled = false;
    }

    public void Flash() {
        mfLight.enabled = true;
        this.StopAllCoroutines();
        disable = Disable();
        this.StartCoroutine(disable);
    }
    IEnumerator disable;
    IEnumerator Disable() {
        yield return new WaitForSeconds(0.05f);
        mfLight.enabled = false;
    }
}
