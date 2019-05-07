
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityEntity {

    protected GameScene m_Scene;

    protected float RotateSpeed;

    protected Vector3 TargetRotation;

    protected float m_MeshHeight;

    public UnityEntity(Protomsg.UnitDatas data, GameScene scene)
    {

        RotateSpeed = 480;

        m_Scene = scene;
        Name = data.Name;
        Level = data.Level;
        HP = data.HP;
        MP = data.MP;
        MaxHP = data.MaxHP;
        MaxMP = data.MaxMP;
        ModeType = data.ModeType;
        ID = data.ID;
        ControlID = data.ControlID;
        X = data.X;
        Y = data.Y;
        DirectionX = data.DirectionX;
        DirectionY = data.DirectionY;

        AttackMode = data.AttackMode;//攻击模式(1:和平模式 2:组队模式 3:全体模式 4:阵营模式(玩家,NPC) 5:行会模式)
        UnitType = data.UnitType; //需要数据 ModeType 
        AttackAcpabilities = data.AttackAcpabilities;
        IsMain = data.IsMain;





        m_Mode.transform.position = new Vector3(data.X, 0, data.Y);

        //if(ControlID == LoginUI.UID)
        //{
        //    var fogwar = m_Mode.AddComponent<FogOfWarExplorer>();
        //    fogwar.radius = 8;
        //}

        FreshAnim(data.AnimotorState, data.AttackTime);


    }
    public void FreshAnim(int anim,float time)
    {
        if (m_Mode != null)
        {
            if (anim != 0)
            {
                //Debug.Log("AniState: "+ anim);
                m_Mode.GetComponent<Animator>().SetInteger("AniState", anim);
                
                //攻击动画
                if (anim == 3)
                {
                    float animtime = Tool.GetClipLength(m_Mode.GetComponent<Animator>(), "attack");
                    if(time <= 0)
                    {
                        time = 1;
                    }
                    float speed = animtime / time;
                    m_Mode.GetComponent<Animator>().speed = speed;
                    //Debug.Log("animlen: attack " + speed);
                    //Debug.Log("animlen: walk " + Tool.GetClipLength(m_Mode.GetComponent<Animator>(), "walk"));
                    //Debug.Log("animlen: idle1t " + Tool.GetClipLength(m_Mode.GetComponent<Animator>(), "idle1t"));
                    //Debug.Log("animlen: idle2 " + Tool.GetClipLength(m_Mode.GetComponent<Animator>(), "idle2"));
                    //m_Mode.GetComponent<Animator>().speed = 0.2f;
                }
                else
                {
                    m_Mode.GetComponent<Animator>().speed = 1.0f;
                }
                
            }
        }
    }

    //显示 箭头指示器
    public void IndicateShow(float len)
    {

    }
    //隐藏 箭头指示器
    public void IndicateHide()
    {

    }


    //目标红色高亮显示
    public void TargetShow(bool isshow)
    {
        //Debug.Log("TargetShow:"+isshow);
        var hlob = m_Mode.GetComponent<HighlightableObject>();
        if (hlob == null)
        {
            hlob = m_Mode.AddComponent<HighlightableObject>();
        }
        if (isshow)
        {
            //红色
            hlob.FlashingOn(new Color(0.9f, 0.3f, 0.3f), new Color(0.9f, 0.3f, 0.3f), 1);
        }
        else
        {
            hlob.FlashingOff();
        }
    }

    //提前更新方向(关闭预判)
    public void PreLookAtDir(float x, float y)
    {
        //if (m_Mode == null)
        //{
        //    return;
        //}
        //var dir = new Vector3(x, 0, y);
        //ChangeDirection(dir);
        
    }

    //更新方向动画
    public void ChangeDirection(Vector3 dir)
    {
        if (m_Mode == null)
        {
            return;
        }
        if (dir != Vector3.zero)
        {
            var endv = new Vector3(0.0f, 0.0f, 0.0f);

            var angle1 = m_Mode.transform.rotation.eulerAngles.y;
            var angle2 = Quaternion.LookRotation(dir, Vector3.up).eulerAngles.y;

            if (angle2 - angle1 > 180)
            {
                angle2 -= 360;
            }
            if (angle1 - angle2 > 180)
            {
                angle2 += 360;
            }

            

            //var t = Math.Abs(angle1 - angle2) / RotateSpeed;
            //var t = 0.025f;
            endv.y = angle2;

            this.TargetRotation = endv;
            //m_Mode.transform.rotation.eulerAngles.y

            //m_Mode.transform.DORotate(endv, t);
            //m_Mode.transform.Rotate(endv- m_Mode.transform.rotation.eulerAngles);
        }
    }
    public void UpdateDirection(float dt)
    {
        float sub = this.TargetRotation.y - m_Mode.transform.rotation.eulerAngles.y;
        if(RotateSpeed * dt > Math.Abs(sub))
        {
            m_Mode.transform.Rotate(new Vector3(0.0f, sub, 0.0f));
        }
        else
        {
            m_Mode.transform.Rotate(new Vector3(0.0f, sub / Math.Abs(sub) * RotateSpeed * dt, 0.0f));
        }
        


    }
    public void Update(float dt)
    {
        UpdateDirection(dt);
    }

    public void Change(Protomsg.UnitDatas data)
    {
        // 更新位置
        if( m_Mode != null)
        {
            //Vector3 add = new Vector3(data.X, 0, data.Y);
            X += data.X;
            Y += data.Y;
            //更新位置
            m_Mode.transform.position = new Vector3(X,0,Y);
            //更新动画
            FreshAnim(data.AnimotorState,data.AttackTime);
            //更新方向
            DirectionX += data.DirectionX;
            DirectionY += data.DirectionY;
            var dir = new Vector3(DirectionX, 0, DirectionY);
            //var dir = new Vector3(1, 0, 1);
            if (dir != Vector3.zero)
            {
                ChangeDirection(dir);
            }

            //-----更新其他数据----
            //Name = data.Name;
            Level += data.Level;
            HP += data.HP;
            MP += data.MP;
            MaxHP += data.MaxHP;
            MaxMP += data.MaxMP;


            if(data.ModeType != "")
            {
                ModeType = data.ModeType;
            }
            if (data.Name != "")
            {
                Name = data.Name;
            }

            ControlID += data.ControlID;
            AttackMode += data.AttackMode;
            UnitType += data.UnitType; //需要数据 ModeType 
            AttackAcpabilities += data.AttackAcpabilities;
            IsMain += data.IsMain;

            UpdateTopBar();

            




        }
    }
    public void ChangeShowPos(float scale,float nextx,float nexty)
    {
        if (m_Mode != null)
        {
            m_Mode.transform.position = new Vector3(X+(scale*nextx), 0, Y + (scale * nexty));
        }
    }

    public Transform GetRenderingTransform
    {
        get
        {
            if (m_Mode != null)
            {
                return m_Mode.transform;
            }
            return null;
        }
    }

    public void Destroy()
    {
        if (m_Mode != null)
        {
            GameObject.Destroy(m_Mode);
        }
    }

    protected GameObject m_Mode;//模型
    public GameObject Mode
    {
        get
        {
            return m_Mode;
        }
    }


    // 模型名字
    protected string m_ModeType;
    public string ModeType
    {
        get
        {
            return m_ModeType;
        }
        set
        {
            if (m_ModeType == value)
            {
                return;
            }
            m_ModeType = value;
            if( m_Mode != null)
            {
                GameObject.Destroy(m_Mode);
            }
            m_Mode = (GameObject)(GameObject.Instantiate(Resources.Load(m_ModeType)));

            m_Mode.transform.parent = m_Scene.transform.parent;
            //m_MeshHeight = 2;

            //Debug.Log("sizey:" + m_Mode.GetComponent<Collider>().bounds.size.y);
            //Debug.Log("scaley:" + m_Mode.transform.localScale.y);

            m_MeshHeight = m_Mode.GetComponent<Collider>().bounds.size.y;
            //m_Mode.renderer
            //m_MeshHeight = m_Mode.renderer.bounds.size;
            Debug.Log("m_MeshHeight:" + m_MeshHeight);
        }
    }
    

    // 名字
    protected string m_Name;
    public string Name
    {
        get
        {
            return m_Name;
        }
        set
        {
            m_Name = value;
        }
    }
    // 等级
    protected int m_Level;
    public int Level
    {
        get
        {
            return m_Level;
        }
        set
        {
            m_Level = value;
        }
    }

    // HP
    protected int m_HP;
    public int HP
    {
        get
        {
            return m_HP;
        }
        set
        {
            m_HP = value;
        }
    }
    // MaxHP
    protected int m_MaxHP;
    public int MaxHP
    {
        get
        {
            return m_MaxHP;
        }
        set
        {
            m_MaxHP = value;
        }
    }
    // MP
    protected int m_MP;
    public int MP
    {
        get
        {
            return m_MP;
        }
        set
        {
            m_MP = value;
        }
    }
    // MaxMP
    protected int m_MaxMP;
    public int MaxMP
    {
        get
        {
            return m_MaxMP;
        }
        set
        {
            m_MaxMP = value;
        }
    }

    // ID
    protected int m_ID;
    public int ID
    {
        get
        {
            return m_ID;
        }
        set
        {
            m_ID = value;
        }
    }
    // ControlID
    protected int m_ControlID;
    public int ControlID
    {
        get
        {
            return m_ControlID;
        }
        set
        {
            m_ControlID = value;
        }
    }
    // IsMain
    protected int m_IsMain;
    public int IsMain
    {
        get
        {
            return m_IsMain;
        }
        set
        {
            m_IsMain = value;
        }
    }

    protected GameObject m_SkillAreaLookAt;//技能效果 箭头
    //float outerRadius = 6;      // 外圆半径
    //float innerRadius = 2f;     // 内圆半径
    float cubeWidth = 1f;       // 矩形宽度 （矩形长度使用的外圆半径）
    int angle = 60;             // 扇形角度
    public void ShowSkillAreaLookAt(bool isshow,Vector2 targetPos)
    {
        float len = Vector2.Distance(targetPos, new Vector2(X, Y));
        float angle = Vector2.SignedAngle(new Vector2(0, 1),
                new Vector2(targetPos.x - X, targetPos.y - Y));

        if (m_SkillAreaLookAt == null)
        {
            m_SkillAreaLookAt = (GameObject)(GameObject.Instantiate(Resources.Load("SkillAreaEffect/Prefabs/Hero_skillarea/chang_hero")));
            //m_Mode.
            m_SkillAreaLookAt.transform.parent = m_Mode.transform;
            //m_SkillAreaLookAt.transform.localEulerAngles = new Vector3(0,,0);

        }

        //m_SkillAreaLookAt.transform.localEulerAngles = new Vector3(0, angle, 0);
        Debug.Log("position:" + m_SkillAreaLookAt.transform.localPosition);
        m_SkillAreaLookAt.transform.localPosition = Vector3.zero;

        Debug.Log("scale:"+ m_Mode.transform.localScale.x);
        m_SkillAreaLookAt.transform.localScale = new Vector3(cubeWidth / m_Mode.transform.localScale.x, 1, len / m_Mode.transform.localScale.x) ;
        m_SkillAreaLookAt.transform.LookAt(new Vector3(targetPos.x, 0, targetPos.y));
        if (isshow)
        {
            m_SkillAreaLookAt.gameObject.SetActive(true);
        }
        else
        {
            m_SkillAreaLookAt.gameObject.SetActive(false);
        }
    }
    //显示外径圆
    protected GameObject m_SkillAreaOutCircle;//外径圆
    public void ShowOutCircle(bool isshow,float r)
    {
        if (m_SkillAreaOutCircle == null)
        {
            m_SkillAreaOutCircle = (GameObject)(GameObject.Instantiate(Resources.Load("SkillAreaEffect/Prefabs/Hero_skillarea/quan_hero")));
            //m_Mode.
            m_SkillAreaOutCircle.transform.parent = m_Mode.transform;

        }
        m_SkillAreaOutCircle.transform.localScale = new Vector3(r * 2, 1, r * 2) / m_Mode.transform.localScale.x;

        if (isshow)
        {
            m_SkillAreaOutCircle.gameObject.SetActive(true);
        }
        else
        {
            m_SkillAreaOutCircle.gameObject.SetActive(false);
        }
    }


    protected GameObject m_TopBar;//头顶血条
    //单位类型(1:英雄 2:普通单位 3:远古 4:boss)
    // UnitType

    public void UpdateTopBar()
    {
        //头顶条显示
        if (m_TopBar != null)
        {
            m_TopBar.GetComponent<UnityEntityTopBar>().SetHP(HP / MaxHP*100);
            m_TopBar.GetComponent<UnityEntityTopBar>().SetMP(MP / MaxMP * 100);
            m_TopBar.GetComponent<UnityEntityTopBar>().SetName(Name);
            m_TopBar.GetComponent<UnityEntityTopBar>().SetLevel(Level);

            if (UnityEntityManager.Instance.CheckIsEnemy(this, GameScene.Singleton.GetMyMainUnit()))
            {
                m_TopBar.GetComponent<UnityEntityTopBar>().SetIsEnemy(true);
            }
            else
            {
                m_TopBar.GetComponent<UnityEntityTopBar>().SetIsEnemy(false);
            }
        }
    }
    protected int m_UnitType;
    public int UnitType
    {
        get
        {
            return m_UnitType;
        }
        set
        {
            if (m_UnitType == value)
            {
                return;
            }

            m_UnitType = value;
            Debug.Log("m_UnitType-:" + m_UnitType);
            if (m_TopBar != null)
            {
                GameObject.Destroy(m_TopBar);
            }
            if (m_UnitType == 1)
            {
                m_TopBar = (GameObject)(GameObject.Instantiate(Resources.Load("UIPref/HeroTopBar")));
                //m_Mode.

                m_TopBar.transform.parent = m_Mode.transform;

                m_TopBar.transform.position = new Vector3(m_TopBar.transform.position.x, m_MeshHeight, m_TopBar.transform.position.z) ;
                //m_MeshHeight
            }
            else
            {
                m_TopBar = (GameObject)(GameObject.Instantiate(Resources.Load("UIPref/NpcTopBar")));
                //m_Mode.

                m_TopBar.transform.parent = m_Mode.transform;

                m_TopBar.transform.position = new Vector3(m_TopBar.transform.position.x, m_MeshHeight, m_TopBar.transform.position.z);

            }

            UpdateTopBar();
        }
    }
    // AttackAcpabilities
    protected int m_AttackAcpabilities;
    public int AttackAcpabilities
    {
        get
        {
            return m_AttackAcpabilities;
        }
        set
        {
            m_AttackAcpabilities = value;
        }
    }

    // AttackMode
    protected int m_AttackMode;
    public int AttackMode
    {
        get
        {
            return m_AttackMode;
        }
        set
        {
            m_AttackMode = value;
        }
    }

    // x
    protected float m_X;
    public float X
    {
        get
        {
            return m_X;
        }
        set
        {
            m_X = value;
        }
    }
    // x
    protected float m_Y;
    public float Y
    {
        get
        {
            return m_Y;
        }
        set
        {
            m_Y = value;
        }
    }
    // x
    protected float m_DirectionX;
    public float DirectionX
    {
        get
        {
            return m_DirectionX;
        }
        set
        {
            m_DirectionX = value;
        }
    }
    // x
    protected float m_DirectionY;
    public float DirectionY
    {
        get
        {
            return m_DirectionY;
        }
        set
        {
            m_DirectionY = value;
        }
    }
    //


}
