using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [SerializeField] private float MaxSizeX, MaxSizeY, MaxSizeZ;
    [SerializeField] private float y_offSet;

    [SerializeField] private GameObject Prefab;
    [SerializeField] private LayerMask whatCantBeTouched;

    public void SpawnWeapon()
    {
        Vector3 pos = new Vector3(Random.Range(-MaxSizeX / 2, MaxSizeX / 2), MaxSizeY , Random.Range(-MaxSizeZ / 2, MaxSizeZ / 2));
        GameObject temp = Instantiate(Prefab);
        temp.transform.parent = this.transform;
        temp.transform.localPosition = pos;
        

        Collider[] touching = Physics.OverlapSphere(temp.transform.position, 1, whatCantBeTouched, QueryTriggerInteraction.Ignore);
        if (touching.Length == 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(temp.transform.position.x, temp.transform.position.y, temp.transform.position.z), -transform.up, out hit, whatCantBeTouched))
            {
                pos.y = hit.point.y;
            }
            Destroy(temp);
            GameObject obj = Instantiate(Prefab, this.transform);
            obj.transform.position = new Vector3(hit.point.x, hit.point.y + y_offSet, hit.point.z);
            FindObjectOfType<WeaponSpawnManager>().loot.Add(obj);
        }
        else
        {
            Destroy(temp);
            SpawnWeapon();
        }


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(MaxSizeX, MaxSizeY, MaxSizeZ));
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(MaxSizeX, MaxSizeY, MaxSizeZ));
    }
}
