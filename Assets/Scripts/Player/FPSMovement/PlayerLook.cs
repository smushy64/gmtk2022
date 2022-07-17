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
        Vector2 mouseDelta = Vector2.zero;
        Vector2 mouseOffset = Vector2.zero;

        public Vector2 LastDelta => lastDelta;
        const float RECOIL_OFFSET = 0.2f;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            m_PlayerInput = GetComponent<PlayerInput>();
            m_LookAction = m_PlayerInput.actions["Look"];
        }

        private void LateUpdate()
        {
            if( PauseMenu.Paused ) {
                return;
            }
            mouseDelta = m_LookAction.ReadValue<Vector2>() * (m_Sensitivity / 100f) + mouseOffset;
            lastDelta = mouseDelta;
            Vector3 eulerAngles = ConvertEulerToHalfRotation(m_Orientation.localEulerAngles);
            // Yaw - Horizontal Mouse Movement
            eulerAngles.y += mouseDelta.x;
            // Pitch - Vertical Mouse Movement
            eulerAngles.x = Mathf.Clamp(mouseDelta.y + eulerAngles.x, m_PitchLimits.x, m_PitchLimits.y);

            m_Orientation.localRotation = Quaternion.Euler(eulerAngles);

            mouseOffset = Vector2.Lerp(mouseOffset, Vector2.zero, Time.deltaTime * 25f);
        }

        public void Recoil() {
            mouseOffset += new Vector2(Random.Range(-RECOIL_OFFSET, RECOIL_OFFSET), Random.Range(-RECOIL_OFFSET, -RECOIL_OFFSET / 2f));
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