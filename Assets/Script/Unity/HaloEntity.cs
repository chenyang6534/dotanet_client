
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaloEntity
{

    protected GameScene m_Scene;
    protected Vector3 m_Position;
    protected string m_ModeType;//
    protected int m_ID;
    protected GameObject m_Mode;

    public HaloEntity(Protomsg.HaloDatas data, GameScene scene)
    {
        //数据赋值
        m_Scene = scene;
        m_ID = data.ID;
        m_Position = new Vector3(data.X, data.Z, data.Y);
       

        m_ModeType = BulletEntityManager.Instance.GetBulletModePath(data.ModeType);

        //数据处理
        m_Mode = (GameObject)(GameObject.Instantiate(Resources.Load(m_ModeType)));
        m_Mode.transform.parent = m_Scene.transform.parent;
        m_Mode.transform.position = m_Position;
        //m_Mode.transform.LookAt(m_EndPos);
      
        //Debug.Log("1111bullet pos:" + m_Mode.transform.position + "  end pos:" + m_EndPos + "   state:" + m_State);
    }

    

    public void Change(Protomsg.HaloDatas data)
    {
        // 更新位置
        if (m_Mode != null)
        {
            //Vector3 add = new Vector3(data.X, 0, data.Y);
            m_Position += new Vector3(data.X, data.Z, data.Y);
           
            
            //更新位置
            //m_Mode.transform.LookAt(m_Position);
            m_Mode.transform.position = m_Position;
            

            

           
        }
    }
    public void ChangeShowPos(float scale,Vector3 next)
    {
        if (m_Mode != null)
        {
            m_Mode.transform.position = new Vector3(m_Position.x + (scale * next.x), m_Position.y + (scale * next.z), m_Position.z + (scale * next.y));
            
        }
    }

    public void Destroy()
    {
        if (m_Mode != null)
        {
            GameObject.Destroy(m_Mode);
            m_Mode = null;
        }
    }
}
