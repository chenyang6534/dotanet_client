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
    public void CreateUnityEntity(GameScene gs, Protomsg.UnitDatas data)
    {
        UnityEntity hero = new UnityEntity(data, gs);
        m_UnityEntitys[hero.ID] = hero;
    }
    public void DestroyUnityEntity(int id)
    {
        UnityEntity unity = m_UnityEntitys[id];
        if( unity != null)
        {
            unity.Destroy();
            m_UnityEntitys.Remove(id);
        }

    }
    public void ChangeUnityEntity(Protomsg.UnitDatas data)
    {
        UnityEntity unity = m_UnityEntitys[data.ID];
        if (unity != null)
        {
            unity.Change(data);
        }
    }
    public void ChangeShowPos(Protomsg.UnitDatas data, float scale)
    {
        UnityEntity unity = m_UnityEntitys[data.ID];
        if (unity != null)
        {
            unity.ChangeShowPos(scale,data.X,data.Y);
        }
    }

    public void Clear()
    {
        foreach (var item in m_UnityEntitys)
        {
            item.Value.Destroy();
        }
        m_UnityEntitys.Clear();
    }

    public Dictionary<int, UnityEntity> GetAllUnity()
    {
        return m_UnityEntitys;
    }


}
