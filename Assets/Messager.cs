using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Messager : MonoBehaviour
{
    public static Messager instance = null;
    Camera camera;
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject MessageWindow;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public static Coroutine ViewText(string text)
    {
        return instance.ViewTexts(text);
    }

    public Coroutine ViewTexts(string text)
    {
        camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        Transform obj = Instantiate(MessageWindow, canvas.transform).transform;
        obj.GetChild(0).GetComponent<Text>().text = text;
        return StartCoroutine(ViewText(obj));


    }

    IEnumerator ViewText(Transform obj)
    {
        obj.DOScale(new Vector2(1, 1), 0.1f);
        yield return new WaitForSeconds(0.3f);

        obj.GetChild(1).gameObject.SetActive(true);

        while (true)
        {
            if (Input.anyKeyDown) break;
            yield return null;
        }

        obj.DOScale(new Vector2(1, 0), 0.1f);
        yield return new WaitForSeconds(0.1f);

        Destroy(obj.gameObject);

    }

    public static void ViewText(string text, float duration)
    {
        instance.ViewTexts(text, duration);
    }

    public void ViewTexts(string text, float duration)
    {
        camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        Transform obj = Instantiate(MessageWindow, canvas.transform).transform;
        obj.GetChild(0).GetComponent<Text>().text = text;
        StartCoroutine(ViewText(obj, duration));


    }

    IEnumerator ViewText(Transform obj, float duration)
    {
        obj.DOScale(new Vector2(1, 1), 0.1f);
        yield return new WaitForSeconds(0.1f);

        yield return new WaitForSeconds(duration);

        obj.DOScale(new Vector2(1, 0), 0.1f);
        yield return new WaitForSeconds(0.1f);

        Destroy(obj.gameObject);

    }
}
