using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    Dictionary<string, Stack<PoolObject>> stackDictionary = new Dictionary<string, Stack<PoolObject>>();
    // Start is called before the first frame update
    void Start()
    {
        PoolManager.Instance.Load();
    }
    private void Load()
    {
        //we need to create stacks for every single PoolObject type
        PoolObject[] myPoolObjects = Resources.LoadAll<PoolObject>("PoolObjects");
        foreach (PoolObject poolObject in myPoolObjects)
        {
            //we create a stack for each prefab
            Stack<PoolObject> objStack = new Stack<PoolObject>();
            objStack.Push(poolObject);
            //every object has a name (here the name for the object would "Arrow" since it is, of course, an Arrow
            stackDictionary.Add(poolObject.name, objStack); //this creates a dictionary item where a particular pool object's name is used as a key to access that stack (i.e. the name "Arrow" is used to reference the stack of arrows)
        }
    }
    public PoolObject Spawn(string name) //our Spawn function takes in the name of an object to spawn and gives it back to whomever needs it
    {
        //if we have only have one object left, we don't want to pop this object off the stack
        Stack<PoolObject> objStack = stackDictionary[name]; //we access a dictionary's value by using it like an array but with a key
        if(objStack.Count == 1)
        {
            //how do we look at the top of a stack but not pop an item off?
            PoolObject myPrefab = objStack.Peek(); //peek to see what's on top, cloen it, and then return the clone
            PoolObject clone = Instantiate(myPrefab);
            clone.name = name;
            return clone;
        }
        PoolObject prefab = objStack.Pop(); //pop the item off the top otherwise
        prefab.gameObject.SetActive(true);//hey, come back to life
        return prefab;
    }
    public void DeSpawn(PoolObject poolObject)
    {
        //steps for despawning: 2 parts. 1) we set the object to be inactive, 2) we add it back to its stack
        Stack<PoolObject> objStack = stackDictionary[poolObject.name]; //find the stack by the name
        poolObject.gameObject.SetActive(false); //the game object it is associated with should no longer be seen
        objStack.Push(poolObject); //put it back on the stack
    }
    // Update is called once per frame

}
