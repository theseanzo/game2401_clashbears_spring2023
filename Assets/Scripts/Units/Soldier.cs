using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Unit
{
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void OnAttackActionEvent()
    {
        base.OnAttackActionEvent(); //tell its parent to do its thing
        if(attackTarget != null)
        {
            attackTarget.OnHit(attackPower);
        }
    }
}
