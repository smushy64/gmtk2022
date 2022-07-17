using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReticleHit : MonoBehaviour
{
    public Image Reticle, ReticleLeft, ReticleRight;
    public Gradient gradient;

    public void SetHit()
    {
        StartCoroutine(Hit());
    }
    public IEnumerator Hit()
    {
        //no idea why the loop didnt work so I made it like that

        Reticle.color = gradient.Evaluate(.5f);
        ReticleLeft.color = gradient.Evaluate(.5f);
        ReticleRight.color = gradient.Evaluate(.5f);

        yield return new WaitForSeconds(.025f);

        Reticle.color = gradient.Evaluate(1);
        ReticleLeft.color = gradient.Evaluate(1);
        ReticleRight.color = gradient.Evaluate(1);


        yield return new WaitForSeconds(.2f);


        Reticle.color = gradient.Evaluate(.5f);
        ReticleLeft.color = gradient.Evaluate(.5f);
        ReticleRight.color = gradient.Evaluate(.5f);

        yield return new WaitForSeconds(.025f);

        Reticle.color = gradient.Evaluate(0);
        ReticleLeft.color = gradient.Evaluate(0);
        ReticleRight.color = gradient.Evaluate(0);

    }
}
