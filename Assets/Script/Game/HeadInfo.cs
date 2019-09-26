using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

public class HeadInfo{

    private GComponent maininfo;
    private int id;
    public HeadInfo(GComponent info)
    {
        maininfo = info;
        //id = unitid;
        Init();
        //FreshData(unitid);
        


    }

    public void Init()
    {
        
        maininfo.GetChild("headbtn").asButton.onClick.Add(() =>
        {
            //玩家自己
            if (GameScene.Singleton.GetMyMainUnit().ID == id)
            {
                UnityEntity unit = UnityEntityManager.Instance.GetUnityEntity(id);
                if (unit == null)
                {
                    return;
                }
                MyInfo myinfo = new MyInfo(unit);
                
            }
            else
            {
                UnityEntity unit = UnityEntityManager.Instance.GetUnityEntity(id);
                if (unit == null)
                {
                    return;
                }
                MyInfo myinfo = new MyInfo(unit);
            }
        });
        
        
    }

    public void FreshData(int unitid)
    {
        UnityEntity unit = UnityEntityManager.Instance.GetUnityEntity(unitid);
        if(unit == null)
        {
            maininfo.visible = false;
            
            return;
        }
        else
        {
            maininfo.visible = true;
            
        }

        if (id != unitid)
        {
            id = unitid;
            ////模型
            //var modeeffect = (GameObject)(GameObject.Instantiate(Resources.Load(unit.ModeType)));
            //modeeffect.transform.localPosition = new Vector3(0, 10, 1000);
            //modeeffect.transform.localScale = new Vector3(50, 50, 50);
            //Vector3 rotation = modeeffect.transform.localEulerAngles;
            //rotation.x = 10; // 在这里修改坐标轴的值
            //rotation.y = 180;
            //rotation.z = 0;
            ////将旋转的角度赋值给预制出来需要打出去的麻将
            //modeeffect.transform.localEulerAngles = rotation;
            //GGraph holder = maininfo.GetChild("head").asCom.GetChild("n1").asGraph;
            //GoWrapper wrapper = new GoWrapper();
            //wrapper.supportStencil = true;
            //wrapper.SetWrapTarget(modeeffect, true);
            //holder.SetNativeObject(wrapper);

            Debug.Log("---------unittype:" + unit.TypeID);

            var unitinfo = ExcelManager.Instance.GetUnitInfoManager().GetUnitInfoByID(unit.TypeID);
            if(unitinfo != null && unitinfo.IconPath.Length > 0)
            {
                Debug.Log("---------iconpath:" + unitinfo.IconPath);
                maininfo.GetChild("head").asCom.GetChild("head_icon").asLoader.url = unitinfo.IconPath;
            }

            
            //head_icon
        }
        //名字
        maininfo.GetChild("name").asTextField.text = unit.Name;
        //血量数字
        maininfo.GetChild("hp_num").asTextField.text = unit.HP+"/"+unit.MaxHP;
        //蓝量数字
        maininfo.GetChild("mp_num").asTextField.text = unit.MP + "/" + unit.MaxMP;
        //血量进度条
        maininfo.GetChild("hp").asProgress.value = (int)((float)unit.HP / unit.MaxHP * 100);
        //蓝量进度条
        maininfo.GetChild("mp").asProgress.value = (int)((float)unit.MP / unit.MaxMP * 100);
        //等级数字
        maininfo.GetChild("level").asTextField.text = unit.Level+"";

        //经验条
        maininfo.GetChild("experience").asProgress.value = (int)((float)unit.Experience / (float)unit.MaxExperience * 100);





    }
}
