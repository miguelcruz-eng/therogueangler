using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{   
    [Header("Horizontal movement Settings")]
    [SerializeField] private float walkSpeed = 1;
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

    [HideInInspector] public PlayerStats pState;
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    //Input Variables
    private float xAxis, yAxis;
    
    public static PlayerController Instance;

    private void Awake()
    {
        if(Instance !=null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        Health = maxHealth;
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
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(sideAttackTrasnform.position, sideAttackArea);
        Gizmos.DrawWireCube(upAttackTrasnform.position, sideAttackArea);
        Gizmos.DrawWireCube(downAttackTrasnform.position, sideAttackArea);
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
        RestoreTimeScale();
        FlashWhileInvinclible();
        Heal();
    }

    private void FixedUpdate()
    {
        if(pState.dashing) return;
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
        if(xAxis < 0)
        {
            transform.localScale = new Vector2(-1f, transform.localScale.y);
            pState.lookingRight = false;
        }
        else if(xAxis > 0)
        {
            transform.localScale = new Vector2(1f, transform.localScale.y);
            pState.lookingRight = true;
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
        if(Grounded()) Instantiate(dash, transform);
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
                Hit(sideAttackTrasnform, sideAttackArea, ref pState.recoilingX, recoilXSpeed);
                Instantiate(slash, sideAttackTrasnform);
            }else if(yAxis > 0)
            {
                Hit(upAttackTrasnform, upAttackArea, ref pState.recoilingY, recoilYSpeed);
                SlashAngle(slash, 80, upAttackTrasnform);
            }else if(yAxis < 0 && !Grounded())
            {
                Hit(downAttackTrasnform, downAttackArea, ref pState.recoilingY, recoilYSpeed);
                SlashAngle(slash, -80, downAttackTrasnform);
            }
        }
    }

    private void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilDir, float _recoilStrenght)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);
        List<Enemy> hitEnemies = new List<Enemy>();

        if(objectsToHit.Length > 0)
        {
            _recoilDir = true;
        }
        for(int i = 0; i < objectsToHit.Length; i++)
        {
            Enemy e = objectsToHit[i].GetComponent<Enemy>();
            if(e && !hitEnemies.Contains(e))
            {
                e.EnemyHit(damage, (transform.position - objectsToHit[i].transform.position).normalized, _recoilStrenght);
                hitEnemies.Add(e);
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
        if(pState.recoilingX)
        {
            if(pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            }else 
            {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }
        }

         if(pState.recoilingY)
        {
            rb.gravityScale = 0;
            if(yAxis < 0)
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
        if(pState.recoilingX && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++;
        }else
        {
            StopRecoilX();
        }

        if(pState.recoilingY && stepsYRecoiled < recoilYSteps)
        {
            stepsYRecoiled++;
        }else
        {
            StopRecoilY();
        }

        if(Grounded())
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
        Health -= Mathf.RoundToInt(_damage);
        StartCoroutine(StopTakingDamge());
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

    void FlashWhileInvinclible()
    {
        sr.material.color = pState.invincible ? 
            Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f)) : 
            Color.white;
    }

    void RestoreTimeScale()
    {
        if(restoreTime)
        {
            if(Time.timeScale < 1)
            {
                Time.timeScale += Time.deltaTime * restoreTimeSpeed;
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
        if(_dealay > 0)
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
        restoreTime = true;
        yield return new WaitForSeconds(_dealay);
    }

    public int Health
    {
        get { return health; }
        set
        {
            if(health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);

                if(onHealthChangedCallBack != null)
                {
                    onHealthChangedCallBack.Invoke();
                }
            }
        }
    }

    GameObject _healingPotion = null; // Declaração fora do if

    void Heal()
    {
        if (Input.GetButton("Healing") && Health < maxHealth && Energy > 0 && !pState.jumping && !pState.dashing)
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
            // if energy stats change
            if(energy != value)
            {
               energy =  Mathf.Clamp(value, 0, 1);
               energyStorage.GetComponent<Image>().fillAmount = Energy;
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
