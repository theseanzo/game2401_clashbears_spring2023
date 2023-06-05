using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="WorldData", menuName ="ScriptableObjects/CreateWorldData", order =50)]
public class WorldData : ScriptableObject
{
    public List<BuildingData> buildings;
}

[System.Serializable] //this allows for building data to be serialized (aka stored and displayed in the inspector)
public struct BuildingData
{
    public string name;
    public Vector3 position;
}
