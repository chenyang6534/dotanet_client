using ExcelData;
using FairyGUI;
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
    public GObject TopBarObj;

    public Protomsg.BuffDatas Data;
    //public 
    public BuffEffect(Protomsg.BuffDatas buffdata, UnityEntity parent)
    {
        Data = buffdata;
        TypeID = buffdata.TypeID;
        BIdata = ExcelManager.Instance.GetBuffIM().GetBIByID(buffdata.TypeID);
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
        ////模型脚底的特效
        //Debug.Log("init:" + BIdata.FootEffect);
        //if (BIdata.FootEffect.Length > 0)
        //{
        //    var modeeffect = (GameObject)(GameObject.Instantiate(Resources.Load(BIdata.FootEffect)));
        //    if (modeeffect != null)
        //    {
        //        modeeffect.transform.parent = ParentEntity.Mode.transform;
        //        modeeffect.transform.position = ParentEntity.Mode.transform.position;
        //        Debug.Log("modeeffect:" + modeeffect);

        //        ModeEffect.Add(modeeffect);
        //    }
        //}
        //头顶
        if (BIdata.HeadEffect.Length > 0)
        {
            var modeeffect = (GameObject)(GameObject.Instantiate(Resources.Load(BIdata.HeadEffect)));
            if (modeeffect != null)
            {
                modeeffect.transform.parent = ParentEntity.Mode.transform;
                modeeffect.transform.position = ParentEntity.Mode.transform.position+new Vector3(0, ParentEntity.MeshHeight, 0);
                Debug.Log("modeeffect:" + modeeffect);

                ModeEffect.Add(modeeffect);
            }
        }
        //手上的特效
        if (BIdata.HandsEffect.Length > 0)
        {
            Debug.Log("hand:" + BIdata.HandsEffect);
            var bodypart = ParentEntity.Mode.GetComponent<BodyPart>();
            if (bodypart != null)
            {
                Debug.Log("1111:");
                foreach (var item in bodypart.Hands)
                {
                    Debug.Log("222:");
                    var modeeffect = (GameObject)(GameObject.Instantiate(Resources.Load(BIdata.HandsEffect)));
                    if (modeeffect != null)
                    {
                        //modeeffect.transform.parent = item.transform;
                        //modeeffect.transform.localScale = new Vector3(1, 1, 1);
                        var fol = modeeffect.AddComponent<FollowTrans>();
                        fol.FollowTarget = item.transform;
                        //item.

                        //modeeffect.transform.parent = item.transform;
                        //modeeffect.transform.position = item.transform.position;
                        //modeeffect.transform.localScale = new Vector3(1, 1, 1);
                        //Debug.Log("scale:" + modeeffect.transform.localScale+"  :"+ item.transform.localScale);

                        ModeEffect.Add(modeeffect);
                    }
                }
            }
           
        }

        //topbar
        if (BIdata.TopBarEffect.Length > 0)
        {
            if(ParentEntity != null)
            {
                var topbar = ParentEntity.GetTopBar();
                TopBarObj = topbar.AddBuff(BIdata.TopBarEffect);
            }
        }
        //NativeTopBarEffect
        if (BIdata.NativeTopBarEffect.Length > 0)
        {
            if (ParentEntity != null)
            {
                var topbar = ParentEntity.GetTopBar();
                TopBarObj = topbar.AddNativeBuff(BIdata.NativeTopBarEffect);
            }
        }

        //变色特效
        this.CreateEntitySpecial();

        //OutEffect
        if (BIdata.OutEffect.Length > 0)
        {
            var modeeffect = (GameObject)(GameObject.Instantiate(Resources.Load(BIdata.OutEffect)));
            if (modeeffect != null)
            {
                //modeeffect.transform.parent = ParentEntity.Mode.transform;
                modeeffect.transform.position = ParentEntity.Mode.transform.position;
                Debug.Log("modeeffect:" + modeeffect);

                ModeEffect.Add(modeeffect);
            }
        }

        //隐藏模型
        if (BIdata.Enable == 0)
        {
            ParentEntity.Mode.SetActive(false);
        }

        UpdateData();
    }
    public void CreateEntitySpecial()
    {
        var es = ParentEntity.Mode.GetComponent<UnityEntitySpecial>();
        if(es == null)
        {
            return;
        }
        switch (BIdata.RenderColor)
        {
            case 1:
                es.AddWhite();
                break;
            case 2:
                es.AddGreen();
                break;
            case 3:
                es.AddAngerRed();
                break;
        }
    }
    public void RemoveEntitySpecial()
    {
        if(ParentEntity == null || ParentEntity.Mode == null)
        {
            return;
        }
        var es = ParentEntity.Mode.GetComponent<UnityEntitySpecial>();
        if (es == null)
        {
            return;
        }
        switch (BIdata.RenderColor)
        {
            case 1:
                es.RemoveWhite();
                break;
            case 2:
                es.RemoveGreen();
                break;
            case 3:
                es.RemoveAngerRed();
                break;
        }
    }

    public void UpdateData()
    {
        //更新显示的数据
        foreach (GameObject p in ModeEffect)
        {
            MagicBeamScript magicbeam = p.GetComponent<MagicBeamScript>();
            if(magicbeam != null)
            {
                if( Data.ConnectionType != 0)
                {
                    magicbeam.ShootBeamInDir(ParentEntity.Mode.transform.position+new Vector3(0, ParentEntity.MeshHeight/2,0),
                        new Vector3(Data.ConnectionX, Data.ConnectionZ, Data.ConnectionY) + new Vector3(0, ParentEntity.MeshHeight / 2, 0));
                    //magicbeam.ShootBeamInDir(ParentEntity.Mode.transform.position,
                    //    new Vector3(Data.ConnectionX, 2, Data.ConnectionY));
                }
                
            }           
        }
    }

    public void FreshData(Protomsg.BuffDatas buffdata)
    {
        Data = buffdata;
        UpdateData();
    }
    public void Delete()
    {
        //ParentEntity.Mode.GetComponent<UnityEntitySpecial>().RemoveGreen();
        RemoveEntitySpecial();
        if (BIdata.Enable == 0)
        {
            ParentEntity.Mode.SetActive(true);
        }
        if (TopBarObj != null)
        {
            if (ParentEntity != null)
            {
                var topbar = ParentEntity.GetTopBar();
                topbar.RemoveBuff(TopBarObj);
            }
        }
        Debug.Log("Delete:" + TypeID);
        foreach (GameObject p in ModeEffect)
        {
            Object.Destroy(p);
        }



        //ParentEntity.Mode.SetActive(true);

    }

    public static BuffEffect CreateBuffEffect(Protomsg.BuffDatas buffdata, UnityEntity parent)
    {
        var data = ExcelManager.Instance.GetBuffIM().GetBIByID(buffdata.TypeID);
        if(data == null || parent == null || parent.Mode == null)
        {
            return null;
        }
        return new BuffEffect(buffdata, parent);
    }
}
