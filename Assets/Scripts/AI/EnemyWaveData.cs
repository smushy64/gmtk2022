using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Wave", menuName = "Enemy Wave Data", order = 1)]
public class EnemyWaveData : ScriptableObject
{
    public GameObject BasicEnemyPrefab, RangedEnemyPrefab, ExposiveEnemyPrefab, FlyingEnemyPrefab, GiantEnemyPrefab;

    [Header("Waves")]
    public int CurrentWave;
    public EnemyWave[] Waves;
}


[System.Serializable]
public class EnemyWave
{
    public float TimeToLoot = 30;

    [Header("Amout of Enemies to Spawn")]
    public int BasicEnemy;
    public int RangedEnemies, ExplosiveEnemies, FlyingEnemies, GiantEnemies;

    public int TotalEnemies => BasicEnemy + RangedEnemies + ExplosiveEnemies + FlyingEnemies + GiantEnemies;

    //for example you want to spawn flying enemies 10 seconds after spawning ranged ones.
    [Header("Delay to Spawn Enemy Type")]
    public float TimeToSpawnBasicEnemy;
    public float TimeToSpawnRangedEnemy, TimeToSpawnExplosiveEnemy, TimeToSpawnFlyingEnemy, TimeToSpawnGiantEnemy;

    //Time between spawning enemies, can be 0 if we want to spawn all enemies of enemy type immediatly or can be smth like 1 seconds delayed
    [Header("Delay between Spawning Indivudual Enemy Type")]
    public float TimeBetweenBasicEnemy;
    public float TimeBetweenRangedEnemy, TimeBetweenExplosiveEnemy, TimeBetweenFlyingEnemy, TimeBetweenGiantEnemy;

    [Header("Wave Music")]
    public bool ChangeMusic = false;
    public float MusicStartTime;
    public AudioClip MusicClip;
}

