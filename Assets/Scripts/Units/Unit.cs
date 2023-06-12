using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //Gameplay variables
    public float moveSpeed;
    public float rotationSpeed;
    public float attackRange;

    //Components
    private Seeker seeker;

    //Run-time variables
    private Path currentPath;
    private int currentIndex = 0;
    private Coroutine currentState;
    private Vector3 lastPos;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("I am the seeker");
        seeker = GetComponent<Seeker>();
        
        SetState(OnIdle());
    }

    private void FixedUpdate()
    {

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
                Vector3 targetDirection = nextPoint - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

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

            }
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
