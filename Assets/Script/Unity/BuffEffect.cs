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

    //在单位身上 播放一次性特效
    public static void PlayEffectOnUnitEntity(string path,UnityEntity unit)
    {
        if(unit == null || path.Length <= 0)
        {
            return;
        }

        var modeeffect = (GameObject)(GameObject.Instantiate(Resources.Load(path)));
        if (modeeffect != null)
        {
            modeeffect.transform.parent = unit.Mode.transform;
            modeeffect.transform.position = unit.Mode.transform.position;
            //modeeffect.transform.rotation = ParentEntity.Mode.transform.rotation;
            var ps = modeeffect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                GameObject.Destroy(modeeffect, ps.main.duration);
            }
            else
            {
                GameObject.Destroy(modeeffect, 2);
            }
        }
    }

    public void Init()
    {
        if(ParentEntity == null || ParentEntity.Mode == null)
        {
            return;
        }

        Debug.Log("init:"+ BIdata.BodyEffect);


        if(BIdata.IsShowControl == 1)
        {
            ParentEntity.m_ControlBuffEffect = this;
        }

        //模型身上的特效
        if (BIdata.BodyEffect.Length > 0)
        {
            //ParentEntity.ModeType = "Hero/Sand_Pillar";
            //BIdata.BodyEffect = "Hero/Sand_Pillar";
            //ParentEntity.Mode.transform.FindChild("GameObject").gameObject.SetActive(false);

            var modeeffect = (GameObject)(GameObject.Instantiate(Resources.Load(BIdata.BodyEffect)));
            if (modeeffect != null)
            {
                modeeffect.transform.parent = ParentEntity.Mode.transform;
                modeeffect.transform.localPosition = Vector3.zero;
                modeeffect.transform.localRotation = Quaternion.identity;
                //modeeffect.transform.position = ParentEntity.Mode.transform.position;
                //modeeffect.transform.rotation = ParentEntity.Mode.transform.rotation;
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
                foreach (var item in bodypart.Hands)
                {
                    var modeeffect = (GameObject)(GameObject.Instantiate(Resources.Load(BIdata.HandsEffect)));
                    if (modeeffect != null)
                    {
                        var fol = modeeffect.AddComponent<FollowTrans>();
                        fol.FollowTarget = item.transform;
                        ModeEffect.Add(modeeffect);
                    }
                }
            }
           
        }

        //脚上的特效
        if (BIdata.FootsEffect.Length > 0)
        {
            Debug.Log("foot:" + BIdata.FootsEffect);
            var bodypart = ParentEntity.Mode.GetComponent<BodyPart>();
            if (bodypart != null)
            {
                foreach (var item in bodypart.Foots)
                {
                    var modeeffect = (GameObject)(GameObject.Instantiate(Resources.Load(BIdata.FootsEffect)));
                    if (modeeffect != null)
                    {
                        var fol = modeeffect.AddComponent<FollowTrans>();
                        fol.FollowTarget = item.transform;
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
                //Debug.Log("------- NativeTopBarEffect :" + BIdata.NativeTopBarEffect);
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
        //叠加材质特效
        if(BIdata.MaterialEffect.Length > 0)
        {
            var es = ParentEntity.Mode.GetComponent<UnityEntitySpecial>();
            if(es != null)
            {
                es.AddMat(BIdata.MaterialEffect);

                //es.AddWhite();
            }
        }

        //隐藏所有
        if (BIdata.Enable == 0)
        {
            ParentEntity.Mode.SetActive(false);
            //ParentEntity.Mode.renderer.enabled
        }

        //隐藏模型
        if (BIdata.ModeEnable == 0)
        {
            var go = ParentEntity.Mode.transform.Find("GameObject");
            if(go != null)
            {
                go.gameObject.SetActive(false);
            }
            
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
        //Debug.Log("------UpdateData---:");
        //如果自己是单位显示的控制技能特效
        if(ParentEntity.m_ControlBuffEffect == this)
        {
            ParentEntity.SetControlEffectTime((int)(Data.RemainTime / Data.Time * 100));
            
        }
        //更新显示的数据
        foreach (GameObject p in ModeEffect)
        {
            MagicBeamScript magicbeam = p.GetComponent<MagicBeamScript>();
            if(magicbeam != null)
            {
                Debug.Log("------magicbeam---:"+p+" "+ Data.ConnectionType);
                if ( Data.ConnectionType != 0)
                {
                    Debug.Log("------ShootBeamInDir---:" + Data.ConnectionX+" "+ Data.ConnectionZ + " " + Data.ConnectionY);
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

        if (ParentEntity.m_ControlBuffEffect == this)
        {
            ParentEntity.SetControlEffectTime(0);

        }


        RemoveEntitySpecial();
        if (BIdata.Enable == 0)
        {
            ParentEntity.Mode.SetActive(true);
        }
        //隐藏模型
        if (BIdata.ModeEnable == 0)
        {
            var go = ParentEntity.Mode.transform.Find("GameObject");
            if (go != null)
            {
                go.gameObject.SetActive(true);
            }

        }
        if (TopBarObj != null)
        {
            if (ParentEntity != null)
            {
                var topbar = ParentEntity.GetTopBar();
                topbar.RemoveBuff(TopBarObj);
            }
        }
        if(BIdata.MaterialEffect.Length > 0)
        {
            var es = ParentEntity.Mode.GetComponent<UnityEntitySpecial>();
            if (es != null)
            {
                es.RemoveMat(BIdata.MaterialEffect);
                //es.RemoveWhite();
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
