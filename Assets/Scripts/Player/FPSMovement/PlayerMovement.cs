using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Docien.Math;

namespace Docien.FPSMovement
{
    [RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
    public class PlayerMovement : MonoBehaviour
    {
        public Action<float> onLanded;
        public Action onJump;
        public Action<bool> onStanceChanged;

        [SerializeField] private Transform m_Orientation;
        [SerializeField] private MovementSettings m_GroundMovementSettings;
        [SerializeField] private MovementSettings m_AirMovementSettings;
        [SerializeField] private MovementSettings m_CrouchMovementSettings;
        [SerializeField] private MovementSettings m_SlideMovementSettings;
        [SerializeField, Range(0f, 90f)] private float m_MaxSlopeAngle = 50f;
        [Header("Jumping"), SerializeField] private float m_JumpHeight = 10f;
        [SerializeField] private float m_CoyoteTimeDuration = 0.2f;
        [SerializeField] private float m_JumpBufferDuration = 0.2f;
        [Header("Sliding"), SerializeField] private bool m_CrouchingEnabled = false;
        [SerializeField] private bool m_EnableSliding = false;
        [SerializeField] private float m_SlideStartSpeed = 7f; // speed at which the player can start a slide
        [SerializeField] private float m_SlideExitSpeed = 4f; // speed at which a slide will automatically end
        [SerializeField] private float m_SlideBoost = 2f;
        [SerializeField] private AnimationCurve m_SlideBoostFalloff;
        [SerializeField] private float m_SlideSlopeAcceleration = 2f;
        [Header("Water"), SerializeField] private bool m_SwimmingEnabled = false;
        [SerializeField] private float m_SubmerganceThreshold = 0.75f;
        [SerializeField] private float m_SwimSpeed = 5f;
        [SerializeField] private float m_WaterAscensionSpeed = 5f;
        [SerializeField] private float m_WaterFriction = 4f;
        [SerializeField] private float m_WaterStoppingSpeed = 10f;
        [SerializeField] private float m_WaterAcceleration = 0f;
        [SerializeField] private float m_WaterFallSpeed = 5f;
        [SerializeField] private float m_WaterExitBoost = 10f;
        [SerializeField] private AnimationCurve m_WaterAccelerationMulitplier;
        [Header("Wall Jumping"), SerializeField] private bool m_WallJumpEnabled = false;
        [SerializeField] private float m_WallJumpForce;
        [SerializeField] private float m_WallJumpHeight;
        [Header("Wall Running"), SerializeField] private bool m_WallRunEnabled = false;
        [SerializeField] private float m_WallRunSpeed = 10f;
        [SerializeField] private float m_MinWallRunSpeed = 5f;
        [SerializeField] private float m_WallRunAcceleration = 10f;
        [SerializeField] private float m_WallRunGravity = 10f;
        [SerializeField] private float m_WallRunDotClamp = 0.8f;
        [Header("Ground Checking"), SerializeField] private float m_GroundCheckRadius = 0.3f;
        [SerializeField] private float m_GroundCheckDistance = 0.1f;
        [SerializeField] private float m_GroundSnapDistance = 0.3f;
        [SerializeField] private float m_StandingColliderHeight = 2f, m_CrouchingColliderHeight = 1f;
        [SerializeField] private bool m_RotateWithGround = true;
        [SerializeField] private LayerMask m_GroundMask, m_WaterMask = -1;
        [SerializeField] private bool m_Debug = false;
        [Header("Wall Checking"), SerializeField] private float m_WallCheckDistance;
        [SerializeField] private float m_MinWallAngle = 85f;
        [SerializeField] private float m_WallSnapStrength = 1f;

        private Rigidbody m_Rigidbody, m_ConnectedBody, m_PreviousConnectedBody;
        private CapsuleCollider m_Collider;
        private PlayerInput m_PlayerInput;
        private InputAction m_MoveAction, m_JumpAction, m_CrouchAction;

        private Vector3 m_UnitGoal, m_GoalVel = Vector3.zero;
        private Vector3 m_GroundVel, m_GroundNormal, m_WallNormal = Vector3.zero;
        private Vector3 m_PreviousVel = Vector3.zero;
        private Vector3 m_GroundConnectionLocalPosition, m_GroundConnectionWorldPosition = Vector3.zero;
        private float m_TimeOnGround, m_TimeInAir, m_JumpBufferTimer, m_TimeSinceLastJump, m_TimeSinceLastSlide = 0f;
        private float m_Submergance = 0f;
        private bool m_JumpBuffer, m_HoldingJump, m_DidJump, m_DesiresCrouch = false;
        private bool m_IsCrouched, m_IsSliding, m_IsSwimming, m_IsWallRunning = false;
        private bool m_IsGrounded, m_WasGrounded = true;
        private bool m_IsTouchingWall = false;

        Vector2 input;

        public bool IsCrouching => m_IsCrouched;
        public bool IsSliding => m_IsSliding;
        public bool IsWallRunning => m_IsWallRunning;
        public bool IsInWater => m_Submergance > 0;
        public bool IsSubmerged => m_Submergance >= m_SubmerganceThreshold;
        public bool IsGrounded => m_IsGrounded;
        public Vector2 InputDirection => input;
        public Vector3 TransverseVelocity => new Vector3(m_Rigidbody.velocity.x, 0f, m_Rigidbody.velocity.z);
        public Vector3 RelativeVelocity => m_Rigidbody.velocity - m_GroundVel;
        public Vector3 RelativeTransverseVelocity => new Vector3(m_Rigidbody.velocity.x - m_GroundVel.x, 0f, m_Rigidbody.velocity.z - m_GroundVel.z);
        public Vector3 WorldPositionCenter => transform.position + m_Collider.center;
        public Vector3 WallNormal => m_WallNormal;

        public void ToggleCrouching(bool toggle) => m_CrouchingEnabled = toggle;
        public void ToggleSliding(bool toggle) => m_EnableSliding = toggle;
        public void ToggleWalljumping(bool toggle) => m_WallJumpEnabled = toggle;
        public void ToggleWallrunning(bool toggle) => m_WallRunEnabled = toggle;

        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_PlayerInput = GetComponent<PlayerInput>();
            m_Collider = GetComponent<CapsuleCollider>();

            m_MoveAction = m_PlayerInput.actions["Move"];
            m_JumpAction = m_PlayerInput.actions["Jump"];
            m_CrouchAction = m_PlayerInput.actions["Crouch"];

            m_JumpAction.performed += OnJumpPerformed;
            m_JumpAction.canceled += ctx => m_HoldingJump = false;
            m_CrouchAction.performed += OnCrouchPerformed;
        }

        private void OnJumpPerformed(InputAction.CallbackContext ctx)
        {
            m_HoldingJump = true;

            if (IsSubmerged)
                return;

            m_JumpBuffer = true;
            m_JumpBufferTimer = 0f;
        }

        private void OnCrouchPerformed(InputAction.CallbackContext ctx)
        {
            m_DesiresCrouch = !m_DesiresCrouch;
        }

        // Returns false if we're unable to stand up.
        private bool Uncrouch()
        {
            // Check if there's anything above us. If so, return false and say we can't uncrouch.
            // This just prevents our collider from expanding in an area too small for it.
            var ray = new Ray(WorldPositionCenter, Vector3.up);
            float distance = (m_StandingColliderHeight - m_CrouchingColliderHeight + m_CrouchingColliderHeight / 2f) - m_GroundCheckRadius;
            if (Physics.SphereCast(ray, m_GroundCheckRadius, distance, m_GroundMask, QueryTriggerInteraction.Ignore))
            {
                return false;
            }

            m_IsCrouched = false;
            ChangeColliderHeight(m_StandingColliderHeight);

            onStanceChanged?.Invoke(m_IsCrouched);
            return true;
        }

        private void Crouch()
        {
            m_IsCrouched = true;
            ChangeColliderHeight(m_CrouchingColliderHeight);

            onStanceChanged?.Invoke(m_IsCrouched);
        }

        private void ChangeColliderHeight(float newHeight)
        {
            // Calculates how much we have to shift our center so the bottom of the new collider is
            // in the same position as the old collider. Makes switching stances and states much smoother.
            float moveY = (newHeight - m_StandingColliderHeight) / 2f;
            m_Collider.center = Vector3.up * moveY;
            m_Collider.height = newHeight;
        }

        private void Jump(float height)
        {
            float jumpForce = Mathf.Sqrt(2f * -Physics.gravity.y * height);
            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, jumpForce + m_GroundVel.y, m_Rigidbody.velocity.z);

            m_DidJump = true;
            m_JumpBuffer = false;
            m_TimeSinceLastJump = 0f;
            onJump?.Invoke();
        }

