using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeSpritePingPong : MonoBehaviour

{
    [SerializeField] SpriteRenderer[] spr = new SpriteRenderer[4];
    [SerializeField] bool fragg = false;
    [SerializeField] float time = 1;
    [SerializeField] float timer = 0;

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < 4; i++)
        {
            spr[i].color = Color.Lerp(Color.white, new Color(1, 1, 1, 0.3f), timer / time);

            if (fragg)
            {
                Mathf.Clamp(timer += Time.deltaTime, 0, time);
            }
            else
            {
                Mathf.Clamp(timer -= Time.deltaTime, 0, time);

            }

            if ((timer > time && fragg) || (timer < 0 && !fragg))
            {
                fragg = !fragg;
            }
        }


    }
}
