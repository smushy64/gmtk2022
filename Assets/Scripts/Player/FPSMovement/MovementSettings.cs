using UnityEngine;

namespace Docien.FPSMovement
{
    [CreateAssetMenu(menuName = "Player/Movement Settings")]
    public class MovementSettings : ScriptableObject
    {
        [SerializeField, Min(0f)] private float m_MaxSpeed = 10f;
        [SerializeField] private float m_Acceleration = 10f;
        [SerializeField] private float m_Friction = 10f;
        [SerializeField] private float m_StoppingSpeed = 1f;
        [SerializeField] private AnimationCurve m_AccelerationMultiplierFromDot;

        public float MaxSpeed => m_MaxSpeed;
        public float Acceleration => m_Acceleration;
        public float Friction => m_Friction;
        public float StoppingSpeed => m_StoppingSpeed;
        public AnimationCurve AccelerationMultiplier => m_AccelerationMultiplierFromDot;

        public float CalculateAcceleration(Vector3 goalDir, Vector3 currentDir)
        {
            float dot = Vector3.Dot(goalDir, currentDir);
            return m_Acceleration * m_AccelerationMultiplierFromDot.Evaluate(dot);
        }

        public float CalculateAcceleration(float dot) => m_Acceleration * m_AccelerationMultiplierFromDot.Evaluate(dot);
    }
}