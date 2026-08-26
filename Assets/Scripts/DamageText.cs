using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] float lifetime = 1;
    [SerializeField] Vector2 upPos;
    [SerializeField] float randomX = 0.5f;
    IEnumerator Start()
    {
        float _randomX = Random.Range(-randomX, randomX);
        Vector2 A = transform.position;
        Vector2 B = (Vector2)transform.position + upPos;

        A.x += _randomX;
        B.x += _randomX;

        float _lifetime = lifetime;

        while (lifetime > 0)
        {
            transform.position = Vector2.Lerp(A, B, 1 - (lifetime / _lifetime));
            lifetime -= Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        Destroy(gameObject);
    }
}
