using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueFish : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        enemyrb.gravityScale = 12f;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(!isRecoiling)
        {
            float directionX = PlayerController.Instance.transform.position.x - transform.position.x;
            transform.position = Vector2.MoveTowards
                (transform.position, new Vector2(PlayerController.Instance.transform.position.x, transform.position.y),
                speed * Time.deltaTime);
            base.Flip(directionX);
        }
    }

    public override void EnemyHit(float _damegeDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damegeDone, _hitDirection, _hitForce);
    }
}
