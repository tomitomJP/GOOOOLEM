using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHilight : MonoBehaviour
{
    [SerializeField] float time = 0.5f;
    [SerializeField] Sprite[] sprite;
    SpriteRenderer spr;
    float timer = 0;
    int count = 0;

    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer >= time)
        {
            count++;
            spr.sprite = sprite[count % sprite.Length];
            timer = 0;

        }
        else
        {
            timer += Time.deltaTime;
        }
    }
}
