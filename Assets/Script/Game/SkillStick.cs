using DG.Tweening;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skillstick : EventDispatcher
{
    //事件的监听者
    //public EventListener onMove { get; private set; }//设置了一个安全权限
    //public EventListener onEnd { get; private set; }
    //public EventListener onDown { get; private set; }//设置了一个安全权限

    //UI里的对象
    //private GButton joystickButton;
    private GObject thumb;
    private GObject touchArea;
    private GObject center;
    private GObject no;
    private GObject my;
    private bool isNo;
    private bool isMy;

    private bool showmy;
    public Vector2 nopos = new Vector2(80,-300);
    public Vector2 mypos = new Vector2(0, -300);

    //
    private GObject m_ui;

    //摇杆的属性
    private Vector2 initPos;
    private Vector2 startStagePos;
    private Vector2 lastStagePos;
    private int touchID;
    public int radius { get; set; }
    public float m_MoveCallNeedDir; //move回调需要移动的距离
    public bool m_IsMoved;//是否已经开始移动
    private Tweener tweener;

    protected Protomsg.SkillDatas m_SkillDatas;
    public Protomsg.SkillDatas SkillDatas
    {
        get
        {
            return m_SkillDatas;
        }
        set
        {
            m_SkillDatas = value;
            //Debug.Log("m_SkillDatas:"+ m_SkillDatas);
            FreshData();
        }
    }
    //刷新数据
    protected void FreshData()
    {

        //被动技能缩小图标
        if (m_SkillDatas.CastType != 1)
        {
            m_ui.scale = new Vector2(0.8f,0.8f);
            touchArea.asCom.touchable = false;
        }
        else
        {
            m_ui.scale = new Vector2(1.0f, 1.0f);
            touchArea.asCom.touchable = true;
        }
        //对目标施法 且能对所有单位包括自己施法
        if(m_SkillDatas.CastTargetType == 2 && m_SkillDatas.UnitTargetTeam == 3)
        {
            showmy = true;
        }

        //耗蓝
        if (m_SkillDatas.ManaCost <= 0)
        {
            touchArea.asCom.GetChild("manacost").asTextField.text = "";
        }
        else
        {
            var t3 = touchArea.asCom.GetChild("manacost").asLabel;
            touchArea.asCom.GetChild("manacost").asTextField.text = "10";
            touchArea.asCom.GetChild("manacost").asTextField.text = m_SkillDatas.ManaCost+"";
        }
        //cd
        if (m_SkillDatas.RemainCDTime <= 0)
        {
            touchArea.asCom.GetChild("cdtime").asTextField.text = "";
        }
        else
        {
            touchArea.asCom.GetChild("cdtime").asTextField.text = m_SkillDatas.RemainCDTime.ToString("F1");
        }
        touchArea.asCom.GetChild("progress").asProgress.value = (int)((float)m_SkillDatas.RemainCDTime / m_SkillDatas.Cooldown * 100);

        if (m_SkillDatas.CastType == 1 && m_SkillDatas.CastTargetType == 4 && m_SkillDatas.AttackAutoActive == 1)
        {
            touchArea.asCom.GetChild("active").visible = true;
        }
        else
        {
            touchArea.asCom.GetChild("active").visible = false;
        }

    }

    public Skillstick(GComponent UI)
    {
        //onMove = new EventListener(this, "onMove");
        //onEnd = new EventListener(this, "onEnd");
        //onDown = new EventListener(this, "onDown");


        thumb = UI.GetChild("thumb");
        thumb.visible = false;
        touchArea = UI.GetChild("toucharea");
        center = UI.GetChild("centerpic");
        center.visible = false;
        no = UI.GetChild("no");
        my = UI.GetChild("my");
        no.visible = false;
        my.visible = false;
        showmy = false;

        m_ui = UI;

        initPos = new Vector2(center.x, center.y);


        touchID = -1;
        radius = 100;
        m_MoveCallNeedDir = 20;//
        m_IsMoved = false;

        touchArea.onTouchBegin.Add(OnTouchBegin);
        touchArea.onTouchMove.Add(OnTouchMove);
        touchArea.onTouchEnd.Add(OnTouchEnd);

        Debug.Log("touchArea:" + touchArea);

    }
    protected void DoTouchDown(Vector2 pos)
    {


        GameScene.Singleton.PressSkillBtn(1, pos,m_SkillDatas,false,false);
        Debug.Log("Skill DoTouchDown:" + pos);
    }
    protected void DoTouchMove(Vector2 pos,float len)
    {
        if(m_SkillDatas.CastTargetType == 5)
        {
            len = 1;
        }
        Debug.Log("Skill DoTouchMove:" + pos);
        pos.y = -pos.y;
        
        GameScene.Singleton.PressSkillBtn(2, pos.normalized * (m_SkillDatas.CastRange * len), m_SkillDatas,false,false);
    }
    protected void DoTouchEnd(Vector2 pos, float len,bool isno,bool ismy)
    {
        if (m_SkillDatas.CastTargetType == 5)
        {
            len = 1;
        }
        Debug.Log("Skill DoTouchEnd:" + pos+"   "+ (pos * (m_SkillDatas.CastRange * len)));
        pos.y = -pos.y;
        GameScene.Singleton.PressSkillBtn(3, pos.normalized * (m_SkillDatas.CastRange * len), m_SkillDatas,isno,ismy);
    }

    //开始触摸
    private void OnTouchBegin(EventContext context)
    {
        Debug.Log("OnTouchBegin");
        if (touchID == -1)//第一次触摸
        {
            InputEvent inputEvent = (InputEvent)context.data;
            touchID = inputEvent.touchId;

            if (tweener != null)
            {
                tweener.Kill();//杀死上一个动画
                tweener = null;
            }
            isNo = false;
            isMy = false;

            Vector2 localPos = m_ui.GlobalToLocal(inputEvent.position);


            lastStagePos = localPos;
            startStagePos = localPos;

            no.SetXY(localPos.x + nopos.x, localPos.y + nopos.y);
            my.SetXY(localPos.x + mypos.x, localPos.y + mypos.y);
            no.visible = true;
            if (showmy)
            {
                my.visible = true;
            }
            //private GObject no;showmy
            //public Vector2 nopos = new Vector2(120, -200);


            center.visible = true;
            thumb.visible = true;
            center.SetXY(localPos.x, localPos.y);
            thumb.SetXY(localPos.x, localPos.y);
            context.CaptureTouch();
            //onDown.Call(1);
            DoTouchDown(localPos);
            m_IsMoved = false;


        }
    }

    public void SetTouchSize(int width, int height)
    {
        touchArea.SetSize(width, height);
    }

    //移动触摸
    private void OnTouchMove(EventContext context)
    {
        InputEvent inputEvent = (InputEvent)context.data;
        if (touchID != -1 && inputEvent.touchId == touchID)
        {
            //Vector2 localPos = GRoot.inst.GlobalToLocal(new Vector2(inputEvent.x, inputEvent.y));
            Vector2 localPos = m_ui.GlobalToLocal(inputEvent.position);

            var dir = (localPos - startStagePos);
            //点击到 取消施法
            if (Vector2.Distance(localPos, no.xy) < 30)
            {
                no.scale = new Vector2(1.5f, 1.5f);
                no.asTextField.color = new Color(1, 0, 0);
                isNo = true;
            }
            else
            {
                no.scale = new Vector2(1.0f, 1.0f);
                no.asTextField.color = new Color(1, 1, 1);
                isNo = false;
            }
            //点击到 施法自己
            if (showmy)
            {
                if (Vector2.Distance(localPos, my.xy) < 30)
                {
                    my.scale = new Vector2(1.5f, 1.5f);
                    my.asTextField.color = new Color(1, 0, 0);
                    isMy = true;
                }
                else
                {
                    my.scale = new Vector2(1.0f, 1.0f);
                    my.asTextField.color = new Color(1, 1, 1);
                    isMy = false;
                }
            }
            

            var len = Vector2.Distance(dir, new Vector2(0, 0));
            if (len > radius)
            {
                len = radius;
            }
            var dest = dir.normalized * len + startStagePos;


            thumb.SetXY(dest.x, dest.y);

            thumb.rotation = Vector2.SignedAngle(new Vector2(0, -1), dir);

            if (len >= m_MoveCallNeedDir)
            {
                m_IsMoved = true;

            }
            if (m_IsMoved)
            {
                //onMove.Call(dir);
                DoTouchMove(dir,len/ radius);
            }

        }
    }

    //结束触摸
    private void OnTouchEnd(EventContext context)
    {
        InputEvent inputEvent = (InputEvent)context.data;
        if (touchID != -1 && inputEvent.touchId == touchID)
        {
            touchID = -1;
            center.visible = false;
            thumb.visible = false;
            no.visible = false;
            my.visible = false;
            no.scale = new Vector2(1.0f, 1.0f);
            my.scale = new Vector2(1.0f, 1.0f);
            no.asTextField.color = new Color(1, 1, 1);
            my.asTextField.color = new Color(1, 1, 1);

            Vector2 localPos = m_ui.GlobalToLocal(inputEvent.position);

            var dir = (localPos - startStagePos);
            var len = Vector2.Distance(dir, new Vector2(0, 0));
            if (len > radius)
            {
                len = radius;
            }

            //DoTouchEnd(Vector2.SignedAngle(new Vector2(0, 1), new Vector2(dir.x, -dir.y)));
            //isMy = false;
            DoTouchEnd(dir, len / radius,isNo,isMy);

        }

    }


}
