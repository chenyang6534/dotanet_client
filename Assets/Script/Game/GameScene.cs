using cocosocket4unity;
using FairyGUI;
using Google.Protobuf;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScene : MonoBehaviour {

    // Use this for initialization
    public UnityEntity m_TargetUnit; //目标单位
    protected GameObject hero;
    public GameObject m_GameScene;
    protected Plane m_PlaneScene;
    protected int m_CurFrame;
    protected int m_LogicFps;
    protected double m_GameServerStartTime;//游戏场景服务器第0帧的时候的本地时间
    protected float m_LogicDelayTime;//逻辑延时
    public string m_ServerName;
    public int m_SceneID;
    public string m_ScenePath;

    public int m_MaxFrame = 0;//当前收到的逻辑帧数据 帧号
    protected Dictionary<int,Protomsg.SC_Update> m_LogicFrameData;//逻辑帧数据


    protected List<int> m_MyControlUnit;//我自己控制的单位的ID
    public UnityEntity m_MyMainUnit;//我自己的主单位

    public int NoDataCount;//没有数据次数

    private static GameScene singleton = null;
    public static GameScene Singleton
    {
        get
        {
            return singleton;
        }
    }
    void Awake()
    {
        singleton = this;
    }

    void Start () {
        
        Init();

    }

    void Init()
    {
        NextAutoAttackTime = 10000;

        m_LogicDelayTime = 0.05f;//延时0.02s
        MsgManager.Instance.AddListener("SC_Update", new HandleMsg(this.SC_Update));
        MsgManager.Instance.AddListener("SC_NewScene", new HandleMsg(this.SC_NewScene));

        
        
        m_LogicFrameData = new Dictionary<int, Protomsg.SC_Update>();

        Application.targetFrameRate = 30;
    }
    void OnDestroy()
    {
        MsgManager.Instance.RemoveListener("SC_Update");
        MsgManager.Instance.RemoveListener("SC_NewScene");
        
        MyKcp.Instance.Stop();
        Debug.Log("OnDestroy");
    }


    public UnityEntity GetMyMainUnit()
    {
        return m_MyMainUnit;
    }
    
    public bool SC_Update(Protomsg.MsgBase d1)
    {
        //Debug.Log("SC_Update:");
        //var m1 = System.GC.GetTotalMemory(true);

        IMessage IMperson = new Protomsg.SC_Update();
        Protomsg.SC_Update p1 = (Protomsg.SC_Update)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);

        m_LogicFrameData[p1.CurFrame] = p1;
        m_MaxFrame = p1.CurFrame;
        //Debug.Log("SC_Update time:" + Tool.GetTime()+"  frame:"+p1.CurFrame);
        //var m2 = System.GC.GetTotalMemory(true);

        //Debug.Log("gc size:"+ d1.Datas.Length+" str:"+ d1.Datas.ToString());

        return true;
    }
    

    

    //进入新场景
    public bool SC_NewScene(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_NewScene:");
        IMessage IMperson = new Protomsg.SC_NewScene();
        Protomsg.SC_NewScene p1 = (Protomsg.SC_NewScene)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        bool needNewScene = false;
        if(m_ScenePath != p1.Name)
        {
            needNewScene = true;
        }
        //p1.Name
        m_CurFrame = p1.CurFrame;
        m_LogicFps = p1.LogicFps;
        m_ServerName = p1.ServerName;
        m_SceneID = p1.SceneID;
        m_ScenePath = p1.Name;
        m_GameServerStartTime = Tool.GetTime() - 1.0f/p1.LogicFps* p1.CurFrame;
        Debug.Log("starttime:"+ m_GameServerStartTime+ " LogicFps: "+ p1.LogicFps+"  curframe:"+m_CurFrame+"  time:"+ Time.realtimeSinceStartup);
        CleanScene();
        if (needNewScene)
        {
            LoadScene(p1.SceneID, p1.Name);
        }
        else
        {
            //通知服务器加载完成
            Protomsg.CS_LodingScene msg1 = new Protomsg.CS_LodingScene();
            msg1.SceneID = m_SceneID;
            MyKcp.Instance.SendMsg(m_ServerName, "CS_LodingScene", msg1);
        }
        

        return true;
    }

    void LoadScene(int sceneid,string name)
    {
        if(m_GameScene != null)
        {
            CleanScene();
        }


        GComponent loading = UIPackage.CreateObject("GameUI", "Loading").asCom;
        GRoot.inst.AddChild(loading);
        Vector2 screenPos = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 logicScreenPos = GRoot.inst.GlobalToLocal(screenPos);
        loading.xy = logicScreenPos;

        ResourceLoadAsync.Instance.AddLoad(name,(re)=> {
            Debug.Log("--ResourceLoadAsync:" + re.isDone + "   :" + re.progress+"  time:"+Tool.GetTime());
            loading.GetChild("loadingbar").asProgress.value = re.progress * 100;
            if (re.isDone)
            {
                m_GameScene = (GameObject)Instantiate(re.asset);

                //System.Threading.Thread.Sleep(5000);
                //ab
                //ABManager.LoadAB();

                var pos = new Vector3(m_GameScene.transform.position.x, 0, m_GameScene.transform.position.z);
                m_PlaneScene.SetNormalAndPosition(m_GameScene.transform.up, pos);
                loading.Dispose();

                //SceneManager.LoadScene(0);

                //通知服务器加载完成
                Protomsg.CS_LodingScene msg1 = new Protomsg.CS_LodingScene();
                msg1.SceneID = sceneid;
                MyKcp.Instance.SendMsg(m_ServerName, "CS_LodingScene", msg1);
            }
            
        });
        

        //m_GameScene = (GameObject)Instantiate(Resources.Load(name));

        //var pos = new Vector3(m_GameScene.transform.position.x, 0, m_GameScene.transform.position.z);
        //m_PlaneScene.SetNormalAndPosition(m_GameScene.transform.up, pos);

        Debug.Log("SC_NewScene:"+name);
    }

    //清除场景资源 包括场景 单位 特效 
    void CleanScene()
    {
        m_LogicFrameData = new Dictionary<int, Protomsg.SC_Update>();
        //Destroy(m_GameScene);
        //m_GameScene = null;
        UnityEntityManager.Instance.Clear();
        HaloEntityManager.Instance.Clear();
        BulletEntityManager.Instance.Clear();
        SceneItemManager.Instance.Clear();


    }


    public void FreshControl()
    {
        //System.Array.Clear(m_MyControlUnit,0, m_MyControlUnit.Length);
        m_MyControlUnit = new List<int>();
        Dictionary<int, UnityEntity> all = UnityEntityManager.Instance.GetAllUnity();
        foreach (var item in all)
        {
            if (item.Value.ControlID == LoginUI.UID)
            {
                m_MyControlUnit.Add(item.Key);

                

                if (item.Value.IsMain == 1)
                {
                    m_MyMainUnit = item.Value;
                    m_MyMainUnit.MySelefShowGreenCircle(true);
                    DHCameraManager.SetFollowingTarget(item.Value);
                }
            }
            
        }

            
    }

    //动态处理延时 使过程平滑
    void DoDelay(double frame)
    {
        //动态处理延时 使过程平滑
        if (m_MaxFrame - frame > 2)
        {
            //超过2帧缓存数据 降低延时
            m_LogicDelayTime -= 0.01f;//10毫秒
            //Debug.Log("m_LogicDelayTime+:" + m_LogicDelayTime);
        }
        //else if (frame - m_MaxFrame >= 0)
        else if(m_MaxFrame-frame <= 1.5)
        {
            //缓存中没有需要执行的数据 增加延时
            m_LogicDelayTime += 0.015f;//20毫秒
            //Debug.Log("m_LogicDelayTime-:" + m_LogicDelayTime);
        }
    }

    //处理游戏逻辑
    void LogicUpdate()
    {
        //Debug.Log("33time:" + Tool.GetTime());
        //m_GameServerStartTime = Tool.GetTime() - 1.0f / p1.LogicFps * p1.CurFrame;
        var frame = (Tool.GetTime() - m_GameServerStartTime-m_LogicDelayTime) * m_LogicFps;
        //Debug.Log("frame:" + frame+"     curframe:"+ m_CurFrame+ " MaxFrame: " + m_MaxFrame);

        DoDelay(frame);
        

        for (var i = m_CurFrame; i <= frame; i++)
        {
            //帧数据存在
            if (m_LogicFrameData.ContainsKey(i))
            {
                var p1 = m_LogicFrameData[i];
                //new
                foreach (var item in p1.NewUnits)
                {
                    UnityEntityManager.Instance.CreateUnityEntity(this, item);
                }
                foreach (var item in p1.OldUnits)
                {
                    UnityEntityManager.Instance.ChangeUnityEntity(item);
                }
                foreach (var item in p1.RemoveUnits)
                {
                    UnityEntityManager.Instance.DestroyUnityEntity(item);
                    if(m_TargetUnit != null && m_TargetUnit.ID == item)
                    {
                        m_TargetUnit = null;
                    }
                }
                //子弹
                foreach (var item in p1.NewBullets)
                {
                    BulletEntityManager.Instance.CreateBulletEntity(this, item);
                }
                foreach (var item in p1.OldBullets)
                {
                    BulletEntityManager.Instance.ChangeBulletEntity(item);
                }
                foreach (var item in p1.RemoveBullets)
                {
                    BulletEntityManager.Instance.DestroyBulletEntity(item);
                }
                //光环
                foreach (var item in p1.NewHalos)
                {
                    //Debug.Log("new halo:" + item.ID);
                    HaloEntityManager.Instance.CreateHaloEntity(this, item);
                }
                foreach (var item in p1.OldHalos)
                {
                    //Debug.Log("old halo:" + item.ID);
                    HaloEntityManager.Instance.ChangeHaloEntity(item);
                }
                foreach (var item in p1.RemoveHalos)
                {
                    //Debug.Log("remove halo:" + item);
                    HaloEntityManager.Instance.DestroyHaloEntity(item);
                }
                //场景道具
                foreach (var item in p1.NewSceneItems)
                {
                    //Debug.Log("new sceneitem:" + item.ID+" typeid:"+item.TypeID);
                    //HaloEntityManager.Instance.CreateHaloEntity(this, item);
                    SceneItemManager.Instance.CreateSceneItem(this, item);
                }
                foreach (var item in p1.RemoveSceneItems)
                {
                    //Debug.Log("remove sceneitem:" + item);
                    //HaloEntityManager.Instance.DestroyHaloEntity(item);
                    SceneItemManager.Instance.DestroySceneItem(item);
                }

                //伤害数字提示
                foreach ( var item in p1.PlayerHurt)
                {
                    //Debug.Log("--id:"+item.HurtUnitID+"  value:"+item.HurtAllValue+"  gold:"+item.GetGold);
                    var unit = UnityEntityManager.Instance.GetUnityEntity(item.HurtUnitID);
                    if(unit != null)
                    {
                        unit.CreateHurtWords(item);
                    }
                }
                
                //UnityEntityManager

                m_CurFrame = i+1;//下一帧序号
                FreshControl();

                //删除上上帧数据
                m_LogicFrameData.Remove(i - 1);
            }
            else
            {
                Debug.Log("no data-------------------------------");
            }
        }

        //位置进行线性插值计算
        if (frame - (m_CurFrame - 1) > 0.001)
        {
            float scale = (float)(frame - (m_CurFrame - 1));
            if (m_LogicFrameData.ContainsKey(m_CurFrame))
            {
                var p1 = m_LogicFrameData[m_CurFrame];
                foreach (var item in p1.OldUnits)
                {
                    UnityEntityManager.Instance.ChangeShowPos(item, scale);
                }
                foreach (var item in p1.OldBullets)
                {
                    BulletEntityManager.Instance.ChangeShowPos(item, scale);
                }
                //BulletEntityManager.Instance.DestroyBulletEntity(item);
            }
            else
            {
                Debug.Log("no pos data-------------------------------");
            }
        }

        //删除已经计算的帧数据
        for (var i = m_CurFrame-15; i <= m_CurFrame - 5; i++)
        {
            if (m_LogicFrameData.ContainsKey(i))
            {
                m_LogicFrameData.Remove(i);
            }
        }

        //Debug.Log("44time:" + Tool.GetTime());

    }


    private float m_LastDegree = 0;
    private double m_LastControlTime = 0;
    private bool m_IsMove = false;
    public void SendControlData(float degree,bool isstart)
    {
        //Debug.Log(" SendControlData data:"+ isstart+" " + m_LastDegree + "  :" + degree + " " + Tool.GetTime() + " " + m_LastControlTime + " " + 1 / m_LogicFps);
        //if (m_IsMove == true && isstart == true && (Mathf.Abs(m_LastDegree-degree) <= 5 || Tool.GetTime()- m_LastControlTime <= 1.0f/m_LogicFps))
        if (m_IsMove == true && isstart == true && (Mathf.Abs(m_LastDegree - degree) <= 5))
        {
            //Debug.Log(" SendControlData no:");
            return;
        }
        else
        {
           // Debug.Log(" SendControlData yes:");
        }
        m_IsMove = isstart;
        m_LastControlTime = Tool.GetTime();
        m_LastDegree = degree;
        var dir = Tool.Vec2Rotate(new Vector2(0, 1), degree);

        Protomsg.CS_PlayerMove msg1 = new Protomsg.CS_PlayerMove();
        msg1.IDs.AddRange(m_MyControlUnit);
        msg1.IsStart = isstart;
        msg1.X = dir.x;
        msg1.Y = dir.y;
        if(isstart == false)
        {
            //Debug.Log("-----------no move:");
        }
        //

        MyKcp.Instance.SendMsg(m_ServerName, "CS_PlayerMove", msg1);
        //操作提前旋转
        //foreach (var item in m_MyControlUnit)
        //{
        //    UnityEntityManager.Instance.GetUnityEntity(item).PreLookAtDir(dir.x, dir.y);
        //}
    }



    public void PressSkillBtn(int touchstate,Vector2 dir,Protomsg.SkillDatas skilldata,bool isno,bool ismy)
    {
        if (m_MyMainUnit == null )
        {
            return;
        }

        float showattackrange = AttackSelectTargetDis;
        if (skilldata.CastRange > showattackrange)
        {
            showattackrange = skilldata.CastRange;
        }

        

        //CastTargetType 施法目标类型 1:自身为目标 2:以单位为目标 3:以地面1点为目标 
        //4:攻击时自动释放(攻击特效)5:以地面一点为方向
        if (touchstate == 1)
        {

            //第一次选择目标
            if (m_TargetUnit == null)
            {
                m_TargetUnit = UnityEntityManager.Instance.GetNearestBigUnitForSkillTarget(m_MyMainUnit, skilldata, showattackrange);
                if (m_TargetUnit == null)
                {
                    m_TargetUnit = UnityEntityManager.Instance.GetNearestUnitForSkillTarget(m_MyMainUnit, skilldata);
                }


            }
            else
            {
                m_TargetUnit.TargetShow(false);
                m_TargetUnit.TargetShowRedCircle(false);

                if (UnityEntityManager.Instance.CheckCastSkillTarget(m_TargetUnit, m_MyMainUnit, skilldata) == false)
                {
                    var target1 = UnityEntityManager.Instance.GetNearestBigUnitForSkillTarget(m_MyMainUnit, skilldata, showattackrange);
                    //m_TargetUnit = UnityEntityManager.Instance.GetNearestBigUnitForSkillTarget(m_MyMainUnit, skilldata, showattackrange);
                    if (target1 == null)
                    {
                        target1 = UnityEntityManager.Instance.GetNearestUnitForSkillTarget(m_MyMainUnit, skilldata);
                    }
                    if(target1 != null)
                    {
                        m_TargetUnit = target1;
                    }
                }

                //不在范围内都重新寻找
                //如果是普通单位且不在寻找攻击范围内 则重新寻找最近单位
                var dis = Vector2.Distance(new Vector2(m_MyMainUnit.X, m_MyMainUnit.Y), new Vector2(m_TargetUnit.X, m_TargetUnit.Y));
                if (showattackrange < dis)
                {
                    m_TargetUnit = UnityEntityManager.Instance.GetNearestBigUnitForSkillTarget(m_MyMainUnit, skilldata, showattackrange);
                    if (m_TargetUnit == null)
                    {
                        m_TargetUnit = UnityEntityManager.Instance.GetNearestUnitForSkillTarget(m_MyMainUnit, skilldata);
                    }
                }
                else
                {
                    //单位类型(1:英雄 2:普通单位 3:远古 4:boss)
                    //如果不是大仇恨敌方单位
                    if (m_TargetUnit.IsBigUnit() == false)
                    {
                        var bigenemy = UnityEntityManager.Instance.GetNearestBigUnitForSkillTarget(m_MyMainUnit, skilldata, showattackrange);
                        if (bigenemy != null)
                        {
                            m_TargetUnit = bigenemy;
                        }
                        else
                        {
                            //不在攻击范围内
                            if (skilldata.CastRange < dis)
                            {
                                m_TargetUnit = UnityEntityManager.Instance.GetNearestUnitForSkillTarget(m_MyMainUnit, skilldata);
                            }

                        }

                    }

                }

            }
            
            




            switch (skilldata.CastTargetType) {
                case 4:
                    Protomsg.CS_PlayerSkill msg4 = new Protomsg.CS_PlayerSkill();
                    msg4.ID = m_MyMainUnit.ID;
                    msg4.TargetUnitID = 0;
                    msg4.X = 0;
                    msg4.Y = 0;
                    msg4.SkillID = skilldata.TypeID;
                    MyKcp.Instance.SendMsg(m_ServerName, "CS_PlayerSkill", msg4);
                    break;
                case 1:
                    if (skilldata.HurtRange > 0.1)
                    {
                        m_MyMainUnit.ShowOutCircle(true, skilldata.HurtRange);
                    }
                    break;
                case 2:
                    if (m_TargetUnit == null)
                    {
                        m_TargetUnit = UnityEntityManager.Instance.GetNearestUnitForSkillTarget(m_MyMainUnit,skilldata);
                        if(m_TargetUnit == null)
                        {
                            return;
                        }

                    }
                    m_TargetUnit.TargetShow(true);
                    m_TargetUnit.TargetShowRedCircle(true);
                    m_MyMainUnit.ShowSkillAreaLookAt(true, new Vector2(m_TargetUnit.X, m_TargetUnit.Y));
                    //m_MyMainUnit.ShowOutCircle(true, skilldata.CastRange);
                    break;
                case 3:
                    {
                        var targetPos = new Vector2(m_MyMainUnit.X, m_MyMainUnit.Y);
                        if (m_TargetUnit != null)
                        {
                            targetPos = new Vector2(m_TargetUnit.X, m_TargetUnit.Y);
                        }
                        else
                        {
                            //进攻性技能
                            var skillclientitem = ExcelManager.Instance.GetSkillManager().GetSkillByID(skilldata.TypeID);
                            if(skillclientitem != null)
                            {
                                //技能自动瞄准类型 1:普通 2:位移技能(朝目标方向最大距离) 3:进攻技能(瞄准敌人)
                                if (skillclientitem.AutoAimType == 3)
                                {
                                    var targetunit = UnityEntityManager.Instance.GetNearestEnemy(m_MyMainUnit);
                                    if (targetunit != null)
                                    {
                                        targetPos = new Vector2(targetunit.X, targetunit.Y);
                                    }
                                }else if(skillclientitem.AutoAimType == 2)
                                {
                                    var targetDir = new Vector2(m_MyMainUnit.DirectionX, m_MyMainUnit.DirectionY);
                                    targetPos = new Vector2(m_MyMainUnit.X, m_MyMainUnit.Y) + (targetDir.normalized * skilldata.CastRange);
                                }
                                
                            }
                        }
                        m_MyMainUnit.ShowInCircle(true, skilldata.HurtRange, new Vector3(targetPos.x - m_MyMainUnit.X, 0, targetPos.y - m_MyMainUnit.Y));

                        break;
                    }
                    
                case 5:
                    {
                        var targetDir = new Vector2(m_MyMainUnit.DirectionX, m_MyMainUnit.DirectionY);
                        if (m_TargetUnit != null)
                        {
                            targetDir = new Vector2(m_TargetUnit.X- m_MyMainUnit.X, m_TargetUnit.Y- m_MyMainUnit.Y);
                        }
                        else
                        {
                            //进攻性技能
                            var skillclientitem = ExcelManager.Instance.GetSkillManager().GetSkillByID(skilldata.TypeID);
                            if (skillclientitem != null)
                            {
                                //技能自动瞄准类型 1:普通 2:位移技能(朝目标方向最大距离) 3:进攻技能(瞄准敌人)
                                if (skillclientitem.AutoAimType == 3)
                                {
                                    var targetunit = UnityEntityManager.Instance.GetNearestEnemy(m_MyMainUnit);
                                    if (targetunit != null)
                                    {
                                        targetDir = new Vector2(targetunit.X - m_MyMainUnit.X, targetunit.Y - m_MyMainUnit.Y);
                                    }
                                }
                                else if (skillclientitem.AutoAimType == 2)
                                {

                                }

                            }
                        }
                        var targetpos = new Vector2(m_MyMainUnit.X, m_MyMainUnit.Y) + (targetDir.normalized * skilldata.CastRange);
                        //m_MyMainUnit.ShowInCircle(true, 1, new Vector3(targetPos.x - m_MyMainUnit.X, 0, targetPos.y - m_MyMainUnit.Y));
                        m_MyMainUnit.ShowSkillAreaLookAt(true, targetpos);
                        break;
                    }
                    
            }
            if(skilldata.CastRange > 0.1)
            {
                m_MyMainUnit.ShowOutCircle(true, skilldata.CastRange);
            }

            

        }
        else if (touchstate == 2)
        {
            switch (skilldata.CastTargetType)
            {
                case 1:
                    break;
                case 2:
                    var target = UnityEntityManager.Instance.GetMinAngleUnitForSkillTarget(m_MyMainUnit, dir, skilldata);
                    if (target != null)
                    {
                        if (m_TargetUnit != null)
                        {
                            m_TargetUnit.TargetShow(false);
                            m_TargetUnit.TargetShowRedCircle(false);
                        }
                        m_TargetUnit = target;
                        m_TargetUnit.TargetShow(true);
                        m_TargetUnit.TargetShowRedCircle(true);

                        var targetpos = new Vector2(m_MyMainUnit.X, m_MyMainUnit.Y) + (dir.normalized * skilldata.CastRange);

                        m_MyMainUnit.ShowSkillAreaLookAt(true, targetpos);

                    }
                    else
                    {
                        if (m_TargetUnit != null)
                        {
                            m_TargetUnit.TargetShow(true);
                            m_TargetUnit.TargetShowRedCircle(true);
                            var targetpos = new Vector2(m_MyMainUnit.X, m_MyMainUnit.Y) + (dir.normalized * skilldata.CastRange);
                            m_MyMainUnit.ShowSkillAreaLookAt(true, targetpos);
                        }
                    }
                    break;
                case 3:
                    {
                        if (m_TargetUnit != null)
                        {
                            m_TargetUnit.TargetShow(false);
                            m_TargetUnit.TargetShowRedCircle(false);
                            m_TargetUnit = null;
                        }
                        m_MyMainUnit.ShowInCircle(true, skilldata.HurtRange, new Vector3(dir.x, 0, dir.y));
                    }
                   
                    break;
                case 5:
                    {
                        if (m_TargetUnit != null)
                        {
                            m_TargetUnit.TargetShow(false);
                            m_TargetUnit.TargetShowRedCircle(false);
                            m_TargetUnit = null;
                        }
                        var targetpos = new Vector2(m_MyMainUnit.X, m_MyMainUnit.Y) + (dir.normalized * skilldata.CastRange);

                        m_MyMainUnit.ShowSkillAreaLookAt(true, targetpos);
                        //m_MyMainUnit.ShowInCircle(true, 1, new Vector3(dir.x, 0, dir.y));
                    }

                    break;
            }
            if (skilldata.CastRange > 0.1)
            {
                m_MyMainUnit.ShowOutCircle(true, skilldata.CastRange);
            }


        }
        else if (touchstate == 3)
        {
            if(isno == true)
            {
                if(m_TargetUnit != null)
                {
                    m_TargetUnit.TargetShow(false);
                }
                m_MyMainUnit.ShowOutCircle(false, 10);
                m_MyMainUnit.ShowSkillAreaLookAt(false, Vector2.zero);
                m_MyMainUnit.ShowInCircle(false, 1, new Vector3(0, 0, 0));
                return;
            }
            if(ismy == true)
            {
                Protomsg.CS_PlayerSkill msgmy = new Protomsg.CS_PlayerSkill();
                msgmy.ID = m_MyMainUnit.ID;
                msgmy.TargetUnitID = m_MyMainUnit.ID;
                msgmy.X = 0;
                msgmy.Y = 0;
                msgmy.SkillID = skilldata.TypeID;
                MyKcp.Instance.SendMsg(m_ServerName, "CS_PlayerSkill", msgmy);

                if (m_TargetUnit != null)
                {
                    m_TargetUnit.TargetShow(false);
                }
                m_MyMainUnit.ShowOutCircle(false, 10);
                m_MyMainUnit.ShowSkillAreaLookAt(false, Vector2.zero);
                m_MyMainUnit.ShowInCircle(false, 1, new Vector3(0, 0, 0));
                return;
            }
            switch (skilldata.CastTargetType)
            {
                case 1:
                    Protomsg.CS_PlayerSkill msg2 = new Protomsg.CS_PlayerSkill();
                    msg2.ID = m_MyMainUnit.ID;
                    msg2.TargetUnitID = 0;
                    msg2.X = 0;
                    msg2.Y = 0;
                    msg2.SkillID = skilldata.TypeID;
                    MyKcp.Instance.SendMsg(m_ServerName, "CS_PlayerSkill", msg2);
                    m_MyMainUnit.ShowOutCircle(false, 10);

                    Debug.Log("CS_PlayerSkill 1111");
                    break;
                case 2:
                    if (m_TargetUnit == null)
                    {
                        return;
                    }

                    Protomsg.CS_PlayerSkill msg1 = new Protomsg.CS_PlayerSkill();
                    msg1.ID = m_MyMainUnit.ID;
                    msg1.TargetUnitID = m_TargetUnit.ID;
                    msg1.X = 0;
                    msg1.Y = 0;
                    msg1.SkillID = skilldata.TypeID;
                    MyKcp.Instance.SendMsg(m_ServerName, "CS_PlayerSkill", msg1);

                    Debug.Log("CS_PlayerSkill");
                    m_TargetUnit.TargetShow(false);
                    m_MyMainUnit.ShowOutCircle(false, 10);
                    m_MyMainUnit.ShowSkillAreaLookAt(false, Vector2.zero);
                    break;
                case 3:
                    {
                        var targetPos = new Vector2(m_MyMainUnit.X + dir.x, m_MyMainUnit.Y + dir.y);
                        if (m_TargetUnit != null)
                        {
                            targetPos = new Vector2(m_TargetUnit.X, m_TargetUnit.Y);
                        }

                        Protomsg.CS_PlayerSkill msg3 = new Protomsg.CS_PlayerSkill();
                        msg3.ID = m_MyMainUnit.ID;
                        msg3.TargetUnitID = -1;
                        //msg3.X = targetPos.x;
                        //msg3.Y = targetPos.y;
                        msg3.X = m_MyMainUnit.m_SkillAreaInCircleOffsetPos.x+ m_MyMainUnit.X;
                        msg3.Y = m_MyMainUnit.m_SkillAreaInCircleOffsetPos.z+ m_MyMainUnit.Y;

                        msg3.SkillID = skilldata.TypeID;
                        MyKcp.Instance.SendMsg(m_ServerName, "CS_PlayerSkill", msg3);

                        Debug.Log("CS_PlayerSkill");
                        m_MyMainUnit.ShowOutCircle(false, 10);
                        m_MyMainUnit.ShowInCircle(false, skilldata.HurtRange, new Vector3(dir.x, 0, dir.y));
                        m_MyMainUnit.ShowSkillAreaLookAt(false, Vector2.zero);
                    }
                    
                    break;
                case 5:
                    {
                        var targetPos = new Vector2(m_MyMainUnit.X + dir.x, m_MyMainUnit.Y + dir.y);
                        if (m_TargetUnit != null)
                        {
                            targetPos = new Vector2(m_TargetUnit.X, m_TargetUnit.Y);
                        }

                        Protomsg.CS_PlayerSkill msg3 = new Protomsg.CS_PlayerSkill();
                        msg3.ID = m_MyMainUnit.ID;
                        msg3.TargetUnitID = -1;
                        //msg3.X = targetPos.x;
                        //msg3.Y = targetPos.y;
                        msg3.X = m_MyMainUnit.m_SkillAreaLookAtDir.x+ m_MyMainUnit.X;
                        msg3.Y = m_MyMainUnit.m_SkillAreaLookAtDir.z+ m_MyMainUnit.Y;
                        msg3.SkillID = skilldata.TypeID;
                        MyKcp.Instance.SendMsg(m_ServerName, "CS_PlayerSkill", msg3);

                        Debug.Log("CS_PlayerSkill");
                        m_MyMainUnit.ShowOutCircle(false, 10);
                        m_MyMainUnit.ShowInCircle(false, 1, new Vector3(0, 0, 0));
                        m_MyMainUnit.ShowSkillAreaLookAt(false, Vector2.zero);
                    }

                    break;
            }


            
        }

        return;
    }



    //1:down 2:move 3:end
    float AttackSelectTargetDis = 8;//攻击目标选择范围
    public void PressAttackBtn(int touchstate,Vector2 dir)
    {
        if(m_MyMainUnit == null)
        {
            return;
        }
        //AttackRange
        float showattackrange = AttackSelectTargetDis;
        if( m_MyMainUnit.AttackRange > showattackrange)
        {
            showattackrange = m_MyMainUnit.AttackRange;
        }

        if (touchstate == 1)
        {
            if (m_TargetUnit == null )
            {
                m_TargetUnit = UnityEntityManager.Instance.GetNearestBigEnemy(m_MyMainUnit, showattackrange);
                if(m_TargetUnit == null)
                {
                    m_TargetUnit = UnityEntityManager.Instance.GetNearestEnemy(m_MyMainUnit);
                }
                

            }
            else
            {
                m_TargetUnit.TargetShow(false);
                m_TargetUnit.TargetShowRedCircle(false);

                if (UnityEntityManager.Instance.CheckIsEnemy(m_TargetUnit, m_MyMainUnit) == false)
                {
                    m_TargetUnit = UnityEntityManager.Instance.GetNearestBigEnemy(m_MyMainUnit, showattackrange);
                    if (m_TargetUnit == null)
                    {
                        m_TargetUnit = UnityEntityManager.Instance.GetNearestEnemy(m_MyMainUnit);
                    }
                }

                //不在范围内都重新寻找
                //如果是普通单位且不在寻找攻击范围内 则重新寻找最近单位
                var dis = Vector2.Distance(new Vector2(m_MyMainUnit.X, m_MyMainUnit.Y), new Vector2(m_TargetUnit.X, m_TargetUnit.Y));
                if (showattackrange < dis)
                {
                    m_TargetUnit = UnityEntityManager.Instance.GetNearestBigEnemy(m_MyMainUnit, showattackrange);
                    if (m_TargetUnit == null)
                    {
                        m_TargetUnit = UnityEntityManager.Instance.GetNearestEnemy(m_MyMainUnit);
                    }
                }else
                {
                    //单位类型(1:英雄 2:普通单位 3:远古 4:boss)
                    //如果不是大仇恨敌方单位
                    if (m_TargetUnit.IsBigUnit() == false)
                    {
                        var bigenemy = UnityEntityManager.Instance.GetNearestBigEnemy(m_MyMainUnit, showattackrange);
                        if (bigenemy != null)
                        {
                            m_TargetUnit = bigenemy;
                        }
                        else
                        {
                            //不在攻击范围内
                            if (m_MyMainUnit.AttackRange < dis)
                            {
                                m_TargetUnit = UnityEntityManager.Instance.GetNearestEnemy(m_MyMainUnit);
                            }
                                
                        }

                    }
                    
                }
                
            }

            if(m_TargetUnit == null)
            {
                return;
            }
            
            m_TargetUnit.TargetShow(true);
            m_TargetUnit.TargetShowRedCircle(true);
            m_MyMainUnit.ShowSkillAreaLookAt(true, new Vector2(m_TargetUnit.X, m_TargetUnit.Y));
            m_MyMainUnit.ShowOutCircle(true, showattackrange);

        }
        else if (touchstate == 2)
        {
            var target = UnityEntityManager.Instance.GetMinAngleEnemy(m_MyMainUnit, dir, showattackrange);
            if( target != null)
            {
                if(m_TargetUnit != null)
                {
                    m_TargetUnit.TargetShow(false);
                    m_TargetUnit.TargetShowRedCircle(false);
                }
                m_TargetUnit = target;
                m_TargetUnit.TargetShow(true);
                m_TargetUnit.TargetShowRedCircle(true);

                var targetpos = new Vector2(m_MyMainUnit.X, m_MyMainUnit.Y) + (dir.normalized * showattackrange);

                //m_MyMainUnit.ShowSkillAreaLookAt(true, new Vector2(m_TargetUnit.X, m_TargetUnit.Y));
                m_MyMainUnit.ShowSkillAreaLookAt(true, targetpos);

            }
            else
            {
                if(m_TargetUnit != null)
                {
                    m_TargetUnit.TargetShow(true);
                    m_TargetUnit.TargetShowRedCircle(true);
                    var targetpos = new Vector2(m_MyMainUnit.X, m_MyMainUnit.Y) + (dir.normalized * showattackrange);
                    m_MyMainUnit.ShowSkillAreaLookAt(true, targetpos);
                }
            }
            
        }
        else if(touchstate == 3)
        {
            if(m_TargetUnit == null)
            {
                return;
            }

            Protomsg.CS_PlayerAttack msg1 = new Protomsg.CS_PlayerAttack();
            msg1.IDs.AddRange(m_MyControlUnit);
            msg1.TargetUnitID = m_TargetUnit.ID;
            MyKcp.Instance.SendMsg(m_ServerName, "CS_PlayerAttack", msg1);

            NextAutoAttackTime = 1;

            //Debug.Log("PressAttackBtn");

            m_TargetUnit.TargetShow(false);
            m_MyMainUnit.ShowOutCircle(false, 10);
            m_MyMainUnit.ShowSkillAreaLookAt(false, Vector2.zero);
        }

        return;
        
    }




    public void ClickPos(Vector2 pos)
    {
        Debug.Log("click pos:"+pos);
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Raycast:");
            //测试
            UnityEntity unit = UnityEntityManager.Instance.GetUnityEntityFromObject(hit.transform.gameObject);
            if (unit != null && unit != m_MyMainUnit)
            {
                if (m_TargetUnit != null)
                {
                    m_TargetUnit.TargetShow(false);
                    m_TargetUnit.TargetShowRedCircle(false);
                }
                m_TargetUnit = unit;
                //m_TargetUnit.TargetShow(true);
                m_TargetUnit.TargetShowRedCircle(true);

                //Protomsg.CS_PlayerAttack msg1 = new Protomsg.CS_PlayerAttack();
                //msg1.IDs.AddRange(m_MyControlUnit);
                //msg1.TargetUnitID = unit.ID;
                //MyKcp.Instance.SendMsg(m_ServerName, "CS_PlayerAttack", msg1);
            }

            //Debug.Log(hit.collider.gameObject);
        }
    }
    //自动攻击
    protected double NextAutoAttackTime; //下一次自动攻击时间
    void AutoAttack()
    {
        NextAutoAttackTime -= Time.deltaTime;
        if(NextAutoAttackTime <= 0)
        {
            GameScene.Singleton.PressAttackBtn(3, Vector2.zero);
        }
        
    }


    // Update is called once per frame
    void Update () {
        
        //Debug.Log("update time:" + Tool.GetTime());
        MsgManager.Instance.UpdateMessage();


        LogicUpdate();
        //删除目标
        if (m_TargetUnit != null)
        {
            if(m_TargetUnit.IsDeath == 1 || UnityEntityManager.Instance.GetUnityEntity(m_TargetUnit.ID) == null)
            {
                m_TargetUnit.TargetShow(false);
                m_TargetUnit.TargetShowRedCircle(false);
                m_TargetUnit = null;
            }
            else
            {
                //距离我自己太远
               
                
            }
            
        }
        
        
        UnityEntityManager.Instance.Update(Time.deltaTime);
        if(Time.deltaTime >= 0.033)
        {
            //Debug.Log("deltaTime:" + Time.deltaTime);
        }


        //AutoAttack();
    }

    
    
}
