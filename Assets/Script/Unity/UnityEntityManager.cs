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

    //清除所有单位
    public void Clear()
    {
        foreach (var item in m_UnityEntitys)
        {
            item.Value.Destroy();
        }
        m_UnityEntitys.Clear();
    }

    //获取所有单位
    public Dictionary<int, UnityEntity> GetAllUnity()
    {
        return m_UnityEntitys;
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
