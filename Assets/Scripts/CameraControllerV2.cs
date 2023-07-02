using DG.Tweening.Core.Easing;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraControllerV2 : MonoBehaviour
{
    private EventSystem _eventSystem;

    [SerializeField] public Bounds CameraBounds;
    [SerializeField][Range(0, 1000)] public float MinZoomDistance = 5.0f;
    [SerializeField][Range(0, 1000)] public float MaxZoomDistance = 50.0f;
    [SerializeField][Range(0, 90)] public float ZoomAngle = 45.0f;
    [SerializeField][Range(0, 1000)] public float HorizontalOffset = 5.0f;
    [SerializeField][Range(0, 1)] public float StartingZoomAmount = 0.5f;
    [SerializeField] public float ZoomSpeed;
    [SerializeField][Range(0, 360)] public float StartingYAngle;
    [SerializeField] public float RotationSpeed;
    [SerializeField] public float MaxRaycastDistance = 5000.0f;

    //private float _currentZoomAmount;

    private void Start()
    {
        //get UI EventSystem
        _eventSystem = EventSystem.current;

        //init _currentZoomAmount
        //_currentZoomAmount = StartingZoomAmount;

        //position camera based on initial zoom
        ApplyZoom();
    }

    private void Update()
    {
        MoveCamera();
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
        //only update camera zoom if zooming
        if (Input.GetAxis("Mouse ScrollWheel") != 0.0f)
        {
            //get zoom focus point
            RaycastHit screenHit;
            RaycastHit mouseHit;
            if (Input.GetAxis("Mouse ScrollWheel") > 0.0f && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out mouseHit, MaxRaycastDistance) && Physics.Raycast(Camera.main.ScreenPointToRay(Camera.main.ViewportToScreenPoint(Vector3.one / 2.0f)), out screenHit, MaxRaycastDistance))
            {
                //move towards mouseHit on zoom in
                //transform.position = new Vector3(Mathf.Lerp(transform.position.x, mouseHit.point.x, 0.5f), Mathf.Lerp(transform.position.y, mouseHit.point.y, 0.5f), Mathf.Lerp(transform.position.z, mouseHit.point.z, 0.5f));

                //move pivot
                transform.position = new Vector3(screenHit.point.x, screenHit.point.y, screenHit.point.z);
            }
            else if (Physics.Raycast(Camera.main.ScreenPointToRay(Camera.main.ViewportToScreenPoint(Vector3.one / 2.0f)), out screenHit, MaxRaycastDistance))
            {
                //zoom out focus
                transform.position = new Vector3(screenHit.point.x, screenHit.point.y, screenHit.point.z);
            }

            //clamp pivot within bounds
            transform.position = CameraBounds.ClosestPoint(transform.position);

            //update zoom amount
            //_currentZoomAmount += -Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * Time.deltaTime;

            //zoom in/out
            if (!(Input.GetAxis("Mouse ScrollWheel") > 0.0f && Camera.main.transform.position.y < MinZoomDistance) && !(Input.GetAxis("Mouse ScrollWheel") < 0.0f && Camera.main.transform.position.y > MaxZoomDistance))
            {
                ApplyZoom();
                //Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, zoomFocus, Input.GetAxis("Mouse ScrollWheel") * FullZoomTime * Time.deltaTime);
            }
        }
    }

    private void ApplyZoom()
    {
        //move camera
        //Camera.main.transform.position = transform.position - (transform.forward * HorizontalOffset) + new Vector3(0.0f, Mathf.Lerp(MinZoomDistance, MaxZoomDistance, _currentZoomAmount), 0.0f);
        Camera.main.transform.LookAt(transform.position);
    }

    private void PanCamera()
    {
        
    }

    private void RotateCamera()
    {
        
    }

    private void ClampCamera(Vector3 zoomFocus)
    {
        //clamp zoom
        if (Vector3.Distance(transform.position, Camera.main.transform.position) < MinZoomDistance)
        {
            Camera.main.transform.position = zoomFocus - (Camera.main.transform.forward * MinZoomDistance);
        }
        else if (Vector3.Distance(transform.position, Camera.main.transform.position) > MaxZoomDistance)
        {
            ////////todo clamp max
        }

        //clamp pan
        transform.position = CameraBounds.ClosestPoint(transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(CameraBounds.center, CameraBounds.size);
    }
}
