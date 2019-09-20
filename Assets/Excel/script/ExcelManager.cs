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
    protected ItemManager ItemManager = Resources.Load<ItemManager>("Conf/Item");
    protected SkillManager SkillManager = Resources.Load<SkillManager>("Conf/Skill");
    protected SceneManager SceneManager = Resources.Load<SceneManager>("Conf/Scene");
    private ExcelManager()
    {
        //m_BIM.Init();
        BulletIM.Init();
        BuffIM.Init();
        ItemManager.Init();
        SkillManager.Init();
        SceneManager.Init();
    }

    public BulletItemManager GetBulletIM()
    {
        return BulletIM;
    }
    public BuffItemManager GetBuffIM()
    {
        return BuffIM;
    }
    public ItemManager GetItemManager()
    {
        return ItemManager;
    }
    public SkillManager GetSkillManager()
    {
        return SkillManager;
    }
    public SceneManager GetSceneManager()
    {
        return SceneManager;
    }
}
