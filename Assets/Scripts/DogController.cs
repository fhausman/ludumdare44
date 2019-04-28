using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : MonoBehaviour
{
    public float Speed = 10f;

    private GameObject playerRef;
    private Rigidbody2D rb;
    private bool triggered = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerRef = GameObject.FindGameObjectsWithTag("Player")[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (triggered)
        {
            var previous_postion = transform.position;
            transform.position =
                Vector2.MoveTowards(transform.position, playerRef.transform.position, Speed * Time.deltaTime);

            var ls = transform.localScale;
            ls.x = Mathf.Sign((transform.position - previous_postion).x);
            transform.localScale = ls;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            GetComponent<Animator>().speed = 1f;
            triggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            GetComponent<Animator>().speed = 0.1f;
            triggered = false;
        }
    }
}
