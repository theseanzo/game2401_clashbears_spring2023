using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicList : MonoBehaviour
{
    public ListButton buttonOriginal; //the original button that we will clone for all of our buttons
    public void CreateButtons(GameObject[] objs) //to create the list of buttons we iterate over each and every button: clone them, set them to active (so that they show up), and initialize them (i.e. associate them with a particular game object. For example, a barracks button will be given a barracks object)
    {
        foreach(GameObject obj in objs)
        {
            ListButton buttonClone = Instantiate(buttonOriginal, buttonOriginal.transform.parent);
            buttonClone.gameObject.SetActive(true);
            buttonClone.Init(obj);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        buttonOriginal.gameObject.SetActive(false); //hide the original button so it doesn't show up (this is also why we setActive to true in all the clones since the original is hidden
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
