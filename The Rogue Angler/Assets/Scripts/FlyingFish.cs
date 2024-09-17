using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingFish : Enemy
{
    [SerializeField] private float chaseDistance;
    [SerializeField] private float stunDuration;

    float timer;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Idle);
    }

   protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Idle:
                rb.velocity = new Vector2(0, 0);
                if (_dist < chaseDistance)
                {
                    ChangeState(EnemyStates.Chase);
                }
                break;

            case EnemyStates.Chase:
                rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, Time.deltaTime * speed));

                FlipVoador();

                if(_dist > chaseDistance)
                {
                    ChangeState(EnemyStates.Idle);
                }
                break;
            case EnemyStates.Stunned:
                timer += Time.deltaTime;
                if (timer > stunDuration)
                {
                    ChangeState(EnemyStates.Idle);
                    timer = 0;
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

        if (health > 0)
        {
            ChangeState(EnemyStates.Stunned);
        }
        else
        {
            ChangeState(EnemyStates.Death);
        }
    }

    protected override void Death(float _destroyTime){
        rb.gravityScale = 12;

        base.Death(_destroyTime);
    }

    protected override void ChangeCurrentAnimation()
    {
        anim.SetBool("Idle", GetCurrentEnemyState == EnemyStates.Idle);

        anim.SetBool("Chase", GetCurrentEnemyState == EnemyStates.Chase);

        anim.SetBool("Stunned", GetCurrentEnemyState == EnemyStates.Stunned);

        if (GetCurrentEnemyState == EnemyStates.Death)
        {
            anim.SetTrigger("Death");
        }
    }

    void FlipVoador()
    {
        sr.flipX = PlayerController.Instance.transform.position.x > transform.position.x;
    }
}
