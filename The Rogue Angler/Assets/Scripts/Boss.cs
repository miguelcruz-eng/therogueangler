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
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip bashSound;
    [SerializeField] AudioClip poundSound;

    [SerializeField] GameObject music;
    [SerializeField] GameObject stun;
    private GameObject _stun;

    private bool lookingRight = true;
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
        // Gizmos.color = Color.red;
        // Gizmos.DrawWireCube(sideAttackTrasnform.position, sideAttackArea);
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

                Debug.DrawRay(transform.position + _ledgeCheckStart, Vector2.down * ledgeCheckY, Color.green);
                Debug.DrawRay(transform.position, _wallCheckDir * ledgeCheckX, Color.blue);

                if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround)
                    || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                }

                RaycastHit2D _hit = Physics2D.Raycast(transform.position + _ledgeCheckStart, _wallCheckDir, ledgeCheckX * 10);
                Debug.DrawRay(transform.position + _ledgeCheckStart, _wallCheckDir * (ledgeCheckX * 10), Color.green);
                if (_hit.collider != null && _hit.collider.gameObject.CompareTag("Player") && gameObject.layer != LayerMask.NameToLayer("BackGround2"))
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
                    
                    // Verifica se colidiu com uma parede
                    Vector2 wallCheckDirection = Vector2.right * Mathf.Sign(transform.localScale.x);
                    RaycastHit2D wallHit = Physics2D.Raycast(transform.position, wallCheckDirection, ledgeCheckX, whatIsGround);
                    Debug.DrawRay(transform.position, wallCheckDirection * ledgeCheckX, Color.blue);
                    
                    if (wallHit.collider != null)
                    {
                        timer = 0;
                        audioSource.PlayOneShot(bashSound);
                        ChangeState(EnemyStates.Stunned);
                        _stun = Instantiate(stun, sideAttackTrasnform.position, Quaternion.identity);
                        if (lookingRight)
                        {
                            _stun.transform.eulerAngles = new Vector3(0, 180, 0); // Gira corretamente em 2D
                        } 
                        else
                        {
                            _stun.transform.eulerAngles = Vector3.zero;
                        }
                    }
                }
                else
                {
                    timer = 0;
                    // Obtém a posição do jogador e ajusta a direção do inimigo
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null)
                    {
                        float directionToPlayer = player.transform.position.x - transform.position.x;
                        if ((directionToPlayer > 0 && transform.localScale.x < 0) || (directionToPlayer < 0 && transform.localScale.x > 0))
                        {
                            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                            lookingRight = !lookingRight;
                        }
                    }
                    // Verifica a altura do jogador em relação ao inimigo
                    if (player.transform.position.y > transform.position.y)
                    {
                        Debug.Log("entrando no pulo");
                        // O jogador está acima do inimigo
                        ChangeState(EnemyStates.Flip);
                    }
                    else ChangeState(EnemyStates.Chase);
                }
                break;
            case EnemyStates.Stunned:
                timer += Time.deltaTime;
                if (timer > stunDuration)
                {
                    Destroy(_stun);
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
            case EnemyStates.Flip:
                // Limita o número de pulos para 3
                int maxJumps = 3;
                int jumpCount = 0;
                while (jumpCount < maxJumps)
                {
                    rb.velocity = new Vector2(0, jumpForce);
                    audioSource.PlayOneShot(poundSound);
                    // Verifica se o jogador está no chão
                    if (PlayerController.Instance.Grounded())
                    {
                        // Aplica dano ao jogador
                        PlayerController.Instance.TakeDamage(5);
                        break;  // Sai do loop de pulos
                    }

                    jumpCount++; // Incrementa o contador de pulos
                }

                // Se o inimigo terminou os pulos, volta ao estado de Chase
                ChangeState(EnemyStates.Chase);
                break;
            case EnemyStates.Death:
                Death(Random.Range(4, 5));
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
        audioSource.PlayOneShot(roarSound);
        StartCoroutine(SpawnBloodRoutine(_destroyTime));
        base.Death(_destroyTime);
    }

    private IEnumerator SpawnBloodRoutine(float duration)
    {
        yield return new WaitForSeconds(duration * Random.Range(0.5f, 1f));

        // Apenas 25% de chance de criar o sangue
        if (Random.value <= 0.25f) 
        {
            Vector2 randomOffset = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
            GameObject _orangeBlood = Instantiate(orangeBlood, (Vector2)transform.position + randomOffset, Quaternion.identity);
            Destroy(_orangeBlood, 5.5f);
        }
    }

    protected override void ChangeCurrentAnimation()
    {
        anim.SetBool("Idle", GetCurrentEnemyState == EnemyStates.Idle);

        anim.SetBool("Attack", GetCurrentEnemyState == EnemyStates.Attack);

        anim.SetBool("Stunned", GetCurrentEnemyState == EnemyStates.Stunned);

        if (GetCurrentEnemyState == EnemyStates.Death)
        {
            anim.SetTrigger("Death");
            int LayerIgnorePlayer = LayerMask.NameToLayer("BobLayer");
            gameObject.layer = LayerIgnorePlayer;

            // 🚀 Para a música e troca para victoryTheme após 5s
            BackGroundMusic musicScript = music.GetComponent<BackGroundMusic>();
            if (musicScript != null)
            {
                StartCoroutine(ChangeMusic(musicScript));
            }
        }
    }

    IEnumerator ChangeMusic(BackGroundMusic musicScript)
    {
        musicScript.StopMusic(); // Para a música
        yield return new WaitForSeconds(3.5f); // Espera segundos
        musicScript.Victory(); // Troca a música
    }
}
