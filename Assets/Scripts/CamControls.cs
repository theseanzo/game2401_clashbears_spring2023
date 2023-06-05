using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControls : MonoBehaviour
{
    public Bounds bounds;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            float mouseXMovement = Input.GetAxis("Mouse X");
            float mouseYMovement = Input.GetAxis("Mouse Y");
            this.transform.Translate(-mouseXMovement, 0, mouseYMovement);
            transform.position = bounds.ClosestPoint(transform.position);//this clamps our position to be the closest point to our boundary
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
