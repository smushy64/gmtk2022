using UnityEngine;
using Docien.FPSMovement;

public class Sway : MonoBehaviour
{
    PlayerMovement movement;
    PlayerLook look;

    [SerializeField, Min(0f)]
    float maxVerticalDistance = 1f;
    [SerializeField, Min(0f)]
    float maxHorizontalDistance = 1f;
    [SerializeField, Min(0f)]
    float maxForwardDistance = 1f;

    [SerializeField, Min(0f)]
    float maxVerticalRotation = 25f;
    [SerializeField, Min(0f)]
    float maxHorizontalRotation = 25f;

    [SerializeField]
    Vector3 moveSwaySpeed = Vector3.one;
    [SerializeField]
    float rotationSwaySpeed = 2f;

    Vector3 moveSway = Vector3.zero;

    Vector3 homePosition = Vector3.zero;
    Vector3 homeRotation = Vector3.zero;

    Vector3 rotationSway = Vector3.zero;

    private void Awake()
    {
        movement = GetComponentInParent<PlayerMovement>();
        look = GetComponentInParent<PlayerLook>();

        homePosition = transform.localPosition;
    }

    private void Update()
    {
        MovementSway();
        RotationSway();
        ReturnToCenter();
    }

    void MovementSway() {
        if( !movement.IsGrounded )
            moveSway.y = movement.RelativeVelocity.y > 0f ? -maxVerticalDistance : maxVerticalDistance;
        else if( movement.IsCrouching )
            moveSway.y = maxVerticalDistance;
        else
            moveSway.y = 0f;

        if( movement.InputDirection.sqrMagnitude > 0f ) {
            moveSway.x = -movement.InputDirection.x * maxHorizontalDistance;
            moveSway.z = -movement.InputDirection.y * maxForwardDistance;
        }
        else {
            moveSway.x = 0f;
            moveSway.z = 0f;
        }

        transform.localPosition = new Vector3(
            Mathf.Lerp( transform.localPosition.x, homePosition.x + moveSway.x, moveSwaySpeed.x * Time.deltaTime ),
            Mathf.Lerp( transform.localPosition.y, homePosition.y + moveSway.y, moveSwaySpeed.y * Time.deltaTime ),
            Mathf.Lerp( transform.localPosition.z, homePosition.z + moveSway.z, moveSwaySpeed.z * Time.deltaTime )
        );
    }

    void RotationSway() {
        Vector2 lookInput = Vector2.ClampMagnitude(look.LastDelta, 1f);
        rotationSway.y = -lookInput.x * maxHorizontalRotation;
        rotationSway.z = -lookInput.y * maxVerticalRotation;

        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            Quaternion.AngleAxis( rotationSway.y, Vector3.up ) * Quaternion.AngleAxis( rotationSway.z, Vector3.right ),
            Time.deltaTime * rotationSwaySpeed
        );
    }

    void ReturnToCenter() {
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            homePosition,
            Time.deltaTime
        );

        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            Quaternion.identity,
            Time.deltaTime
        );
    }
}
