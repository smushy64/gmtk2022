using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private int Score;
    [SerializeField] private float ScoreTime;
    public float curTime;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        WaveManager.onWaveStarted = WaveStarted;
        FindObjectOfType<WaveManager>().onWaveEnded = WaveEnded;
    }
    float extraScore;

    public int BetterChance = 0;

    private WeaponSpawnManager weaponSpawner;

    //I will just speedrun this cuz we dont have much time so dont change anything might not work
    private int TotalEnemiesKilled, chanceincrease;

    private void Start()
    {
        weaponSpawner = FindObjectOfType<WeaponSpawnManager>();
    }

    void WaveStarted(int x)
    {
        curTime = ScoreTime;
        BetterChance = 0;
        chanceincrease = 0;
        TotalEnemiesKilled = 0;
    }

    void WaveEnded(int x)
    {
        BetterChance = chanceincrease / TotalEnemiesKilled;
        weaponSpawner.SpawnRandomLoot();
        TotalEnemiesKilled = 0;
        print(BetterChance);
    }
    public int PublicScore
    {
        set
        {
            //Score = value;
            ScoreText.text = "Score: " + Score.ToString("0");
        }
        get
        {
            return Score;
        }
    }
    public void AddScore(int score)
    {
        extraScore = 0;
        if (curTime > 50)
        {
            extraScore = score * 4;
            chanceincrease += 50;
        }
        else if (curTime > 40)
        {
            extraScore = score * 3;
            chanceincrease += 40;
        }
        else if (curTime > 25)
        {
            extraScore = score * 2;
            chanceincrease += 30;
        }
        else if (curTime > 15)
        {
            extraScore = score * 1.5f;
            chanceincrease += 20;
        }
        else if (curTime > 1)
        {
            extraScore = score * 1.25f;
            chanceincrease += 10;
        }

        TotalEnemiesKilled++;

        if (extraScore < 0)
            extraScore = 0;

        print(score);
        print(extraScore);
        print(Score);

        this.Score += score + Mathf.FloorToInt(extraScore);
        ScoreText.text = "Score: " + this.Score.ToString("0");
    }
    private void FixedUpdate()
    {
        if (curTime > 0)
            curTime -= 1 * Time.deltaTime;
    }
}
