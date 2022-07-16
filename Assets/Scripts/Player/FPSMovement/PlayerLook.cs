using UnityEngine;
using UnityEngine.InputSystem;

namespace Docien.FPSMovement
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField] private Transform m_Orientation;
        [SerializeField] private float m_Sensitivity;
        [SerializeField] private Vector2 m_PitchLimits;

        private PlayerInput m_PlayerInput;
        private InputAction m_LookAction;
        private Vector2 lastDelta = Vector2.zero;

        public Vector2 LastDelta => lastDelta;
        float mouseOffset = 0f;
        float mouseOffsetTarget = 0f;
        bool recoilRightBias = true;
        const float RECOIL_OFFSET = 0.2f;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            m_PlayerInput = GetComponent<PlayerInput>();
            m_LookAction = m_PlayerInput.actions["Look"];
        }

        private void LateUpdate()
        {
            Vector2 mouseDelta = m_LookAction.ReadValue<Vector2>() * (m_Sensitivity / 100f);
            mouseDelta.y -= mouseOffset;
            mouseDelta.x -= recoilRightBias ? -(mouseOffset / 2f) : (mouseOffset / 2f);
            lastDelta = mouseDelta;
            Vector3 eulerAngles = ConvertEulerToHalfRotation(m_Orientation.localEulerAngles);
            // Yaw - Horizontal Mouse Movement
            eulerAngles.y += mouseDelta.x;
            // Pitch - Vertical Mouse Movement
            eulerAngles.x = Mathf.Clamp(mouseDelta.y + eulerAngles.x, m_PitchLimits.x, m_PitchLimits.y);

            m_Orientation.localRotation = Quaternion.Euler(eulerAngles);

            InterpolateMouseOffset();
        }

        void InterpolateMouseOffset() {
            mouseOffset = Mathf.LerpUnclamped(mouseOffset, mouseOffsetTarget, Time.deltaTime * 50f);
            if( Mathf.Abs( mouseOffset ) >= Mathf.Abs( mouseOffset ) ) {
                mouseOffsetTarget = Mathf.Lerp( mouseOffsetTarget, 0f, Time.deltaTime * 100f );
            }
        }

        public void Recoil() {
            mouseOffsetTarget += RECOIL_OFFSET;
            recoilRightBias = (Random.Range( 0, 2 ) > 0) ? !recoilRightBias : recoilRightBias;
        }

        /// <summary>
        /// Converts euler angles from a range of 0 to 360 to a range of -180 and 180.
        /// </summary>
        /// <param name="eulerAngles"></param>
        /// <returns></returns>
        private Vector3 ConvertEulerToHalfRotation(Vector3 eulerAngles)
        {
            while (eulerAngles.x > 180f)
                eulerAngles.x -= 360f;

            while (eulerAngles.y > 180f)
                eulerAngles.y -= 360f;

            while (eulerAngles.z > 180f)
                eulerAngles.z -= 360f;

            return eulerAngles;
        }
    }
}