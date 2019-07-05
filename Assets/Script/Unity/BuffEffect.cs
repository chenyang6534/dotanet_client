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
    public GameObject ParentMode;
    //public 
    public BuffEffect(int typeid, GameObject parent)
    {
        TypeID = typeid;
        BIdata = ExcelManager.Instance.GetBuffIM().GetBIByID(typeid);
        ParentMode = parent;
        Init();
    }
    public void Init()
    {
        Debug.Log("init:"+ BIdata.BodyEffect);
        if (BIdata.BodyEffect.Length > 0)
        {
            var modeeffect = (GameObject)(GameObject.Instantiate(Resources.Load(BIdata.BodyEffect)));
            if (modeeffect != null)
            {
                modeeffect.transform.parent = ParentMode.transform;
                modeeffect.transform.position = ParentMode.transform.position;
                Debug.Log("modeeffect:" + modeeffect);

                ModeEffect.Add(modeeffect);
            }
        }
    }
    public void Delete()
    {
        Debug.Log("Delete:" + TypeID);
        foreach (GameObject p in ModeEffect)
        {
            Object.Destroy(p);
        }
        
    }

    public static BuffEffect CreateBuffEffect(int typeid, GameObject parent)
    {
        var data = ExcelManager.Instance.GetBuffIM().GetBIByID(typeid);
        if(data == null)
        {
            return null;
        }
        return new BuffEffect(typeid, parent);
    }
}
