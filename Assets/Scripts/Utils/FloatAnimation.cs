using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAnimation : MonoBehaviour
{   
    [SerializeField] private float frequency;
    [SerializeField] private float amplitude;
    [SerializeField] private float rotationSpeed;

    private float y;

    private void Awake()
    {
        y = transform.position.y;
    }

    private void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.World);
        transform.position = new Vector3(transform.position.x, y + Mathf.Sin(Time.time * frequency) * amplitude, transform.position.z);
    }
}
