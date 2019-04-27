using System;
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

    private bool dead = false;
    private bool Dead { get { return dead; } set { dead = value; OnDead(); } }

    private float distToGround;

    private List<GameObject> inventory = new List<GameObject>();
    private GameObject interactiveObject;
    private new GameObject particleSystem;

    bool IsOnGround { get { return Physics2D.Linecast(trans.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground")); } }

    bool InRangeOfInteractiveObject { get { return interactiveObject != null; } }

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
        particleSystem = trans.GetChild(1).gameObject;

        distToGround = collider.bounds.extents.y;
    }

    void Update()
    {
        if (Dead) return;

        if (Input.GetButtonDown("Jump") && IsOnGround)
        {
            StartCoroutine("JumpTimer");
            jump = true;
        }

        if (Input.GetButtonUp("Jump"))
        {
            StopCoroutine("JumpTimer");
            jump = false;
        }

        if (Input.GetButtonDown("Take") && InRangeOfInteractiveObject)
        {
            if (interactiveObject.tag == "Item")
            {
                Take();
            }
            else if (interactiveObject.tag == "Interactive")
            {
                Interact();
            }
        }
    }

    void FixedUpdate()
    {
        if (Dead) return;

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
        if (collision.gameObject.tag == "Item"
            || collision.gameObject.tag == "Interactive")
        {
            interactiveObject = collision.gameObject;
        }
        else if (collision.gameObject.tag == "Spikes")
        {
            Dead = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item"
            || collision.gameObject.tag == "Interactive")
        {
            interactiveObject = null;
        }
    }

    void Take()
    {
        interactiveObject.transform.parent = trans;
        interactiveObject.SetActive(false);
        inventory.Add(interactiveObject);

        interactiveObject = null;
    }

    void Interact()
    {
        var interactive = interactiveObject.GetComponent<InteractiveObject>();
        if(interactive.CanInteract(inventory))
        {
            interactive.Interact();
        }
        else
        {
            Debug.Log("Items missing ;(");
        }
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

    private void OnDead()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        particleSystem.gameObject.SetActive(true);
    }

    void FlipTransform(Transform transf)
    {
        var ls = transf.localScale;
        ls.x *= -1;
        transf.localScale = ls;
    }

    void FlipParticleSystem()
    {
        FlipTransform(particleSystem.transform);
        var rot = particleSystem.transform.localRotation;
        rot.z *= -1;
        particleSystem.transform.localRotation = rot;
    }

    void Flip()
    {
        facingRight = !facingRight;
        FlipTransform(trans);
        FlipParticleSystem();
    }
}
