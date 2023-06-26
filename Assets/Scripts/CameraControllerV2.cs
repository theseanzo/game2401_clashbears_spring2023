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
    [SerializeField][Range(0, 1000)] public float MinZoomHeight;
    [SerializeField][Range(0, 1000)] public float MaxZoomHeight;
    [SerializeField][Range(0, Mathf.Infinity)] public float MinZoomAngle;
    [SerializeField][Range(0, Mathf.Infinity)] public float MaxZoomAngle;
    [SerializeField][Range(0, 1)] public float StartingZoomAmount = 0.5f;
    [SerializeField] public float FullZoomTime;
    [SerializeField][Range(0, 360)] public float StartingYAngle;
    [SerializeField] public float RotationSpeed;
    [SerializeField] public float MaxRaycastDistance = 5000.0f;

    private float _currentZoomAmount;

    private void Start()
    {
        //get UI EventSystem
        _eventSystem = EventSystem.current;

        //init _currentZoomAmount
        _currentZoomAmount = StartingZoomAmount;
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
        if (Input.GetAxis("Mouse ScrollWheel") != 0.0f)
        {
            //get zoom focus point
            Vector3 zoomFocus;
            RaycastHit hit;
            if (Input.GetAxis("Mouse ScrollWheel") > 0.0f && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, MaxRaycastDistance))
            {
                //zoom in focus
                zoomFocus = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }
            else if (Physics.Raycast(Camera.main.ScreenPointToRay(Camera.main.ViewportToScreenPoint(Vector3.one / 2.0f)), out hit, MaxRaycastDistance))
            {
                //zoom out focus
                zoomFocus = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }
            else
            {
                //no object to focus on
                zoomFocus = transform.position;
            }

            //update zoom amount
            _currentZoomAmount += Input.GetAxis("Mouse ScrollWheel") * 1.0f / FullZoomTime * Time.deltaTime;

            //zoom in/out
            if (!(Input.GetAxis("Mouse ScrollWheel") > 0.0f && Camera.main.transform.position.y < MinZoomHeight) && !(Input.GetAxis("Mouse ScrollWheel") < 0.0f && Camera.main.transform.position.y > MaxZoomHeight))
            {
                ApplyZoom(zoomFocus);
                //Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, zoomFocus, Input.GetAxis("Mouse ScrollWheel") * FullZoomTime * Time.deltaTime);
            }
        }
    }

    private void ApplyZoom(Vector3 zoomFocus)
    {
        //move camera//////////////////////////////////
        Camera.main.transform.position = zoomFocus - transform.eulerAngles * Mathf.Lerp(MinZoomHeight / Mathf.Tan(MinZoomAngle), MaxZoomAngle / Mathf.Tan(MaxZoomAngle), _currentZoomAmount) + new Vector3(0.0f, Mathf.Lerp(MinZoomHeight, MaxZoomHeight, _currentZoomAmount), 0.0f);
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
        if (Vector3.Distance(transform.position, Camera.main.transform.position) < MinZoomHeight)
        {
            Camera.main.transform.position = zoomFocus - (Camera.main.transform.forward * MinZoomHeight);
        }
        else if (Vector3.Distance(transform.position, Camera.main.transform.position) > MaxZoomHeight)
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
