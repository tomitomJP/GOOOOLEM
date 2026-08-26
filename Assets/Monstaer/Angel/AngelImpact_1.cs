using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AngelImpact_1 : MonoBehaviour
{
    public Monsters angel;
    [SerializeField] float spd = 6;
    [SerializeField] float lifetime = 6;
    [SerializeField] GameObject exp;

    IEnumerator Start()
    {
        Destroy(gameObject, 30);
        // transform.DOScale(Vector3.one, 0.1f);
        //yield return new WaitForSeconds(0.1f);

        while (true)
        {
            transform.localScale = Vector3.one * Random.Range(0.8f, 1.2f);
            yield return new WaitForSeconds(0.05f);
        }
    }

    void Update()
    {
        transform.Translate(Time.deltaTime * Vector2.right * spd);
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Monster") || collision.gameObject.CompareTag("House"))
        {
            Monsters monsters = collision.gameObject.GetComponent<Monsters>();
            if(angel.player != monsters.player){
                Instantiate(exp, monsters.transform.position, Quaternion.identity);
                angel.Attack(monsters, angel.atk, false);
                Destroy(gameObject);
            }
            
        }

    }
}
