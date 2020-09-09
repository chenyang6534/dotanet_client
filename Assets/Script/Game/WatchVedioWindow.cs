using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.Threading;

public class WatchVedioWindow
{
    private GComponent main;

    public BtnCallBack UseCallBack;
    
    public WatchVedioWindow(string title,string des,string rewards)
    {

        init();

        main.GetChild("title").asTextField.text = title;
        main.GetChild("des").asTextField.text = des;
        showrewards(rewards);
    }
    public WatchVedioWindow(string rewards)
    {
        init();
        
        showrewards(rewards);
    }
    public WatchVedioWindow(string title,  string rewards)
    {

        init();

        main.GetChild("title").asTextField.text = title;
        showrewards(rewards);
    }
    public void init()
    {
        main = UIPackage.CreateObject("GameUI", "watchvediorewards").asCom;
        GRoot.inst.AddChild(main);
        Vector2 screenPos = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 logicScreenPos = GRoot.inst.GlobalToLocal(screenPos);
        main.xy = logicScreenPos;
    }
    public void showrewards(string rewards)
    {
        //奖励

        main.GetChild("rewardslist").asList.RemoveChildren(0, -1, true);
        var itemarr = rewards.Split(';');
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
            threedropitem.GetChild("level").asTextField.text = "Lv." + itemlevel;
            if (clientitemone.ShowLevel == 1)
            {
                threedropitem.GetChild("level").visible = true;
            }
            else
            {
                threedropitem.GetChild("level").visible = false;
            }
            threedropitem.GetChild("count").asTextField.text = itemcount + "";
            if (itemcount <= 1)
            {
                threedropitem.GetChild("count").visible = false;
            }
            else
            {
                threedropitem.GetChild("count").visible = true;
            }
            main.GetChild("rewardslist").asList.AddChild(threedropitem);
        }
    }
    
    public void SetBtnCallBack(BtnCallBack callback)
    {
        UseCallBack = callback;
        main.GetChild("close").asButton.onClick.Set(() =>
        {
            if (UseCallBack != null)
            {
                UseCallBack(0);
            }
            Destroy();
        });
        main.GetChild("no").asButton.onClick.Set(() =>
        {
            if (UseCallBack != null)
            {
                UseCallBack(2);
            }
            Destroy();
        });
        main.GetChild("get").asButton.onClick.Set(() =>
        {
            if (UseCallBack != null)
            {
                UseCallBack(1);
            }
            Destroy();
        });
    }

    public delegate void BtnCallBack(int code); //0取消 1观看 2不看
    

    //
    public void Destroy()
    {
        if (main != null)
        {
            main.Dispose();
        }
    }

    
}
