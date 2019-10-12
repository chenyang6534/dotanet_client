
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEntity
{

    protected GameScene m_Scene;
    protected Vector3 m_StartPos;
    protected Vector3 m_Position;
    protected Vector3 m_EndPos;
    protected int m_State;////子弹状态(1:创建,2:移动,3:到达后计算结果(伤害和回血) 4:完成 可以删除了)
    protected string m_ModeType;//
    protected int m_ID;
    protected GameObject m_Mode;

    public BulletEntity(Protomsg.BulletDatas data, GameScene scene)
    {
        //Debug.Log("----------------11time:" + Tool.GetTime());

        //数据赋值
        m_Scene = scene;
        m_ID = data.ID;
        m_Position = new Vector3(data.X, data.Z, data.Y);
        m_StartPos = new Vector3(data.StartX, data.StartZ, data.StartY);
        m_EndPos = new Vector3(data.EndX, data.EndZ, data.EndY);
        m_State = data.State;

        m_ModeType = BulletEntityManager.Instance.GetBulletModePath(data.ModeType);
        //Debug.Log("ddd------------------------22time:" + Tool.GetTime());
        //数据处理
        m_Mode = (GameObject)(GameObject.Instantiate(Resources.Load(m_ModeType)));
        m_Mode.transform.parent = m_Scene.transform.parent;
        m_Mode.transform.position = m_Position;
        m_Mode.transform.LookAt(m_EndPos);
        //Debug.Log("aaa------------------------22time:" + Tool.GetTime());
        if (m_State == 1)
        {
            m_Mode.GetComponent<ProjectileScript>().ShowStartParticle(m_StartPos, m_EndPos);
            //Debug.Log("bbb------------------------22time:" + Tool.GetTime());
            //Debug.Log("11start:" + m_StartPos+ "pos:" + m_Position + "end:" + m_EndPos);
        }else if(m_State == 3)
        {
            //Quaternion rotation = Quaternion.FromToRotation(m_StartPos, m_EndPos);
            m_Mode.GetComponent<ProjectileScript>().ShowEndParticle(m_EndPos- m_StartPos);
            
        }
        //Debug.Log("------------------------22time:" + Tool.GetTime());

        //Debug.Log("1111bullet pos:" + m_Mode.transform.position + "  end pos:" + m_EndPos + "   state:" + m_State);
    }

    public Vector3 GetPosition()
    {
        float distanse1 = Vector3.Distance(m_EndPos, m_StartPos);
        float distanse2 = Vector3.Distance(m_StartPos, m_Position);
        float height = 1.5f;
        var addy = (0.5f - Math.Abs(distanse2 / distanse1 - 0.5)) * height * 2.0f;
        Debug.Log("addy:" + addy + "dis1 :" + distanse1 + " dis2:" + distanse2 + " abs:" + Math.Abs(distanse2 / distanse1 - 0.5));
        var pos = new Vector3(m_Position.x, m_Position.y + (float)addy, m_Position.z);

        return pos;
    }

    public void Change(Protomsg.BulletDatas data)
    {
        // 更新位置
        if (m_Mode != null)
        {
            //Vector3 add = new Vector3(data.X, 0, data.Y);
            m_Position += new Vector3(data.X, data.Z, data.Y);
            m_StartPos += new Vector3(data.StartX, data.StartZ, data.StartY);
            m_EndPos += new Vector3(data.EndX, data.EndZ, data.EndY);
            m_State += data.State;

            //var pos = GetPosition();
            //m_Position.y += (float)addy;
            //更新位置

            if (m_State == 3)
            {
                //Quaternion rotation = Quaternion.FromToRotation(m_StartPos, m_EndPos);
                m_Mode.GetComponent<ProjectileScript>().ShowEndParticle(m_EndPos - m_StartPos);
                //Debug.Log("22start:" + m_StartPos + "pos:" + m_Position + "end:" + m_EndPos);
            }

            m_Mode.transform.LookAt(m_Position);
            //Debug.Log("111 rotation:" + m_Mode.transform.localRotation);
            m_Mode.transform.position = m_Position;

            m_Mode.GetComponent<ProjectileScript>().ChangePos(m_StartPos, m_Position);
            

            

            //Debug.Log("2222bullet pos:" + m_Mode.transform.position+"  end pos:"+ m_EndPos+"   state:"+m_State);
        }
    }
    public void ChangeShowPos(float scale,Vector3 next)
    {
        if (m_Mode != null)
        {
            //var pos = GetPosition();
            m_Mode.transform.position = new Vector3(m_Position.x + (scale * next.x), m_Position.y + (scale * next.z), m_Position.z + (scale * next.y));
            //m_Mode.transform.position = new Vector3(m_Position.x + (scale * next.x), m_Mode.transform.position.y, m_Position.z + (scale * next.y));
            //Debug.Log("3333bullet pos:" + m_Mode.transform.position + "  end pos:" + m_EndPos);
        }
    }

    public void Destroy()
    {
        if (m_Mode != null)
        {
            //Debug.Log("4444bullet pos:" + m_Mode.transform.position);
            m_Mode.GetComponent<ProjectileScript>().DestroyProject();


        }
    }
}
