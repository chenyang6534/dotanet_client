
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityEntity {

    protected GameScene m_Scene;

    public UnityEntity(Protomsg.UnitDatas data, GameScene scene)
    {
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



        m_Mode.transform.position = new Vector3(data.X, 0, data.Y);

        

    }

    public void Change(Protomsg.UnitDatas data)
    {
        // 更新位置
        if( m_Mode != null)
        {
            Vector3 add = new Vector3(data.X, 0, data.Y);
            m_Mode.transform.position = m_Mode.transform.position + add;
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



}
