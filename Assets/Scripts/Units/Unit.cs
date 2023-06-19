using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Seeker))]
public class Unit : BaseObject
{
    //Gameplay variables
    public float moveSpeed;
    public float rotationSpeed;
    public float attackRange = 10f;
    public float attackInterval = 2f;
    public int attackPower = 10;
    private float lastAttackTime = 0;//we will use to increment our time for attacking to make sure we don't attack constantly

    //Components
    protected Seeker seeker;
    protected Animator anim;

    //Run-time variables
    private Path currentPath;
    private int currentIndex = 0;
    private Coroutine currentState;
    private Vector3 lastPos;

    protected Building attackTarget; //the building we are attacking; it is protected because we most likely will want to access this variable within our children of Unit

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position; //our current lastPos is where we are
        seeker = GetComponent<Seeker>();
        anim = GetComponentInChildren<Animator>();
        SetState(OnIdle());
    }

    private void FixedUpdate()
    {
        SetAnimationSpeed();
    }

    private void SetAnimationSpeed()
    {
        Vector3 movement = transform.position - lastPos;
        anim.SetFloat("Speed", movement.magnitude);
        //we now need to store the last position at the end of each frame
        lastPos = transform.position;
    }

    private void OnPathComplete(Path p)
    {
        currentPath = p;
    }

    //Instead of providing an enum, we provide an IEnumerator as a parameter
    private void SetState(IEnumerator newState)
    {
        //The very first time we call SetState, currentState will be null
        if (currentState != null)
        {
            //We stop the current state from running
            StopCoroutine(currentState);
        }
        //We run the next state
        currentState = StartCoroutine(newState);
    }

    IEnumerator OnIdle()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            LookForBuilding();
        }
    }
    IEnumerator OnAttack(Building target)
    {
        attackTarget = target;
        while (true)
        {
            if(target != null)
            {
                LookTowards(target.transform.position);//this way we are not swinging wildly in the wrong direction

            }

            lastAttackTime += Time.deltaTime; //update our attack time based on the amount of time passed in frames
            if(lastAttackTime >= attackInterval)
            {
                lastAttackTime = 0;
                Attack(); 
            }
            if(target == null)
            {
                SetState(OnIdle());
            }
            yield return new WaitForFixedUpdate();
        }
    }
    protected virtual void Attack()
    {
        //do some damage, and we want to play our animation
        anim.SetTrigger("Attack");
    }
    public virtual void OnAttackActionEvent()
    {
        //this is going to called by our animation 
    }
    IEnumerator OnMoveToTarget(Building target)
    {
        //The seeker calculates the path to this position
        //When the path has been calculated, the OnPathComplete method is called automatically
        //The name for this is a 'callback'
        seeker.StartPath(transform.position, target.transform.position, OnPathComplete);

        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (currentPath != null)
            {
                Vector3 nextPoint = currentPath.vectorPath[currentIndex];

                transform.position = Vector3.MoveTowards(transform.position, nextPoint, moveSpeed * Time.fixedDeltaTime);

                //We calculate the target rotation by calculating the direction vector, and from that the rotation

                LookTowards(nextPoint);

                //If the Unit reached the next point in the path....
                if (transform.position == nextPoint)
                {
                    currentIndex++;
                }

                //Once we reached the final destination
                if (currentIndex >= currentPath.vectorPath.Count)
                {
                    currentIndex = 0;
                    currentPath = null;
                }
                //Instead of going towards the center of the building, we are going to go to the closest point of the building to us; this insures that our units will attack the sides of buildings rather than the center
                Vector3 targetPos = target.GetComponent<BoxCollider>().ClosestPointOnBounds(transform.position);
                if(Vector3.Distance(transform.position, targetPos) <= attackRange)
                {
                    SetState(OnAttack(target));//we now set a new state which is for attacking the target
                    //we are going to set our state to OnAttacking
                }


            }
        }
    }
    void LookTowards(Vector3 position)
    {
        //for our look direction, we calculate the target's rotation by calculating the direction vector, and then, from that, our rotation
        Vector3 targetDirection = position - transform.position;
        //we only want to rotate if there is an actual direction
        if(targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void LookForBuilding()
    {
        //TODO can we optimize this?
        Building[] allBuildings = FindObjectsOfType<Building>();


        float shortestDistance = Mathf.Infinity;
        Building closestBuilding = null;

        foreach (Building b in allBuildings)
        {
            float distance = Vector3.Distance(transform.position, b.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestBuilding = b;
            }
        }

        //We have now found the closest building!
        SetState(OnMoveToTarget(closestBuilding));
    }

}
