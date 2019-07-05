using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExcelData;
public class ExcelManager
{
    private static readonly ExcelManager _instance = new ExcelManager();
    public static ExcelManager Instance
    {
        get
        {
            return _instance;
        }
    }
    protected BulletItemManager BulletIM = Resources.Load<BulletItemManager>("Conf/BulletItem");
    protected BuffItemManager BuffIM = Resources.Load<BuffItemManager>("Conf/BuffItem");
    private ExcelManager()
    {
        //m_BIM.Init();
        BulletIM.Init();
        BuffIM.Init();
    }

    public BulletItemManager GetBulletIM()
    {
        return BulletIM;
    }
    public BuffItemManager GetBuffIM()
    {
        return BuffIM;
    }
}
