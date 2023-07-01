using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq; 

public class CamControls : MonoBehaviour
{
    public Bounds bounds;
    public float zoomSpeed = 10f; //The rate of speed for zooming in and out.
    public float minZoom = 20f; // The closest the camera can get.
    public float maxZoom = 200f; // The farthest the camera can get.
    public float panSpeed = 20f; // The speed of the camera when being panned
    public float smoothTime = 0.2f;
    private Vector3 velocity = Vector3.zero; 
    public float rotationSpeed = 10f; // Speed of camera turning when mouse moves in along an axis.

    public float maxVerticalAngle = 80f;
    public float minVerticalAngle = 10f;


    //The minimum and maximum points that the camera can go to in world coordinates
    public Vector3 minPosition = new Vector3(-10, -10, -10);
    public Vector3 maxPosition = new Vector3(10, 10, 10);

    private Vector3 nextPosition;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
        // Disable camera controls when the mouse is over UI
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            // Get the height of the terrain
            float terrainHeight = hit.point.y;

            // Move the camera's position
            transform.position = new Vector3(transform.position.x, terrainHeight, transform.position.z);
        }
        //Getting the proposed next position of the camera
        nextPosition = transform.position + new Vector3(-Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"));

        if (nextPosition.x > minPosition.x && nextPosition.x < maxPosition.x &&
            nextPosition.y > minPosition.y && nextPosition.y < maxPosition.y &&
            nextPosition.z > minPosition.z && nextPosition.z < maxPosition.z)
        {
            transform.position = nextPosition;
        }


        if (Input.GetMouseButton(0))
        {
            float mouseXMovement = Input.GetAxis("Mouse X");
            float mouseYMovement = Input.GetAxis("Mouse Y");
            this.transform.Translate(-mouseXMovement, 0, mouseYMovement);
            transform.position = bounds.ClosestPoint(transform.position);//this clamps our position to be the closest point to our boundary.
        }

        float scrollData;
        scrollData = Input.GetAxis("Mouse ScrollWheel");
        Zoom(scrollData, zoomSpeed);

        if (Input.GetMouseButton(1)) // Right mouse button for panning.
        {
            // Calculate the new position.
            float mouseXMovement = Input.GetAxis("Mouse X") * panSpeed * Time.deltaTime;
            float mouseYMovement = Input.GetAxis("Mouse Y") * panSpeed * Time.deltaTime;

            // Apply the movement.
            transform.Translate(-mouseXMovement, -mouseYMovement, 0);

            // Clamp the position within the bounds.
            transform.position = bounds.ClosestPoint(transform.position);
        }

        //Rotate the camera based on the mouse position
        if (Input.GetMouseButton(1)) //Right click
        {

            float h = rotationSpeed * Input.GetAxis("Mouse X");
            float v = rotationSpeed * Input.GetAxis("Mouse Y");
            //transform.Rotate(v, h, 0);

            //Clamp the verticle rotation
            float eulerY = transform.eulerAngles.y + h;
            float eulerX = transform.eulerAngles.x + v;

            //Restiction the camera's rotation along the x-axis between 10 and 80 degrees.
            eulerX = Mathf.Clamp(eulerX, minVerticalAngle, maxVerticalAngle);
            transform.rotation = Quaternion.Euler(eulerX, eulerY, 0);

            if (Input.GetMouseButton(2)) // Middle mouse button
            {
                float horizontalInput = Input.GetAxis("Mouse X") * panSpeed * Time.deltaTime;
                float verticalInput = Input.GetAxis("Mouse Y") * panSpeed * Time.deltaTime;

                Vector3 targetPosition = transform.position + new Vector3(-horizontalInput, -verticalInput, 0);
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
                //transform.Translate(-horizontalInput, -verticalInput, 0);
            }
        }

        //Keyboard based Panning
        float moveX = 0f;
        float moveZ = 0f;

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetKey(KeyCode.W))
            {
                moveZ = +1f;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                moveZ = -1f;
            }

            if (Input.GetKey(KeyCode.A))
            {
                moveX = -1f;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                moveX = +1f;
            }
        }

        Vector3 direction = new Vector3(moveX, 0, moveZ).normalized;
        transform.Translate(direction * panSpeed * Time.deltaTime, Space.World);

    }
    private void Zoom(float increment, float speed)
    {
        // Compute the current zoom level as a percentage between minZoom and maxZoom.
        float zoomLevel = (Camera.main.fieldOfView - minZoom) / (maxZoom - minZoom);

        // Apply quadratic easing to the zoom level
        zoomLevel = zoomLevel * zoomLevel;

        // Compute the desired vertical rotation angle based on the current zoom level.
        float desiredAngle = Mathf.Lerp(minVerticalAngle, maxVerticalAngle, zoomLevel);

        // Adjust the current vertical rotation angle towards the desired angle.
        Vector3 currentAngles = transform.eulerAngles;
        currentAngles.x = Mathf.Lerp(currentAngles.x, desiredAngle, Time.deltaTime * rotationSpeed);
        transform.eulerAngles = currentAngles;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
