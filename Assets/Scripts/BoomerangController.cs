using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangController : MonoBehaviour
{
    public float RotatationSpeed = 1000f;
    public float Speed = 12f;
    private GameObject boomerangRange;
    private GameObject playerRef;
    private Rigidbody2D rb;
    private bool isDeadly;

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

    bool Flying(Vector3 target)
    {
        transform.position =
        Vector2.MoveTowards(
            transform.position,
            target,
            Speed * Time.deltaTime
        );

        transform.Rotate(Vector3.forward, RotatationSpeed * Time.deltaTime);

        return target != transform.position;
    }

    IEnumerator BoomerangThrow()
    {
        isDeadly = true;
        rb.constraints = RigidbodyConstraints2D.FreezePositionY;

        var target = boomerangRange.transform.position;
        var player_pos_at_throw = playerRef.transform.position;
        var dir = (player_pos_at_throw - target).x > 0 ? Vector2.right : Vector2.left;

        while (Flying(target))
        {
            yield return null;
        }
        while(Flying(player_pos_at_throw))
        {
            yield return null;
        }

        isDeadly = false;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.AddForce(dir * 200f);
    }
}
