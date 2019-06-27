using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExcelData;

public class BulletEntityManager
{
    private static readonly BulletEntityManager _instance = new BulletEntityManager();
    public static BulletEntityManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private BulletEntityManager()
    {
        m_BIM.Init();
    }



    protected Dictionary<int, BulletEntity> m_BulletEntitys = new Dictionary<int, BulletEntity>();

    protected BulletItemManager m_BIM = Resources.Load<BulletItemManager>("Conf/BulletItem");

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
            var bulletitem = m_BIM.GetBIByID(int.Parse(item));
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


    //创建新单位
    public void CreateBulletEntity(GameScene gs, Protomsg.BulletDatas data)
    {
        BulletEntity hero = new BulletEntity(data, gs);
        m_BulletEntitys[data.ID] = hero;
    }
    //删除单位
    public void DestroyBulletEntity(int id)
    {
        BulletEntity unity = m_BulletEntitys[id];
        if( unity != null)
        {
            unity.Destroy();
            m_BulletEntitys.Remove(id);
        }

    }

    //更改单位数据
    public void ChangeBulletEntity(Protomsg.BulletDatas data)
    {
        BulletEntity unity = m_BulletEntitys[data.ID];
        if (unity != null)
        {
            unity.Change(data);
        }
    }

    //更改显示位置
    public void ChangeShowPos(Protomsg.BulletDatas data, float scale)
    {
        BulletEntity unity = m_BulletEntitys[data.ID];
        if (unity != null)
        {
            unity.ChangeShowPos(scale,new Vector3(data.X,data.Y,data.Z));
        }
    }

    //通过ID获取单位
    public BulletEntity GetUnityEntity(int id)
    {
        return m_BulletEntitys[id];
    }
    

    //清除所有单位
    public void Clear()
    {
        foreach (var item in m_BulletEntitys)
        {
            item.Value.Destroy();
        }
        m_BulletEntitys.Clear();
    }
    
    

    //获取所有单位
    public Dictionary<int, BulletEntity> GetAllUnity()
    {
        return m_BulletEntitys;
    }

    


}
