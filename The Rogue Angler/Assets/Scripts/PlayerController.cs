using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    [Header("Horizontal movement Settings")]
    [SerializeField] private float walkSpeed = 1;
    
    private float jumpHight = 30;
    private int jumpBufferCounter = 0;
    [SerializeField] private int jumpBufferFrames;

    [Header("Ground check Settings")]
    [SerializeField] private Transform gorundCheckPoint;
    [SerializeField] private float gorundCheckY = 0.2f;
    [SerializeField] private float gorundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;

    PlayerStats pState;
    private Rigidbody2D rb;
    private float xAxis;
    Animator anim;

    public static PlayerController Instance;

    void Awake()
    {
        if(Instance !=null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        pState = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        UpdateJumpingVariables();
        Flip();
        Move();
        Jump();
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
    }

    void Flip()
    {
        if(xAxis < 0)
        {
            transform.localScale = new Vector2(-0.5f, transform.localScale.y);
        }
        else if(xAxis > 0)
        {
            transform.localScale = new Vector2(0.5f, transform.localScale.y);
        }
    }

    private void Move()
    {
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
 
        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
    }

    public bool Grounded()
    {
        if(Physics2D.Raycast(gorundCheckPoint.position, Vector2.down, gorundCheckY, whatIsGround)
            || Physics2D.Raycast(gorundCheckPoint.position + new Vector3(gorundCheckX, 0, 0), Vector2.down, gorundCheckY, whatIsGround)
            || Physics2D.Raycast(gorundCheckPoint.position + new Vector3(-gorundCheckX, 0, 0), Vector2.down, gorundCheckY, whatIsGround))
        {
            return true;
        }else
        {
            return false;
        }
    }

    void Jump()
    {
        if(Input.GetButtonDown("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0);

            pState.jumping = false;
        }

        if(!pState.jumping)
        {
            if(jumpBufferCounter > 0 && Grounded())
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpHight);

                pState.jumping = true;
            }
        }
        

        anim.SetBool("Jumping", !Grounded());
    }

    void UpdateJumpingVariables()
    {
        if(Grounded())
        {
            pState.jumping = false;
        }

        if(Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }else
        {
            jumpBufferCounter--;
        }
    }
}
