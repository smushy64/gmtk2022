using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Docien.Springs;

namespace Docien.FPSMovement
{
    public class WeaponSway : MonoBehaviour
    {
        private PlayerMovement movement;

        [Header("Leaning")]
        [SerializeField] private float maxSpeed;
        [SerializeField] private float maxLeanDegrees = 5f;
        [Header("Side Movement")]
        [SerializeField] private Vector3 positionLeft;
        [SerializeField] private Vector3 positionRight;
        [Header("Vertical Movement")]
        [SerializeField] private Vector2 landingVelocity;
        [SerializeField] private AnimationCurve landingForceCurve;
        [SerializeField] private Vector2 jumpingVelocity;

        [Header("Spring Settings")]
        [SerializeField] private float damping = 1f;
        [SerializeField] private float frequency = 10f;

        private Vector2 target = Vector2.zero;
        private Vector2 current = Vector2.zero;
        private Vector2 velocity = Vector2.zero;

        private void Awake()
        {
            movement = GetComponentInParent<PlayerMovement>();
            movement.onJump = OnJump;
            movement.onLanded = OnLand;
        }

        private void Update()
        {
            // Get velocity relative to orientation
            Vector3 relativeVelocity = transform.InverseTransformDirection(movement.RelativeTransverseVelocity);
            // Negated so that the math works out to negative for moving left and positive for moving right.
            float lateralSpeed = -relativeVelocity.x;
            target.x = Mathf.InverseLerp(-maxSpeed, maxSpeed, lateralSpeed);

            SpringMotion.CalcDampedSimpleHarmonicMotion(ref current, ref velocity, target,
                Time.deltaTime, frequency, damping);

            Vector3 newPosition = Vector3.Lerp(positionLeft, positionRight, current.x);
            float newLean = (current.x * 2f - 1f) * maxLeanDegrees;

            newPosition.y += current.y;
            transform.localPosition = newPosition;
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, newLean);
        }

        private void OnLand(float force)
        {
            velocity = landingVelocity * landingForceCurve.Evaluate(force);
        }

        private void OnJump()
        {
            velocity = jumpingVelocity;
        }
    }
}
