using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Golem : MonoBehaviour
{
    public int player;
    public int GolemSize;
    [SerializeField] GameObject golemBody;
    List<GameObject> golemBodies = new List<GameObject>();

    public int buffType;
    public float speed = 1f;
    public float atkSpeed = 1f;
    public float atk = 20f;

    [SerializeField] LayerMask enemyMask;
    [SerializeField] LayerMask allyMask;
    [SerializeField] GameObject[] arms;
    GameManager gameManager;
    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Golem" + player.ToString());
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        float Y = 1;
        for (int i = 0; i < GolemSize; i++)
        {
            GameObject G = Instantiate(golemBody, transform.position - new Vector3(0, 0.9f * (i + 1)), Quaternion.identity, transform);
            golemBodies.Add(G);
            G.layer = LayerMask.NameToLayer("Golem" + player.ToString());
            Y += 0.9f;

        }

        if (player == 2)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);

        }

        if (buffType == 0)
        {
            speed *= 1.2f;
        }
        else if (buffType == 1)
        {
            atk *= 1.3f;
        }
        else if (buffType == 2)
        {
            atkSpeed *= 0.7f;
        }
        else if (buffType == 3)
        {
            speed *= 1.1f;
            atk *= 1.1f;
            atkSpeed *= 1.1f;
        }

        transform.localPosition = new Vector3(transform.localPosition.x, Y);

    }

    // Update is called once per frame
    void Update()
    {
        GameObject Enemy = EnemyCheck();
        GameObject Ally = AllyCheck();

        if (Enemy == null && Ally == null)
        {
            Move();
            GolemAnim();
        }
        else
        {
            if (!Attackking && Enemy != null & Ally == null)
            {
                Attackking = true;
                StartCoroutine(Attack());
            }
            if (!Attackking)
            {
                GolemAnim();
            }
        }
    }

    void Move()
    {
        transform.Translate(Vector3.right * Time.deltaTime * speed);

    }

    int AttackHund = 0;
    public bool Attackking = false;
    IEnumerator Attack()
    {
        GameObject hund = arms[AttackHund];

        Vector3 hundPosition = hund.transform.localPosition;

        hund.transform.localEulerAngles = new Vector3(0, 0, 60);

        float angleZ = hund.transform.localEulerAngles.z + 90;
        Vector3 direction = new Vector2(Mathf.Cos(angleZ * Mathf.Deg2Rad), Mathf.Sin(angleZ * Mathf.Deg2Rad)).normalized;


        float timer = 0;
        float time = 0.3f;
        Vector3 A = hund.transform.localPosition;
        Vector3 B = Vector2.zero;


        timer = 0;
        time = 0.5f * atkSpeed;
        A = hund.transform.localPosition;
        B = direction * 1.3f;
        while (timer <= time)
        {
            float t = timer / time;
            hund.transform.localPosition = Vector3.Lerp(A, B, t);

            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0;
        time = 0.2f * atkSpeed;
        A = hund.transform.localPosition;
        B = -direction * 0.2f;
        while (timer <= time)
        {
            float t = timer / time;
            hund.transform.localPosition = Vector3.Lerp(A, B, t);

            timer += Time.deltaTime;
            yield return null;
        }

        hund.transform.localPosition = B;

        GameObject enemy = EnemyCheck();

        if (enemy != null && enemy != gameObject)
        {
            Golem golem = null;
            if (enemy.CompareTag("House"))
            {
                //gameManager.HouseAtk(player, atk);
            }
            else if (enemy.GetComponent<Golem>() != null)
            {
                golem = enemy.GetComponent<Golem>();
            }
            else
            {
                golem = enemy.transform.parent.GetComponent<Golem>();
            }

            if (golem != null && golem.player != player)
            {
                golem.Damaged();
            }
        }

        yield return new WaitForSeconds(0.2f * atkSpeed);

        timer = 0;
        time = 0.3f * atkSpeed;
        A = hund.transform.localPosition;
        B = hundPosition;
        while (timer <= time)
        {
            float t = timer / time;
            hund.transform.localPosition = Vector3.Lerp(A, B, t);
            hund.transform.localEulerAngles = Vector3.Lerp(new Vector3(0, 0, 30), new Vector3(0, 0, 0), t);

            timer += Time.deltaTime;
            yield return null;
        }

        AttackHund = (AttackHund + 1) % 2;
        Attackking = false;
    }


    GameObject EnemyCheck()
    {
        GameObject golemBodyLast = null;
        if (golemBodies.Count > 0)
        {
            golemBodyLast = golemBodies[golemBodies.Count - 1];
        }
        else
        {
            return null;
        }


        Vector2 dir;

        if (player == 1)
        {
            dir = Vector2.right;
        }
        else
        {
            dir = Vector2.left;
        }

        Vector3 startPos = golemBodyLast.transform.position + (Vector3)dir * 0.6f;
        Vector3 endPos = startPos + (Vector3)dir * 1.5f;

        Debug.DrawLine(startPos, endPos, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(startPos, dir, 1.5f, enemyMask);

        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }

    GameObject AllyCheck()
    {
        GameObject golemBodyLast = null;
        if (golemBodies.Count > 0)
        {
            golemBodyLast = golemBodies[golemBodies.Count - 1];
        }
        else
        {
            return null;
        }


        Vector2 dir;

        if (player == 1)
        {
            dir = Vector2.right;
        }
        else
        {
            dir = Vector2.left;
        }

        Vector3 startPos = golemBodyLast.transform.position + (Vector3)dir * 0.6f;
        Vector3 endPos = startPos + (Vector3)dir * 0.3f;

        Debug.DrawLine(startPos, endPos, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(startPos, dir, 0.3f, allyMask);

        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        else
        {
            return null;
        }
    }



    public void Damaged()
    {
        Destroy(golemBodies[golemBodies.Count - 1]);
        golemBodies.RemoveAt(golemBodies.Count - 1);
        GolemSize--;
        if (GolemSize == 0)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.localPosition -= new Vector3(0, 0.9f);
        }
    }


    bool walkAnimfragg = true;
    float walkAnimTime = 1;
    float walkAnimTimer = 0;
    void GolemAnim()
    {
        if (GolemSize > 0)
        {
            if (walkAnimfragg)
            {
                walkAnimTimer = Mathf.Clamp(walkAnimTimer += Time.deltaTime, 0, walkAnimTime);
                if (walkAnimTime <= walkAnimTimer)
                {
                    walkAnimfragg = false;
                }
            }
            else
            {
                walkAnimTimer = Mathf.Clamp(walkAnimTimer -= Time.deltaTime, 0, walkAnimTime);
                if (0 >= walkAnimTimer)
                {
                    walkAnimfragg = true;
                }

            }

            float t = walkAnimTimer / walkAnimTime;
            for (int i = 0; i < GolemSize; i++)
            {

                Vector3 A = new Vector2(0.1f, golemBodies[i].transform.localPosition.y);
                Vector3 B = new Vector2(-0.1f, golemBodies[i].transform.localPosition.y);

                if (i % 2 == 0)
                {
                    golemBodies[i].transform.localPosition = Vector3.Lerp(A, B, t);
                }
                else
                {
                    golemBodies[i].transform.localPosition = Vector3.Lerp(B, A, t);
                }

            }

            Vector3 C = new Vector3(0, 0, 80);
            Vector3 D = new Vector3(0, 0, -80);

            arms[0].transform.localEulerAngles = Vector3.Lerp(C, D, t);
            arms[1].transform.localEulerAngles = Vector3.Lerp(C, D, 1 - t);

        }

    }
}
