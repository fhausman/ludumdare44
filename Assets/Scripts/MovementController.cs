using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float Speed = 10f;
    public float JumpForce = 1f;
    public float JumpTime = 10f;
    public Transform groundCheck;

    private Rigidbody2D rb;
    private Transform trans;
    private new Collider2D collider;

    private bool facingRight = true;
    private bool jump = false;
    private float distToGround;

    private List<GameObject> inventory = new List<GameObject>();
    private GameObject itemNearby;

    bool IsOnGround { get { return Physics2D.Linecast(trans.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground")); } }

    bool CanTake { get { return itemNearby != null; } }

    IEnumerator JumpTimer()
    {
        yield return new WaitForSeconds(JumpTime);
        jump = false;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trans = GetComponent<Transform>();
        collider = GetComponent<Collider2D>();

        distToGround = collider.bounds.extents.y;
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && IsOnGround)
        {
            StartCoroutine("JumpTimer");
            jump = true;
        }

        if(Input.GetButtonUp("Jump"))
        {
            StopCoroutine("JumpTimer");
            jump = false;
        }

        if (Input.GetButtonDown("Take") && CanTake)
        {
            Take();
        }
    }

    void FixedUpdate()
    {
        var h = Input.GetAxis("Horizontal");

        var current_velocity = rb.velocity;
        rb.velocity = new Vector2(h * Speed, rb.velocity.y);

        if (ShouldFlip(h))
        {
            Flip();
        }

        if (jump)
        {
            Jump();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            itemNearby = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            itemNearby = null;
        }
    }

    void Take()
    {
        itemNearby.transform.parent = trans;
        itemNearby.SetActive(false);
        inventory.Add(itemNearby);

        itemNearby = null;
    }

    void Jump()
    {
        var current_velocity = rb.velocity;
        rb.velocity = new Vector2(rb.velocity.x, JumpForce);
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
