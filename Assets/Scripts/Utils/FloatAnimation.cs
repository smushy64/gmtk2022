using UnityEngine;

public class FloatAnimation : MonoBehaviour
{   
    [SerializeField] private float frequency;
    [SerializeField] private float amplitude;
    [SerializeField] private float rotationSpeed;

    float y = 0f;

    private void Awake()
    {
        y = amplitude / 2f;
    }

    private void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);
        transform.localPosition = new Vector3(transform.localPosition.x,
            y + Mathf.Sin(Time.time * frequency) * amplitude,
            transform.localPosition.z);
    }
}
