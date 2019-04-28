using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float Speed = 10f;
    public float JumpForce = 1f;
    public float JumpTime = 10f;
    public Transform groundCheck;
    public Cinemachine.CinemachineVirtualCamera VirtualCamera;
    public Sprite SpikesDeathSprite;

    private Rigidbody2D rb;
    private Transform trans;
    private new Collider2D collider;

    private bool facingRight = true;
    private bool jump = false;
    private bool dead = false;
    private float distToGround = 0f;
    private List<GameObject> inventory = new List<GameObject>();
    private List<string> interactiveTags = new List<string>() { "Item", "Interactive", "Throwable" };

    private GameObject interactiveObject;
    private new GameObject particleSystem;
    private Cinemachine.CinemachineBasicMultiChannelPerlin noiseSystem;

    bool IsOnGround { get { return Physics2D.Linecast(trans.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground")); } }

    bool InRangeOfInteractiveObject { get { return interactiveObject != null; } }

    bool HasAnyThrowableObject { get { return inventory.Any(o => o.tag == "Throwable"); } }

    bool IsInteractive(string tag) { return interactiveTags.Contains(tag); }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trans = GetComponent<Transform>();
        collider = GetComponent<Collider2D>();
        particleSystem = trans.GetChild(1).gameObject;
        noiseSystem = VirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();

        distToGround = collider.bounds.extents.y;
    }

    void Update()
    {
        if (dead) return;

        #region Jump
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
        #endregion

        #region Interaction
        if (Input.GetButtonDown("Take") && InRangeOfInteractiveObject)
        {
            if (interactiveObject.tag == "Item"
                || interactiveObject.tag == "Throwable")
            {
                Take();
            }
            else if (interactiveObject.tag == "Interactive")
            {
                Interact();
            }
        }
        #endregion

        #region Throwing
        if (Input.GetButtonDown("Fire1") && HasAnyThrowableObject)
        {
            Throw();
        } 
        #endregion
    }

    void FixedUpdate()
    {
        if (dead) return;

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
        if (IsInteractive(collision.gameObject.tag))
        {
            //Collision is with child trigger so the parent has to be retrieved
            interactiveObject = collision.gameObject.transform.parent.gameObject;
        }
        else if (collision.gameObject.tag == "Spikes")
        {
            OnDead(collision.gameObject.tag);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsInteractive(collision.gameObject.tag))
        {
            interactiveObject = null;
        }
    }

    IEnumerator JumpTimer()
    {
        yield return new WaitForSeconds(JumpTime);
        jump = false;
    }

    void DeathOnSpikes()
    {
        //Disable animator
        var anim = GetComponent<Animator>();
        anim.enabled = false;

        //Replace sprite
        var rend = GetComponent<SpriteRenderer>();
        rend.sprite = SpikesDeathSprite;

        //Snap position to grid
        var position = trans.position;
        Func<float, float> half_between = pos => (Mathf.Floor(pos) + Mathf.Ceil(pos)) / 2.0f;
        position.x = half_between(position.x);
        position.y = half_between(position.y);
        trans.position = position;

        //Sprites will perfectly overlap only on Scale.X = 1
        if (trans.localScale.x != 1)
            Flip();

        //Disable gravity
        rb.bodyType = RigidbodyType2D.Static;
    }

    void OnDead(string dead_place)
    {
        dead = true;
        //cancel horizontal movement
        rb.velocity = new Vector2(0, rb.velocity.y);

        //launch blood
        particleSystem.gameObject.SetActive(true);

        //camera shake
        StartCoroutine("CameraShake");

        //place specific death behaviour
        switch(dead_place)
        {
            case "Spikes":
                DeathOnSpikes();
                break;
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

    IEnumerator CameraShake()
    {
        noiseSystem.m_AmplitudeGain = 20;
        yield return new WaitForSeconds(0.5f);
        noiseSystem.m_AmplitudeGain = 0;
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

    void Throw()
    {
        var throw_force_x = 10f;
        var throw_force_y = 3f;

        var throwable = inventory.First(o => o.tag == "Throwable");
        inventory.Remove(throwable);

        throwable.transform.parent = null;
        throwable.transform.position = trans.position;
        throwable.SetActive(true);

        var trb = throwable.GetComponent<Rigidbody2D>();
        var dir = facingRight ? Vector2.right : Vector2.left;
        trb.velocity = (dir * throw_force_x + Vector2.up * throw_force_y)
                + dir * rb.velocity.x * Mathf.Sign(rb.velocity.x);
        //trb.AddForce(dir * throw_force_x + Vector2.up * throw_force_y);
    }
}
