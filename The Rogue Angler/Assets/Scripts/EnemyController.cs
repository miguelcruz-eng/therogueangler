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

    // Valor máximo que pode ser adicionado ou subtraído ao eixo X
    private float maxChange = 1f;

    // Intervalo de tempo mínimo e máximo entre cada mudança
    private float minInterval = 1f;
    private float maxInterval = 3f;

    // Contador para controlar o tempo entre cada mudança
    private float changeTimer = 0f;
    private float changeInterval = 0f;
    Animator enemyanim;

    // Start is called before the first frame update
    void Start()
    {
        enemyrb = GetComponent<Rigidbody2D>();
        enemyanim = GetComponent<Animator>();
        // Define o intervalo de tempo inicial antes da primeira mudança
        changeInterval = Random.Range(minInterval, maxInterval);
    }

    // Update is called once per frame
    void Update()
    {
        controllEnemy();
        Move();
        Jump();
        Flip();
    }

    private void controllEnemy()
    {
        // Incrementa o contador de tempo
        changeTimer += Time.deltaTime;

        // Verifica se é hora de fazer uma mudança na posição do inimigo
        if (changeTimer >= changeInterval)
        {
            // Gera um valor aleatório entre -1 e 1 para representar a mudança no eixo X
            float changeAmount = Random.Range(-maxChange, maxChange);

            // Adiciona o valor gerado à posição atual do inimigo no eixo X
            enemyXAxis = changeAmount;

            // Define um novo intervalo de tempo para a próxima mudança
            changeInterval = Random.Range(minInterval, maxInterval);

            // Reinicia o contador de tempo
            changeTimer = 0f;
        }
    }

    void Flip()
    {
        if(enemyxAxis < 0)
        {
            transform.localScale = new Vector2(-0.3f, transform.localScale.y);
        }
        else if(enemyxAxis > 0)
        {
            transform.localScale = new Vector2(0.3f, transform.localScale.y);
        }
    }

    private void Move()
    {
        enemyrb.velocity = new Vector2(enemywalkSpeed * enemyxAxis, enemyrb.velocity.y);
 
        enemyanim.SetBool("Walking", enemyrb.velocity.x != 0 && Grounded());
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
