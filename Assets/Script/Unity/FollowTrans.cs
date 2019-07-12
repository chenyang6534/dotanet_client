using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTrans : MonoBehaviour {
    public Transform FollowTarget;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(FollowTarget == null)
        {
            return;
        }
        transform.position = FollowTarget.position;
        transform.rotation = FollowTarget.transform.rotation;

    }
}
