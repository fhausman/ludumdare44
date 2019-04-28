using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangController : MonoBehaviour
{
    public float RotatationSpeed = 1000f;
    public float Speed = 12f;
    public bool IsDeadly = false;

    private GameObject boomerangRange;
    private GameObject playerRef;
    private GameObject killingCollider;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boomerangRange = GameObject.Find("BoomerangRange");
        killingCollider = GameObject.Find("KillTrigger");
        playerRef = GameObject.FindGameObjectWithTag("Player");
        SetDeadly(false);
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

    void SetDeadly(bool deadly)
    {
        IsDeadly = deadly;
        killingCollider.SetActive(deadly);
    }

    IEnumerator BoomerangThrow()
    {
        rb.constraints = RigidbodyConstraints2D.FreezePositionY;

        var target = boomerangRange.transform.position;
        var player_pos_at_throw = playerRef.transform.position;
        var distance = player_pos_at_throw.x - target.x;

        var dir = distance > 0 ? Vector2.right : Vector2.left;

        Func<float, float, float> percent_passed = (pos, start) => Mathf.Abs((pos - start) / distance)*100;

        while (Flying(target))
        {
            SetDeadly(percent_passed(transform.position.x, player_pos_at_throw.x) > 10f);
            yield return null;
        }
        while(Flying(player_pos_at_throw))
        {
            yield return null;
        }

        SetDeadly(false);
        rb.constraints = RigidbodyConstraints2D.None;
        rb.AddForce(dir * 200f);
    }
}
