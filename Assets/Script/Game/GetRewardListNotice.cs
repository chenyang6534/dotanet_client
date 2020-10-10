using cocosocket4unity;
using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using System.Threading;

public class GetRewardListNotice : MonoBehaviour
{
    private GComponent main;
    Protomsg.MailRewards[] data;
    bool isdes;
    public GetRewardListNotice(Protomsg.MailRewards[] d)
    {
        data = d;
        main = UIPackage.CreateObject("GameUI", "GetRewardListNotice").asCom;
        GRoot.inst.AddChild(main);
        main.xy = Tool.GetPosition(0.5f, 0.5f);
        isdes = false;
        Init();
    }
    
    //初始化
    public void Init()
    {
        if (main == null)
        {
            return;
        }
        //关闭按钮
        main.GetChild("ok").asButton.onClick.Set(() => { Destroy(); });
        main.GetChild("ok").visible = false;

        ////需要等级
        //main.GetChild("needlevel").asTextField.text = "";
        var k = 0;
        var jiange = 0.09f;
        var delaytime = 0.0f;
        foreach (var item in data)
        {
            var posx = 0.5f - (data.Length - 1) * jiange * 0.5f + jiange * k;
            delaytime = k * 0.2f;
            Debug.Log("AddItem:" + posx);
            GameScene.Singleton.StartCoroutine(AddItem(item, posx, delaytime));
            k++;
        }
        GameScene.Singleton.StartCoroutine(ShowCloseBtn(delaytime+0.1f));
    }

    public IEnumerator ShowCloseBtn(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        main.GetChild("ok").visible = true;


    }

    public IEnumerator AddItem(Protomsg.MailRewards p1,float posx, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if(isdes == false)
        {
            var teamrequest = UIPackage.CreateObject("GameUI", "Reward").asCom;
            teamrequest.SetPivot(0.5f, 0.5f, true);// = new Vector2(0.5f, 0.5f);
            teamrequest.xy = new Vector2(posx * main.width, 0.4f * main.height);//Tool.GetPosition(posx, 0.4f);
            main.AddChild(teamrequest);
            teamrequest.onClick.Add(() => {
                new ItemInfo(p1.ItemType, p1.ItemDBID, p1.Level);
            });
            var clientitem = ExcelManager.Instance.GetItemManager().GetItemByID(p1.ItemType);
            if (clientitem != null)
            {
                teamrequest.GetChild("icon").asLoader.url = clientitem.IconPath;
                if (clientitem.ShowLevel == 1)
                {
                    teamrequest.GetChild("level").visible = true;
                    teamrequest.GetChild("count").asTextField.text = "";
                }
                else
                {
                    teamrequest.GetChild("level").visible = false;
                    teamrequest.GetChild("count").asTextField.text = p1.Count + "";
                }
            }
            teamrequest.GetChild("level").asTextField.text = "Lv." + p1.Level + ""; 
        }

        

    }


    //
    public void Destroy()
    {
        isdes = true;
        if (main != null)
        {
            main.Dispose();
        }
    }

    
}
