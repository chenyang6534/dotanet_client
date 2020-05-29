using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.Threading;

public class StoreInfo
{
    private GComponent main;
    
    public StoreInfo()
    {
        MsgManager.Instance.AddListener("SC_StoreData", new HandleMsg(this.SC_StoreData));
        //获取数据
        Protomsg.CS_GetStoreData msg1 = new Protomsg.CS_GetStoreData();
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetStoreData", msg1);
        AudioManager.Am.Play2DSound(AudioManager.Sound_OpenUI);

        main = UIPackage.CreateObject("GameUI", "StoreMain").asCom;
        GRoot.inst.AddChild(main);
        main.xy = Tool.GetPosition(0.5f,0.5f);
        //main.fairyBatching = true;

        main.GetChild("close").asButton.onClick.Add(() =>
        {
            this.Destroy();
        });
    }
    public bool SC_StoreData(Protomsg.MsgBase d1)
    {
        //Debug.Log("SC_NoticeWords:");
        IMessage IMperson = new Protomsg.SC_StoreData();
        Protomsg.SC_StoreData p1 = (Protomsg.SC_StoreData)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        main.GetChild("n3").asList.RemoveChildren();

        //排序
        //处理排序
        Protomsg.CommodityDataProto[] allplayer = new Protomsg.CommodityDataProto[p1.Commoditys.Count];
        int index = 0;
        foreach (var item in p1.Commoditys)
        {
            allplayer[index++] = item;
        }
        System.Array.Sort(allplayer,(a,b)=> {

            if(a.PriceType == b.PriceType)
            {
                if (a.Price > b.Price)
                {
                    return 1;
                }
                else if (a.Price == b.Price)
                {
                    return 0;
                }
                return -1;
            }else if(a.PriceType > b.PriceType)
            {
                return 1;
            }
            else
            {
                return -1;
            }

            
        });
        foreach ( var item in allplayer)
        {
            var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(item.ItemID);
            if(clientitem == null)
            {
                continue;
            }

            var onedropitem = UIPackage.CreateObject("GameUI", "Commodity").asCom;
            onedropitem.GetChild("n0").asLoader.url = clientitem.IconPath;
            onedropitem.GetChild("n3").asLoader.url = Tool.GetPriceTypeIcon(item.PriceType);
            onedropitem.GetChild("n0").onClick.Add(() =>
            {
                new ItemInfo(item.ItemID);
            });
            onedropitem.GetChild("n4").asTextField.text = item.Price + "";
            onedropitem.GetChild("level").asTextField.text = "Lv." + item.Level + "";
            onedropitem.GetChild("n1").asButton.onClick.Add(() =>
            {
                //购买
                Protomsg.CS_BuyCommodity msg1 = new Protomsg.CS_BuyCommodity();
                msg1.TypeID = item.TypeID;
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_BuyCommodity", msg1);
            });
            main.GetChild("n3").asList.AddChild(onedropitem);
        }
        return true;
    }


    //
    public void Destroy()
    {
        MsgManager.Instance.RemoveListener("SC_StoreData");
        AudioManager.Am.Play2DSound(AudioManager.Sound_CloseUI);
        if (main != null)
        {
            main.Dispose();
        }
    }

    
}
