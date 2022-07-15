using UnityEngine;
using Docien.Springs;
using Docien.Math;

namespace Docien.FPSMovement
{
    public class PlayerCameraLean : MonoBehaviour
    {
        [Header("Default Leaning Settings"), SerializeField] private float m_MaxLean;
        [SerializeField] private float m_MaxSpeed;
        [SerializeField] private float m_Damping;
        [SerializeField] private float m_Frequency;
        [Header("Wallrun Leaning Settings"), SerializeField] private float m_MaxWallRunLean;
        [SerializeField] private float m_WallRunDamping;
        [SerializeField] private float m_WallRunFrequency;

        private float m_Velocity = 0f;
        private float m_Lean = 0f;

        private PlayerMovement m_Movement;

        private void Awake()
        {
            m_Movement = GetComponentInParent<PlayerMovement>();
        }

        private void LateUpdate()
        {

            float targetLean = 0f;
            if (!m_Movement.IsWallRunning)
            {
                // Get velocity relative to orientation
                Vector3 relativeVelocity = transform.InverseTransformDirection(m_Movement.RelativeTransverseVelocity);
                // Negated so that the math works out to negative for moving left and positive for moving right.
                float lateralSpeed = -relativeVelocity.x;
                float leanStrength = Mathf.InverseLerp(-m_MaxSpeed, m_MaxSpeed, lateralSpeed) * 2f - 1f;
                targetLean = leanStrength * m_MaxLean;

                if (m_Movement.IsSliding)
                    targetLean *= -1f;

                SpringMotion.CalcDampedSimpleHarmonicMotion(ref m_Lean, ref m_Velocity, targetLean,
                    Time.deltaTime, m_Frequency, m_Damping);
            }
            else
            {
                // If the dot product of our orientation and the direction parallel to the wall is positive, the wall
                // is to our left. Otherwise, the wall will be to our right.
                Vector3 orientation = transform.forward.FlattenToWorld();
                Vector3 cross = Vector3.Cross(Vector3.up, m_Movement.WallNormal).normalized;
                float dot = Vector3.Dot(orientation, cross);

                targetLean = dot * m_MaxWallRunLean;
                SpringMotion.CalcDampedSimpleHarmonicMotion(ref m_Lean, ref m_Velocity, targetLean,
                    Time.deltaTime, m_WallRunFrequency, m_WallRunDamping);
            }

            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, m_Lean);
        }
    }
}
