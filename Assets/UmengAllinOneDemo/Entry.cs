using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Umeng;

public class Entry : MonoBehaviour {

	void OnGUI() {


		if (GUI.Button (new Rect (150, 100, 500, 100), "AnalyticsEntry")) {

			Application.LoadLevel ("AnalyticsEntry");


		}

		if (GUI.Button (new Rect (150, 300, 500, 100), "Push")) {
			
			Application.LoadLevel ("PushDemo");


		}

		if (GUI.Button (new Rect (150, 500, 500, 100), "Social")) {
			
			Application.LoadLevel ("SocialDemo");

		}
	}
}
