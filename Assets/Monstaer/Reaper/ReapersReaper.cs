using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class ReapersReaper : MonoBehaviour
{

    public float damage { get; set; }
    public Vector2 startPos { get; set; }
    public Vector2 endPos { get; set; }
    public int playerNum { get; set; }
    public Reaper_1 reaper;
    public float atkTime = 0.3f;
    public float atkTimer { get; set; } = 0f;
    public float rate { get; set; } = 0f;

    [SerializeField] List<Monsters> targets = new List<Monsters>();
    [SerializeField] Transform sprTra;
    [SerializeField] float rotSpd;

    IEnumerator Start()
    {
        transform.DOMove(endPos, 0.6f).SetEase(Ease.InQuad);
        yield return new WaitForSeconds(0.6f);

        yield return new WaitForSeconds(1f);

        transform.DOMove(startPos, 0.6f).SetEase(Ease.InQuart);
        yield return new WaitForSeconds(0.6f);

        yield return null;
        Destroy(gameObject);
        yield break;
    }

    void Update()
    {
        if (atkTimer >= atkTime)
        {
            Attack();
            atkTimer = 0;
        }
        else
        {
            atkTimer += Time.deltaTime * rate;
        }

        sprTra.Rotate(new Vector3(0, 0, 1) * Time.deltaTime * rotSpd);
    }

    void Attack()
    {
        for (int i = targets.Count - 1; i >= 0; i--)
        {
            reaper.Attack(targets[i], damage, false);
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster") || collision.CompareTag("House"))
        {
            Monsters monsters = collision.gameObject.GetComponent<Monsters>();
            if (playerNum != monsters.player)
            {
                targets.Add(monsters);

            }
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster") || collision.CompareTag("House"))
        {
            Monsters monsters = collision.gameObject.GetComponent<Monsters>();
            if (playerNum != monsters.player)
            {
                targets.Remove(monsters);

            }
        }
    }



}
