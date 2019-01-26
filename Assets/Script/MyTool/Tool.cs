using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool {

	public static double GetTime()
    {
        //var t1 = (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
        var t1 = Time.realtimeSinceStartup;
        return t1;
    }
}
