using System;
using UnityEngine;

public class AIViewCone : MonoBehaviour
{
    [SerializeField]
    Vector3 offset = Vector3.zero;
    [SerializeField, Min(0.1f)]
    float viewLength = 1f;
    float halfLength = 0f;
    [SerializeField, Min(0.1f)]
    float viewRadius = 0f;
    float halfRadius;
    [SerializeField]
    LayerMask targetLayer;

    public float ViewConeLength => viewLength;

    public Action OnView;
    public bool IsEnabled = true;
    public bool CanSeeObject = false;
    bool check = false;

    private void Awake() {
        halfRadius = viewRadius / 2f;
        halfLength = viewLength / 2f;
    }

    private void Update() {
        if(!IsEnabled) {
            CanSeeObject = false;
            return;
        }
        // check only every other frame, dont wanna check every frame cause we got
        // collisions + array allocations, dont wanna make the cpu cry
        check = !check;
        if( !check ) {
            return;
        }

        Collider[] hitColliders = Physics.OverlapBox(
            transform.position + (transform.rotation * offset) + (transform.forward * halfLength),
            new Vector3(halfRadius, halfRadius, halfLength), transform.rotation,
            targetLayer, QueryTriggerInteraction.Ignore
        );

        CanSeeObject = false;
        if(hitColliders.Length > 0) {
            float targetDotProduct = TargetDotProduct();
            foreach( Collider collider in hitColliders ) {
                Vector3 directionToCollider = (collider.transform.position - (transform.position + offset)).normalized;
                if( Vector3.Dot(directionToCollider, transform.forward) > targetDotProduct ) {
                    CanSeeObject = true;
                    OnView?.Invoke();
                    return;
                }
            }
        }
    }

    float TargetDotProduct() {
        Vector3 end = (transform.position + offset) + (transform.forward * viewLength) + (transform.up * viewRadius);
        return Vector3.Dot(end.normalized, transform.forward);
    }

    private void OnDrawGizmosSelected() {
        DebugDrawViewCone();
        DebugDrawBoundingBox();
    }

    void DebugDrawViewCone() {
        Gizmos.color = CanSeeObject ? new Color( 0f, 0f, 1f, 0.5f ) : new Color( 1f, 0f, 0f, 0.5f );
        Vector3 start = transform.position + (transform.rotation * offset);
        Vector3 end = start + (transform.forward * viewLength);
        int resolution = 24;
        Vector3[] endPoints = new Vector3[resolution];
        for (int i = 0; i < resolution; ++i) {
            Vector3 newEnd = end + (Quaternion.AngleAxis(
                (360f / (float)resolution) * (float)i,
                transform.forward
            ) * (Vector3.up * viewRadius));
            Gizmos.DrawLine(start, newEnd);
            endPoints[i] = newEnd;
        }
        Gizmos.DrawLine(endPoints[resolution - 1], endPoints[0]);
        for (int i = 0; i < resolution - 1; ++i) {
            Gizmos.DrawLine(endPoints[i], endPoints[i + 1]);
        }
    }

    void DebugDrawBoundingBox() {
        Gizmos.color = new Color(0f, 1f, 1f, 0.2f);
        Vector3 origin = transform.position + offset;
        Vector3 top = origin + (transform.up * viewRadius);
        Vector3 bottom = origin - (transform.up * viewRadius);
        Vector3 front = transform.forward * viewLength;


        Vector3 topLeftBack = top + (-transform.right * viewRadius);
        Vector3 topRightBack = top + (transform.right * viewRadius);

        Vector3 bottomLeftBack = bottom + (-transform.right * viewRadius);
        Vector3 bottomRightBack = bottom + (transform.right * viewRadius);

        Vector3 topLeftFront = topLeftBack + front;
        Vector3 topRightFront = topRightBack + front;

        Vector3 bottomLeftFront = bottomLeftBack + front;
        Vector3 bottomRightFront = bottomRightBack + front;

        Gizmos.DrawLine(topLeftBack, topRightBack);
        Gizmos.DrawLine(topRightBack, bottomRightBack);
        Gizmos.DrawLine(topLeftBack, bottomLeftBack);
        Gizmos.DrawLine(bottomLeftBack, bottomRightBack);

        Gizmos.DrawLine(topLeftFront, topRightFront);
        Gizmos.DrawLine(topRightFront, bottomRightFront);
        Gizmos.DrawLine(topLeftFront, bottomLeftFront);
        Gizmos.DrawLine(bottomLeftFront, bottomRightFront);

        Gizmos.DrawLine(topLeftBack, topLeftFront);
        Gizmos.DrawLine(topRightBack, topRightFront);
        Gizmos.DrawLine(bottomLeftBack, bottomLeftFront);
        Gizmos.DrawLine(bottomRightBack, bottomRightFront);
    }
}