        private void WallJump()
        {
            Jump(m_WallJumpHeight);
            // Flatten the normal's y component so that our force vector does not depend on the wall's slope angle.
            Vector3 wallNormal = m_WallNormal;
            wallNormal.y = 0f;
            wallNormal.Normalize();
            m_Rigidbody.AddForce(wallNormal * m_WallJumpForce, ForceMode.VelocityChange);
        }

        private void FixedUpdate()
        {
            UpdateJumpBuffer();

            m_WasGrounded = m_IsGrounded;
            m_IsGrounded = GroundCheck() || SnapToGround();
            m_IsWallRunning = false;

            UpdateGroundVelocity();
            UpdateState();

            m_TimeSinceLastJump += Time.fixedDeltaTime;
            m_PreviousVel = m_Rigidbody.velocity;
            m_PreviousConnectedBody = m_ConnectedBody;
            m_ConnectedBody = null;
            m_IsTouchingWall = false;
            m_GroundVel = Vector3.zero;
        }

        private void UpdateJumpBuffer()
        {
            if (m_JumpBuffer)
            {
                m_JumpBufferTimer += Time.fixedDeltaTime;
                if (m_JumpBufferTimer > m_JumpBufferDuration)
                {
                    m_JumpBuffer = false;
                }
            }
        }

