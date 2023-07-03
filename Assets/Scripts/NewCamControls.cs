using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCamControls : MonoBehaviour
{

    private Vector3 MouseStartPos;

    [SerializeField]
    private float ZoomScale = 5.0f;     //Zoom in speed

    [SerializeField]
    private float ZoomedIn = 25.0f;     //  Closest as the camera can zoom into

    [SerializeField]
    private float ZoomedOut = 50.0f;    //   Furthest zoom distance


    float inputX; // input for moving the camera up and down
    float inputZ; // input for moving the camera left to right


    [SerializeField]
    private float MoveSpeed = 10.0f;    // Speed the camera moves at


    private Vector3 Rotation;


    [SerializeField]
    private float RotateSpeed = 10.0f;  // Speed the camera rotates at



    public Bounds bounds;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxis("Horizontal");       // Sets up and down arrows and W S as controls
        inputZ = Input.GetAxis("Vertical");         // Sets left and right arrows and A D as controls

        if (inputX != 0)
            move();
        if (inputZ != 0)
            move();


        transform.Rotate(Rotation * RotateSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.Q))        // Rotates the camera to the left when holding Q
        {
            Rotation = Vector3.up;
        }
        else if (Input.GetKey(KeyCode.E))  // Rotates the camera to the Right when holding E
        {
            Rotation = Vector3.down;
        }
        else
        {
            Rotation = Vector3.zero;      // Stops camera from rotating when no input is pressed
        }


        Zoom(Input.GetAxis("Mouse ScrollWheel"));  // Sets scrollwheel as control for the camera zoom


    }


    private void move()
    {
        transform.position += transform.forward * inputZ * MoveSpeed * Time.deltaTime;  // moves the camera up and down
        transform.position += transform.right * inputX * MoveSpeed * Time.deltaTime;    // moves the camera left and right
    }




    private void Zoom(float ZoomDiff)
    {
        if (ZoomDiff != 0)
        {
            MouseStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - ZoomDiff * ZoomScale, ZoomedIn, ZoomedOut);
            Vector3 mouseWorldPosDiff = MouseStartPos - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position += mouseWorldPosDiff;
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
