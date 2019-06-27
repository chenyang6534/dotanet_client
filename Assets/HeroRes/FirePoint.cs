using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePoint : MonoBehaviour {
    public GameObject firepiontObj;
	// Use this for initialization
	void Start ()
    {
		gameObject.GetComponent<Animator>().SetInteger("AniState", 3);
    }
	
	// Update is called once per frame
	void Update () {
		if(firepiontObj != null)
        {
            var time = gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime;
            Debug.Log("time:" + time+" pos:"+ firepiontObj.transform.position);
        }
	}
}
