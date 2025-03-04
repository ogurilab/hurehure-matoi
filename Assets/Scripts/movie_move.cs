using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class movie_move : MonoBehaviour
{
    [SerializeField]
    VideoPlayer videoPlayer;

    public FadeSceneLoader fadeSceneLoader;

    void Start()
    {
        fadeSceneLoader.StartFadeIn();
        videoPlayer.loopPointReached += LoopPointReached;
        videoPlayer.Play();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StopVideoPlayer();
                StartCoroutine(LoadSceneAsync("StartScene"));
            }
        }
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                StopVideoPlayer();
                fadeSceneLoader.CallCoroutine();
            }
        }
    }

    public void LoopPointReached(VideoPlayer vp)
    {
        StopVideoPlayer();
        fadeSceneLoader.CallCoroutine();
    }

    void StopVideoPlayer()
    {
        videoPlayer.Stop();
        videoPlayer.targetTexture.Release();
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
