using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Projectile : PoolObject
{
    public float speed;
    private int attackPower;
    private BaseObject attackTarget;

    public void Init(BaseObject target, int attackPower)
    {
        this.attackPower = attackPower;
        this.attackTarget = target;

        //instead of moving to the transform.position of the object we want to move to the center of the object so the arrow doesn't go to the ground
        Vector3 targetPos = target.GetComponent<Collider>().bounds.center;
        //rotates projectile towards its target
        transform.LookAt(targetPos);

        Tweener moveTween = transform.DOMove(targetPos, speed);
        moveTween.SetSpeedBased(true);
        moveTween.OnComplete(OnProjectileArrived);
    }

    private void OnProjectileArrived()
    {
        //make sure that the building wasn't destroyed while arrow flying towards it
        if(attackTarget != null)
        {
            attackTarget.OnHit(attackPower);
        }
        PoolManager.Instance.DeSpawn(this);
    }
    // Start is called before the first frame update
}
