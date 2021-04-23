using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    SpriteRenderer sr;
    CapsuleCollider2D capCollider;

    [SerializeField]
    private float walkSpeed = 3f;

    private float xAxis;
    private float yAis;

    private bool isRunning;

    private bool isIdleJump;
    private bool isJumpPressed;
    private float jumpForce = 850;

    //private int groundMask;
    private bool isGrounded;

    private bool isAttackPressed;
    private bool isAttacking;
    private float attackDelay = 2f;

    //Animation States
    private string currentState;

    const string idle = "Idle";
    const string walk = "Walk";
    const string run = "Run";
    const string jump = "Jump";
    const string runJump = "RunJump";
    const string turn = "180Turn";
    const string attack1 = "Attack_1";
    const string attack2 = "Attack_2";
    const string attack3 = "Attack_3";
    const string attack4 = "Attack_4";

    [SerializeField]
    private LayerMask groundMask;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        capCollider = GetComponent<CapsuleCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //groundMask = 1 << LayerMask.NameToLayer("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isIdleJump)
        {
            xAxis = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            xAxis = 0;
        }
        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isJumpPressed = true;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            isAttackPressed = true;
        }

    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(capCollider.bounds.center, Vector2.down, capCollider.bounds.extents.y + 0.1f, groundMask);

        //Color color;

        if(hit.collider != null)
        {
            isGrounded = true;
            //color = Color.green;
        }
        else
        {
            isGrounded = false;
            //color = Color.red;
        }

        //Debug.DrawRay(collider.bounds.center, Vector2.down * (collider.bounds.extents.y + 0.1f), color);

        ///////////////////////////////////////////
        /// Movement based on input
        //////////////////////////////////////////

        Vector2 vel = new Vector2(0, rb.velocity.y);

        if(xAxis < 0)
        {
            if (isAttacking)
            {
                vel.x = -1.5f;
            }
            else
            {
                vel.x = -walkSpeed;
                
            }

            transform.localScale = new Vector2(-1, 1);

        }
        else if(xAxis > 0)
        {

            if (isAttacking)
            {
                vel.x = 1.5f;
            }
            else
            {
                vel.x = walkSpeed;
                
            }

            transform.localScale = new Vector2(1, 1);

        }
        else
        {
            vel.x = 0;
        }

        if (isGrounded && !isAttacking)
        {
            isIdleJump = false;
            if (xAxis != 0)
            {
                isRunning = true;
                changeAnimationState(run);
            }
            else
            {
                isRunning = false;
                changeAnimationState(idle);
            }
        }
        

        /////////////////////////////////////////
        if(isJumpPressed && isGrounded)
        {
            if (isAttacking)
            {
                AttackComplete();
            }

            rb.AddForce(new Vector2(0, jumpForce));
            isJumpPressed = false;

            if (isRunning)
            {
                changeAnimationState(runJump);
            }
            else
            {
                isIdleJump = true;
                changeAnimationState(jump);
            }
        }

        ////////////////////////////////////////
        rb.velocity = vel;

        ////////////////////////////////////////
        ///    Attack
        if (isAttackPressed)
        {
            isAttackPressed = false;

            if (!isAttacking)
            {
                if (isGrounded)
                {
                    isAttacking = true;

                    changeAnimationState(attack2);
                    Invoke("AttackComplete", attackDelay);
                }

                //attackDelay = animator.GetCurrentAnimatorClipInfo(0).Length;
            }
        }


    }

    void AttackComplete()
    {
        isAttacking = false;
    }


    void changeAnimationState(string newState)
    {
        if (currentState == newState)
            return;


        //animator.Play(newState);
        animator.CrossFade(newState, 0.1f);

        currentState = newState;
    }

}
