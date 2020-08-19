using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.Threading;
using System;

public class AoShuInfo
{
    private GComponent main;
    private bool IsMy;
    public AoShuInfo(int characterid)
    {
        //通过角色ID 获取角色信息
        MsgManager.Instance.AddListener("SC_GetAoShuInfo", new HandleMsg(this.SC_GetAoShuInfo));
       

        main = UIPackage.CreateObject("GameUI", "AoshuInfo").asCom;
        GRoot.inst.AddChild(main);
        main.xy = Tool.GetPosition(0.5f, 0.5f);
        main.GetChild("close").asButton.onClick.Add(() =>
        {
            this.Destroy();
        });
        //重置
        main.GetChild("buy").asButton.onClick.Add(() =>
        {
            if(GameScene.Singleton.m_MyMainUnit == null)
            {
                return;
            }
            if (GameScene.Singleton.m_MyMainUnit.CharacterID != characterid)
            {
                return;
            }

            Tool.NoticeWindonw("你确定要重置奥数吗？(奥数点数将100%返还)", () =>
            {
                Protomsg.CS_ResetAoShu msg2 = new Protomsg.CS_ResetAoShu();
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_ResetAoShu", msg2);
            });
        });

        Protomsg.CS_GetAoShuInfo msg1 = new Protomsg.CS_GetAoShuInfo();
        msg1.CharacterID = characterid;
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetAoShuInfo", msg1);
        IsMy = false;
        if (GameScene.Singleton.m_MyMainUnit != null)
        {
            if (characterid == GameScene.Singleton.m_MyMainUnit.CharacterID)
            {
                IsMy = true;
            }
        }

        if(IsMy == true)
        {
            main.GetController("c1").selectedPage = "my";
        }
        else
        {
            main.GetController("c1").selectedPage = "other";
        }
        
    }

    class aoshudata
    {
        public int id;
        public int state;
    }

    public bool SC_GetAoShuInfo(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetAoShuInfo:");
        IMessage IMperson = new Protomsg.SC_GetAoShuInfo();
        Protomsg.SC_GetAoShuInfo p1 = (Protomsg.SC_GetAoShuInfo)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);
        main.GetChild("myaoshu").asTextField.text = p1.AoShuCount+"";

        main.GetChild("pricetype").asLoader.url = Tool.GetPriceTypeIcon(p1.PriceType);
        main.GetChild("price").asTextField.text = p1.Price + "";

        main.GetChild("lilianglist").asList.RemoveChildren(0, -1, true);
        main.GetChild("minjielist").asList.RemoveChildren(0, -1, true);
        main.GetChild("zhililist").asList.RemoveChildren(0, -1, true);

        var count = 0;
        var items = p1.AoShuInfoStr.Split(';');
        for (var i = 0; i < items.Length; i++)
        {
            var oneitem = items[i].Split(',');
            if (oneitem.Length < 2)
            {
                continue;
            }
            count++;
        }
        aoshudata[] allplayer = new aoshudata[count];
        var index = 0;
        for (var i = 0; i < items.Length; i++)
        {
            var oneitem = items[i].Split(',');
            if (oneitem.Length < 2)
            {
                continue;
            }
            var itemid = int.Parse(oneitem[0]);
            var state = int.Parse(oneitem[1]);
            aoshudata d = new aoshudata();
            d.id = itemid;
            d.state = state;
            allplayer[index] = d;
            index++;

        }
        System.Array.Sort(allplayer, (a, b) => {

            if (a.id > b.id)
            {
                return 1;
            }
            else if (a.id < b.id)
            {
                return -1;
            }
            return 0;
        });

        //遍历
        foreach (var item in allplayer)
        {
            var clientitem = ExcelManager.Instance.GetAoShuManager().GetAoShuByID(item.id);
            if (clientitem == null)
            {
                continue;
            }

            var onedropitem = UIPackage.CreateObject("GameUI", "AoshuIcon").asCom;

            //onedropitem.GetChild("item").asCom.GetChild("icon").asLoader.url = clientitem.IconPath;
            onedropitem.GetChild("iconcom").asCom.GetChild("icon").asLoader.url = clientitem.IconPath;
            onedropitem.GetChild("iconcom").asCom.GetChild("icon").onClick.Add(() =>
            {
                new OneAoShuInfo(item.id,item.state, IsMy);
            });

            // 0:未解锁 1:已经解锁未激活 2:已经激活
            onedropitem.grayed = false;
            if (item.state == 0)
            {
                onedropitem.grayed = true;
            }
            else if (item.state == 1)
            {
                onedropitem.GetChild("iconcom").asCom.GetChild("icon").alpha = 0.2f;
            }
            else if (item.state == 2)
            {

            }

            if (clientitem.Type == 1)//力量
            {
                main.GetChild("lilianglist").asList.AddChild(onedropitem);
            }
            else if(clientitem.Type == 2)//敏捷
            {
                main.GetChild("minjielist").asList.AddChild(onedropitem);
            }
            else if (clientitem.Type == 3)//智力
            {
                main.GetChild("zhililist").asList.AddChild(onedropitem);
            }

            

        }


        return true;
    }


   

    //
    public void Destroy()
    {
        MsgManager.Instance.RemoveListener("SC_GetAoShuInfo");
        AudioManager.Am.Play2DSound(AudioManager.Sound_CloseUI);
        if (main != null)
        {
            main.Dispose();
        }
    }

    
}
