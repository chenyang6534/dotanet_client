using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cocosocket4unity;
public class GameUI : MonoBehaviour {


    private GComponent mainUI;
    private GTextField gPing;
    private Joystick joystick;
    private int touchID;
    private Vector2 startPos;
    private double startTime;

    private Btnstick attackstick;
    // Use this for initialization
    void Start () {
        mainUI = GetComponent<UIPanel>().ui;
        touchID = -1;
        startTime = Tool.GetTime();
        GObject touch = mainUI.GetChild("touchArea");
        touch.onTouchBegin.Add(OnTouchBegin);
        touch.onTouchMove.Add(OnTouchMove);
        touch.onTouchEnd.Add(OnTouchEnd);

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
    //屏幕点击
    private void OnTouchBegin(EventContext context)
    {
        
        if (touchID == -1)//第一次触摸
        {
            InputEvent inputEvent = (InputEvent)context.data;
            touchID = inputEvent.touchId;
            //Debug.Log("OnTouchBegin111:" + inputEvent.touchId);
            startTime = Tool.GetTime();

            startPos = GRoot.inst.LocalToGlobal(inputEvent.position);
            startPos.y = Screen.height - startPos.y;
            //startStagePos = localPos;

            context.CaptureTouch();
            //Vector2 screenPos = GRoot.inst.LocalToGlobal(localPos);
            ////原点位置转换
            //screenPos.y = Screen.height - screenPos.y;
            ////一般情况下，还需要提供距离摄像机视野正前方distance长度的参数作为screenPos.z(如果需要，将screenPos改为Vector3类型）
            //Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

            //Debug.Log("touchpos11111111111:" + inputEvent.position);
            //Debug.Log("OnTouchBegin11111111111:"+ startPos);

            //m_IsMoved = false;
        }
    }

    //移动触摸
    private void OnTouchMove(EventContext context)
    {
        InputEvent inputEvent = (InputEvent)context.data;
        //Debug.Log("OnTouchMove111:" + inputEvent.touchId);
        if (touchID != -1 && inputEvent.touchId == touchID)
        {
            //Vector2 localPos = GRoot.inst.GlobalToLocal(new Vector2(inputEvent.x, inputEvent.y));
            Vector2 localPos = GRoot.inst.LocalToGlobal(inputEvent.position);
            localPos.y = Screen.height - localPos.y;

            var dir = (localPos - startPos);
            

            DHCameraManager.Singleton.SetTempCameraMoveOffset(dir);
        }
    }

    //结束触摸
    private void OnTouchEnd(EventContext context)
    {
        InputEvent inputEvent = (InputEvent)context.data;
        //Debug.Log("OnTouchEnd111:" + inputEvent.touchId);
        if (touchID != -1 && inputEvent.touchId == touchID)
        {
            touchID = -1;
            //Debug.Log("OnTouchEnd:" + inputEvent.position);
            Vector2 localPos = GRoot.inst.LocalToGlobal(inputEvent.position);
            localPos.y = Screen.height - localPos.y;
            var dir = (localPos - startPos);

            DHCameraManager.Singleton.AddCameraMoveOffset(dir);

            //点击距离范围
            var len = Vector2.Distance(dir, new Vector2(0, 0));
            if (Tool.GetTime()-startTime <= 0.2 && len <= 10)
            {
                Vector2 clickpos = inputEvent.position;
                clickpos.y = Screen.height - clickpos.y;
                GameScene.Singleton.ClickPos(clickpos);
            }
        }

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
