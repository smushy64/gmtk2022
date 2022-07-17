using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    [SerializeField] private GameObject ammoPickupPrefab;
    [SerializeField, Range(0,1)] private float ammoSpawnChance;

    private void Awake()
    {
        EnemyActions.OnEnemyKilled += RollForAmmoCrateSpawn;
    }

    private void RollForAmmoCrateSpawn(EnemyNavMesh obj)
    {
        float chance = ammoSpawnChance * 100;
        float roll = Random.Range(0, 100);

        if (roll <= chance)
        {
            Instantiate(ammoPickupPrefab, obj.transform.position, obj.transform.rotation);
        }
    }
}
