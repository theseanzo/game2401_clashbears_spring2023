using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildScreen : MonoBehaviour
{
    public DynamicList buildingButtons; //we specify the dynamic list we are going to use

    //we want this to populate a list of building buttons
    //so we need to start by finding all of our buildings
    void Start()
    {
        GameObject[] buildings = Resources.LoadAll<GameObject>("Buildings"); //we grab all of the gameobjects from our buildings folder (which is, coincidentally, all of our buildings)
        buildingButtons.CreateButtons(buildings); //when our build screen starts, it will attempt to populate the list of buttons 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnBuildButton(ListButton button)
    {
        BuildingManager.Instance.SetCurrent(button.linkedObject.GetComponent<Building>()); //since our linked buttons will have a building associated with their game object, we simply grab that building.
    }
}
