using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeSceneLoader : MonoBehaviour
{
    public Image fadePanel;
    public float fadeDuration = 0.3f;
    public string sceneName;

    // フェードアウトしてシーンをロード
    public void CallCoroutine()
    {
        StartCoroutine(FadeOutAndLoadScene());
    }

    // フェードアウト処理
    public IEnumerator FadeOutAndLoadScene()
    {
        fadePanel.enabled = true;
        float elapsedTime = 0.0f;
        Color startColor = fadePanel.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1.0f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadePanel.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        fadePanel.color = endColor;
        SceneManager.LoadScene(sceneName);
    }

    // フェードインを呼び出すメソッド
    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    // フェードイン処理
    public IEnumerator FadeIn()
    {
        fadePanel.enabled = true;
        float elapsedTime = 0.0f;
        Color startColor = fadePanel.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0.0f);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadePanel.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        fadePanel.color = endColor;
        fadePanel.enabled = false;
    }
}
