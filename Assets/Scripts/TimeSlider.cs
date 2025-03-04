using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TimeSlider : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI uiText;

    [SerializeField]
    private float CountTime;
    public float timer;
    public CinemachineCamera camera1;
    public CinemachineCamera camera2;
    public CinemachineCamera camera3;
    public CinemachineCamera camera4;
    public GameObject mato_cam;
    public GameObject count_1;
    public GameObject count_2;
    public GameObject count_3;
    public GameObject count_4;
    public GameObject playerblock;
    public GameObject waku;
    private bool timeflag = false;
    public bool stopflag = true;
    private bool musicflag = false;
    public GameObject finish;

    AudioSource audioSource;
    public AudioClip sound1;
    public AudioClip sound2;

    public FadeSceneLoader fadeSceneLoader;

    private void Start()
    {
        fadeSceneLoader.StartFadeIn();
        mato_cam.SetActive(false);
        StartCoroutine(CameraSwitch());
        audioSource = GetComponent<AudioSource>();
        timer = CountTime;
    }

    void Update()
    {
        if (timeflag)
        {
            timer -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(timer / 60);
            int seconds = Mathf.FloorToInt(timer % 60);
            if (timer >= 0)
            {
                uiText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
            }
            if (timer <= 1.0 && musicflag == false)
            {
                StartCoroutine(Finish());
                audioSource.PlayOneShot(sound2);
                musicflag = true;
            }
        }
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartCoroutine(LoadSceneAsync("StartScene"));
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

    private IEnumerator CameraSwitch()
    {
        camera1.Priority = 200;

        yield return new WaitForSeconds(2.0f);

        camera1.Priority = 0;
        camera2.Priority = 200;

        yield return new WaitForSeconds(0.5f);

        camera2.Priority = 0;
        camera3.Priority = 200;

        yield return new WaitForSeconds(2.0f);

        camera3.Priority = 0;
        camera4.Priority = 200;

        yield return new WaitForSeconds(2.0f);
        mato_cam.SetActive(true);
        waku.SetActive(true);
        //３
        count_4.SetActive(true);
        audioSource.PlayOneShot(sound1);
        yield return new WaitForSeconds(1.0f);
        //２
        count_4.SetActive(false);
        count_3.SetActive(true);
        audioSource.PlayOneShot(sound1);
        yield return new WaitForSeconds(1.0f);
        //１
        count_3.SetActive(false);
        count_2.SetActive(true);
        audioSource.PlayOneShot(sound1);
        yield return new WaitForSeconds(1.0f);
        count_2.SetActive(false);
        count_1.SetActive(true);
        audioSource.PlayOneShot(sound2);
        yield return new WaitForSeconds(0.5f);
        count_1.SetActive(false);

        stopflag = false;

        timeflag = true;
    }

    private IEnumerator Finish()
    {
        stopflag = true;
        finish.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        fadeSceneLoader.CallCoroutine();
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
