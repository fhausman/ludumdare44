using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using UnityEngine;

public class TrampolineController : MonoBehaviour
{
    public float jumpForce = 10000f;

    private bool dont_affect = false;
    IEnumerator Delay()
    {
        dont_affect = true;
        yield return new WaitForSeconds(0.05f);
        dont_affect = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!dont_affect)
        {
            var rb = collision.
                gameObject.
                GetComponent<Rigidbody2D>();

            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce);

            StartCoroutine("Delay");
        }
    }
}
