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

    [Header("Camera Panning")]
    public float panSpeed = 15.0f;
    
    void Start()
    {
        mainCamera = GetComponentInChildren<Camera>();
        if (mainCamera == null)
            mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    void Update()
    {
        HandleCameraMovement();
        HandleCameraZoom();
        HandleCameraPanning();
    }

    private void HandleCameraMovement()
    {
        // We only want to move the camera if the mouse is down and we are not clicking on a UI element
        if (!Input.GetMouseButton(0) || EventSystem.current.IsPointerOverGameObject())
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
        var zoomAmount = scroll * zoomSpeed;
        mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView - zoomAmount, zoomMin, zoomMax);
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
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}