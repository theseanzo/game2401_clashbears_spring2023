using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Unit
{
    public Transform arrowStartPos;
    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnAttackActionEvent()
    {
        base.OnAttackActionEvent();
        //if building wasn't destroyed while we were animating towards shooting
        if(attackTarget != null)
        {
            PoolObject arrow = PoolManager.Instance.Spawn("Arrow");
            arrow.transform.position = arrowStartPos.position;
            arrow.transform.rotation = arrowStartPos.rotation;
            arrow.GetComponent<Projectile>().Init(attackTarget, attackPower);
        }
    }
}
