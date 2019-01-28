using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cocosocket4unity;
public class GameUI : MonoBehaviour {


    private GComponent mainUI;
    private GTextField gPing;
    private Joystick joystick;
    // Use this for initialization
    void Start () {
        mainUI = GetComponent<UIPanel>().ui;

        //Debug.Log("ui pos:"+(new Vector2(mainUI.GetChild("n1").x, mainUI.GetChild("n1").y)));

        gPing = mainUI.GetChild("n3").asTextField;
        mainUI.GetChild("n1").SetXY(mainUI.GetChild("n1").x + 100, mainUI.GetChild("n1").y - 100);
        joystick = new Joystick(mainUI.GetChild("n1").asCom);
        joystick.onMove.Add(JoystickMove);

        //joystick.SetTouchSize(100, 100);
        joystick.onEnd.Add(JoystickEnd);
    }
    private void JoystickMove(EventContext context)
    {
        float degree = (float)context.data;
        Debug.Log(Tool.Vec2Rotate(new Vector2(0, 1), degree).ToString());
        //gPing.text = Tool.Vec2Rotate(new Vector2(0,1), degree).ToString();
        
    }
    private void JoystickEnd(EventContext context)
    {
        float degree = (float)context.data;
        Debug.Log("JoystickEnd");

    }

    // Update is called once per frame
    void Update () {
        gPing.text = "" + MyKcp.PingValue;

    }
}
