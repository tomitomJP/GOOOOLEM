using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ResultVeiwer : MonoBehaviour
{
    [SerializeField] Camera camera;
    [SerializeField] GameObject ResultCanvas;

    [System.Serializable]
    public class resultData
    {
        public bool won = false;
        public int spawnCount = 0;
        public int killCount = 0;
        public int deletePeaceCount = 0;

    }

    public resultData[] resultDatas = new resultData[2];

    [System.Serializable]
    public class ResultTexts
    {
        public Text WinTitle;
        public Text spawnCount;
        public Text killCount;
        public Text deletePeaceCount;

    }
    public ResultTexts[] resultTexts = new ResultTexts[2];


    IEnumerator Start()
    {

        for (int i = 0; i < 2; i++)
        {
            if (resultDatas[i].won)
            {
                resultTexts[i].WinTitle.text = "WIN";
            }

        }

        Vector3 targetPos = new Vector3(ResultCanvas.transform.position.x, ResultCanvas.transform.position.y, -10);
        // 1秒かけて target の位置へ加速移動
        camera.transform.DOMove(targetPos, 1f)
                 .SetEase(Ease.InQuad);
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < 2; i++)
        {
            int index = i;

            DOTween.To(
                () => 0,
                x => resultTexts[index].spawnCount.text =
                    $"{x}",
                resultDatas[index].spawnCount,
                2f
            ).SetEase(Ease.OutQuad);
        }


        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 2; i++)
        {
            int index = i;

            DOTween.To(
                () => 0,
                x => resultTexts[index].killCount.text =
                    $"{x}",
                resultDatas[index].killCount,
                2f
            ).SetEase(Ease.OutQuad);
        }


        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 2; i++)
        {
            int index = i;

            DOTween.To(
                () => 0,
                x => resultTexts[index].deletePeaceCount.text =
                    $"{x}",
                resultDatas[index].deletePeaceCount,
                2f
            ).SetEase(Ease.OutQuad);
        }


        yield return new WaitForSeconds(5f);
        Messager.ViewText("ロビーへ戻ります", 1);
        yield return new WaitForSeconds(1.3f);

        if (LobbyManager.MobileCheck()) LoadSceneManager.FadeLoadScene("MoblieLobby");
        else LoadSceneManager.FadeLoadScene("Lobby");

    }


}
