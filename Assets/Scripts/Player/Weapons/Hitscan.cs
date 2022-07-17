using UnityEngine;

public class Hitscan : MonoBehaviour
{
    
    [SerializeField]
    float spread = 5f;
    [SerializeField]
    private LayerMask hitscanMask;
    float multiplier = 1f;
    public void SetSpreadMultiplier(float multiplier) => this.multiplier = multiplier;

    void RayCast( Vector3 direction, int damage ) {
        Ray ray = new Ray(transform.position, direction);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, hitscanMask, QueryTriggerInteraction.Ignore)) {
            Debug.DrawLine(transform.position, hit.point, Color.blue, 3f);
            var enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        else {
            Debug.DrawRay(transform.position, direction * 100f, Color.red, 3f);
        }
    }

    public void Fire(int damage) {
        RayCast(CalculateSpread(this.multiplier), damage);
    }
    public void Fire( int pelletCount, int damage ) {
        for (int x = 0; x < pelletCount; ++x) {
            RayCast( CalculateSpread( this.multiplier * 1.2f ), damage );
        }
    }

    private Vector3 CalculateSpread( float multiplier )
    {
        Vector3 rotatedDir = Quaternion.AngleAxis(Random.Range(0f, (spread * multiplier)), Vector3.up) * transform.forward;
        return Quaternion.AngleAxis(Random.Range(0f, 360f), transform.forward) * transform.forward;
    }
}
