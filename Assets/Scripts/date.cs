using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class date : MonoBehaviour
{
    public static int score = 0;
    public static int comboCount = 0;
    public static int maxComboCount = 0;
    public float comboResetTime = 5f;
    private float lastPunchTime;
    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textCombo;

    void Start()
    {
        score = 0;
        comboCount = 0;
        maxComboCount = 0;
        SetScoreText(score);
        SetComboText(comboCount);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (Time.time - lastPunchTime > comboResetTime)
        {
            ResetCombo();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "StartScene")
        {
            score = 0;
            comboCount = 0;
            maxComboCount = 0;
            SetScoreText(score);
            SetComboText(comboCount);
        }
    }

    public void AddScore(int point)
    {
        score += point;
        SetScoreText(score);
        IncreaseCombo();
        lastPunchTime = Time.time;
    }

    public void SubtractScore(int point)
    {
        if (score > 0)
        {
            score -= point;
            SetScoreText(score);
        }
        ResetCombo();
    }

    private void SetScoreText(int score)
    {
        textScore.text = score.ToString();
    }

    private void SetComboText(int combo)
    {
        if (textCombo != null)
        {
            textCombo.text = combo.ToString();
        }
    }

    public static int getScore()
    {
        return score;
    }

    public void IncreaseCombo()
    {
        comboCount++;
        if (comboCount > maxComboCount)
        {
            maxComboCount = comboCount;
        }
        if (comboCount % 10 == 0)
        {
            score += comboCount / 10;
            SetScoreText(score);
        }
        SetComboText(comboCount);
    }

    public void ResetCombo()
    {
        comboCount = 0;
        SetComboText(comboCount);
    }

    public static int GetMaxCombo()
    {
        return maxComboCount;
    }
}
