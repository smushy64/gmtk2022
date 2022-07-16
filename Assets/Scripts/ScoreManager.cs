using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private int Score;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public int PublicScore
    {
        set
        {
            Score = value;
            ScoreText.text = "Score: " + Score.ToString("0");
        }
        get
        {
            return Score;
        }
    }

}
