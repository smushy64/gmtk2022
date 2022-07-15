using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
    private Rigidbody m_Rigidbody;

    [SerializeField] private float m_Speed = 5f;
    [SerializeField] private float m_Rotation = 90f;
    [SerializeField] private List<Vector3> m_Waypoints;
    private int m_CurrentWaypoint;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.isKinematic = true;
    }

    private void FixedUpdate()
    {
        Quaternion newRot = Quaternion.AngleAxis(m_Rotation * Time.fixedDeltaTime, Vector3.up) * m_Rigidbody.rotation;
        m_Rigidbody.MoveRotation(newRot);

        if (m_Waypoints.Count <= 0)
            return;

        Vector3 newPos = Vector3.MoveTowards(transform.position, m_Waypoints[m_CurrentWaypoint], m_Speed * Time.fixedDeltaTime);
        m_Rigidbody.MovePosition(newPos);

        if (Vector3.Distance(m_Rigidbody.position, m_Waypoints[m_CurrentWaypoint]) <= Mathf.Epsilon)
        {
            if (m_CurrentWaypoint + 1 < m_Waypoints.Count)
                m_CurrentWaypoint++;
            else
                m_CurrentWaypoint = 0;
        }
    }
}
