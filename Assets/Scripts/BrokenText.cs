using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrokenText : MonoBehaviour
{
    [SerializeField] Color[] TextColor0;
    [SerializeField] Color[] TextColor1;

    Text text;
    Outline outline;
    public GameManager.Mode mode = GameManager.Mode.play;

    public bool Spwan = false;
    public int num;

    IEnumerator Start()
    {
        text = GetComponent<Text>();
        outline = GetComponent<Outline>();

        if (mode == GameManager.Mode.play)
        {
            text.text = num.ToString();
        }

        if (Spwan)
        {
            StartCoroutine(SpwanMonster());
            text.color = TextColor0[0];

        }
        else
        {
            StartCoroutine(NonSpwanMonster());
            text.color = TextColor1[0];

        }

        float time = 1f;
        float timer = 0;

        while (time >= timer)
        {
            float t = timer / time;
            text.fontSize = (int)Mathf.Lerp(20f, 80f, t);

            if (mode == GameManager.Mode.deathMatch)
            {
                int bonus = Mathf.FloorToInt(num * num / 15f);
                text.text = $"{num}\n<size={text.fontSize - 5}>BONUS {bonus}</size>";

            }
            else if (mode != GameManager.Mode.play)
            {
                break;
            }

            timer += Time.deltaTime;
            yield return null;
        }
        if (mode == GameManager.Mode.deathMatch || mode == GameManager.Mode.play)
        {
            yield return new WaitForSeconds(0.2f);
        }
        Destroy(gameObject);
    }


    IEnumerator NonSpwanMonster()
    {

        int ColorNum = 0;
        while (true)
        {
            outline.effectColor = TextColor1[ColorNum];
            ColorNum = (ColorNum + 1) % 2;
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator SpwanMonster()
    {
        int ColorNum = 0;
        while (true)
        {
            outline.effectColor = TextColor0[ColorNum];
            ColorNum = (ColorNum + 1) % 2;
            yield return new WaitForSeconds(0.05f);
        }
    }


}
