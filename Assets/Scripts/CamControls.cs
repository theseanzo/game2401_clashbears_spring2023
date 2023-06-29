using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CamControls : MonoBehaviour
{
    [Header("Camera properties")]
    public Bounds bounds;
    private Camera mainCamera;

    [Header("Camera Zoom")]
    public float zoomSpeed = 5.0f;
    public float zoomMin = 5.0f;
    public float zoomMax = 60.0f;
    public float minAngle = 10.0f;
    public float maxAngle = 60.0f;
    public float translationSpeed = 1.0f;
    
    [Header("Camera Panning")]
    public float panSpeed = 15.0f;
    
    [Header("Camera Rotation")]
    public float rotationSpeed = 50.0f;
    
    void Start()
    {
        mainCamera = GetComponentInChildren<Camera>();
        if (mainCamera == null)
            mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    void Update()
    {
        // We disable any movement if the mouse is over a UI element
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
        HandleCameraMovement();
        HandleCameraZoom();
        HandleCameraPanning();
        HandleCameraRotation();
    }

    private void HandleCameraMovement()
    {
        // We only want to move the camera if the mouse is down and we are not clicking on a UI element
        if (!Input.GetMouseButton(0))
            return;

        var mouseXMovement = Input.GetAxis("Mouse X");
        var mouseYMovement = Input.GetAxis("Mouse Y");
            
        transform.Translate(-mouseXMovement, 0, mouseYMovement);
            
        // Clamp our position to be the closest point to our boundary
        transform.position = bounds.ClosestPoint(transform.position);
    }

    private void HandleCameraZoom()
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        var zoomInput = scroll * zoomSpeed;
        var newFieldOfView = Mathf.Clamp(mainCamera.fieldOfView - zoomInput, zoomMin, zoomMax);

        // Check if the new field of view is equal to the current field of view
        // This prevents the camera from moving when we are already at the min or max zoom
        if (Mathf.Approximately(newFieldOfView, mainCamera.fieldOfView))
            return;

        mainCamera.fieldOfView = newFieldOfView;
        
        // We calculate the zoom percentage
        var zoomPercentage = (mainCamera.fieldOfView - zoomMin) / (zoomMax - zoomMin);
        
        // We lerp the desired pitch based on the zoom percentage
        var targetPitch = Mathf.Lerp(maxAngle, minAngle, zoomPercentage);
        
        // We get the current rotation of the camera
        var currentRotation = transform.rotation.eulerAngles;
        
        // We create a new rotation with the desired pitch
        var targetRotation = Quaternion.Euler(targetPitch, currentRotation.y, currentRotation.z);
        
        // We lerp the rotation of the camera
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, zoomSpeed * Time.deltaTime);
        
        // Debug.Log(Input.mousePosition);
        // We raycast from the mouse position to the ground
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            // We calculate the translation direction based on the mouse position
            var direction = hit.point - transform.position;

            // We move the camera in the direction of the translation
            var translationInput = zoomInput * translationSpeed;
            transform.Translate(direction * translationInput, Space.World);
        }

        // Clamp the camera position to the bounds
        transform.position = bounds.ClosestPoint(transform.position);
    }

    private void HandleCameraPanning()
    {
        // We use the arrow keys to pan the camera around
        var horizontalMovement = Input.GetAxis("Horizontal");
        var verticalMovement = Input.GetAxis("Vertical");

        // We normalize the direction so that we don't move faster diagonally
        var panDirection = new Vector3(horizontalMovement, 0, verticalMovement).normalized;
        if (panDirection == Vector3.zero)
            return;
        
        // We move the camera in the direction of the pan
        var panTranslation = panDirection * panSpeed * Time.deltaTime;
        transform.Translate(panTranslation);
        transform.position = bounds.ClosestPoint(transform.position);
    }

    private void HandleCameraRotation()
    {
        var rotationDirection = 0.0f;
        
        if (Input.GetKey(KeyCode.E))
            rotationDirection += 1.0f;

        if (Input.GetKey(KeyCode.Q))
            rotationDirection -= 1.0f;

        var rotationAmount = rotationDirection * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotationAmount);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}