using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{   
    [Header("Horizontal movement Settings")]
    [SerializeField] private float enemywalkSpeed = 1;
    

    [Header("Ground check Settings")]
    private float enemyjumpHight = 30;
    [SerializeField] private Transform enemygroundCheckPoint;
    [SerializeField] private float enemygorundCheckY = 0.2f;
    [SerializeField] private float enemygorundCheckX = 0.5f;
    [SerializeField] private LayerMask enemywhatIsGround;

    private Rigidbody2D enemyrb;
    private float enemyXAxis;

    [SerializeField]float health;
    [SerializeField]float recoilLength;
    [SerializeField]float recoilFactor;
    [SerializeField]bool isRecoiling = false;

    float recoilTimer;

    // Valor máximo que pode ser adicionado ou subtraído ao eixo X
    //private float maxChange = 1f;

    // Intervalo de tempo mínimo e máximo entre cada mudança
    private float minInterval = 1.5f;
    private float maxInterval = 2f;

    // Contador para controlar o tempo entre cada mudança
    private float changeTimer = 0f;
    private float changeInterval = 4f;
    private float changeStop = 2f;
    Animator enemyanim;

    // Start is called before the first frame update
    void Start()
    {
        enemyrb = GetComponent<Rigidbody2D>();
        enemyanim = GetComponent<Animator>();
        
        // Define o intervalo de tempo inicial para a primeira parada
        changeStop = Random.Range(minInterval, maxInterval);
        
        // verifica se a posição está de acordo com a velocidade
        if(transform.localScale.x == 1f){
        	enemywalkSpeed *= -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }
        if(isRecoiling)
        {
            if(recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
        controllEnemy();
        Move();
        //Jump();
    }

    public void EnemyHit(float _damegeDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damegeDone;
        if(!isRecoiling)
        {
            enemyrb.AddForce(-_hitForce * recoilFactor * _hitDirection);
        }
    }
    private void controllEnemy()
    {
        // Incrementa o contador de tempo
        changeTimer += Time.deltaTime;

        // Verifica se é hora de fazer uma mudança na posição do inimigo
        if (changeTimer >= changeInterval)
        {

           // Define um novo intervalo de tempo para a próxima parada
           changeStop = Random.Range(minInterval, maxInterval);

           // Reinicia o contador de tempo
           changeTimer = 0f;
           
           // vira o inimigo
           Flip();
        }
    }

    void Flip()
    {
    	
        if(transform.localScale.x == 1f)
        {
            transform.localScale = new Vector2(-1f, transform.localScale.y);
            enemywalkSpeed *= -1;
            
        }
        else
        {
            transform.localScale = new Vector2(1f, transform.localScale.y);
            enemywalkSpeed *= -1;
         
        }
        
    }

    private void Move()
    {
        if(changeTimer <= changeStop){
        	enemyrb.velocity = new Vector2(enemywalkSpeed, enemyrb.velocity.y);
        }else{
        	enemyrb.velocity = new Vector2(0, enemyrb.velocity.y);
        }
    }

    public bool Grounded()
    {
        if(Physics2D.Raycast(enemygroundCheckPoint.position, Vector2.down, enemygorundCheckY, enemywhatIsGround)
            || Physics2D.Raycast(enemygroundCheckPoint.position + new Vector3(enemygorundCheckX, 0, 0), Vector2.down, enemygorundCheckY, enemywhatIsGround)
            || Physics2D.Raycast(enemygroundCheckPoint.position + new Vector3(-enemygorundCheckX, 0, 0), Vector2.down, enemygorundCheckY, enemywhatIsGround))
        {
            return true;
        }else
        {
            return false;
        }
    }

    void Jump()
    {
        if(Input.GetButtonDown("Jump") && enemyrb.velocity.y > 0)
        {
            enemyrb.velocity = new Vector3(enemyrb.velocity.x, 0);
        }

        if(Input.GetButtonDown("Jump") && Grounded())
        {
            enemyrb.velocity = new Vector3(enemyrb.velocity.x, enemyjumpHight);
        }

        enemyanim.SetBool("Jumping", !Grounded());
    }
}
