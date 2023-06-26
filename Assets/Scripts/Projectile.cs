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
        //any field that belongs to an object can be referred to by using "this"
        this.attackPower = attackPower; //if we didn't say "this.attackPower" it would be ambiguous between the two
        this.attackTarget = target;

        Vector3 targetPos = target.GetComponent<Collider>().bounds.center; //instead of targeting the position of our target, we want to attack the center of that object
        
        transform.LookAt(targetPos);//we rotate our projectile towards the target

        Tweener moveTween = transform.DOMove(targetPos, speed);
        moveTween.SetSpeedBased(true);
        moveTween.OnComplete(OnProjectileArrived);
    }
    private void OnProjectileArrived()
    {
        //we have to do two things: 1) if the target currently exists, do some damager; 2) despawn this object 
        attackTarget?.OnHit(attackPower); //recall that the "attackTarget?" means only call the function if the object is not null 
        //after this, despawn it
        PoolManager.Instance.DeSpawn(this);
    }
}
