using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PurificationParticle : MonoBehaviour
{
    [SerializeField] GameObject par0;
    [SerializeField] GameObject par1;
    [SerializeField] Light2D holyLight;


    IEnumerator Start()
    {
        Vector3 holyScale = holyLight.gameObject.transform.localScale;
        holyLight.gameObject.transform.localScale = Vector3.zero;

        holyLight.gameObject.transform.DOScale(holyScale, 0.3f);
        yield return new WaitForSeconds(0.14f);


        par0.SetActive(true);
        par1.SetActive(true);

        float duration = 3f;
        float elapsed = 0f;


        while (elapsed < duration)
        {
            holyLight.intensity = Mathf.Lerp(10f, 15f, Mathf.PingPong(elapsed * 20f, 1f));
            elapsed += Time.deltaTime;
            yield return null;
        }

        holyLight.intensity = 120;
        yield return new WaitForSeconds(1.3f);

        holyScale.x = 0;
        holyLight.gameObject.transform.DOScale(holyScale, 0.3f);
        yield return new WaitForSeconds(0.35f);
        Destroy(gameObject);
    }



    void Update()
    {

    }
}
