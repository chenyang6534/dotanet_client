using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMainCamera : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        
        //var pos = Camera.main.transform.position;
        //this.transform.LookAt(pos);
        //this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
        //    Quaternion.LookRotation(Camera.main.transform.position - this.transform.position), 0);
    }
    // Update is called once per frame
    void Update()
    {
        this.transform.rotation = Camera.main.transform.rotation;
    }

    void LateUpdate()
    {
        //this.transform.rotation = Camera.main.transform.rotation;
    }

    
}