        private void UpdateGroundVelocity()
        {
            m_Rigidbody.angularVelocity = Vector3.zero;
            if (m_ConnectedBody)
            {
                if (m_ConnectedBody.isKinematic || m_ConnectedBody.mass > m_Rigidbody.mass)
                {
                    if (m_ConnectedBody == m_PreviousConnectedBody)
                    {
                        // Transform our local position relative to the ground from the last frame into the
                        // world position of this frame.
                        Vector3 worldPosition = m_ConnectedBody.transform.TransformPoint(m_GroundConnectionLocalPosition);
                        Vector3 connectionMovement = worldPosition - m_GroundConnectionWorldPosition;
                        m_GroundVel = connectionMovement / Time.fixedDeltaTime;
                    }

                    m_GroundConnectionWorldPosition = m_Rigidbody.position;
                    m_GroundConnectionLocalPosition = m_ConnectedBody.transform.InverseTransformPoint(m_GroundConnectionWorldPosition);

                    if (m_RotateWithGround)
                        m_Rigidbody.angularVelocity = m_ConnectedBody.angularVelocity.y * Vector3.up;
                }
            }
        }

        private void UpdateState()
        {
            if (!IsSubmerged)
            {
                m_GoalVel = GetDesiredMovementDirection();
                m_UnitGoal = m_GoalVel.normalized;

                if (m_CrouchingEnabled)
                {
                    if (m_IsCrouched && !m_DesiresCrouch)
                        Uncrouch();
                    else if (!m_IsCrouched && m_DesiresCrouch)
                        Crouch();
                }

                if (m_IsGrounded)
                {
                    GroundedState();
                }
                else
                {
                    AirborneState();
                }
            }
            else
            {
                WaterMove();
            }
        }

