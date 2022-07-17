using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawnManager : MonoBehaviour
{
    public List<WeaponSpawner> spawns = new List<WeaponSpawner>();
    public List<GameObject> loot = new List<GameObject>();

    public int TotalWeaponsToSpawns;

    private void Start()
    {
        WeaponSpawner[] allSpawners = FindObjectsOfType<WeaponSpawner>();
        foreach (WeaponSpawner item in allSpawners)
        {
            spawns.Add(item);
        }
        SpawnRandomLoot();
    }

    public void SpawnRandomLoot()
    {
        List<WeaponSpawner> remainingSpawns = new List<WeaponSpawner>(spawns);
        for (int i = 0; i < TotalWeaponsToSpawns; i++)
        {
            if (remainingSpawns.Count > 0)
            {
                int rand = Random.Range(0, remainingSpawns.Count);
                remainingSpawns[rand].SpawnWeapon();
                remainingSpawns.RemoveAt(rand);
            }
        }

    }
    public void RemoveRandomLoot()
    {
        if (loot.Count > 0)
        {
            for (int i = loot.Count - 1; i >= 0; i--)
            {
                Destroy(loot[i], .5f);
                loot.Remove(loot[i]);
            }
        }
    }
}
