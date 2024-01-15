using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{   
    [Header("Horizontal movement Settings")]
    private Rigidbody2D rb;
    [SerializeField] private float walkSpeed = 1;
    private float xAxis;

    [Header("Ground check Settings")]
    private float jumpHight = 45;
    [SerializeField] private Transform gorundCheckPoint;
    [SerializeField] private float gorundCheckY = 0.2f;
    [SerializeField] private float gorundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        Move();
        Jump();
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
    }

    private void Move()
    {
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
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
        if(Input.GetButtonDown("Jump") && Grounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpHight);
        }
    }
}
