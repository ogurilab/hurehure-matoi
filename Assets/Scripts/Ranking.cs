using UnityEngine;
using UnityEngine.UI;

public class Ranking : MonoBehaviour
{
    [SerializeField, Header("数値")]
    int point;

    string[] ranking =
    {
        "ランキング1位",
        "ランキング2位",
        "ランキング3位",
        "ランキング4位",
        "ランキング5位",
    };
    int[] rankingValue = new int[5];

    [SerializeField, Header("表示させるテキスト")]
    Text[] rankingText = new Text[5];

    [SerializeField, Header("現在のスコアを表示するテキスト")]
    Text currentScoreText;

    void Start()
    {
        int i = date.getScore();
        point = i;

        currentScoreText.text = point.ToString();

        GetRanking();
        SetRanking(point);

        for (int k = 0; k < rankingText.Length; k++)
        {
            rankingText[k].text = rankingValue[k].ToString();
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetRanking();
            }
        }
    }

    void ResetRanking()
    {
        for (int i = 0; i < ranking.Length; i++)
        {
            rankingValue[i] = 0;
            PlayerPrefs.SetInt(ranking[i], 0);
            rankingText[i].text = "0";
        }
        PlayerPrefs.Save();
        Debug.Log("ランキングが初期化されました。");
    }

    void GetRanking()
    {
        for (int i = 0; i < ranking.Length; i++)
        {
            rankingValue[i] = PlayerPrefs.GetInt(ranking[i], 0); // 初期値0を設定
        }
    }

    void SetRanking(int _value)
    {
        int[] tempRanking = new int[rankingValue.Length];
        rankingValue.CopyTo(tempRanking, 0);

        bool added = false;

        for (int i = 0; i < tempRanking.Length; i++)
        {
            if (_value > tempRanking[i])
            {
                for (int j = tempRanking.Length - 1; j > i; j--)
                {
                    tempRanking[j] = tempRanking[j - 1];
                }
                tempRanking[i] = _value;
                added = true;
                break;
            }
        }

        if (added)
        {
            for (int i = 0; i < ranking.Length; i++)
            {
                rankingValue[i] = tempRanking[i];
                PlayerPrefs.SetInt(ranking[i], rankingValue[i]);
            }
            PlayerPrefs.Save();
        }
    }
}
