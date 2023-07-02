using DG.Tweening.Core.Easing;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraControllerV2 : MonoBehaviour
{
    /*  
     *  ------------------------
     *  Controls:
     *      Zoom:   scroll wheel
     *      Pan:    arrow keys
     *      Rotate: hold alt + mouse movement
     *  ------------------------
     */

    private EventSystem _eventSystem;

    [SerializeField] public Bounds CameraBounds;
    [SerializeField][Range(0, 1000)] public float MinZoomDistance = 5.0f;
    [SerializeField][Range(0, 1000)] public float MaxZoomDistance = 50.0f;
    [SerializeField][Range(0, 90)] public float MinZoomAngle = 30.0f;
    [SerializeField][Range(0, 90)] public float MaxZoomAngle = 60.0f;
    [SerializeField][Range(0, 1)] public float StartingZoomAmount = 0.5f;
    [SerializeField] public float ZoomSpeed = 100.0f;
    [SerializeField] public float PanSpeed = 15.0f;
    [SerializeField][Range(0, 360)] public float StartingYAngle = 45.0f;
    [SerializeField] public float RotationSpeed = 90.0f;
    [SerializeField] public float MaxRaycastDistance = 5000.0f;

    private void Start()
    {
        //get UI EventSystem
        _eventSystem = EventSystem.current;

        //initialize camera
        InitCamera();
    }

    private void Update()
    {
        MoveCamera();
    }

    private void InitCamera()
    {
        //set camera pivot initial angle
        transform.rotation = Quaternion.Euler(new Vector3(Mathf.Lerp(MinZoomAngle, MaxZoomAngle, StartingZoomAmount), StartingYAngle, 0.0f));

        //position camera based on zoom angle and initial zoom amount
        Camera.main.transform.position = -transform.forward * ((MaxZoomDistance - MinZoomDistance) * StartingZoomAmount + MinZoomDistance);

        //set camera initial direction
        Camera.main.transform.LookAt(transform.position);
    }

    private void MoveCamera()
    {
        //no camera movement if hovering over UI
        if (!_eventSystem.IsPointerOverGameObject())
        {
            ZoomCamera();
            PanCamera();
            RotateCamera();
        }
    }

    private void ZoomCamera()
    {
        //only update camera zoom if zooming (with scroll wheel)
        if (Input.GetAxis("Mouse ScrollWheel") != 0.0f)
        {
            RaycastHit mouseHit;
            if (Input.GetAxis("Mouse ScrollWheel") > 0.0f //zoom in
                && Vector3.Distance(Camera.main.transform.position, transform.position) > MinZoomDistance //zoom in if not fully zoomed in
                && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out mouseHit, MaxRaycastDistance))
            {
                //zoom in towards mouseHit
                Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, mouseHit.point, ZoomSpeed * Time.deltaTime);

                //move pivot based on new camera position
                RaycastHit screenHit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Camera.main.ViewportToScreenPoint(Vector3.one / 2.0f)), out screenHit, MaxRaycastDistance))
                {
                    //get displacement between what the camera is looking at and the pivot 
                    Vector3 displacement = transform.position - screenHit.point;
                    //move the pivot to where the camera is looking
                    transform.position = new Vector3(screenHit.point.x, screenHit.point.y, screenHit.point.z);
                    //keep the camera where it was before moving the pivot
                    Camera.main.transform.position += displacement;
                }

                //clamp pivot within bounds
                transform.position = CameraBounds.ClosestPoint(transform.position);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0.0f //zoom out
                && Vector3.Distance(Camera.main.transform.position, transform.position) < MaxZoomDistance) //zoom out if not fully zoomed out
            {
                //zoom out
                Camera.main.transform.position -= Camera.main.transform.forward * ZoomSpeed * Time.deltaTime;
            }

            //adjust zoom angle based on zoom amount
            transform.rotation = Quaternion.Euler(Mathf.Lerp(MinZoomAngle, MaxZoomAngle, (Vector3.Distance(Camera.main.transform.position, transform.position) - MinZoomDistance) / (MaxZoomDistance - MinZoomDistance)), transform.eulerAngles.y, 0.0f);
        }
    }

    private void PanCamera()
    {
        //move up
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += new Vector3(transform.forward.x, 0.0f, transform.forward.z).normalized * PanSpeed * Time.deltaTime;
        }

        //move down
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += new Vector3(-transform.forward.x, 0.0f, -transform.forward.z).normalized * PanSpeed * Time.deltaTime;
        }

        //move right
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += new Vector3(transform.right.x, 0.0f, transform.right.z).normalized * PanSpeed * Time.deltaTime;
        }

        //move left
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-transform.right.x, 0.0f, -transform.right.z).normalized * PanSpeed * Time.deltaTime;
        }

        //clamp pivot within bounds
        transform.position = CameraBounds.ClosestPoint(transform.position);
    }

    private void RotateCamera()
    {
        if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
        {
            //enable cursor when not rotating
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            //disable cursor when not rotating
            Cursor.lockState = CursorLockMode.Locked;

            //rotate
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + Input.GetAxis("Mouse X") * RotationSpeed * Time.deltaTime, transform.eulerAngles.z);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(CameraBounds.center, CameraBounds.size);
    }
}
