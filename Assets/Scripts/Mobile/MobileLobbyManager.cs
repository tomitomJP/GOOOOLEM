using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileLobbyManager : MonoBehaviour
{

    [SerializeField] Transform titleLogo;

    [SerializeField] AudioClip buttonSE;
    [SerializeField] AudioClip titleBgm;
    void Awake()
    {
        Application.targetFrameRate = 60;

    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        StartCoroutine(TitleAnim());

        AudioManager.FadeOutBGM(0.5f);
        yield return new WaitForSeconds(0.5f);
        AudioManager.PlayBGM(titleBgm);

    }

    IEnumerator TitleAnim()
    {
        Vector2 ooo = Vector2.up;
        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                titleLogo.Translate(0.01f * ooo);
                yield return new WaitForSeconds(0.5f);
            }
            ooo *= -1;
            yield return null;
        }
    }

    public void _StartGameSolo()
    {
        StartCoroutine(StartGameSolo());
    }

    IEnumerator StartGameSolo()
    {
        Messager.ViewText("ゲームを開始します", 1);
        yield return new WaitForSeconds(1f);
        AudioManager.FadeOutBGM(0.5f);
        yield return new WaitForSeconds(0.2f);
        LoadSceneManager.FadeLoadScene("MobileGame");
    }
}
