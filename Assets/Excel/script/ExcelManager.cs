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
    protected TalentManager TalentManager = Resources.Load<TalentManager>("Conf/Talent");
    protected AoShuManager AoShuManager = Resources.Load<AoShuManager>("Conf/AoShu");

    protected SceneManager SceneManager = Resources.Load<SceneManager>("Conf/Scene");
    protected UnitInfoManager UnitInfoManager = Resources.Load<UnitInfoManager>("Conf/UnitInfo");
    protected NoticeWordsManager NoticeWordsManager = Resources.Load<NoticeWordsManager>("Conf/NoticeWords");

    private ExcelManager()
    {
        //Debug.Log("ExcelManager init");
        //m_BIM.Init();
        BulletIM.Init();
        BuffIM.Init();
        ItemManager.Init();
        SkillManager.Init();
        TalentManager.Init();
        AoShuManager.Init();
        SceneManager.Init();
        //Debug.Log("1111ExcelManager init");
        UnitInfoManager.Init();
        //Debug.Log("2222ExcelManager init");
        NoticeWordsManager.Init();
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
    public TalentManager GetTalentManager()
    {
        return TalentManager;
    }
    public AoShuManager GetAoShuManager()
    {
        return AoShuManager;
    }

    public SceneManager GetSceneManager()
    {
        return SceneManager;
    }
    public UnitInfoManager GetUnitInfoManager()
    {
        return UnitInfoManager;
    }
    public NoticeWordsManager GetNoticeWordsManager()
    {
        return NoticeWordsManager;
    }
}
