using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangController : MonoBehaviour
{
    public bool Thrown = false;
    private GameObject boomerangRange;
    private GameObject playerRef;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boomerangRange = GameObject.Find("BoomerangRange");
        playerRef = GameObject.FindGameObjectWithTag("Player");
    }

    public void Throw()
    {
        StartCoroutine("BoomerangThrow");
    }

    IEnumerator BoomerangThrow()
    {
        rb.constraints = RigidbodyConstraints2D.FreezePositionY;

        var target = boomerangRange.transform.position;
        var player_pos_at_throw = playerRef.transform.position;
        while (transform.position != target)
        {
            transform.position =
                Vector2.MoveTowards(
                    transform.position,
                    boomerangRange.transform.position,
                    8f * Time.deltaTime
                );

            yield return null;
        }
        while(transform.position != player_pos_at_throw)
        {
            transform.position =
                Vector2.MoveTowards(
                    transform.position,
                    boomerangRange.transform.position,
                    8f * Time.deltaTime
                );

            yield return null;
        }

        rb.constraints = RigidbodyConstraints2D.None;
    }
}
