using ExcelData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//buff 特效
public class BuffEffect
{
    public int TypeID;
    public BuffItem BIdata;
    List<GameObject> ModeEffect = new List<GameObject>();
    public UnityEntity ParentEntity;
    //public 
    public BuffEffect(int typeid, UnityEntity parent)
    {
        TypeID = typeid;
        BIdata = ExcelManager.Instance.GetBuffIM().GetBIByID(typeid);
        ParentEntity = parent;
        Init();
    }
    public void Init()
    {
        if(ParentEntity == null || ParentEntity.Mode == null)
        {
            return;
        }
        Debug.Log("init:"+ BIdata.BodyEffect);
        //模型身上的特效
        if (BIdata.BodyEffect.Length > 0)
        {
            var modeeffect = (GameObject)(GameObject.Instantiate(Resources.Load(BIdata.BodyEffect)));
            if (modeeffect != null)
            {
                modeeffect.transform.parent = ParentEntity.Mode.transform;
                modeeffect.transform.position = ParentEntity.Mode.transform.position;
                Debug.Log("modeeffect:" + modeeffect);

                ModeEffect.Add(modeeffect);
            }
        }
        //隐藏模型
        //ParentEntity.Mode.SetActive(false);
        if(BIdata.Enable == 0)
        {
            ParentEntity.Mode.SetActive(false);
        }
    }
    public void Delete()
    {
        if (BIdata.Enable == 0)
        {
            ParentEntity.Mode.SetActive(true);
        }
        Debug.Log("Delete:" + TypeID);
        foreach (GameObject p in ModeEffect)
        {
            Object.Destroy(p);
        }
        //ParentEntity.Mode.SetActive(true);

    }

    public static BuffEffect CreateBuffEffect(int typeid, UnityEntity parent)
    {
        var data = ExcelManager.Instance.GetBuffIM().GetBIByID(typeid);
        if(data == null || parent == null || parent.Mode == null)
        {
            return null;
        }
        return new BuffEffect(typeid, parent);
    }
}
