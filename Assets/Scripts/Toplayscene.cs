using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Toplayscene : MonoBehaviour
{
    private Animator anim;

    private const float gyroYThresholdHigh = 300f;
    private const float gyroYThresholdLow = -300f;
    private const float accYThresholdHigh = 2f;
    private const float accYThresholdLow = -0.5f;
    public string receivedData;

    private int conditionMetCount = 0;
    public TextMeshProUGUI RotateCount;
    public int rcount = 5;
    private bool rcountflag = false;
    private bool rcountstop = true;
    private float rcountTimer = 10f;
    public FadeSceneLoader fadeSceneLoader;

    Nisesasu play;

    void Start()
    {
        fadeSceneLoader.StartFadeIn();
        GameObject sasuKunObject = GameObject.Find("nisesasu");

        if (sasuKunObject != null)
        {
            DontDestroyOnLoad(sasuKunObject);
            play = sasuKunObject.GetComponent<Nisesasu>();
        }
        SetRotate(rcount);
    }

    void Update()
    {
        if (rcountflag)
        {
            rcountTimer -= Time.deltaTime;
            if (rcountTimer <= 0)
            {
                rcount = 5;
                rcountflag = false;
                rcountTimer = 10f;
                SetRotate(rcount);
            }
        }

        if (play != null)
        {
            ProcessForwardSerialData();
        }

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
        {
            conditionMetCount++;
            rcount--;
            SetRotate(rcount);

            if (conditionMetCount == 5)
            {
                fadeSceneLoader.CallCoroutine();
            }
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
        {
            fadeSceneLoader.CallCoroutine();
        }

        if (
            (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            && Input.GetKeyDown(KeyCode.Q)
        )
        {
            SceneManager.LoadScene("StartScene");
        }

        if (
            (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            && Input.GetKeyDown(KeyCode.S)
        )
        {
            fadeSceneLoader.CallCoroutine();
        }
    }

    private void ProcessForwardSerialData()
    {
        try
        {
            receivedData = play.receivedData;
            string[] parts = receivedData.Split(';');

            if (parts.Length == 2)
            {
                ProcessForwardDataParts(parts);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error reading from forward serial port: {e.Message}");
        }
    }

    private void ProcessForwardDataParts(string[] parts)
    {
        string[] accelValues = parts[0].Substring(4).Split(',');
        string[] gyroValues = parts[1].Substring(5).Split(',');

        if (accelValues.Length == 3 && gyroValues.Length == 3)
        {
            float accY = float.Parse(accelValues[1]);
            float gyroY = float.Parse(gyroValues[1]);

            if (
                (gyroY <= gyroYThresholdHigh || gyroY >= gyroYThresholdLow)
                && (accY > accYThresholdHigh || accY < accYThresholdLow)
            )
            {
                HandlePunch();
            }
        }
    }

    private void HandlePunch()
    {
        conditionMetCount++;
        if (conditionMetCount < 5)
        {
            rcountflag = true;
        }
        rcount--;
        if (rcountstop)
        {
            SetRotate(rcount);
        }

        if (conditionMetCount == 5)
        {
            rcountstop = false;
            fadeSceneLoader.CallCoroutine();
        }
    }

    private void SetRotate(int count)
    {
        RotateCount.text = Mathf.Max(0, count).ToString();
    }
}
