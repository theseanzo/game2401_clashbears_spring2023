using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingManager : Singleton<BuildingManager>
{
    //our building manager needs to keep track of the current building that we have selected, as well it needs to have a reference to the grid  in order to get the grid to show up when we have selected a building
    public Building current;// our current building
    public Renderer grid;
    [SerializeField]
    float gridFadeSpeed;

    float gridAlpha; //keep track of the alpha value for the transparency for our grid
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //we are going to start by creating a Ray object, from ouyr mouse position pointing into the world
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        //if we are currently placing a building do the following
        if(current != null) 
        {
            if(Physics.Raycast(mouseRay, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Terrain"))) //if we hit the terrain, continue
            {
                //we want to set our building to be where the ray hits our terrain
                //if we just set the building position to the current hit position, would this accomplish snapping?
                //we won't use current.transform.position
                Vector3 pos = hitInfo.point;
                pos.x = Mathf.Round(pos.x);
                pos.y = current.transform.position.y;
                pos.z = Mathf.Round(pos.z);
                current.transform.position = pos;
            }
            //slowly transition to 1
            gridAlpha = Mathf.MoveTowards(gridAlpha, 1, Time.deltaTime * gridFadeSpeed);
        }
        else
        {
            gridAlpha = Mathf.MoveTowards(gridAlpha, 0, Time.deltaTime * gridFadeSpeed);
        }
        grid.material.SetColor("_LineColor", new Color(1, 1, 1, gridAlpha));
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (Input.GetMouseButtonDown(0))
        {
            //check if we are currently not placing a building
            if(current == null)
            {
                //let's see if we hit any buildings
                if(Physics.Raycast(mouseRay, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Building")))
                {
                    current = hitInfo.collider.GetComponent<Building>(); //if we hit a building, let's select it
                }
            }
            else
            {
                if (!current.isOverlapping) //if we are not overlapping, then drop it
                {
                    current = null;
                }
            }
        }
    }
}