        private void GroundedState()
        {
            m_TimeOnGround += Time.fixedDeltaTime;
            m_TimeInAir = 0f;

            if (m_EnableSliding)
            {
                UpdateSlide();
            }

            if (m_IsSliding)
            {
                SlideMove(m_SlideMovementSettings);
            }
            else
            {
                var movementSettings = m_IsCrouched ? m_CrouchMovementSettings : m_GroundMovementSettings;
                GroundMove(movementSettings);
            }

            if (m_JumpBuffer && m_TimeOnGround >= Time.fixedDeltaTime * 3f)
            {
                Jump(m_JumpHeight);
            }

            if (!m_WasGrounded)
            {
                m_DidJump = false;
                onLanded?.Invoke(m_PreviousVel.y);
            }
        }

        private void UpdateSlide()
        {
            if (!m_IsSliding)
            {
                m_TimeSinceLastSlide += Time.fixedDeltaTime;
                // Start slide
                if (m_IsCrouched && RelativeVelocity.magnitude >= m_SlideStartSpeed)
                {
                    m_IsSliding = true;
                    Vector3 boostDir = ProjectVectorOnSlope(m_UnitGoal, m_GroundNormal);
                    Vector3 boostForce = boostDir * m_SlideBoost * m_SlideBoostFalloff.Evaluate(m_TimeSinceLastSlide);
                    m_Rigidbody.AddForce(boostForce, ForceMode.VelocityChange);
                    m_TimeSinceLastSlide = 0f;
                }
            }
            else
            {
                // End slide
                if (!m_IsCrouched || RelativeVelocity.magnitude <= m_SlideExitSpeed)
                {
                    m_IsSliding = false;
                }
            }
        }

        private void AirborneState()
        {
            if (m_IsTouchingWall || SnapToWall())
            {
                if (m_WallJumpEnabled)
                {
                    if (m_JumpBuffer)
                        WallJump();
                }

                if (m_WallRunEnabled)
                {
                    m_IsWallRunning = true;

                    if (m_IsCrouched)
                        Uncrouch();

                    WallrunMove();
                }
            }

            if (!m_IsWallRunning)
            {
                m_TimeInAir += Time.fixedDeltaTime;
                m_TimeOnGround = 0f;

                AirMove(m_AirMovementSettings);

                float gravityMult = 1f;
                if (m_DidJump)
                {
                    if (!m_HoldingJump && m_Rigidbody.velocity.y > 0f)
                    {
                        gravityMult = 2f;
                    }
                }

                m_Rigidbody.AddForce(Physics.gravity * gravityMult, ForceMode.Acceleration);

                if (m_JumpBuffer)
                {
                    if (m_TimeSinceLastJump > m_CoyoteTimeDuration + 0.1f && m_TimeInAir <= m_CoyoteTimeDuration)
                    {
                        Jump(m_JumpHeight);
                    }
                }
            }
        }

        private void WallrunMove()
        {
            Vector3 cross = Vector3.Cross(Vector3.up, WallNormal).normalized;
            float dot = Vector3.Dot(m_UnitGoal, cross);
            float magnitude = m_GoalVel.magnitude;
            m_UnitGoal = Vector3.ProjectOnPlane(m_UnitGoal, WallNormal);

            // If the player's goal direction is approximately parallel to the wall, help them out and
            // assume they want to move full speed along the wall.
            if (dot > m_WallRunDotClamp || dot < -m_WallRunDotClamp)
                m_UnitGoal.Normalize();

            m_GoalVel = m_UnitGoal * m_WallRunSpeed * magnitude;

            Accelerate(m_WallRunAcceleration, Vector3.ProjectOnPlane(RelativeTransverseVelocity, WallNormal));

            if (m_Rigidbody.velocity.y <= 0f)
            {
                float gravityStrength = 1f - Mathf.InverseLerp(0f, m_MinWallRunSpeed, TransverseVelocity.magnitude);

                if (gravityStrength <= 0f)
                    m_Rigidbody.velocity = TransverseVelocity;
                else
                    m_Rigidbody.AddForce(Vector3.down * m_WallRunGravity * gravityStrength, ForceMode.Acceleration);

            }
            else
            {
                m_Rigidbody.AddForce(Vector3.down * m_WallRunGravity, ForceMode.Acceleration);
            }
        }

