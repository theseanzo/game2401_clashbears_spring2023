using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    //these are for animating units and so they need a reference to the unit that they are attached to
    private Unit unit;

    private void Awake()
    {
        unit = GetComponentInParent<Unit>();
    }
    public void AttackAnimationEvent() //our animation is actually looking for a particular function to be called
    {
        unit.OnAttackActionEvent();
    }
}
