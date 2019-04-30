using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cocosocket4unity;
public class GameUI : MonoBehaviour {


    private GComponent mainUI;
    private GTextField gPing;
    private Joystick joystick;

    private Btnstick attackstick;
    // Use this for initialization
    void Start () {
        mainUI = GetComponent<UIPanel>().ui;

        //Debug.Log("ui pos:"+(new Vector2(mainUI.GetChild("n1").x, mainUI.GetChild("n1").y)));

        gPing = mainUI.GetChild("n3").asTextField;
        //mainUI.GetChild("n1").SetXY(mainUI.GetChild("n1").x + 100, mainUI.GetChild("n1").y - 100);
        joystick = new Joystick(mainUI.GetChild("n1").asCom);
        joystick.onMove.Add(JoystickMove);
        joystick.onEnd.Add(JoystickEnd);

        attackstick = new Btnstick(mainUI.GetChild("n5").asCom);
        attackstick.onMove.Add(AttackstickMove);
        attackstick.onEnd.Add(AttackstickEnd);
        attackstick.onDown.Add(AttackstickDown);
        
    }

    //1:down 2:move 3:end
    private void AttackstickMove(EventContext context)
    {
        Vector2 dir = (Vector2)context.data;
        dir.y = -dir.y;
        Debug.Log("AttackstickMove:"+ dir);
        GameScene.Singleton.PressAttackBtn(2, dir);
        //GameScene.Singleton.SendControlData(degree, true);


    }
    private void AttackstickDown(EventContext context)
    {
        Debug.Log("AttackstickDown");

        GameScene.Singleton.PressAttackBtn(1,Vector2.zero);

        //GameScene.Singleton.SendControlData(degree, false);

    }
    private void AttackstickEnd(EventContext context)
    {
        float degree = (float)context.data;
        Debug.Log("AttackstickEnd");

        GameScene.Singleton.PressAttackBtn(3,Vector2.zero);

        //GameScene.Singleton.SendControlData(degree, false);

    }


    private void JoystickMove(EventContext context)
    {
        float degree = (float)context.data;

        GameScene.Singleton.SendControlData(degree, true);


    }
    private void JoystickEnd(EventContext context)
    {
        float degree = (float)context.data;
        Debug.Log("JoystickEnd");

        GameScene.Singleton.SendControlData(degree, false);

    }

    // Update is called once per frame
    void Update () {
        gPing.text = "" + MyKcp.PingValue;

    }
}
