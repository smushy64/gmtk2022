using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public Action<int> onWaveStarted;
    public Action<int> onWaveEnded;

    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private EnemyWaveData WaveData;
    [SerializeField] private float phaseOneDuration = 30f;
    [SerializeField] private int startingWave = 0;

    [Header("UI")]
    [SerializeField] private GameObject waveUI;
    [SerializeField] private TextMeshProUGUI nextWaveTimerText, enemiesLeftText;
    [SerializeField] private MusicManager music;

    private int currentWave = 0;
    private int currentPhase = 0;
    private int totalEnemiesKilled = 0;
    private float phaseOneTimer = 0f;

    public int EnemiesLeft => WaveData.Waves[currentWave].TotalEnemies - totalEnemiesKilled;

    private void Awake()
    {
        EnemyActions.OnEnemyKilled += OnEnemyKill;
        currentWave = startingWave;

        if (spawnPoints.Count <= 0)
        {
            Debug.LogError("[EnemyManager] No spawn points have been added!");
        }

        BeginPhaseOne();
    }

    private void Update()
    {
        if (currentPhase == 0)
        {
            phaseOneTimer += Time.deltaTime;
            nextWaveTimerText.text = "Next wave in: " + (phaseOneDuration - phaseOneTimer).ToString("0");
            if (phaseOneTimer > phaseOneDuration)
            {
                StartNextWave();
            }
        }
    }

    private void StartNextWave()
    {
        music.CrossfadeToCombat();
        onWaveStarted?.Invoke(currentWave);
        currentPhase = 1;
        waveUI.SetActive(false);

        EnemyWave wave = WaveData.Waves[currentWave];
        StartSpawningEnemies(wave);
    }

    private void StartSpawningEnemies(EnemyWave wave)
    {
        StartCoroutine(SpawnEnemies(
            WaveData.BasicEnemyPrefab,
            wave.BasicEnemy,
            wave.TimeToSpawnBasicEnemy,
            wave.TimeBetweenBasicEnemy));

        StartCoroutine(SpawnEnemies(
            WaveData.ExposiveEnemyPrefab,
            wave.ExplosiveEnemies,
            wave.TimeToSpawnExplosiveEnemy,
            wave.TimeBetweenExplosiveEnemy));

        StartCoroutine(SpawnEnemies(
            WaveData.FlyingEnemyPrefab,
            wave.FlyingEnemies,
            wave.TimeToSpawnFlyingEnemy,
            wave.TimeBetweenFlyingEnemy));

        StartCoroutine(SpawnEnemies(
            WaveData.GiantEnemyPrefab,
            wave.GiantEnemies,
            wave.TimeToSpawnGiantEnemy,
            wave.TimeBetweenGiantEnemy));

        StartCoroutine(SpawnEnemies(
            WaveData.RangedEnemyPrefab,
            wave.RangedEnemies,
            wave.TimeToSpawnRangedEnemy,
            wave.TimeBetweenRangedEnemy));
    }

    private IEnumerator SpawnEnemies(GameObject enemyPrefab, int enemiesToSpawn, float spawnDelay, float spawnRate)
    {
        yield return new WaitForSeconds(spawnDelay);

        for (int i = 0; i < enemiesToSpawn;)
        {
            var point = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Count)];
            Instantiate(enemyPrefab, point.position, Quaternion.identity);
            enemiesLeftText.text = "Enemies Left: " + EnemiesLeft.ToString("0");
            i++;
            yield return new WaitForSeconds(spawnRate);
        }
    }

    private void OnEnemyKill(EnemyNavMesh enemy)
    {
        //if (enemy.Exploded == false)
        //{
        //    CanGetCombo = true;
        //    AddCombo(enemy.ScoreToAdd);
        //}

        totalEnemiesKilled++;
        enemiesLeftText.text = "Enemies Left: " + EnemiesLeft.ToString("0");
        if (EnemiesLeft <= 0)
        {
            EndWave();
        }
    }

    private void EndWave()
    {
        if (WaveData.Waves.Length > currentWave)
            currentWave++;

        BeginPhaseOne();
        onWaveEnded?.Invoke(currentWave);
    }

    private void BeginPhaseOne()
    {
        music.CrossfadeToAmbient();
        waveUI.SetActive(true);
        totalEnemiesKilled = 0;
        currentPhase = 0;
        phaseOneTimer = 0f;
    }
}
