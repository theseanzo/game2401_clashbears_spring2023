using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class CamControls : MonoBehaviour
{
    public Bounds bounds;
    public Camera cam;
    public float fovMax = 60;
    public float fovMin = 30;
    public float scrollSpeed = 20;

    public float moveSpeed = 0.5f;

    public EventSystem eventSys;

    // Start is called before the first frame update
    void Start()
    {
        eventSys = GameObject.Find("EventSystem").GetComponent<EventSystem>(); //I am so glad this is so simple.
    }

    // Update is called once per frame
    void Update()
    {
        //moving out of the mouse movement if so I can use it for rotation too
        float mouseXMovement = Input.GetAxis("Mouse X");
        float mouseYMovement = Input.GetAxis("Mouse Y");

        #region Move Cam w/ Mouse

        if (Input.GetMouseButton(0) && !eventSys.IsPointerOverGameObject())
        {
            transform.Translate(-mouseXMovement, 0, -mouseYMovement); //...I hope it's okay if I inverse the y movement, too. It feels werid for me. This is my own branch, anywho...
        }

        #endregion

        #region Rotate Cam w/ Mouse

        if (!Input.GetMouseButton(0) && Input.GetMouseButton(1) && !eventSys.IsPointerOverGameObject()) //rotating and moving at the same time is weird. Just use the WASD movement if you really have to
        {
            transform.Rotate(Vector3.up * mouseXMovement);
        }

        #endregion

        #region Move Cam w/ WASD

        float xMovement = Input.GetAxis("Horizontal");
        float yMovement = Input.GetAxis("Vertical");
        transform.Translate(xMovement * moveSpeed, 0, yMovement * moveSpeed);

        #endregion

        #region Cam Zoom

        float mouseScroll = -Input.GetAxis("Mouse ScrollWheel"); //Need to inverse it!
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, Mathf.Clamp(cam.fieldOfView, fovMin, fovMax) + mouseScroll * scrollSpeed, 0.25f);
        //Fixing what would be some jitter on the cam at the min/max with a lerp... now it just looks like you're overzooming and popping back to default. Devious......

        #endregion

        #region Rotate Cam Pitch w/ Zoom

        //I don't get why this is necessary...? It just feels weird and awkward to do. I feel like I'm misunderstanding this part of the assignment...
        cam.transform.localRotation = Quaternion.Euler(cam.fieldOfView, 0, 0); //rotating just the camera because it's easy and I don't have to cope with the rotation already going on in the parent object

        #endregion

        transform.position = bounds.ClosestPoint(transform.position); //this clamps our position to be the closest point to our boundary
        //moving to the end so I don't have to double-update for both the mouse and wasd movement.
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
