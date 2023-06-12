using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ListButton : MonoBehaviour
{
    //this is going to be associated with a game object and essentially it will be initialized and clicked
    // Start is called before the first frame update
    [HideInInspector]
    public GameObject linkedObject; //each list button is linked to an object

    public void Init(GameObject obj)
    {
        linkedObject = obj;
        //we will be using our linkedObject's name to attach to the button (hence it being dynamic)
        GetComponentInChildren<TMP_Text>().text = linkedObject.name;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
