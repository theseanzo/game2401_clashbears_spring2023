using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<string, Stack<PoolObject>> nameToObjectStack = new Dictionary<string, Stack<PoolObject>>();
    private void Start()
    {
        PoolManager.Instance.Load();
        
    }
    // Start is called before the first frame update
    void Load()
    {

        PoolObject[] myPoolObjects = Resources.LoadAll<PoolObject>("PoolObjects");
        
        //loop through all prefabs in folder and create one
        foreach(PoolObject poolObjPrefab in myPoolObjects)
        {
            //we create a stack for each prefab
            Stack<PoolObject> objStack = new Stack<PoolObject>();

            objStack.Push(poolObjPrefab);
            nameToObjectStack.Add(poolObjPrefab.name, objStack);
        }
    }
    public PoolObject Spawn(string name)
    {
        Stack<PoolObject> objStack = nameToObjectStack[name];

        if(objStack.Count == 1) //this means there is only a prefab left)
        {
            PoolObject prefab = objStack.Peek();
            PoolObject clone = Instantiate(prefab);

            clone.name = name;
            return clone;
        }

        //this is where we recycle old objects
        PoolObject topObjectFromStack = objStack.Pop();
        topObjectFromStack.gameObject.SetActive(true);
        return topObjectFromStack;
    }
    public void DeSpawn(PoolObject cloneToDespawn)
    {
        //we use the name of our clone to get the correct stack
        Stack<PoolObject> objStack = nameToObjectStack[cloneToDespawn.name];

        //we then add the clone to this stack, so we can re-use it later
        objStack.Push(cloneToDespawn);

        //we then deactivate the object
        cloneToDespawn.gameObject.SetActive(false);
       
    }
    // Update is called once per frame
}
