using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityEntityManager  {
    private static readonly UnityEntityManager _instance = new UnityEntityManager();
    public static UnityEntityManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private UnityEntityManager()
    {

    }



    protected Dictionary<int, UnityEntity> m_UnityEntitys = new Dictionary<int, UnityEntity>();

    //创建新单位
    public void CreateUnityEntity(GameScene gs, Protomsg.UnitDatas data)
    {
        UnityEntity hero = new UnityEntity(data, gs);
        m_UnityEntitys[hero.ID] = hero;
    }
    //删除单位
    public void DestroyUnityEntity(int id)
    {
        UnityEntity unity = m_UnityEntitys[id];
        if( unity != null)
        {
            unity.Destroy();
            m_UnityEntitys.Remove(id);
        }

    }

    //更改单位数据
    public void ChangeUnityEntity(Protomsg.UnitDatas data)
    {
        UnityEntity unity = m_UnityEntitys[data.ID];
        if (unity != null)
        {
            unity.Change(data);
        }
    }

    //更改显示位置
    public void ChangeShowPos(Protomsg.UnitDatas data, float scale)
    {
        UnityEntity unity = m_UnityEntitys[data.ID];
        if (unity != null)
        {
            unity.ChangeShowPos(scale,data.X,data.Y);
        }
    }

    //通过ID获取单位
    public UnityEntity GetUnityEntity(int id)
    {
        return m_UnityEntitys[id];
    }

    //检查单位是否是 玩家的敌人
    public bool CheckIsEnemy(UnityEntity target, UnityEntity my)
    {
        if(my == null || target == null)
        {
            return false;
        }
        //攻击模式(1:和平模式 2:组队模式 3:全体模式 4:阵营模式(玩家,NPC) 5:行会模式)
        if (my.AttackMode == 1)
        {
            //单位类型(1:英雄 2:普通单位 3:远古 4:boss)
            //不是英雄单位且不是玩家控制的单位
            if (target.UnitType != 1 && target.ControlID < 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if(my.AttackMode == 3)
        {
            //不是玩家自己控制的单位
            if (target.ControlID != my.ControlID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    //清除所有单位
    public void Clear()
    {
        foreach (var item in m_UnityEntitys)
        {
            item.Value.Destroy();
        }
        m_UnityEntitys.Clear();
    }
    //更新
    public void Update(float dt)
    {
        foreach (var item in m_UnityEntitys)
        {
            item.Value.Update(dt);
        }
    }

    //获取所有单位
    public Dictionary<int, UnityEntity> GetAllUnity()
    {
        return m_UnityEntitys;
    }

    //获取离本单位 夹角最小的敌人 且在distanse范围内 且夹角小于45度
    float MaxAngle = 45;
    public UnityEntity GetMinAngleEnemy(UnityEntity my,Vector2 dir,float maxDis)
    {
        if( my == null || dir == Vector2.zero || maxDis <= 0)
        {
            return null;
        }
        float minangle = 0;
        UnityEntity minangelUnityEntity = null;
        foreach (var item in m_UnityEntitys)
        {
            if (item.Key == my.ID)
            {
                continue;
            }
            //是否是敌人
            if (CheckIsEnemy(item.Value, my) == false)
            {
                continue;
            }

            var dis = Vector2.Distance(new Vector2(item.Value.X, item.Value.Y), new Vector2(my.X, my.Y));

            if (maxDis > dis)
            {

                float angle1 = Vector2.Angle(dir, new Vector2(item.Value.X-my.X,item.Value.Y-my.Y));

                //是否超过最大角度
                if(angle1 > MaxAngle)
                {
                    continue;
                }

                if(minangelUnityEntity == null)
                {
                    minangelUnityEntity = item.Value;
                    minangle = angle1;
                }
                else if(angle1 < minangle)
                {
                    minangelUnityEntity = item.Value;
                    minangle = angle1;
                }
            }
        }

        return minangelUnityEntity;

    }

    //获取离本单位最近的敌人
    public UnityEntity GetNearestEnemy(UnityEntity my)
    {
        var myunityentity = my;
        if (myunityentity == null)
        {
            Debug.Log("GetNearestEnemy  null");
            return null;
        }
        var mindis = 100000000.0f;
        UnityEntity nearrestUnityEntity = null;
        foreach (var item in m_UnityEntitys)
        {

            if (item.Key == myunityentity.ID)
            {
                continue;
            }
            //是否是敌人 
            if(CheckIsEnemy(item.Value,myunityentity) == false)
            {
                continue;
            }

            var dis = Vector2.Distance(new Vector2(item.Value.X, item.Value.Y), new Vector2(myunityentity.X, myunityentity.Y));

            if (mindis > dis)
            {
                Debug.Log("dis  " + dis);
                mindis = dis;
                nearrestUnityEntity = item.Value;
            }
        }

        return nearrestUnityEntity;
    }
    

    //获取离本单位最近的单位
    public UnityEntity GetNearestUnityEntity(int id)
    {
        Debug.Log("GetNearestUnityEntity"+id);
        var myunityentity = m_UnityEntitys[id];
        if (myunityentity  == null)
        {
            Debug.Log("GetNearestUnityEntity  null");
            return null;
        }
        var mindis = 100000000.0f;
        UnityEntity nearrestUnityEntity = null;
        foreach (var item in m_UnityEntitys)
        {

            if(item.Key == id)
            {
                continue;
            }

            var dis = Vector2.Distance(new Vector2(item.Value.X, item.Value.Y), new Vector2(myunityentity.X, myunityentity.Y));

            if (mindis > dis)
            {
                Debug.Log("dis  "+dis);
                mindis = dis;
                nearrestUnityEntity = item.Value;
            }
        }

        return nearrestUnityEntity;


    }


}
