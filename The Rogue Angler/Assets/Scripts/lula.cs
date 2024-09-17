using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lula : Enemy
{
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;
    [SerializeField] private float chargeSpeedMultiplier;
    [SerializeField] private float chargeDuration;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask whatIsGround;

    float timer;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Idle);
        rb.gravityScale = 12f;
    }

    private void OnCollisionEnter2D(Collision2D _other) 
    {
        if(_other.gameObject.CompareTag("Enemy"))
        {
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }
    }

   protected override void UpdateEnemyStates()
    {   
        Vector3 _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
        Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;

        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Idle:

                if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround)
                    || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                }

                RaycastHit2D _hit = Physics2D.Raycast(transform.position + _ledgeCheckStart, _wallCheckDir, ledgeCheckX * 10);
                if (_hit.collider != null && _hit.collider.gameObject.CompareTag("Player"))
                {
                    ChangeState(EnemyStates.Surpised);
                }
                
                if (transform.localScale.x > 0)
                {
                    rb.velocity = new Vector2(speed, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(-speed, rb.velocity.y);
                }
                break;
            case EnemyStates.Surpised:
                rb.velocity = new Vector2(0, jumpForce);

                ChangeState(EnemyStates.Attack);
                break;
            case EnemyStates.Attack:
                timer += Time.deltaTime;

                if (timer <= chargeDuration)
                {
                    if (Physics2D.Raycast(transform.position, Vector2.down, ledgeCheckY, whatIsGround))
                    {
                        if (transform.localScale.x > 0)
                        {
                            rb.velocity = new Vector2(speed * chargeSpeedMultiplier, rb.velocity.y);
                        }
                        else
                        {
                            rb.velocity = new Vector2(-speed * chargeSpeedMultiplier, rb.velocity.y);
                        }
                    }
                    else
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                    }
                }
                else
                {
                    timer = 0;
                    ChangeState(EnemyStates.Idle);
                }
                break;
            case EnemyStates.Death:
                Death(Random.Range(5, 10));
                break;
        }
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);

        if (GetCurrentEnemyState == EnemyStates.Idle)
        {
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }

        if (health <= 0)
        {
            ChangeState(EnemyStates.Death);
        }
    }

    protected override void Death(float _destroyTime){
        base.Death(_destroyTime);
    }

    protected override void ChangeCurrentAnimation()
    {
        anim.SetBool("Idle", GetCurrentEnemyState == EnemyStates.Idle);

        anim.SetBool("Attack", GetCurrentEnemyState == EnemyStates.Attack);

        if (GetCurrentEnemyState == EnemyStates.Death)
        {
            anim.SetTrigger("Death");
            int LayerIgnorePlayer = LayerMask.NameToLayer("Ground");
            gameObject.layer = LayerIgnorePlayer;
        }
    }
}
