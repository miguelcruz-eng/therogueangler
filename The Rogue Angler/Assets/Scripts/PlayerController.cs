using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    [Header("Horizontal movement Settings")]
    [SerializeField] private float walkSpeed = 1;
    
    [Header("Vertical movement Settings")]
    private float jumpHight = 20;
    private int jumpBufferCounter = 0;
    [SerializeField] private int jumpBufferFrames;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;
    private int airJumpCounter = 0;
    [SerializeField] private int maxAirJumps;

    [Header("Ground check Settings")]
    [SerializeField] private Transform gorundCheckPoint;
    [SerializeField] private float gorundCheckY = 0.2f;
    [SerializeField] private float gorundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCoolDown;
    PlayerStats pState;
    private Rigidbody2D rb;
    private float xAxis, yAxis;
    private float gravity;
    Animator anim;
    private bool canDash = true;
    private bool dashed;
    
    [Header("Attack Settings")]
    bool attack;
    float timeBetwenAttack, timeSinceAttack;
    [SerializeField] Transform sideAttackTrasnform;
    [SerializeField] Vector2 sideAttackArea;
    [SerializeField] LayerMask attackableLayer;
    [SerializeField] float damage;

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
        gravity = rb.gravityScale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(sideAttackTrasnform.position, sideAttackArea);
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        UpdateJumpingVariables();
        if(pState.dashing) return;
        Flip();
        Move();
        Jump();
        startDash();
        Attack();
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetMouseButtonDown(0);
    }

    void Flip()
    {
        if(xAxis < 0)
        {
            transform.localScale = new Vector2(-1f, transform.localScale.y);
        }
        else if(xAxis > 0)
        {
            transform.localScale = new Vector2(1f, transform.localScale.y);
        }
    }

    private void Move()
    {
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
 
        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
    }

    void startDash()
    {
        if(Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }else
        {
            // Debug.Log(canDash);
        }

        if(Grounded())
        {
            dashed = false;
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        anim.SetTrigger("Dashing");
        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;
    }

    void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if (attack && timeSinceAttack >= timeBetwenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("Attacking");

            if(yAxis == 0 || yAxis < 0 && Grounded())
            {
                Hit(sideAttackTrasnform, sideAttackArea);
            }else if(yAxis > 0)
            {
                //Hit(upAttackTrasnform, upAttackArea);
            }else if(yAxis < 0 && !Grounded())
            {
                //Hit(downAttackTrasnform, downAttackArea);
            }
        }
    }

    private void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        if(objectsToHit.Length > 0)
        {
            Debug.Log("Hit");
        }
        for(int i = 0; i < objectsToHit.Length; i++)
        {
            if(objectsToHit[i].GetComponent<EnemyController>() != null)
            {
                objectsToHit[i].GetComponent<EnemyController>().EnemyHit(damage, (transform.position - objectsToHit[i].transform.position).normalized, 100);
            }
        }
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
            if(jumpBufferCounter > 0 && coyoteTime > 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpHight);

                pState.jumping = true;
            }
            else if(!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpHight);

                pState.jumping = true;

                airJumpCounter++;
            }
        }
        

        anim.SetBool("Jumping", !Grounded());
    }

    void UpdateJumpingVariables()
    {
        if(Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else 
        {
            coyoteTimeCounter -= Time.deltaTime;
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
