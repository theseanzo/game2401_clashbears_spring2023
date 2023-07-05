using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RTSCameraMovement : MonoBehaviour
{
    #region Look At
    [Header("Look At")]
    [SerializeField]
    private bool enableLookAt;
    public Transform LookAtTarget;



    public void LookAt()
    {
        Vector3 relativePos = LookAtTarget.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = rotation;
    }
    #endregion

    #region Movement

    [Header("Movement")]
    public Transform FollowTarget;

    [SerializeField]
    private float cameraMoveSpeed = 10.0f;

    public bool EnableScreenEdgeMovement = false;

    [SerializeField]
    private Vector2 movementInput;
    public void GetMovementInput(Vector2 input)
    {
        movementInput = input;
    }

    private void UpdateMovement()
    {
        //Debug.Log("CameraForward: " + transform.forward.ToString());
        //combine forward and right vectors with position to generate a target position
        Vector3 forwardDelta = new Vector3(transform.forward.x, 0.0f, transform.forward.z) * movementInput.y;
        Vector3 rightDelta = new Vector3(transform.right.x, 0.0f, transform.right.z) * movementInput.x;
        Vector3 deltaPosition = forwardDelta+rightDelta;


        //lerp towards target position at a fixed rate in time measured in meters/second by unity default
        FollowTarget.position += deltaPosition * Time.deltaTime * cameraMoveSpeed;// Vector3.Lerp(FollowTarget.position, deltaPosition, Time.deltaTime * cameraMoveSpeed);
    }

    #endregion

    #region Rotation

    [Header("Rotation")]
    public float RotationRate;

    [SerializeField]
    private float rotationInput;
    public void GetRotationInput(float input)
    {
        rotationInput = input;
    }

    private void UpdateRotation()
    {
        //rotate at fixed rate in degrees/second
        //next steps: ease in/out
        FollowTarget.Rotate(Vector3.up * (rotationInput*Time.deltaTime*RotationRate));

    }

    #endregion

    #region Zoom

    [Header("Zoom")]
    public float ZoomRate;

    [SerializeField]
    private Transform zoomHigh;

    [SerializeField]
    private Transform zoomLow;

    [SerializeField]
    private bool zoomToCursorPosition;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float zoomAlpha = 1.0f;

    //value representing total distance beetween min and max zoom
    //used to generate normalized zoom rate
    private float zoomDistance; 

    [SerializeField]
    private float zoomInput;

    public void GetZoomInput(float input)
    {
        zoomInput = input;
    }

    private void UpdateZoom()
    {
        //express zoom rate as a percentage
        float zoomRateAlpha = ZoomRate / zoomDistance;

        zoomAlpha = Mathf.Clamp(zoomAlpha + (zoomRateAlpha * Time.deltaTime * zoomInput), 0, 1);

        transform.position = Vector3.Lerp(zoomLow.position, zoomHigh.position, zoomAlpha);
    }



    #endregion


    // Start is called before the first frame update
    void Start()
    {
        //cannot initialize from a non static member in the constructor
        //need to derive this value on start or awake
        zoomDistance = Vector3.Distance(zoomLow.position, zoomHigh.position);
    }

    // Update is called once per frame
    void Update()
    {
        LookAt();
        UpdateMovement();
        UpdateRotation();
        UpdateZoom();
    }
}
