using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float MaxSpeed = 10f;
    public float MoveForce = 365f;
    public float JumpForce = 10f;

    private Rigidbody2D rb;
    private Transform trans;

    private bool facingRight = true;
    private bool jump = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trans = GetComponent<Transform>();
    }

    void Update()
    {
        if(Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
    }

    void FixedUpdate()
    {
        var h = Input.GetAxis("Horizontal");

        rb.AddForce(Vector2.right * h * MoveForce);
        if (Mathf.Abs(rb.velocity.x) > MaxSpeed)
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * MaxSpeed, rb.velocity.y);

        if(ShouldFlip(h))
        {
            Flip();
        }

        if(jump)
        {
            Jump();
        }
    }

    void Jump()
    {
        jump = false;
        rb.AddForce(Vector2.up * JumpForce);
    }

    bool ShouldFlip(float input)
    {
        if (input > 0 && !facingRight)
        {
            return true;
        }
        else if (input < 0 && facingRight)
        {
            return true;
        }

        return false;
    }

    void Flip()
    {
        facingRight = !facingRight;

        var ls = trans.localScale;
        ls.x *= -1;
        trans.localScale = ls;
    }
}
