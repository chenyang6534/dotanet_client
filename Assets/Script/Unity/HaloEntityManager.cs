using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExcelData;

public class HaloEntityManager
{
    private static readonly HaloEntityManager _instance = new HaloEntityManager();
    public static HaloEntityManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private HaloEntityManager()
    {
        //m_BIM.Init();
    }



    protected Dictionary<int, HaloEntity> m_HaloEntitys = new Dictionary<int, HaloEntity>();

    //protected BulletItemManager m_BIM = Resources.Load<BulletItemManager>("Conf/BulletItem");

    //获取子弹模型路径
    public string GetBulletModePath(string paths)
    {
        Debug.Log("GetBulletModePath:" + paths);
        if(paths.Length <= 0)
        {
            return "";
        }
        string[] arr1 = paths.Split(',');

        BulletItem modeitem = null;
        foreach (var item in arr1)
        {
            var bulletitem = ExcelManager.Instance.GetBulletIM().GetBIByID(int.Parse(item));
            if(bulletitem != null)
            {
                if(modeitem == null)
                {
                    modeitem = bulletitem;
                }
                else
                {
                    if(bulletitem.Level >= modeitem.Level)
                    {
                        modeitem = bulletitem;
                    }
                }
            }
        }
        if(modeitem == null)
        {
            return "";
        }
        else
        {
            return modeitem.ModePath;
        }
        
    }


    //创建新单位CreateHaloEntity
    public void CreateHaloEntity(GameScene gs, Protomsg.HaloDatas data)
    {
        HaloEntity hero = new HaloEntity(data, gs);
        m_HaloEntitys[data.ID] = hero;
    }
    //删除单位
    public void DestroyHaloEntity(int id)
    {
        HaloEntity unity = m_HaloEntitys[id];
        if( unity != null)
        {
            unity.Destroy();
            m_HaloEntitys.Remove(id);
        }

    }

    //更改单位数据
    public void ChangeHaloEntity(Protomsg.HaloDatas data)
    {
        HaloEntity unity = m_HaloEntitys[data.ID];
        if (unity != null)
        {
            unity.Change(data);
        }
    }

    //更改显示位置
    public void ChangeShowPos(Protomsg.HaloDatas data, float scale)
    {
        HaloEntity unity = m_HaloEntitys[data.ID];
        if (unity != null)
        {
            unity.ChangeShowPos(scale,new Vector3(data.X,data.Y,data.Z));
        }
    }

    //通过ID获取单位
    public HaloEntity GetHaloEntity(int id)
    {
        if (m_HaloEntitys.ContainsKey(id) == false)
        {
            return null;
        }
        return m_HaloEntitys[id];
    }
    

    //清除所有单位
    public void Clear()
    {
        foreach (var item in m_HaloEntitys)
        {
            item.Value.Destroy();
        }
        m_HaloEntitys.Clear();
    }
    
    

    //获取所有单位
    public Dictionary<int, HaloEntity> GetAllUnity()
    {
        return m_HaloEntitys;
    }

    


}
