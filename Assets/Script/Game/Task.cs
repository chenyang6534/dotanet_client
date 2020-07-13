using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.Threading;
using System;

public class Task
{
    private GComponent main;
    private bool IsGetTask2;
    private bool IsGetTask3;
    public Task()
    {

        UMengManager.Instanse.Event_click_taskbtn();

        MsgManager.Instance.AddListener("SC_GetTask", new HandleMsg(this.SC_GetTask));

        main = UIPackage.CreateObject("GameUI", "AllTask").asCom;
        GRoot.inst.AddChild(main);
        main.xy = Tool.GetPosition(0.5f, 0.5f);
        main.GetChild("close").asButton.onClick.Add(() =>
        {
            this.Destroy();
        });

        Protomsg.CS_GetTask msg = new Protomsg.CS_GetTask();
        msg.TaskType = 1;//任务类型(1:主线 2:日常)
        MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetTask", msg);


        main.GetChild("zhuxian").asButton.onClick.Add(() =>
        {
            //Protomsg.CS_GetTask msg1 = new Protomsg.CS_GetTask();
            //msg1.TaskType = 1;//任务类型(1:主线 2:日常)
            //MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetTask", msg1);
        });
        main.GetChild("richang").asButton.onClick.Add(() =>
        {
            if (IsGetTask2)
            {
                return;
            }

            Protomsg.CS_GetTask msg1 = new Protomsg.CS_GetTask();
            msg1.TaskType = 2;//任务类型(1:主线 2:日常)
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetTask", msg1);
            IsGetTask2 = true;
        });
        main.GetChild("gan").asButton.onClick.Add(() =>
        {
            if (IsGetTask3)
            {
                return;
            }

            Protomsg.CS_GetTask msg1 = new Protomsg.CS_GetTask();
            msg1.TaskType = 3;//任务类型(1:主线 2:日常 3:肝)
            MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetTask", msg1);
            IsGetTask3 = true;
        });
    }
  

    //进入公会地图
    public bool SC_GetTask(Protomsg.MsgBase d1)
    {
        Debug.Log("SC_GetTask:");
        IMessage IMperson = new Protomsg.SC_GetTask();
        Protomsg.SC_GetTask p1 = (Protomsg.SC_GetTask)IMperson.Descriptor.Parser.ParseFrom(d1.Datas);

        var tasklist = main.GetChild("zhuxianlist").asList;
        if(p1.TaskType == 2)
        {
            tasklist = main.GetChild("richanglist").asList;
        }else if(p1.TaskType == 3)
        {
            tasklist = main.GetChild("ganlist").asList;
        }
        tasklist.RemoveChildren(0, -1, true);

        Protomsg.OneTaskMsg[] allplayer = new Protomsg.OneTaskMsg[p1.AllTask.Count];
        p1.AllTask.CopyTo(allplayer, 0);
        //完成未领取排在最前面 已经领取排在最后面
        foreach (var item in allplayer)
        {
            if(item.State == 2)
            {
                item.Sort += 10000;
            }else if(item.State == 1)
            {
                item.Sort -= 10000;
            }
        }
        System.Array.Sort(allplayer, (a, b) => {

            if (a.Sort > b.Sort)
            {
                return 1;
            }
            else if (a.Sort < b.Sort)
            {
                return -1;
            }
            return 0;
        });

        //遍历
        foreach (var item in allplayer)
        {
            
            var onedropitem = UIPackage.CreateObject("GameUI", "TaskOne").asCom;

            //onedropitem.GetChild("item").asCom.GetChild("icon").asLoader.url = clientitem.IconPath;
            onedropitem.GetChild("taskname").asTextField.text = item.Name;
            onedropitem.GetChild("des").asTextField.text = item.Des;

            onedropitem.GetChild("jindustate").asTextField.text = item.CurCount + "/" + item.Count;
            //状态 0表示未完成 1表示已经完成未领取奖励 2表示已经领取奖励()
            if (item.State == 2)
            {
                onedropitem.GetChild("jindustate").asTextField.text = "已领取";
                onedropitem.grayed = true;
            }
            //进入
            onedropitem.GetChild("get").asButton.onClick.Add(() =>
            {
                //领取奖励
                Protomsg.CS_GetTaskReward msg1 = new Protomsg.CS_GetTaskReward();
                msg1.ID = item.ID;
                MyKcp.Instance.SendMsg(GameScene.Singleton.m_ServerName, "CS_GetTaskReward", msg1);
            });
            if(item.State == 1)
            {
                onedropitem.GetChild("get").visible = true;
                onedropitem.GetChild("jindustate").asTextField.text = "";
            }
            else
            {
                onedropitem.GetChild("get").visible = false;
            }
            //奖励

            onedropitem.GetChild("rewardslist").asList.RemoveChildren(0, -1, true);
            var itemarr = item.RewardsStr.Split(';');
            foreach (var item2 in itemarr)
            {
                Debug.Log("item2:" + item2);
                var itemdata = item2.Split(':');
                if (itemdata.Length <= 2)
                {
                    continue;
                }
                var itemid = int.Parse(itemdata[0]);
                var itemcount = int.Parse(itemdata[1]);
                var itemlevel = itemdata[2];

                var clientitemone = ExcelManager.Instance.GetItemManager().GetItemByID(itemid);
                if (clientitemone == null)
                {
                    continue;
                }

                var threedropitem = UIPackage.CreateObject("GameUI", "sellable").asCom;
                threedropitem.GetChild("icon").asLoader.url = clientitemone.IconPath;
                threedropitem.GetChild("icon").onClick.Add(() =>
                {
                    new ItemInfo(itemid);

                });
                threedropitem.GetChild("level").asTextField.text = "Lv."+itemlevel;
                if(clientitemone.ShowLevel == 1)
                {
                    threedropitem.GetChild("level").visible = true;
                }
                else
                {
                    threedropitem.GetChild("level").visible = false;
                }
                threedropitem.GetChild("count").asTextField.text = itemcount+"";
                if(itemcount <= 1)
                {
                    threedropitem.GetChild("count").visible = false;
                }
                else
                {
                    threedropitem.GetChild("count").visible = true;
                }
                onedropitem.GetChild("rewardslist").asList.AddChild(threedropitem);
            }
            

            tasklist.AddChild(onedropitem);

        }
        
        return true;
    }
    


    //
    public void Destroy()
    {
        MsgManager.Instance.RemoveListener("SC_GetTask");
        
        AudioManager.Am.Play2DSound(AudioManager.Sound_CloseUI);
        if (main != null)
        {
            main.Dispose();
        }
    }

    
}
