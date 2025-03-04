using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialTime : MonoBehaviour
{
    [SerializeField] private Image uiFill;
    [SerializeField] private TextMeshProUGUI uiText;
    [SerializeField] private float CountTime;
    public Material sky;
    public float timer;
    private void Start()
    {
        timer = CountTime;
    }
    void Update()
    {
        timer -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        if (timer >= 0)
        {
            uiFill.fillAmount = Mathf.InverseLerp(0, CountTime, timer);
            uiText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        }
        else if (timer < 0)
        {
            RenderSettings.skybox = sky;
            SceneManager.LoadScene("PlayScene");
        }
    }
}
