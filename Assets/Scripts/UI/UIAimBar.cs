using System.Collections;
using UnityEngine;

public class UIAimBar : MonoBehaviour
{
    [SerializeField]
    RectTransform left, right;
    [SerializeField]
    float maxDelta = 20f;

    Vector3 homePosition;

    private void Awake() {
        homePosition = right.anchoredPosition3D;
    }

    /// <summary>
    /// multiplies input with 'maxDelta' which gets added to homePosition.x over time
    /// </summary>
    /// <param name="scale"></param>
    public void SetDelta(float scale) {
        Vector3 finalPosition = homePosition + (Vector3.right * (maxDelta * scale));
        if(animate != null) {
            this.StopCoroutine(animate);
        }
        animate = Animate(finalPosition);
        this.StartCoroutine(animate);
    }
    IEnumerator animate;
    IEnumerator Animate( Vector3 finalPosition ) {
        float timer = 0f;
        float maxTime = 2f;
        while(timer < maxTime) {
            Vector3 position = Vector3.Lerp( right.anchoredPosition3D, finalPosition, Time.deltaTime * 10f );
            left.anchoredPosition3D = -position;
            right.anchoredPosition3D = position;
            timer += Time.deltaTime;
            yield return null;
        }
        left.anchoredPosition3D = -finalPosition;
        right.anchoredPosition3D = finalPosition;
    }
}
