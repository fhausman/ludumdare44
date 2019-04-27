using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float MaxSpeed = 10f;
    public float MoveForce = 365f;
    public float JumpForce = 1000f;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        var h = Input.GetAxis("Horizontal");

        rb.AddForce(Vector2.right * h * MoveForce);
        if (Mathf.Abs(rb.velocity.x) > MaxSpeed)
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * MaxSpeed, rb.velocity.y);
    }
}
