using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour //we want our unit to wander around a scene looking for locations
{
    // Start is called before the first frame update
    public float moveSpeed;
    public float rotationSpeed;
    public float attackRange; //our characters will be able to do damage

    //our units are going to need to find paths, attack, do damage, etc.
    private Seeker seeker;//keeps track of the seeker component for looking or paths
    private Path currentPath; //the current path of our character

    //for our state for the AI, in the first semester we used enums to keep track; second semester, what did we do instead?

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
