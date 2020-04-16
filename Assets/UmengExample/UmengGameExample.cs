using UnityEngine;
using Umeng;
using System.Collections.Generic;


public class UmengGameExample : MonoBehaviour
{



    void Start()
    {



        GA.StartWithAppKeyAndChannelId("59892f08310c9307b60023d0", "umeng");


        GA.SetLogEnabled(Debug.isDebugBuild);






    }


    void OnGUI()
    {


        int x = 150;
        int y = 50;
        int w = 500;
        int h = 100;
        int d = 150;

        if (GUI.Button(new Rect(x, y, w, h), "StartLevel1"))
        {

            GA.StartLevel("level1");
        }
        y += d;
        if (GUI.Button(new Rect(x, y, w, h), "FinishLevel"))
        {

            GA.FinishLevel("level1");

        }
        y += d;
        if (GUI.Button(new Rect(x, y, w, h), "Bonus"))
        {

            GA.Bonus(10, GA.BonusSource.Source10);
        }
        y += d;
        if (GUI.Button(new Rect(x, y, w, h), "Pay"))
        {
            GA.Pay(19, GA.PaySource.Source10, 10);
        }
        y += d;
        if (GUI.Button(new Rect(x, y, w, h), "Event"))
        {
            GA.Event("event1");


        }
        y += d;
        if (GUI.Button(new Rect(x, y, w, h), "EventLabel"))
        {
            GA.Event("event1", "label");

        }
        y += d;



        if (GUI.Button(new Rect(x, y, w, h), "EventDict"))
        {
            var dict = new Dictionary<string, string>();
            dict.Add("key", "value");

            GA.Event("event1", dict);

        }
        y += d;




    //    var obj2 = new JSONObject();
    //    obj2["key1"] = 3;
    //    obj2["key2"] = true;
    //    obj2["key3"] = 10.0f;

    //    if (GUI.Button(new Rect(x, y, w, h), "RegisterPreProperties"))
    //    {      //        Analytics.RegisterPreProperties(obj2);
    //        Debug.Log("RegisterPreProperties");     //    }
    //    y += d;
    //    if (GUI.Button(new Rect(x, y, w, h), "GetPreProperties"))
    //    {
    //        Debug.Log("GetPreProperties:" + Analytics.GetPreProperties().ToString());



    //    }
    //    y += d;

    //    if (GUI.Button(new Rect(x, y, w, h), "ClearPreProperties"))     //    {      //        Analytics.ClearPreProperties();
    //        Debug.Log("ClearPreProperties");
    //    }
    //    y += d;
    //    if (GUI.Button(new Rect(x, y, w, h), "UnregisterSuperProperty"))
    //    {
    //        Analytics.UnregisterPreProperty("key3");
    //        Debug.Log("UnregisterPreProperty");

    //    }

    //    y += d;
    //    if (GUI.Button(new Rect(x, y, w, h), "SetFirstLaunchEvent"))
    //    {     //        Analytics.SetFirstLaunchEvent(new[] { "FirstLaunchEvent1", "FirstLaunchEvent2" });
    //        Debug.Log("SetFirstLaunchEvent");
    //    }
    //    y += d;


    //    if (GUI.Button(new Rect(x, y, w, h), "EventObject"))
    //    {
    //        var dict = new Dictionary<string, object>();
    //        dict.Add("key", 1);
    //        dict.Add("key2",true);
    //        dict.Add("key3", 2.0);

    //        Analytics.EventObject("EventObject", dict);
    //        Debug.Log("EventObject");

    //    }



    }


}


