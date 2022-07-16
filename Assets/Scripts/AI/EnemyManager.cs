using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private LayerMask DetectLayer;
    [SerializeField] private List<EnemyNavMesh> EnemiesAttacking = new List<EnemyNavMesh>();
    [SerializeField] private int CountOfEnemiesAttacking = 0;

    [SerializeField] private EnemyWaveData WaveData;
    [SerializeField] private int currentWave;

    [SerializeField] private int TotalEnemies;

    [SerializeField] private float ComboTime;
    private float currentComboTime;
    private int currentCombo, maxCombo;
    private bool CanGetCombo;

    [Header("Wave UI")]
    [SerializeField] private float NextWaveTime;
    private float currentWaveTime;
    [SerializeField] private TextMeshProUGUI NextWaveTimerText, ComboText, MaxComboText, EnemiesLeftText;
    [SerializeField] private GameObject WaveUI_Object;

    public AudioSource MusicSource;



    private void Start()
    {
        currentWaveTime = NextWaveTime;
        EnemyActions.AddEnemyAttacking += AddenemyAttacking;
        EnemyActions.RemoveEnemyAttacking += RemoveenemyAttacking;
        EnemyActions.OnEnemyKilled += OnEnemyKill;

        Invoke("NewWave", 5f);
    }

    private void FixedUpdate()
    {
        if (currentWaveTime <= 0 && StartNewWave == true)
        {
            StartNewWave = false;
            currentWaveTime = NextWaveTime;
            NewWave();
        }
        else if (currentWaveTime > 0 && StartNewWave == true)
        {
            currentWaveTime -= 1 * Time.deltaTime;
            NextWaveTimerText.text = "Next wave in: " + currentWaveTime.ToString("0");
        }

        if (CanGetCombo)
        {
            if (currentComboTime < 0)
            {
                ResetCombo();
            }
            else
                currentComboTime -= 1 * Time.deltaTime;
        }
    }
    private void AddCombo(int score)
    {
        currentCombo++;
        currentComboTime = ComboTime;

        if (currentCombo > maxCombo)
            maxCombo = currentCombo;

        ScoreManager.instance.PublicScore += score * currentCombo;

        MaxComboText.text = "Max Combo: " + maxCombo.ToString("0");

        if(currentCombo >= 2)
            ComboText.transform.gameObject.SetActive(true);

        if (currentCombo == 2)
            ComboText.text = "Double Kill!";
        else if(currentCombo == 3)
            ComboText.text = "Triple Kill!";
        else if (currentCombo == 4)
            ComboText.text = "Multi Kill!";
        else if (currentCombo == 5)
            ComboText.text = "Mega Kill!";
        else if (currentCombo == 6)
            ComboText.text = "Moster Killer!";
        else if (currentCombo >= 7)
            ComboText.text = "MONSTER KILL!!!";


    }
    public void ResetCombo()
    {
        CanGetCombo = false;
        currentCombo = 0;
    }

    private bool WaveStartedDelay, StartNewWave;
    public void NewWave()
    {
        WaveStartedDelay = true;
        StartNewWave = false; 
        WaveUI_Object.SetActive(false);

        if(WaveData.Waves.Length > currentWave)
        currentWave++;

        if (WaveData.Waves[currentWave - 1].ChangeMusic)
        {
            MusicSource.Stop();
            MusicSource.clip = WaveData.Waves[currentWave - 1].MusicClip;

            if(WaveData.Waves[currentWave - 1].MusicStartTime != 0)
                MusicSource.time = WaveData.Waves[currentWave - 1].MusicStartTime;

            MusicSource.Play();

        }
        StartCoroutine(MusicFadeIn());

        StartCoroutine(SpawnBasicEnemy());
        StartCoroutine(SpawnExplosiveEnemy());
        StartCoroutine(SpawnFlyingEnemy());
        StartCoroutine(SpawnGiantEnemy());
        StartCoroutine(SpawnRangedEnemy());

        Invoke("RemoveWaveDelay", 15f);
    }

    IEnumerator MusicFadeOut()
    {
        while (MusicSource.volume > 0)
        {
            MusicSource.volume -= 0.0075f;
            yield return new WaitForSeconds(0.1f);
        }
    }
    IEnumerator MusicFadeIn()
    {
        while (MusicSource.volume < 0.25f)
        {
            MusicSource.volume += 0.0075f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    #region Spawners
    IEnumerator SpawnBasicEnemy()
    {
        yield return new WaitForSeconds(WaveData.Waves[currentWave - 1].TimeToSpawnBasicEnemy);

        for (int i = 0; i < WaveData.Waves[currentWave - 1].BasicEnemy;)
        {
            Vector3 pos = new Vector3(Random.Range(-25, 25), 2, Random.Range(-25, 25));
            Collider[] touching = Physics.OverlapSphere(pos, 1, DetectLayer, QueryTriggerInteraction.Ignore); // chekcs if enemy is spawned inside an object
            if (touching.Length == 0)
            {
                i++;
                Instantiate(WaveData.BasicEnemyPrefab, pos , Quaternion.identity);
                TotalEnemies++;
                yield return new WaitForSeconds(WaveData.Waves[currentWave - 1].TimeBetweenBasicEnemy);
                EnemiesLeftText.text = "Enemies Left: " + TotalEnemies.ToString("0");
            }
        }
    }
    IEnumerator SpawnExplosiveEnemy()
    {
        yield return new WaitForSeconds(WaveData.Waves[currentWave - 1].TimeToSpawnExplosiveEnemy);

        for (int i = 0; i < WaveData.Waves[currentWave - 1].ExplosiveEnemies;)
        {
            Vector3 pos = new Vector3(Random.Range(-25, 25), 2, Random.Range(-25, 25));
            Collider[] touching = Physics.OverlapSphere(pos, 1, DetectLayer, QueryTriggerInteraction.Ignore); // chekcs if enemy is spawned inside an object
            if (touching.Length == 0)
            {
                i++;
                Instantiate(WaveData.ExposiveEnemyPrefab, pos, Quaternion.identity);
                TotalEnemies++;
                yield return new WaitForSeconds(WaveData.Waves[currentWave - 1].TimeBetweenExplosiveEnemy);
                EnemiesLeftText.text = "Enemies Left: " + TotalEnemies.ToString("0");
            }
        }
    }
    IEnumerator SpawnFlyingEnemy()
    {
        yield return new WaitForSeconds(WaveData.Waves[currentWave - 1].TimeToSpawnFlyingEnemy);

        for (int i = 0; i < WaveData.Waves[currentWave - 1].FlyingEnemies;)
        {
            Vector3 pos = new Vector3(Random.Range(-25, 25), 2, Random.Range(-25, 25));
            Collider[] touching = Physics.OverlapSphere(pos, 1, DetectLayer, QueryTriggerInteraction.Ignore); // chekcs if enemy is spawned inside an object
            if (touching.Length == 0)
            {
                i++;
                Instantiate(WaveData.FlyingEnemyPrefab, pos, Quaternion.identity);
                TotalEnemies++;
                yield return new WaitForSeconds(WaveData.Waves[currentWave - 1].TimeBetweenFlyingEnemy);
                EnemiesLeftText.text = "Enemies Left: " + TotalEnemies.ToString("0");
            }
        }
    }
    IEnumerator SpawnGiantEnemy()
    {
        yield return new WaitForSeconds(WaveData.Waves[currentWave - 1].TimeToSpawnGiantEnemy);

        for (int i = 0; i < WaveData.Waves[currentWave - 1].GiantEnemies;)
        {
            Vector3 pos = new Vector3(Random.Range(-25, 25), 2, Random.Range(-25, 25));
            Collider[] touching = Physics.OverlapSphere(pos, 1, DetectLayer, QueryTriggerInteraction.Ignore); // chekcs if enemy is spawned inside an object
            if (touching.Length == 0)
            {
                i++;
                Instantiate(WaveData.GiantEnemyPrefab, pos, Quaternion.identity);
                TotalEnemies++;
                yield return new WaitForSeconds(WaveData.Waves[currentWave - 1].TimeBetweenGiantEnemy);
                EnemiesLeftText.text = "Enemies Left: " + TotalEnemies.ToString("0");
            }
        }
    }
    IEnumerator SpawnRangedEnemy()
    {
        yield return new WaitForSeconds(WaveData.Waves[currentWave - 1].TimeToSpawnRangedEnemy);

        for (int i = 0; i < WaveData.Waves[currentWave - 1].RangedEnemies;)
        {
            Vector3 pos = new Vector3(Random.Range(-25, 25), 2, Random.Range(-25, 25));
            Collider[] touching = Physics.OverlapSphere(pos, 1, DetectLayer, QueryTriggerInteraction.Ignore); // chekcs if enemy is spawned inside an object
            if (touching.Length == 0)
            {
                i++;
                Instantiate(WaveData.RangedEnemyPrefab, pos, Quaternion.identity);
                TotalEnemies++;
                yield return new WaitForSeconds(WaveData.Waves[currentWave - 1].TimeBetweenRangedEnemy);
                EnemiesLeftText.text = "Enemies Left: " + TotalEnemies.ToString("0");
            }
        }
    }
    #endregion

    private void RemoveWaveDelay()
    {
        WaveStartedDelay = false;
        if (TotalEnemies == 0)
        {
            StartNewWave = true;
            WaveUI_Object.SetActive(true);
            StartCoroutine(MusicFadeOut());
        }
    }
    private void OnDisable()
    {
        EnemyActions.AddEnemyAttacking -= AddenemyAttacking;
        EnemyActions.RemoveEnemyAttacking -= RemoveenemyAttacking;
        EnemyActions.OnEnemyKilled -= OnEnemyKill;
    }
    public void AddenemyAttacking(EnemyNavMesh enemy)
    {
        EnemiesAttacking.Add(enemy);
        CountOfEnemiesAttacking = EnemiesAttacking.Count;
    }
    public void RemoveenemyAttacking(EnemyNavMesh enemy)
    {
        EnemiesAttacking.Remove(enemy);
        CountOfEnemiesAttacking = EnemiesAttacking.Count;
    }
    public void OnEnemyKill(EnemyNavMesh enemy)
    {
        if (enemy.Exploded == false)
        {
            CanGetCombo = true;
            AddCombo(enemy.ScoreToAdd);
        }
        TotalEnemies--;
        EnemiesLeftText.text = "Enemies Left: " + TotalEnemies.ToString("0");
        if (TotalEnemies == 0 && !WaveStartedDelay)
            RemoveWaveDelay();
    }
}
