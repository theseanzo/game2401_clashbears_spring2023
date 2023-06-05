using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temple : Building
{
   
    Light templeRadiance;
    float intensityLevel;


    // Start is called before the first frame update
    void Start()
    {
  
        templeRadiance = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {


        
    }
}
