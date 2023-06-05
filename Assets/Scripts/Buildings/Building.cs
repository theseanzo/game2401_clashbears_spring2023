using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public class Building : MonoBehaviour
{
    public Vector2Int size;
    private float colliderHeight = 2f;
    public bool isOverlapping;
    private int nOverlappingBuildings = 0;


    private MeshRenderer rd;

    private void Awake() //recall that Awake is called when a script is loaded
    {
        this.gameObject.layer = LayerMask.NameToLayer("Building");//this puts our buildings on the Building layer
        rd = GetComponentInChildren<MeshRenderer>(); //we should have only one mesh renderer in our children for this object 
    }

    private void OnValidate() //this is called any time a value changes in the 
    {
        BoxCollider coll = GetComponent<BoxCollider>();
        coll.size = new Vector3(size.x, colliderHeight, size.y);
        coll.center = new Vector3(0, colliderHeight * 0.5f, 0);
    }

    private void OnTriggerEnter(Collider other) //this is time any time that our collision box is inside of another collision box
    {
        nOverlappingBuildings++;
        isOverlapping = true;
        rd.material.color = Color.red;//if you are overlapping something, select red.
    }

    private void OnTriggerExit(Collider other)
    {
        nOverlappingBuildings--; //
        if(nOverlappingBuildings == 0) //the reason we check to make sure that we are overlapping 0 buildings is that we could potentially overlap 2 or more at the same time
        {
            isOverlapping = false;
            rd.material.color = Color.white;//white = not overlapping and red = overlapping
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
