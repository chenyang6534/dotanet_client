using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject HKMJFBX = (GameObject)Instantiate(Resources.Load("Map/set_5v5"));

        GameObject hero = (GameObject)Instantiate(Resources.Load("Hero/hero1"));

        hero.transform.position = new Vector3(-28.56f, 0, -46.09f);

        //GameObject hero1 = (GameObject)Instantiate(Resources.Load("Hero/hero1"));

        //hero1.transform.position = new Vector3(-27.09f, 0, -45.81f);

        //GameObject hero2 = (GameObject)Instantiate(Resources.Load("Hero/hero1"));

        //hero2.transform.position = new Vector3(-25.22f, 0, -55.63f);
        //GameObject hero3 = (GameObject)Instantiate(Resources.Load("Hero/hero1"));

        //hero3.transform.position = new Vector3(-26.69f, 0, -55.91f);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
