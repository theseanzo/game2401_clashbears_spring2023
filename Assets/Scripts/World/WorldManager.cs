using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : Singleton<WorldManager>
{
    public WorldData data;
    // Start is called before the first frame update
    void OnAwake()
    {
        
    }
    
    void Start()
    {
        Load(); //makes ure that we have instantiated the instance
    }
    private void Load()//this will be called by our start and will load the data
    {
        foreach(BuildingData bData in data.buildings)
        {
            Building buildingPrefab = Resources.Load<Building>("Buildings/" + bData.name); //we load the data of a specific name
            Building buildingClone = Instantiate(buildingPrefab); //clone that prefab
            buildingClone.name = bData.name; //give it a name and then a position
            buildingClone.transform.position = bData.position;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
