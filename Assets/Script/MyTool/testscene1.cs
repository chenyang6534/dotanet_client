using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testscene1 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Debug.Log("update Input.touchCount:" + Input.touchCount);
        }
       
    }
    void LateUpdate()
    {
        if (Input.touchCount > 0)
        {
            Debug.Log("lateupdate Input.touchCount:" + Input.touchCount);
        }
    }
}
