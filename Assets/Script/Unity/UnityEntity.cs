
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityEntity {

    protected GameScene m_Scene;

    protected float RotateSpeed;

    public UnityEntity(Protomsg.UnitDatas data, GameScene scene)
    {

        RotateSpeed = 180;

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
                Debug.Log("AniState: "+ anim);
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
                    Debug.Log("animlen: attack " + speed);
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

    //提前更新方向(关闭预判)
    public void PreLookAtDir(float x, float y)
    {
        if (m_Mode == null)
        {
            return;
        }
        var dir = new Vector3(x, 0, y);
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
            var t = 0.025f;
            endv.y = angle2;
            m_Mode.transform.DORotate(endv, t);
        }
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
            
            m_ModeType = value;
            if( m_Mode != null)
            {
                GameObject.Destroy(m_Mode);
            }
            m_Mode = (GameObject)(GameObject.Instantiate(Resources.Load(m_ModeType)));
            
            m_Mode.transform.parent = m_Scene.transform.parent;


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
