using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class dateReading : MonoBehaviour
{
    private int i;
    private int c;
    public TextMeshProUGUI Score_1;
    public TextMeshProUGUI Score_2;
    public TextMeshProUGUI textScore2;
    public TextMeshProUGUI textScore3;
    public TextMeshProUGUI Maxcombo_1;
    public TextMeshProUGUI Maxcombo_2;
    public GameObject result1;
    public GameObject result1_1;
    public GameObject result2;
    public GameObject result2_1;
    public GameObject result3;
    public GameObject result3_1;
    public GameObject result4;
    public GameObject result4_1;

    void Start()
    {
        i = date.getScore();
        c = date.GetMaxCombo();
        SetScoreText(i, c);
        result1.SetActive(false);
        result1_1.SetActive(false);
        result2.SetActive(false);
        result2_1.SetActive(false);
        result3.SetActive(false);
        result3_1.SetActive(false);
        result4.SetActive(false);
        result4_1.SetActive(false);
    }

    void Update()
    {
        if (i > 15)
        {
            result1.SetActive(true);
            result1_1.SetActive(true);
        }
        else if (i <= 15 && i > 10)
        {
            result2.SetActive(true);
            result2_1.SetActive(true);
        }
        else if (i <= 10 && i > 5)
        {
            result3.SetActive(true);
            result3_1.SetActive(true);
        }
        else
        {
            result4.SetActive(true);
            result4_1.SetActive(true);
        }
    }

    private void SetScoreText(int score, int combo)
    {
        Score_1.text = score.ToString();
        Score_2.text = score.ToString();
        Maxcombo_1.text = combo.ToString();
        Maxcombo_2.text = combo.ToString();
    }
}
