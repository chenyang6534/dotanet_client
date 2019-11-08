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
    private GButton m_upgrade;

    //长按显示详情
    protected bool IsShowInfo;
    protected double TouchBeginTime;

    //摇杆的属性
    private Vector2 initPos;
    private Vector2 startStagePos;
    private Vector2 lastStagePos;
    private int touchID;
    public int radius { get; set; }
    public float m_MoveCallNeedDir; //move回调需要移动的距离
    public bool m_IsMoved;//是否已经开始移动
    private Tweener tweener;

    private List<GComponent>  LevelShow;

    protected Protomsg.SkillDatas m_SkillDatas;
    protected bool m_IsItem;

    TimerCallback timeCallBack;
    InputEvent moveEventContext;
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
    //设置位置
    public void SetXY(Vector2 xy)
    {
        if(m_ui != null)
        {
            m_ui.xy = xy;
        }
    }
    //
    public Vector2[] ThreeSkillLevelPos = {
        new Vector2(30,115),
        new Vector2(50,120),
        new Vector2(70, 115) };
    public Vector2[] FourSkillLevelPos = {
        new Vector2(12.5f,108),
        new Vector2(37.5f, 117.5f),
        new Vector2(62.5f, 117.5f),
        new Vector2(87.5f, 108)};


    public Vector2[] UpgradeSkillBtnPos = {
        new Vector2(-13,0),
        new Vector2(0, -16),
        new Vector2(24, -28),
        new Vector2(50, -31)};

    //刷新数据
    protected void FreshData()
    {
        //显示
        if(m_SkillDatas.Visible == 1)
        {
            m_ui.visible = true;
        }
        else
        {
            m_ui.visible = false;
        }
        //
        if(m_SkillDatas.RemainSkillCount <= 1)
        {
            touchArea.asCom.GetChild("skillcount").asTextField.text = "";
        }
        else
        {
            touchArea.asCom.GetChild("skillcount").asTextField.text = ""+ m_SkillDatas.RemainSkillCount;
        }
        //图标
        var clientskill = ExcelManager.Instance.GetSkillManager().GetSkillByID(m_SkillDatas.TypeID);
        if (clientskill != null)
        {
            if(clientskill.IconPath.Length > 0)
            {
                touchArea.asCom.GetChild("icon").asLoader.url = clientskill.IconPath;
            }
            
        }


        //被动技能缩小图标
        if (m_SkillDatas.CastType != 1)
        {
            m_ui.scale = new Vector2(0.7f,0.7f);
            //touchArea.asCom.touchable = false;
        }
        else
        {
            m_ui.scale = new Vector2(0.85f, 0.85f);
            touchArea.asCom.touchable = true;
        }
        //道具
        if(m_IsItem == true)
        {
            m_ui.scale = new Vector2(0.7f, 0.7f);
            m_upgrade.visible = false;
        }
        else
        {
            //显示升级按钮
            m_upgrade.visible = false;
            if (GameScene.Singleton.m_MyMainUnit != null)
            {
                var alllevel = GameScene.Singleton.m_MyMainUnit.GetAllSkillLevel();
                if(alllevel < GameScene.Singleton.m_MyMainUnit.Level)
                {
                    //显示升级按钮
                    var nextlevel_needunitlevel = m_SkillDatas.RequiredLevel + m_SkillDatas.LevelsBetweenUpgrades * m_SkillDatas.Level;
                    if (nextlevel_needunitlevel <= GameScene.Singleton.m_MyMainUnit.Level && m_SkillDatas.Level < m_SkillDatas.MaxLevel)
                    {
                        m_upgrade.visible = true;
                        m_upgrade.SetXY(UpgradeSkillBtnPos[m_SkillDatas.Index].x, UpgradeSkillBtnPos[m_SkillDatas.Index].y);
                        
                        
                    }
                }
            }
            


            //等级显示
            //创建
            if(m_SkillDatas.InitLevel == 0)
            {
                var createcount = m_SkillDatas.MaxLevel - LevelShow.Count;
                for (var i = 0; i < createcount; i++)
                {
                    GComponent view = UIPackage.CreateObject("GameUI", "SkillLevel").asCom;
                    m_ui.asCom.AddChild(view);
                    LevelShow.Add(view);
                }
                //刷新信息
                int index = 0;
                foreach (var item in LevelShow)
                {
                    if (m_SkillDatas.MaxLevel == 4)
                    {
                        item.SetXY(FourSkillLevelPos[index].x, FourSkillLevelPos[index].y);
                    }
                    else if (m_SkillDatas.MaxLevel == 3)
                    {
                        item.SetXY(ThreeSkillLevelPos[index].x, ThreeSkillLevelPos[index].y);
                    }

                    if (m_SkillDatas.Level > index)
                    {
                        item.GetChild("level").visible = true;
                    }
                    else
                    {
                        item.GetChild("level").visible = false;
                    }
                    index++;


                }
            }
            
        }

        //0级 图标变淡
        if(m_SkillDatas.Level <= 0)
        {
            touchArea.asCom.alpha = 0.2f;
        }
        else
        {
            touchArea.asCom.alpha = 1;
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
        if (m_SkillDatas.RemainSkillCount > 0 || m_SkillDatas.RemainCDTime <= 0)
        {
            touchArea.asCom.GetChild("cdtime").asTextField.text = "";
            touchArea.asCom.GetChild("progress").asProgress.value = 0;
        }
        else
        {
            touchArea.asCom.GetChild("cdtime").asTextField.text = m_SkillDatas.RemainCDTime.ToString("F1");
            touchArea.asCom.GetChild("progress").asProgress.value = (int)((float)m_SkillDatas.RemainCDTime / m_SkillDatas.Cooldown * 100);
        }
        

        if (m_SkillDatas.CastType == 1 && m_SkillDatas.CastTargetType == 4 && m_SkillDatas.AttackAutoActive == 1)
        {
            touchArea.asCom.GetChild("active").visible = true;
        }
        else
        {
            touchArea.asCom.GetChild("active").visible = false;
        }


        //
        touchArea.asCom.GetChild("mana_not").visible = false;
        touchArea.asCom.GetChild("use_not").visible = false;
        if (GameScene.Singleton.m_MyMainUnit != null)
        {
            if (m_IsItem == false && GameScene.Singleton.m_MyMainUnit.SkillEnable == 2)
            {
                //能否使用主动技能 (比如 被眩晕和沉默不能使用主动技能) 1:可以 2:不可以
                touchArea.asCom.GetChild("use_not").visible = true;
            }
            else if(m_IsItem == true && GameScene.Singleton.m_MyMainUnit.ItemEnable == 2)
            {
                touchArea.asCom.GetChild("use_not").visible = true;
            }
        }
       

    }
    public void Destroy()
    {
        if(m_ui != null)
        {
            m_ui.Dispose();
            m_ui = null;
            thumb = null;
            touchArea = null;
            center = null;
            no = null;
            my = null;
            if(timeCallBack != null)
            {
                Timers.inst.Remove(timeCallBack);
                timeCallBack = null;
            }
        }
    }

    public Skillstick(GComponent UI,bool isItem)
    {
        //onMove = new EventListener(this, "onMove");
        //onEnd = new EventListener(this, "onEnd");
        //onDown = new EventListener(this, "onDown");
        m_IsItem = isItem;

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
        radius = 125;
        m_MoveCallNeedDir = 20;//
        m_IsMoved = false;

        LevelShow = new List<GComponent>();

        touchArea.onTouchBegin.Add(OnTouchBegin);
        touchArea.onTouchMove.Add(OnTouchMove);
        touchArea.onTouchEnd.Add(OnTouchEnd);
        timeCallBack = (p1) =>
        {
            this.TimerUpdate();
        };
        Timers.inst.AddUpdate(timeCallBack);

        Debug.Log("touchArea:" + touchArea);

        m_upgrade = m_ui.asCom.GetChild("upgrade").asButton;
        m_upgrade.onClick.Add(() =>
        {
            //升级技能
            Debug.Log("-------升级技能-------:"+ m_SkillDatas.TypeID);
            Protomsg.CS_PlayerUpgradeSkill msg4 = new Protomsg.CS_PlayerUpgradeSkill();
            msg4.TypeID = m_SkillDatas.TypeID;
            cocosocket4unity.MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_PlayerUpgradeSkill", msg4);
        });

    }
    protected void DoTouchDown(Vector2 pos)
    {


        GameScene.Singleton.PressSkillBtn(1, pos,m_SkillDatas,false,false);
        //Debug.Log("Skill DoTouchDown:" + pos);
    }
    protected void DoTouchMove(Vector2 pos,float len)
    {
        if(m_SkillDatas.CastTargetType == 5)
        {
            len = 1;
        }
        //Debug.Log("Skill DoTouchMove:" + pos);
        pos.y = -pos.y;
        
        GameScene.Singleton.PressSkillBtn(2, pos.normalized * (m_SkillDatas.CastRange * len), m_SkillDatas,false,false);
    }
    protected void DoTouchEnd(Vector2 pos, float len,bool isno,bool ismy)
    {
        if (m_SkillDatas.CastTargetType == 5)
        {
            len = 1;
        }
        //Debug.Log("Skill DoTouchEnd:" + pos+"   "+ (pos * (m_SkillDatas.CastRange * len)));
        pos.y = -pos.y;
        GameScene.Singleton.PressSkillBtn(3, pos.normalized * (m_SkillDatas.CastRange * len), m_SkillDatas,isno,ismy);
    }


    public void TimerUpdate()
    {
        //Debug.Log("----------TimerUpdate111-:" + moveEventContext);
        if (moveEventContext != null)
        {
            //Debug.Log("----------TimerUpdate222-:"+ moveEventContext);
            //var context = moveEventContext;
            InputEvent inputEvent = moveEventContext;// (InputEvent)context.data;
            if (touchID != -1 && inputEvent.touchId == touchID)
            {
                //Vector2 localPos = GRoot.inst.GlobalToLocal(new Vector2(inputEvent.x, inputEvent.y));
                Vector2 localPos = m_ui.GlobalToLocal(inputEvent.position);

                var dir = (localPos - startStagePos);
                //点击到 取消施法
                if (Vector2.Distance(localPos, no.xy) < 40)
                {
                    no.scale = new Vector2(1.5f, 1.5f);
                    no.asTextField.color = new Color(1, 0, 0);
                    isNo = true;
                    center.asImage.color = new Color(1, 0, 0);
                }
                else
                {
                    no.scale = new Vector2(1.0f, 1.0f);
                    no.asTextField.color = new Color(1, 1, 1);
                    isNo = false;
                    center.asImage.color = new Color(1, 1, 1);
                }
                //点击到 施法自己
                if (showmy)
                {
                    if (Vector2.Distance(localPos, my.xy) < 40)
                    {
                        my.scale = new Vector2(1.5f, 1.5f);
                        my.asTextField.color = new Color(0, 0, 1);
                        isMy = true;

                        center.asImage.color = new Color(0, 0, 1);
                    }
                    else
                    {
                        my.scale = new Vector2(1.0f, 1.0f);
                        my.asTextField.color = new Color(1, 1, 1);
                        isMy = false;
                        center.asImage.color = new Color(1, 1, 1);
                    }
                }


                var len = Vector2.Distance(dir, new Vector2(0, 0));
                if (len > radius)
                {
                    //len = radius;
                }
                var dest = dir.normalized * len + startStagePos;


                thumb.SetXY(dest.x, dest.y);

                thumb.rotation = Vector2.SignedAngle(new Vector2(0, -1), dir);

                if (len >= m_MoveCallNeedDir)
                {
                    m_IsMoved = true;

                }

                if( m_IsMoved == false)
                {
                    //详情
                    if (IsShowInfo == false && Tool.GetTime() - TouchBeginTime >= 2)
                    {
                        isNo = true;
                        DoTouchEnd(dir, len / radius, isNo, isMy);

                        IsShowInfo = true;
                        GComponent info = Tool.CreateTouchShowInfo();

                        m_ui.asCom.AddChild(info);

                        var clientskill = ExcelManager.Instance.GetSkillManager().GetSkillByID(m_SkillDatas.TypeID);
                        if (clientskill != null)
                        {
                            if (clientskill.IconPath.Length > 0)
                            {
                                info.GetChild("icon").asLoader.url = clientskill.IconPath;
                            }
                            info.GetChild("name").asTextField.text = clientskill.Name;
                            info.GetChild("des").asTextField.text = clientskill.Des;

                        }
                        info.SetXY(localPos.x - 100, localPos.y - 50);

                    }
                    //
                }

                if (IsShowInfo == true)
                {
                    GComponent info = Tool.CreateTouchShowInfo();
                    info.SetXY(localPos.x - 100, localPos.y - 50);
                    isNo = true;
                    return;
                    //no.scale = new Vector2(1.5f, 1.5f);
                    //no.asTextField.color = new Color(1, 0, 0);
                    //isNo = true;
                    //center.asImage.color = new Color(1, 0, 0);
                }

                if (m_IsMoved)
                {
                    //onMove.Call(dir);
                    

                    if (len > radius)
                    {
                        startStagePos += dir.normalized * (len - radius);
                        center.SetXY(startStagePos.x, startStagePos.y);
                        DoTouchMove(dir, 1);
                    }
                    else
                    {
                        DoTouchMove(dir, len / radius);
                    }
                }

            }
        }
    }

    //开始触摸
    private void OnTouchBegin(EventContext context)
    {
        Debug.Log("OnTouchBegin");
        if (touchID == -1)//第一次触摸
        {
            InputEvent inputEvent = (InputEvent)context.data;
            moveEventContext = (InputEvent)context.data;
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
            TouchBeginTime = Tool.GetTime();

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
        moveEventContext = (InputEvent)context.data;
        Debug.Log("----OnTouchMove:" + context);
        
    }

    //结束触摸
    private void OnTouchEnd(EventContext context)
    {
        
        InputEvent inputEvent = (InputEvent)context.data;
        if (touchID != -1 && inputEvent.touchId == touchID)
        {
            Debug.Log("----OnTouchEnd:" + context);
            moveEventContext = null;
            Tool.CloseTouchShowInfo();
            IsShowInfo = false;

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
