using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{   
    [Header("Horizontal movement Settings")]
    [SerializeField] private float walkSpeed = 2;
    [Space(5)]
    
    [Header("Vertical movement Settings")]
    private float jumpHight = 20;
    private int jumpBufferCounter = 0;
    [SerializeField] private int jumpBufferFrames;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;
    private int airJumpCounter = 0;
    [SerializeField] private int maxAirJumps;
    [Space(5)]

    [Header("Ground check Settings")]
    [SerializeField] private Transform gorundCheckPoint;
    [SerializeField] private float gorundCheckY = 0.2f;
    [SerializeField] private float gorundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;
    [Space(5)]

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCoolDown;
    private float gravity;
    private bool canDash = true;
    private bool dashed;
    [SerializeField] GameObject dash;
    [Space(5)]
    
    [Header("Attack Settings")]
    private bool attack = false;
    [SerializeField] Transform sideAttackTrasnform, upAttackTrasnform, downAttackTrasnform;
    [SerializeField] Vector2 sideAttackArea, upAttackArea, downAttackArea;
    [SerializeField] LayerMask attackableLayer;
    [SerializeField] private float timeBetwenAttack;
    private float timeSinceAttack;
    [SerializeField] float damage;
    [SerializeField] GameObject slash;
    bool restoreTime;
    float restoreTimeSpeed;
    [Space(5)]

    [Header("Recoil Settings")]
    [SerializeField] int recoilXSteps = 5;
    [SerializeField] int recoilYSteps = 5;
    [SerializeField] float recoilXSpeed = 50;
    [SerializeField] float recoilYSpeed = 30;
    private int stepsXRecoiled, stepsYRecoiled;
    [Space(5)]

    [Header("Health Settings")]
    public int health;
    public int maxHealth;
    [SerializeField] GameObject bloodSpurt;
    [SerializeField] GameObject healPotion;
    [SerializeField] GameObject lifeBar;
    GameObject _healingPotion = null;
    [SerializeField] float hitFlashSpeed;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallBack;

    float healTimer;
    [SerializeField] float timeToHeal;
    [Space(5)]

    [Header("Energy Settings")]
    [SerializeField] float energy;
    [SerializeField] float energyDrainSpeed;
    [SerializeField] float energyGain;
    [SerializeField] GameObject energyStorage;
    [Space(5)]

    [Header("Camera Settings")]
    [SerializeField] private float playerFallSpeedTheshold = -10;

    [HideInInspector] public PlayerStats pState;
    [HideInInspector] public Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    //Input Variables
    private float xAxis, yAxis;
    public bool canFlash = true;
    
    public static PlayerController Instance;

    private void Awake()
    {
        if (Instance !=null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        pState = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        gravity = rb.gravityScale;

        Energy = energy;
        energyStorage.GetComponent<Image>().fillAmount = Energy;

        Health = maxHealth;
        lifeBar.GetComponent<Image>().fillAmount = Health / maxHealth;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(sideAttackTrasnform.position, sideAttackArea);
        Gizmos.DrawWireCube(upAttackTrasnform.position, upAttackArea);
        Gizmos.DrawWireCube(downAttackTrasnform.position, downAttackArea);
    }

    // Update is called once per frame
    void Update()
    {
        if (pState.cutscene) return;
        if(pState.alive)
        {
            GetInputs();
        }
        UpdateJumpingVariables();
        // UpdateCameraYDampingForPlayerFall();
        RestoreTimeScale();
        Heal();

        if (pState.dashing || pState.healing) return;
        Flip();
        Move();
        Jump();
        startDash();
        Attack();
        FlashWhileInvinclible();
    }

    private void FixedUpdate()
    {
        if (pState.cutscene) return;

        if (pState.dashing) return;
        Recoil();
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");
    }

    void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-1f, transform.localScale.y);
            pState.lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(1f, transform.localScale.y);
            pState.lookingRight = true;
        }
    }

    private void Move()
    {
        if (pState.healing)
        {
            rb.velocity = new Vector2(0, 0);
            anim.SetBool("Walking", false);
            return;
        } 
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
    }

    //camera nao usada ainda
    void UpdateCameraYDampingForPlayerFall()
    {
        if (rb.velocity.y < playerFallSpeedTheshold && !CameraManager.Instance.isLerpingYDamp && !CameraManager.Instance.hasLerpingYDamp)
        {
            StartCoroutine(CameraManager.Instance.LerpYDamping(true));
        }

        if (rb.velocity.y >= 0 && !CameraManager.Instance.isLerpingYDamp && CameraManager.Instance.hasLerpingYDamp)
        {
            CameraManager.Instance.hasLerpingYDamp = false;
            StartCoroutine(CameraManager.Instance.LerpYDamping(false));
        }
    }

    void startDash()
    {
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }else
        {
            // Debug.Log(canDash);
        }

        if (Grounded())
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
        int _dir = pState.lookingRight ? 1 : -1;
        rb.velocity = new Vector2(_dir * dashSpeed, 0);
        if (Grounded()) Instantiate(dash, transform);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;
    }

    public IEnumerator walkIntoNewScene(Vector2 _exitDir, float _delay)
    {
        pState.invincible = true;
        // se a direção de saida for para cima
        if (_exitDir.y > 0)
        {
            rb.velocity = jumpHight * _exitDir;
        }
        // se a direção de saida requerer um movimento horizontal
        if (_exitDir.x != 0)
        {
            xAxis = _exitDir.x > 0 ? 1 : -1;

            Move();
        }

        Flip();
        yield return new WaitForSeconds(_delay);
        pState.invincible = false;
        pState.cutscene = false;
    }

    void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if (attack && timeSinceAttack >= timeBetwenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("Attacking");

            if (yAxis == 0 || yAxis < 0 && Grounded())
            {
                int _recoilLeftOrRight = pState.lookingRight ? 1 : -1;

                Hit(sideAttackTrasnform, sideAttackArea, ref pState.recoilingX, Vector2.right * _recoilLeftOrRight, recoilXSpeed);
                Instantiate(slash, sideAttackTrasnform);
            }else if (yAxis > 0)
            {
                Hit(upAttackTrasnform, upAttackArea, ref pState.recoilingY, Vector2.up, recoilYSpeed);
                SlashAngle(slash, 80, upAttackTrasnform);
            }else if (yAxis < 0 && !Grounded())
            {
                Hit(downAttackTrasnform, downAttackArea, ref pState.recoilingY, Vector2.down, recoilYSpeed);
                SlashAngle(slash, -80, downAttackTrasnform);
            }
        }
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool, Vector2 _recoilDir, float _recoilStrenght)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);
        List<Enemy> hitEnemies = new List<Enemy>();

        if (objectsToHit.Length > 0)
        {
            _recoilBool = true;
        }
        for(int i = 0; i < objectsToHit.Length; i++)
        {  
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyHit(damage, _recoilDir, _recoilStrenght);
            }
        }
    }

    void SlashAngle(GameObject _slash, int _effectAngle, Transform _attackTransform)
    {
        _slash = Instantiate(_slash, _attackTransform);
        _slash.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slash.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    void Recoil()
    {
        if (pState.recoilingX)
        {
            if (pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            }else 
            {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }
        }

         if (pState.recoilingY)
        {
            rb.gravityScale = 0;
            if (yAxis < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
            }else 
            {
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
            }
            airJumpCounter = 0;
        }else 
        {
            rb.gravityScale = gravity;
        }

        //stop recoil
        if (pState.recoilingX && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++;
        }else
        {
            StopRecoilX();
        }

        if (pState.recoilingY && stepsYRecoiled < recoilYSteps)
        {
            stepsYRecoiled++;
        }else
        {
            StopRecoilY();
        }

        if (Grounded())
        {
            StopRecoilY();
        }
    }

    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }

    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }

    public void TakeDamage(float _damage)
    {
        if (pState.alive)
        {
            Health -= Mathf.RoundToInt(_damage);
            if (Health <= 0)
            {
                Health = 0;
                StartCoroutine(Death());
            }
            else
            {
                StartCoroutine(StopTakingDamge());
            }
        }
        
    }

    IEnumerator StopTakingDamge()
    {
        pState.invincible = true;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        anim.SetTrigger("TakeDamage");
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }

    IEnumerator Flash()
    {
        sr.enabled = !sr.enabled;
        canFlash = false;
        yield return new WaitForSeconds(0.2f);
        canFlash = true;
    }

    void FlashWhileInvinclible()
    {
        if(pState.invincible && !pState.cutscene)
        {
            if(Time.timeScale > 0.2 && canFlash)
            {
                StartCoroutine(Flash());
            }
        }
        else
        {
            sr.enabled = true;
        }
        // sr.material.color = pState.invincible ? 
        //     Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f)) : 
        //     Color.white;
    }

    void RestoreTimeScale()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.unscaledDeltaTime * restoreTimeSpeed;
            }
            else
            {
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }

    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _dealay)
    {
        restoreTimeSpeed = _restoreSpeed;
        Time.timeScale = _newTimeScale;
        if (_dealay > 0)
        {
            StopCoroutine(StartTimeAgain(_dealay));
            StartCoroutine(StartTimeAgain(_dealay));
        }
        else
        {
            restoreTime = true;
        }
    }

    IEnumerator StartTimeAgain(float _dealay)
    {
        yield return new WaitForSecondsRealtime(_dealay);
        restoreTime = true;
    }

    IEnumerator Death()
    {
        pState.alive = false;
        Time.timeScale = 1f;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        anim.SetTrigger("Death");

        yield return new WaitForSeconds(0.9f);
        StartCoroutine(UIManager.Instance.ActivateDeathScreen());
    }

    public void Respawned()
    {
        if(!pState.alive)
        {
            pState.alive = true;
            Health = maxHealth;
            anim.Play("Idle");
        }
    }

    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);
                lifeBar.GetComponent<Image>().fillAmount = (float)health / maxHealth;

                if (onHealthChangedCallBack != null)
                {
                    onHealthChangedCallBack.Invoke();
                }
            }
        }
    }
    void Heal()
    {
        if (Input.GetButton("Healing") && Health < maxHealth && Energy > 0 && Grounded() && !pState.dashing)
        {
            pState.healing = true;

            if (_healingPotion == null) // Instancie a poção de cura apenas se ainda não foi instanciada
            {
                _healingPotion = Instantiate(healPotion, transform);
            }

            // Processo de cura
            healTimer += Time.deltaTime;
            if (healTimer > timeToHeal)
            {
                Health++;
                healTimer = 0;
            }

            // drena energia
            Energy -= Time.deltaTime * energyDrainSpeed;
        }
        else
        {
            if (_healingPotion != null) // Destrua a poção de cura apenas se ela foi instanciada
            {
                Destroy(_healingPotion);
                _healingPotion = null; // Certifique-se de redefinir para null
            }

            pState.healing = false;
            healTimer = 0;
        }
    }

    float Energy
    {
        get {return energy; }
        set
        {
            // se o status de energia muda
            if (energy != value)
            {
               energy =  Mathf.Clamp(value, 0, 1);
               energyStorage.GetComponent<Image>().fillAmount = Energy;
            }
        }
    }

    public bool Grounded()
    {
        if (Physics2D.Raycast(gorundCheckPoint.position, Vector2.down, gorundCheckY, whatIsGround)
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
        if (jumpBufferCounter > 0 && coyoteTime > 0 && !pState.jumping)
        {
            // StartCoroutine(DelayedJump());
            
            rb.velocity = new Vector3(rb.velocity.x, jumpHight);

            pState.jumping = true;
        }

        if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpHight);

            pState.jumping = true;

            airJumpCounter++;
        }

        if (Input.GetButtonDown("Jump") && rb.velocity.y > 3)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0);
            pState.jumping = false;
        }
        
        anim.SetBool("Jumping", !Grounded());
    }

    IEnumerator DelayedJump()
    {
        // Inicia a animação imediatamente
        anim.SetBool("Jumping", !Grounded());

        // Aguarda 5 segundos antes de aplicar o movimento vertical
        yield return new WaitForSeconds(1f);

        rb.velocity = new Vector3(rb.velocity.x, jumpHight);
        pState.jumping = true;
    }

    void UpdateJumpingVariables()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter--;
        }
    }
}