        // Reads movement input, then transforms it into world space.
        private Vector3 GetDesiredMovementDirection()
        {
            input = m_MoveAction.ReadValue<Vector2>();
            // make sure that our input never exceeds one
            float magnitude = Mathf.Clamp01(input.magnitude);

            // world space transformation
            Vector3 movementDirection = m_Orientation.TransformDirection(input.x, 0f, input.y);
            // flatten the vector so we only move horizontally
            movementDirection.y = 0f;
            movementDirection.Normalize();

            return movementDirection * magnitude;
        }

        private void GroundMove(MovementSettings settings)
        {
            m_GoalVel *= settings.MaxSpeed;

            // If we jumped this frame, don't account for slopes and just ignore our y velocity.
            Vector3 relativeVelocity = RelativeVelocity;
            if (m_TimeSinceLastJump <= 0.02f)
                relativeVelocity.y = 0f;
            else
            {
                m_UnitGoal = ProjectVectorOnSlope(m_UnitGoal, m_GroundNormal);
                m_GoalVel = m_UnitGoal * m_GoalVel.magnitude;
            }

            Decelerate(settings.Friction, settings.StoppingSpeed, relativeVelocity);

            relativeVelocity = RelativeVelocity;
            if (m_TimeSinceLastJump <= 0.02f)
                relativeVelocity.y = 0f;

            // Treat not moving at all as though we want to move backwards for quicker deceleration
            float dot = m_GoalVel.magnitude == 0f ? -1f : Vector3.Dot(m_UnitGoal, relativeVelocity);
            float acceleration = settings.CalculateAcceleration(dot);
            Accelerate(acceleration, relativeVelocity);
        }

        private void AirMove(MovementSettings settings)
        {
            m_GoalVel *= settings.MaxSpeed;

            Decelerate(settings.Friction, settings.StoppingSpeed, TransverseVelocity);

            float acceleration = settings.CalculateAcceleration(m_UnitGoal, TransverseVelocity.normalized);
            Accelerate(acceleration, TransverseVelocity);
        }

        private void SlideMove(MovementSettings settings)
        {
            m_GoalVel *= settings.MaxSpeed;
            m_UnitGoal = ProjectVectorOnSlope(m_UnitGoal, m_GroundNormal);
            m_GoalVel = m_UnitGoal * m_GoalVel.magnitude;

            Vector3 relativeVelocity = RelativeVelocity;

            Decelerate(settings.Friction, settings.StoppingSpeed, relativeVelocity);

            float acceleration = settings.CalculateAcceleration(m_UnitGoal, relativeVelocity.normalized);
            Accelerate(acceleration, relativeVelocity);

            // Adds a force straight down a slope. Makes it so that you can slide for longer on steeper slopes.
            Vector3 perpendicularSlopeVector = Vector3.Cross(m_GroundNormal, Vector3.up);
            Vector3 slopeVector = Vector3.Cross(m_GroundNormal, perpendicularSlopeVector).normalized;
            float oneMinusDot = 1f - Vector3.Dot(Vector3.up, m_GroundNormal);
            m_Rigidbody.AddForce(slopeVector * -Physics.gravity.y * oneMinusDot * m_SlideSlopeAcceleration, ForceMode.Acceleration);
        }

        private void WaterMove()
        {
            Vector3 input = m_MoveAction.ReadValue<Vector2>();
            float magnitude = Mathf.Clamp01(input.magnitude);
            input = m_Orientation.TransformDirection(input.x, 0f, input.y);
            m_UnitGoal = input.normalized;
            m_GoalVel = m_UnitGoal * magnitude * m_SwimSpeed;

            if (m_HoldingJump)
                m_GoalVel.y = m_WaterAscensionSpeed;

            Decelerate(m_WaterFriction, m_WaterStoppingSpeed, m_Rigidbody.velocity);
            Accelerate(m_WaterAcceleration, m_Rigidbody.velocity);

            m_Rigidbody.AddForce(Vector3.up * -m_WaterFallSpeed, ForceMode.Acceleration);
        }

