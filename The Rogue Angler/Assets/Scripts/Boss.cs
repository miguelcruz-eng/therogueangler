using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;
    [SerializeField] private float chargeSpeedMultiplier;
    [SerializeField] private float chargeDuration;
    [SerializeField] private float stunDuration;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] GameObject sideFireBall;
    [SerializeField] Transform sideAttackTrasnform;
    [SerializeField] Vector2 sideAttackArea;
    [SerializeField] AudioClip roarSound;

    private bool lookingRight = false;
    private bool isAttacking = false;

    float timer;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Idle);
        rb.gravityScale = 12f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(sideAttackTrasnform.position, sideAttackArea);
        // Gizmos.DrawWireCube(upAttackTrasnform.position, upAttackArea);
        // Gizmos.DrawWireCube(downAttackTrasnform.position, downAttackArea);
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

                Debug.DrawRay(transform.position + _ledgeCheckStart, Vector2.down * ledgeCheckY, Color.red);
                Debug.DrawRay(transform.position, _wallCheckDir * ledgeCheckX, Color.blue);

                if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround)
                    || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                }

                RaycastHit2D _hit = Physics2D.Raycast(transform.position + _ledgeCheckStart, _wallCheckDir, ledgeCheckX * 10);
                Debug.DrawRay(transform.position + _ledgeCheckStart, _wallCheckDir * (ledgeCheckX * 10), Color.green);
                if (_hit.collider != null && _hit.collider.gameObject.CompareTag("Player"))
                {
                    rb.velocity = new Vector2(0, jumpForce);
                    audioSource.PlayOneShot(roarSound);
                    ChangeState(EnemyStates.Surpised);
                }
                
                // if (transform.localScale.x > 0)
                // {
                //     rb.velocity = new Vector2(speed, rb.velocity.y);
                //     lookingRight = true;
                // }
                // else
                // {
                //     rb.velocity = new Vector2(-speed, rb.velocity.y);
                //     lookingRight = false;
                // }
                break;
            case EnemyStates.Surpised:
                rb.velocity = new Vector2(0, jumpForce);

                ChangeState(EnemyStates.Chase);
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
                    ChangeState(EnemyStates.Stunned);
                }
                break;
            case EnemyStates.Stunned:
                timer += Time.deltaTime;
                if (timer > stunDuration)
                {
                    ChangeState(EnemyStates.Surpised);
                    timer = 0;
                    if (lookingRight)
                    {
                        rb.velocity = new Vector2(-speed, rb.velocity.y);
                    }
                    else
                    {
                        rb.velocity = new Vector2(speed, rb.velocity.y);
                    }

                    // Inverte a direção visualmente
                    lookingRight = !lookingRight;
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
                break;
            case EnemyStates.Chase:
                
                if (!isAttacking) // Verifica se o inimigo já está atacando
                {
                    isAttacking = true; // Marca que o inimigo está atacando
                    StartCoroutine(ThrowFireBall());
                }
                                
                break;
            case EnemyStates.Death:
                Death(Random.Range(5, 10));
                break;
        }
    }

    IEnumerator ThrowFireBall()
    {
        // Aguarda o momento certo para instanciar o fireball
        anim.SetTrigger("Shoot");
        yield return new WaitForSeconds(0.60f);

        // Instancia o fireball e remove do Parent
        GameObject _fireBall = Instantiate(sideFireBall, sideAttackTrasnform.position, Quaternion.identity);
        _fireBall.transform.SetParent(null); // Garante que ele não fique preso ao transform de origem

        // Define a direção correta
        if (lookingRight)
        {
            _fireBall.transform.eulerAngles = Vector3.zero;
        } 
        else
        {
            _fireBall.transform.eulerAngles = new Vector3(0, 180, 0); // Gira corretamente em 2D
        }

        yield return new WaitForSeconds(1.5f);
        isAttacking = false;

        // Muda o estado para Idle
        ChangeState(EnemyStates.Attack);
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);

        if (GetCurrentEnemyState == EnemyStates.Idle)
        {
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            rb.velocity = new Vector2(0, jumpForce);
            audioSource.PlayOneShot(roarSound);
            ChangeState(EnemyStates.Surpised);
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

        anim.SetBool("Stunned", GetCurrentEnemyState == EnemyStates.Stunned);

        if (GetCurrentEnemyState == EnemyStates.Death)
        {
            anim.SetTrigger("Death");
            int LayerIgnorePlayer = LayerMask.NameToLayer("Ground");
            gameObject.layer = LayerIgnorePlayer;
        }
    }
}
