using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour
{
    //for our base objects, we need them to have some health, to take some damage, and we also need to be able to kill them
    public int health = 100;

    public virtual void OnHit(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            OnDie();//call our die function
        }
    }
    public virtual void OnDie()
    {
        Destroy(gameObject);//every monobehaviour has a gameObject associated with it
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
