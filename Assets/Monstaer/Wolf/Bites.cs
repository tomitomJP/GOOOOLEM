using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bites : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] tusk;
    IEnumerator Start()
    {
        float time;
        float timer;

        time = 0.2f;
        timer = 0.0f;
        while (time > timer)
        {
            float t = timer / time;
            for (int i = 0; i < 2; i++)
            {
                tusk[i].color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, t);

            }
            tusk[0].transform.localPosition = Vector3.Lerp(new Vector3(0, 0.5f), new Vector3(0, 0.2f), t);
            tusk[1].transform.localPosition = Vector3.Lerp(new Vector3(0, -0.5f), new Vector3(0, -0.2f), t);

            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        time = 0.1f;
        timer = 0.0f;
        while (time > timer)
        {
            float t = timer / time;
            tusk[0].transform.localPosition = Vector3.Lerp(new Vector3(0, 0.2f), new Vector3(0, 0), t);
            tusk[1].transform.localPosition = Vector3.Lerp(new Vector3(0, -0.2f), new Vector3(0, -0f), t);

            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.05f);


        time = 0.1f;
        timer = 0.0f;
        while (time > timer)
        {
            float t = timer / time;
            transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(1.5f, 0.2f), t);

            timer += Time.deltaTime;
            yield return null;
        }

        time = 0.05f;
        timer = 0.0f;
        while (time > timer)
        {
            float t = timer / time;
            for (int i = 0; i < 2; i++)
            {
                tusk[i].color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, 1 - t);

            }
            transform.localScale = Vector3.Lerp(new Vector3(1.5f, 0.2f), new Vector3(4f, 0f), t);

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }


}
