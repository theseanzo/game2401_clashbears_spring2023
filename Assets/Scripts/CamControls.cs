using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;

public class CamControls : MonoBehaviour
{
    private Camera camera;

    //about panning

    private Vector3 mousePosition;
    private Vector3 viewportPosition;
    private float mouseXMovement;
    private float mouseYMovement;
    public Bounds bounds;
    [Header("Pan")] public float mousePanSpeed = 15f;

    //about zoom
    [Header("Zoom")] public int maxZoom = 30;
    public int minZoom = 10;
    private Vector3 direction;
    private float scrollWheel;
    private float distance;
    RaycastHit hit;
    public float scrollWheelSpeed = 300.0f;
    public LayerMask layerMask;

    //about rotation
    [Header("HorizontalRotation")] public float rotationSpeed = 30f;

    //about bird view
    [Header("BirdView")] //private Transform originalTransform;
    private Vector3 originalPosition;

    private Quaternion originalRotation;
    public Vector3 birdPosition = new Vector3(0, 45, 0);
    public Vector3 birdRotation = new Vector3(90, 0, -45);
    private Quaternion targetRotation;
    private bool flag;
    private bool switchFlag = true;

    private enum viewState
    {
        goBirdView,
        backNormalView,
        normal
    }

    private viewState myViewState = viewState.normal;


    // Start is called before the first frame update
    void Start()
    {
        //get camera's z axis
        camera = GetComponentInChildren<Camera>();
        originalPosition = camera.transform.position;
        originalRotation = camera.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        mouseXMovement = Input.GetAxis("Mouse X");
        mouseYMovement = Input.GetAxis("Mouse Y");
        scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        mousePosition = Input.mousePosition;
        //disable zoom and pan only when rotating the camera
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetButton("Fire1"))
            {
                RotateCamera();
            }
            else
            {
                zoomInOut();
                Panning();
            }

            //when we are far away from terrain more than maxZoom, we will begin birdView.
            if (distance >= maxZoom)
            {
                if (scrollWheel < 0)
                {
                    //only store the original position for recovering position once 
                    if (switchFlag == true)
                    {
                        originalPosition = camera.transform.position;
                        originalRotation = camera.transform.rotation;
                        switchFlag = false;
                    }

                    myViewState = viewState.goBirdView;
                }

                //when we get the bird view position, tweak the mouse wheel to back original position we stored
                if (scrollWheel > 0)
                {
                    if (Mathf.Abs((camera.transform.position - birdPosition).magnitude) < 2f)
                    {
                        myViewState = viewState.backNormalView;
                    }
                }
            }

            SetBirdView();
        }
    }

    void SetBirdView()
    {
        switch (myViewState)
        {
            case viewState.goBirdView:
                camera.transform.position =
                    Vector3.Lerp(camera.transform.position, birdPosition, 0.9f * Time.deltaTime);
                camera.transform.rotation = Quaternion.Lerp(camera.transform.rotation, Quaternion.Euler(birdRotation),
                    0.8f * Time.deltaTime);

                //if camera get the target position, make it available to zoom again
                if (Mathf.Abs(camera.transform.position.magnitude - birdPosition.magnitude) < 1f)
                {
                    myViewState = viewState.normal;
                }

                break;
            case viewState.backNormalView:
                //Debug.Log("I am working in normal View");
                //Debug.Log(String.Format("返回正常视角originalTransform.position：{0}", originalPosition));

                camera.transform.position = Vector3.Slerp(camera.transform.position, originalPosition,
                    0.9f * Time.deltaTime);
                camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, originalRotation,
                    0.8f * Time.deltaTime);
                if (Mathf.Abs(camera.transform.position.magnitude - originalPosition.magnitude) < 1f)
                {
                    myViewState = viewState.normal;
                    switchFlag = true;
                }

                break;
            case viewState.normal:
                break;
        }
    }

    void RotateCamera()
    {
        this.transform.Rotate(new Vector3(0, mouseXMovement, 0) * Time.deltaTime * rotationSpeed, Space.Self);
    }

    void Panning()
    {
        viewportPosition = camera.WorldToViewportPoint(mousePosition);
        //when we pan the mouse, the camera will pan in a bounded range
        //notice on Y Axis, we need to make it not bounded by bound
        if (Input.mousePosition.x >= Screen.width)
        {
            HorizontalPan();
        }

        if (Input.mousePosition.x <= 0)
        {
            HorizontalPan();
        }

        if (Input.mousePosition.y >= Screen.height)
        {
            VerticalPan();
        }

        if (Input.mousePosition.y <= 0)
        {
            VerticalPan();
        }
    }

    private void VerticalPan()
    {
        this.transform.Translate(new Vector3(0, 0, mouseYMovement) * Time.deltaTime * mousePanSpeed, Space.Self);
        this.transform.position = new Vector3(bounds.ClosestPoint(this.transform.position).x, transform.position.y,
            bounds.ClosestPoint(this.transform.position).z);
    }

    private void HorizontalPan()
    {
        this.transform.Translate(new Vector3(mouseXMovement, 0, 0) * Time.deltaTime * mousePanSpeed, Space.Self);
        this.transform.position = new Vector3(bounds.ClosestPoint(this.transform.position).x, transform.position.y,
            bounds.ClosestPoint(this.transform.position).z);
    }

    void zoomInOut()
    {
        direction = camera.transform.forward;
        //cast a ray to measure the distance between camera to terrain
        Physics.Raycast(camera.transform.position, direction, out hit, Mathf.Infinity, layerMask);
        distance = hit.distance;
        //Debug.Log(distance);
        //set a distance to limit whether could zoom further
        if (distance > minZoom)
        {
            if (scrollWheel > 0)
            {
                transform.Translate(direction * scrollWheel * scrollWheelSpeed * Time.deltaTime, Space.World);
            }
        }

        if (distance < maxZoom)
        {
            if (scrollWheel < 0)
            {
                transform.Translate(direction * scrollWheel * scrollWheelSpeed * Time.deltaTime, Space.World);
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}