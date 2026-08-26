using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneManager : MonoBehaviour
{
    public static LoadSceneManager instance = null;
    [SerializeField] Image fadeCanvas;
    [SerializeField] Canvas canvas;
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] GameObject LoadingParticle;
    [SerializeField] AudioClip icatchSE;


    void Awake()
    {
        // Singletonとして保持
        if (instance == null)
        {
            fadeCanvas.gameObject.SetActive(true);
            instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいで保持
            StartCoroutine(StartFade());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    static public void FadeLoadScene(string sceneName)
    {
        if (instance != null)
        {
            instance.StartCoroutine(instance.FadeAndLoad(sceneName));
        }
        else
        {
            Debug.LogError("LoadSceneManagerのインスタンスが存在しません！");
        }
    }

    IEnumerator StartFade()
    {
        yield return new WaitForSeconds(0.5f);

        // フェードイン
        yield return Fade(0f);
        fadeCanvas.gameObject.SetActive(false);

    }

    IEnumerator FadeAndLoad(string sceneName, bool OnParticle = true)
    {
        fadeCanvas.gameObject.SetActive(true);
        GameObject par = null;
        GameObject mainCamera = GameObject.FindWithTag("MainCamera");
        canvas.worldCamera = mainCamera.GetComponent<Camera>();

        if (OnParticle)
        {
            Vector2 pos = mainCamera.transform.position;
            par = Instantiate(LoadingParticle, canvas.transform.position, Quaternion.identity, canvas.transform);
            AudioManager.PlaySE(icatchSE);
            yield return new WaitForSeconds(0.5f);
        }

        // フェードアウト
        yield return Fade(1f);

        if (OnParticle) Destroy(par);
        // シーン読み込み
        SceneManager.LoadScene(sceneName);
        yield return new WaitForSeconds(0.5f);

        // フェードイン
        yield return Fade(0f);
        fadeCanvas.gameObject.SetActive(false);

    }

    IEnumerator Fade(float targetAlpha)
    {
        Color startColor = fadeCanvas.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
        float time = 0f;

        while (time < fadeDuration)
        {
            fadeCanvas.color = Color.Lerp(startColor, targetColor, time / fadeDuration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        fadeCanvas.color = targetColor;
    }
}
