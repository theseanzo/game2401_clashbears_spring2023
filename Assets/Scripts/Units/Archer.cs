using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Unit
{
    public Transform arrowStartPos;
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();//we call our parent's start function
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void OnAttackActionEvent() //we need to override the AttackActionEvent because an Archer is not the same as a Soldier, Mage, etc.
    {
        base.OnAttackActionEvent();

        //if the building we were walking towards was not destroyed while we were moving towards it, let's shoot an arrow
        if (attackTarget != null)
        {
            PoolObject arrow = PoolManager.Instance.Spawn("Arrow");
            arrow.transform.position = arrowStartPos.position; //we get our arrow from the Pool and then we set its position on the Archer (essentially attach it to the bow)
            arrow.transform.rotation = arrowStartPos.rotation;
            arrow.GetComponent<Projectile>().Init(attackTarget, attackPower);
        }
    }
}
