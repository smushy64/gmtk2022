using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Docien.Springs;

namespace Docien.FPSMovement
{
    public class PlayerCameraSpring : MonoBehaviour
    {
        [SerializeField] private float m_Frequency;
        [SerializeField, Min(0)] private float m_Damping;
        [SerializeField] private float m_BaseHeight;
        [SerializeField] private float m_CrouchHeight;
        [SerializeField] private float m_SlideHeight;
        [SerializeField] private float m_SwimHeight;
        [SerializeField] private AnimationCurve m_LandingForceCurve;
        [SerializeField] private float m_CrouchForceMultiplier;

        private PlayerMovement m_Movement;
        private float m_Velocity = 0f;

        private void Awake()
        {
            m_Movement = GetComponentInParent<PlayerMovement>();
            m_Movement.onLanded += OnPlayerLanded;
        }

        private void LateUpdate()
        {
            float currentHeight = transform.localPosition.y;
            float targetHeight = m_BaseHeight;
            if (m_Movement.IsSubmerged)
            {
                targetHeight = m_SwimHeight;
            }
            else if (m_Movement.IsCrouching)
            {
                if (m_Movement.IsSliding)
                    targetHeight = m_SlideHeight;
                else
                    targetHeight = m_CrouchHeight;
            }


            SpringMotion.CalcDampedSimpleHarmonicMotion(ref currentHeight, ref m_Velocity, targetHeight,
                Time.deltaTime, m_Frequency, m_Damping);

            transform.localPosition = new Vector3(transform.localPosition.x, currentHeight, transform.localPosition.z);
        }

        private void OnPlayerLanded(float fallingVelocity)
        {
            float totalSpringForce = m_LandingForceCurve.Evaluate(-fallingVelocity);
            if (m_Movement.IsCrouching)
                totalSpringForce *= m_CrouchForceMultiplier;

            // Negative so that the force is downwards.
            m_Velocity = -totalSpringForce;
        }
    }
}

