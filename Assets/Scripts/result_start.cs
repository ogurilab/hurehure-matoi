using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class result_start : MonoBehaviour
{
    private const float gyroYThresholdHigh = 300f;
    private const float gyroYThresholdLow = -300f;
    private const float accYThresholdHigh = 2f;
    private const float accYThresholdLow = -0.5f;

    public GameObject[] objectsToKeepActive;
    public GameObject[] newObjectsToActivate;

    private Nisesasu play;
    private string receivedData;
    private List<GameObject> allObjects;

    private float lastNoDataLogTime;
    public float noDataLogInterval = 1.0f;

    private int conditionMetCount = 0;
    private int conditionMetCount_2 = 0;
    public TextMeshProUGUI RotateCount1;
    public TextMeshProUGUI RotateCount2;

    private bool count1flag;
    private bool count2flag;
    private float rcountTimer = 5f;
    private bool count2stop = true;

    private GameObject sasuKunObject;

    public FadeSceneLoader fadeSceneLoader;

    void Start()
    {
        fadeSceneLoader.StartFadeIn();
        allObjects = new List<GameObject>(GameObject.FindObjectsOfType<GameObject>(true));

        sasuKunObject = GameObject.Find("nisesasu");

        if (sasuKunObject != null)
        {
            play = sasuKunObject.GetComponent<Nisesasu>();
        }
        else
        {
            Debug.Log("nisesasuオブジェクトはまだ存在していません。");
        }

        SetRotate1(20);
        SetRotate2(40);
    }

    void Update()
    {
        if (play != null)
        {
            ProcessForwardSerialData();
        }

        if (count1flag)
        {
            rcountTimer -= Time.deltaTime;
            if (rcountTimer <= 0)
            {
                conditionMetCount = 0;
                conditionMetCount_2 = 0;
                count1flag = false;
                rcountTimer = 5f;
                SetRotate1(20 - conditionMetCount);
                SetRotate2(40 - conditionMetCount_2);
            }
        }
        if (count2flag)
        {
            rcountTimer -= Time.deltaTime;
            if (rcountTimer <= 0)
            {
                conditionMetCount_2 = 20;
                count2flag = false;
                rcountTimer = 5f;
                SetRotate2(40 - conditionMetCount_2);
            }
        }

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
        {
            conditionMetCount++;
            SetRotate1(10 - conditionMetCount);
            SetRotate2(20 - conditionMetCount);

            if (conditionMetCount == 10)
            {
                SwitchActiveObjects();
            }
            if (conditionMetCount >= 20)
            {
                fadeSceneLoader.CallCoroutine();
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SceneManager.LoadScene("StartScene");
            }
        }
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                fadeSceneLoader.CallCoroutine();
            }
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

            HandleObjectDetection(accY, gyroY);
        }
        else
        {
            LogNoData();
        }
    }

    private void HandlePunch()
    {
        conditionMetCount++;
        conditionMetCount_2++;
        SetRotate1(20 - conditionMetCount);
        if (count2stop)
        {
            SetRotate2(40 - conditionMetCount_2);
        }
        if (conditionMetCount >= 1 && conditionMetCount <= 19)
        {
            count1flag = true;
        }
        if (conditionMetCount_2 >= 21 && conditionMetCount_2 <= 39)
        {
            count2flag = true;
        }

        if (conditionMetCount == 20)
        {
            SwitchActiveObjects();
            count1flag = false;
            rcountTimer = 5f;
        }
        if (conditionMetCount_2 >= 40)
        {
            count2stop = false;
            sasuKunObject.SetActive(false);
            fadeSceneLoader.CallCoroutine();
        }
    }

    private void HandleObjectDetection(float accY, float gyroY)
    {
        if (
            (gyroY > gyroYThresholdHigh || gyroY < gyroYThresholdLow)
            && accY <= accYThresholdHigh
            && accY >= accYThresholdLow
        )
        {
            HandlePunch();
        }
    }

    private void LogNoData()
    {
        if (Time.time - lastNoDataLogTime >= noDataLogInterval)
        {
            Debug.Log("データが受信されていません。");
            lastNoDataLogTime = Time.time;
        }
    }

    private bool IsInArray(GameObject[] array, GameObject obj)
    {
        foreach (GameObject element in array)
        {
            if (element == obj)
            {
                return true;
            }
        }
        return false;
    }

    private void SwitchActiveObjects()
    {
        foreach (GameObject obj in allObjects)
        {
            // if (sasuKunObject == null)
            // {
            //     sasuKunObject = GameObject.Find("nisesasu");
            //     if (sasuKunObject != null)
            //     {
            //         DontDestroyOnLoad(sasuKunObject); // nisesasu がシーンを跨いでも破棄されないようにする
            //     }
            // }
            if (
                obj.activeInHierarchy
                && obj != sasuKunObject
                && !IsInArray(objectsToKeepActive, obj)
            )
            {
                obj.SetActive(false);
            }
        }

        foreach (GameObject obj in newObjectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

    private void SetRotate1(int count)
    {
        RotateCount1.text = count.ToString();
    }

    private void SetRotate2(int count)
    {
        RotateCount2.text = count.ToString();
    }
}