        private void Accelerate(float acceleration, Vector3 currentVelocity)
        {
            float currentSpeed = Vector3.Dot(currentVelocity, m_UnitGoal);
            float addSpeed = m_GoalVel.magnitude - currentSpeed;

            if (addSpeed <= 0)
                return;

            Vector3 neededAcceleration = (m_GoalVel - currentVelocity) / Time.fixedDeltaTime;
            neededAcceleration = Vector3.ClampMagnitude(neededAcceleration, acceleration);

            m_Rigidbody.AddForce(neededAcceleration, ForceMode.Acceleration);
        }

        /// <summary>
        /// Applies a force counter to our current velocity, eventually bringing it to a stop if no other forces are involved.
        /// </summary>
        /// <param name="friction"></param>
        /// <param name="velocity"></param>
        private void Decelerate(float friction, float stoppingSpeed, Vector3 velocity)
        {
            float speed = velocity.magnitude;
            if (speed <= 0)
                return;

            float control = speed < stoppingSpeed ? stoppingSpeed : speed;
            float drop = control * friction * Time.fixedDeltaTime;
            if (speed - drop < 0)
                drop = speed;

            Vector3 neededFrictionForce = (-velocity.normalized * drop) / Time.fixedDeltaTime;
            m_Rigidbody.AddForce(neededFrictionForce, ForceMode.Acceleration);
        }

        private bool GroundCheck()
        {
            var ray = new Ray(WorldPositionCenter, Vector3.down);
            float sphereCastDistance = (m_Collider.height / 2f) - m_GroundCheckRadius + m_GroundCheckDistance;
            if (!Physics.SphereCast(ray, m_GroundCheckRadius, out RaycastHit hit, sphereCastDistance, m_GroundMask, QueryTriggerInteraction.Ignore))
                return false;

            float groundAngle = Vector3.Angle(Vector3.up, hit.normal);
            if (groundAngle > m_MaxSlopeAngle)
                return false;

            m_GroundNormal = hit.normal;
            m_ConnectedBody = hit.rigidbody;
            return true;
        }

        private bool SnapToGround()
        {
            if (m_TimeInAir > Time.fixedDeltaTime || m_TimeSinceLastJump <= Time.fixedDeltaTime * 2f || IsSubmerged)
                return false;

            float raycastDistance = (m_Collider.height / 2f) + m_GroundSnapDistance;
            var ray = new Ray(WorldPositionCenter, Vector3.down);
            if (!Physics.Raycast(ray, out RaycastHit hit, raycastDistance, m_GroundMask, QueryTriggerInteraction.Ignore))
                return false;

            Vector3 groundNormal = hit.normal;
            float groundAngle = Vector3.Angle(Vector3.up, groundNormal);
            if (groundAngle > m_MaxSlopeAngle)
                return false;

            Debug.DrawRay(hit.point, hit.normal, Color.blue);
            m_GroundNormal = groundNormal;
            m_ConnectedBody = hit.rigidbody;
            float speed = m_Rigidbody.velocity.magnitude;
            float dot = Vector3.Dot(groundNormal, m_Rigidbody.velocity);
            if (dot > 0f)
            {
                m_Rigidbody.velocity = (m_Rigidbody.velocity - groundNormal).normalized * speed;
            }

            m_Rigidbody.AddForce(Physics.gravity, ForceMode.Acceleration);
            return true;
        }

        private bool SnapToWall()
        {
            if (m_TimeSinceLastJump <= Time.fixedDeltaTime * 5f)
                return false;

            Vector3 right = m_Orientation.right.FlattenToWorld();
            Vector3 left = -right;
            var leftRay = new Ray(transform.position, left);
            var rightRay = new Ray(transform.position, right);
            RaycastHit hit;

            if (CheckForWall(leftRay, out hit) || CheckForWall(rightRay, out hit))
            {
                m_WallNormal = hit.normal;
                float speed = TransverseVelocity.magnitude;
                float dot = Vector3.Dot(WallNormal, TransverseVelocity);
                if (dot > 0f)
                {
                    m_Rigidbody.velocity = (m_Rigidbody.velocity - m_WallNormal * m_WallSnapStrength).normalized * speed;
                }

                return true;
            }

            return false;
        }

