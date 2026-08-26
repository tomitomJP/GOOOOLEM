using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Doragon : Monsters
{
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject fireParticle;
    [SerializeField] Light2D fireLight;
    [SerializeField] AudioClip doragonBreth;



    void Start()
    {
        //StartCoroutine(AtkMotion());
        StartSetup();
    }

    // Update is called once per frame
    void Update()
    {
        Updating();
    }



    public override IEnumerator AtkMotion(Monsters target)//攻撃アニメーションなど
    {
        int num = 0;
        if (atkSprites.Length < 2)
        {
            Debug.LogWarning("攻撃スプライトが足りません");
            yield break;
        }
        mode = Mode.atk;

        spriteRenderer.sprite = atkSprites[num];
        yield return Wait(1.3f);

        num++;
        spriteRenderer.sprite = atkSprites[num];



        Transform fire = Instantiate(fireParticle, firePoint.transform.position, Quaternion.identity, transform).transform;
        fire.transform.localEulerAngles = new Vector3(45, 90, 0);
        StartCoroutine(SwichFireLight(true, 0.2f));

        for (int i = 0; i < 4; i++)
        {
            GetTargets();
            AudioManager.PlaySE(doragonBreth, 0.5f);
            yield return Wait(0.2f);
            if (hp <= 0 || spriteRenderer.flipX) break;
            GetTargets();
            yield return Wait(0.2f);
            if (hp <= 0 || spriteRenderer.flipX) break;
        }

        StartCoroutine(SwichFireLight(false, 0.2f));
        Destroy(fire.gameObject);
        mode = Mode.move;
    }

    void GetTargets()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + rayOrigin, transform.right, enemyDistance * 2f, enemyLayer);
        Debug.DrawRay(transform.position + rayOrigin, transform.right * (enemyDistance * 2f), Color.yellow, 0.3f);

        foreach (RaycastHit2D hit in hits)
        {

            // ここで hit.collider などを使って処理
            Attack(hit.collider.gameObject.GetComponent<Monsters>(), atk / 10);
        }
    }


    IEnumerator SwichFireLight(bool boo, float duration)
    {
        float timer = 0;
        float minValue = 0;
        float macValue = 30;
        while (timer <= duration)
        {
            if (boo)
            {
                fireLight.intensity = Mathf.Lerp(minValue, macValue, timer / duration);

            }
            else
            {
                fireLight.intensity = Mathf.Lerp(minValue, macValue, 1 - timer / duration);

            }
            timer += Time.deltaTime;
            yield return null;
        }
        if (boo)
        {
            fireLight.intensity = macValue;

        }
        else
        {
            fireLight.intensity = minValue;

        }

    }
}