        private bool CheckForWall(Ray ray, out RaycastHit hit)
        {
            if (!Physics.Raycast(ray, out hit, m_WallCheckDistance, m_GroundMask))
                return false;

            Vector3 wallNormal = hit.normal;
            float wallAngle = Vector3.Angle(Vector3.up, wallNormal);
            if (wallAngle < m_MinWallAngle)
                return false;

            return true;
        }

        private Vector3 ProjectVectorOnSlope(Vector3 vectorToProject, Vector3 normal)
        {
            Vector3 axis = Vector3.Cross(Vector3.up, normal);
            float angle = Vector3.SignedAngle(Vector3.up, normal, axis);
            var rotation = Quaternion.AngleAxis(angle, axis);
            return rotation * vectorToProject;
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((m_WaterMask & (1 << other.gameObject.layer)) != 0)
            {
                EvaluateSubmergance();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if ((m_WaterMask & (1 << other.gameObject.layer)) != 0)
            {
                EvaluateSubmergance();
            }
        }

        private void EvaluateSubmergance()
        {
            if (!m_SwimmingEnabled)
                return;

            var ray = new Ray(WorldPositionCenter + Vector3.up * m_Collider.height / 2f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, m_Collider.height + 1f, m_WaterMask, QueryTriggerInteraction.Collide))
            {
                m_Submergance = 1f - hit.distance / m_Collider.height;
            }
            else
            {
                m_Submergance = 1f;
            }

            if (m_IsSwimming && !IsSubmerged)
                StopSwimming();
            else if (!m_IsSwimming && IsSubmerged)
                StartSwimming();
        }

        private void StartSwimming()
        {
            m_IsCrouched = false;
            m_IsSliding = false;
            m_IsSwimming = true;

            ChangeColliderHeight(1f);
        }

        private void StopSwimming()
        {
            m_IsSwimming = false;

            if (!Uncrouch())
            {
                m_IsCrouched = true;
            }

            if (m_HoldingJump)
            {
                m_Rigidbody.AddForce(Vector3.up * m_WaterExitBoost, ForceMode.VelocityChange);
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                var contact = collision.GetContact(i);
                Vector3 contactNormal = contact.normal;
                float contactAngle = Vector3.Angle(Vector3.up, contactNormal);
                if (contactAngle < m_MinWallAngle)
                    continue;

                m_WallNormal = contactNormal;
                m_IsTouchingWall = true;
            }
        }

        private void OnDrawGizmos()
        {
            if (!m_Debug)
                return;

            float height = 2f;
            if (m_Collider != null)
                height = m_Collider.height;

            float sphereCastDistance = (height / 2f) - m_GroundCheckRadius + m_GroundCheckDistance;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, m_GroundCheckRadius);
            Gizmos.DrawWireSphere(transform.position + Vector3.down * sphereCastDistance, m_GroundCheckRadius);
        }

        private void OnGUI()
        {
            if (!m_Debug)
                return;

            GUI.Label(new Rect(10f, 10f, 300f, 30f), $"Velocity: {m_Rigidbody.velocity}", GUI.skin.box);

            float fallingSpeed = m_Rigidbody.velocity.y;
            Vector3 horizontalVelocity = m_Rigidbody.velocity;

            if (!m_IsGrounded)
                horizontalVelocity.y = 0f;

            GUI.Label(new Rect(10f, 40f, 300f, 30f), "Speed: " + horizontalVelocity.magnitude.ToString("F2"), GUI.skin.box);
            GUI.Label(new Rect(10f, 70f, 300f, 30f), "Falling Speed: " + fallingSpeed.ToString("F2"), GUI.skin.box);
            GUI.Label(new Rect(10f, 100f, 300f, 30f), $"Is Grounded: {m_IsGrounded}", GUI.skin.box);
            GUI.Label(new Rect(10f, 130f, 300f, 30f), $"Slope Angle: {Vector3.Angle(Vector3.up, m_GroundNormal)}", GUI.skin.box);
            GUI.Label(new Rect(10f, 160f, 300f, 30f), $"Touching Wall: {m_IsTouchingWall}", GUI.skin.box);
        }
    }
}