using FairyGUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Http;

public static class Tool {

	public static double GetTime()
    {
        var t1 = (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
        //var t1 = Time.realtimeSinceStartup;
        return t1;
    }

    public static string GetShowTime(int addhour,int addminute,int addsecond)
    {
        //Debug.Log(" :" + addhour + " :" + addminute + " :" + addsecond);
        var timenow = DateTime.Now;
        //Debug.Log("---:" + string.Format("{0:D2}:{1:D2}:{2:D2} ", timenow.Hour, timenow.Minute, timenow.Second));
        timenow = timenow.AddHours(addhour);
        timenow = timenow.AddMinutes(addminute);
        timenow = timenow.AddSeconds(addsecond);
        //CurrrentTimeText.text = string.Format("{0:D2}:{1:D2}:{2:D2} " + "{3:D4}/{4:D2}/{5:D2}", hour, minute, second, year, month, day);
        return string.Format("{0:D2}:{1:D2}:{2:D2} ", timenow.Hour, timenow.Minute, timenow.Second);
    }

    public static string Set2(int t)
    {
        if(t < 10)
        {
            return "0" + t;
        }
        return t+"";
    }
    public static string Time2String(int time)
    {
        //Debug.Log("Time2String:" + time+" hour:"+ (time / 60)+ " hour:" + (time / 60/60));
        var hour = time / 60 / 60;
        var minute = time / 60 % 60;
        var second = time % 60;

        if(hour > 0)
        {
            return Set2(hour) + ":" + Set2(minute) + ":" + Set2(second) + "";
        }
        else
        {
            return Set2(minute) + ":" + Set2(second) + "";

        }
        return "00:00:00";
    }

    public static string GetPriceTypeIcon(int pricetype)
    {
        if(pricetype == 10000)
        {
            return "ui://GameUI/gold1";
        }
        else if (pricetype == 10001)
        {
            return "ui://GameUI/jingzuan";
        }
        else 
        {
            return "ui://GameUI/jingzuan";
        }
        return "";
    }
    public static string GetPriceTypeName(int pricetype)
    {
        if (pricetype == 10000)
        {
            return "金币";
        }
        else if (pricetype == 10001)
        {
            return "砖石";
        }
        else
        {
            return "ui://GameUI/jingzuan";
        }
        return "";
    }

    public static Vector2 Vec2Rotate(Vector2 v, float angle)
    {

        var radians = (Mathf.PI / 180) * angle;
            

        var sin = Mathf.Sin(radians);

        var cos = Mathf.Cos(radians);

        var re = new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);

        return re;
       
    }
    public delegate void HandleHttpResponse(WWW data);
    public static IEnumerator SendGet(string _url, HandleHttpResponse fun)
    {
        WWW getData = new WWW(_url);
        yield return getData;
        if(fun != null)
        {
            fun(getData);
        }

        if (getData.error != null)
        {
            Debug.Log(getData.error);
        }
        else
        {
            Debug.Log(getData.text);
        }
    }
    public static IEnumerator SendPost(string _url, WWWForm _wForm, HandleHttpResponse fun)
    {
        WWW postData = new WWW(_url, _wForm);
        yield return postData;
        if (fun != null)
        {
            fun(postData);
        }
        if (postData.error != null)
        {
            Debug.Log(postData.error);
        }
        else
        {
            Debug.Log(postData.text);
        }
    }
    //public static void SetClipLoop(this Animator animator, string clip,bool isloop)
    //{
    //    if (null == animator || string.IsNullOrEmpty(clip) || null == animator.runtimeAnimatorController)
    //        return ;
    //    RuntimeAnimatorController ac = animator.runtimeAnimatorController;
    //    AnimationClip[] tAnimationClips = ac.animationClips;
    //    if (null == tAnimationClips || tAnimationClips.Length <= 0) return ;
    //    AnimationClip tAnimationClip;
    //    for (int tCounter = 0, tLen = tAnimationClips.Length; tCounter < tLen; tCounter++)
    //    {
    //        tAnimationClip = ac.animationClips[tCounter];
    //        if (null != tAnimationClip && tAnimationClip.name == clip)
    //        {
    //            AnimationClipSettings clipSetting = AnimationUtility.GetAnimationClipSettings(tAnimationClip);
    //            clipSetting.loopTime = isloop;
    //            AnimationUtility.SetAnimationClipSettings(tAnimationClip, clipSetting);
    //            Debug.Log("loop-------------------");
    //            return;
    //        }
                
    //    }
    //    return ;
    //}

    public static float GetClipLength(this Animator animator, string clip)
    {
        if (null == animator || string.IsNullOrEmpty(clip) || null == animator.runtimeAnimatorController)
            return 0;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        AnimationClip[] tAnimationClips = ac.animationClips;
        if (null == tAnimationClips || tAnimationClips.Length <= 0) return 0;
        AnimationClip tAnimationClip;
        for (int tCounter = 0, tLen = tAnimationClips.Length; tCounter < tLen; tCounter++)
        {
            tAnimationClip = ac.animationClips[tCounter];
            if (null != tAnimationClip && tAnimationClip.name == clip)
                return tAnimationClip.length;
        }
        return 0F;
    }
    //创建点击显示的信息 (比如点击buff图标显示buff信息，长按技能图标显示技能信息)
    public static GComponent TouchShowCom = null;
    public static GComponent CreateTouchShowInfo()
    {
        //CloseTouchShowInfo();
        if (TouchShowCom != null)
        {
            return TouchShowCom;
        }
        TouchShowCom = UIPackage.CreateObject("GameUI", "TouchShowInfo").asCom;
        GRoot.inst.AddChild(TouchShowCom);

        Debug.Log("-----TouchShowInfo---");
        return TouchShowCom;
    }
    public static void CloseTouchShowInfo()
    {
        if(TouchShowCom != null)
        {
            TouchShowCom.Dispose();
            TouchShowCom = null;
        }
    }

    public static Vector2 GetPosition(float onex,float oney)
    {
        Vector2 screenPos = new Vector2(Screen.width * onex, Screen.height * oney);
        Vector2 logicScreenPos = GRoot.inst.GlobalToLocal(screenPos);
        return logicScreenPos;
    }
    public delegate void OkDo();
    public static void NoticeWindonw(string txt, OkDo ok)
    {
        //弹出断开连接
        var teamrequest = UIPackage.CreateObject("GameUI", "noticeWindow1").asCom;
        GRoot.inst.AddChild(teamrequest);
        teamrequest.xy = Tool.GetPosition(0.5f, 0.5f);
        AudioManager.Am.Play2DSound(AudioManager.Sound_OpenLittleUI);
        teamrequest.GetChild("word1").asTextField.text = txt;
        teamrequest.GetChild("ok").onClick.Add(() => {
            teamrequest.Dispose();
            if(ok != null)
            {
                ok();
            }
        });
        teamrequest.GetChild("no").onClick.Add(() => {
            teamrequest.Dispose();
        });
    }
    public static GComponent PaoMaDengCom = null;
    public static double LastPlayTime = 0;
    public static void PaoMaDeng(string word, Google.Protobuf.Collections.RepeatedField<string> p)
    {
        //处理文字
        Dictionary<string, string> pa = new Dictionary<string, string>();
        if (p != null && p.Count > 0)
        {
            int index = 1;
            foreach (var item in p)
            {
                pa["p" + index] = item;
                index++;
            }
        }
        word = Tool.ParseTemplate(word, pa);
        var speed = 100.0f;
        if (PaoMaDengCom == null)
        {
            PaoMaDengCom = UIPackage.CreateObject("GameUI", "PaoMaDeng").asCom;
            GRoot.inst.AddChild(PaoMaDengCom);
            PaoMaDengCom.xy = Tool.GetPosition(0.5f, 0.2f);

            PaoMaDengCom.GetChild("word").asTextField.text = word;
            

            FairyGUI.Transition trans = PaoMaDengCom.GetTransition("move2left");
            
            var distanse = 800 + PaoMaDengCom.GetChild("word").width;
            var time = distanse / speed;
            trans.SetValue("over", -PaoMaDengCom.GetChild("word").width, 5);
            trans.SetDuration("start", time);

            trans.Play();
            LastPlayTime = GetTime();
            trans.SetHook("over", () => {
                PaoMaDengCom.Dispose();
                PaoMaDengCom = null;
            });
        }
        else
        {
            var lasttext = PaoMaDengCom.GetChild("word").asTextField.text;
            PaoMaDengCom.GetChild("word").asTextField.text = " ";
            for (;;)
            {
                if(PaoMaDengCom.GetChild("word").width >= 800)
                {
                    break;
                }
                PaoMaDengCom.GetChild("word").asTextField.text += " ";
            }
            var space = PaoMaDengCom.GetChild("word").asTextField.text;
            PaoMaDengCom.GetChild("word").asTextField.text = lasttext+ space + word;
            FairyGUI.Transition trans = PaoMaDengCom.GetTransition("move2left");
            //trans.SetPaused(true);
            
            var distanse = 800 + PaoMaDengCom.GetChild("word").width;
            Debug.Log("distanse:" + distanse);
            var time = distanse / speed;
            trans.SetValue("over", -PaoMaDengCom.GetChild("word").width, 5);
            trans.SetDuration("start", time);
            trans.SetHook("over", null);
            
            trans.Play(1,0, (float)(GetTime()- LastPlayTime), time, null);
            trans.SetHook("over", () => {
                PaoMaDengCom.Dispose();
                PaoMaDengCom = null;
            });
            //trans.SetPaused(false);
        }
        
        
        
    }

    public static void NoticeWordsAnim(string word, Google.Protobuf.Collections.RepeatedField<string> p,string anim,int pos = 1)
    {
        
        // pos 1上 2中 3下
        var showpos = Tool.GetPosition(0.5f, 0.2f);
        if (pos == 1)
        {
            showpos = Tool.GetPosition(0.5f, 0.2f);
        }
        else if(pos == 2)
        {
            showpos = Tool.GetPosition(0.5f, 0.5f);
        }
        else if (pos == 3)
        {
            showpos = Tool.GetPosition(0.5f, 0.8f);
        }
        GComponent words = UIPackage.CreateObject("GameUI", "NoticeWords").asCom;
        //1，直接加到GRoot显示出来
        GRoot.inst.AddChild(words);
        //GRoot.inst.SetChildIndex(words, 1);
        words.xy = showpos;// Tool.GetPosition(0.5f, 0.2f);
        //var root = words.GetComponent<FairyGUI.UIPanel>().ui;

        words.GetChild("word").asTextField.text = word;// noticewords.Words;

        if (p != null && p.Count > 0)
        {
            int index = 1;
            foreach (var item in p)
            {
                Debug.Log("---------------NoticeWords:" + item);
                words.GetChild("word").asTextField.SetVar("p" + index, item);
                index++;
            }
            words.GetChild("word").asTextField.FlushVars();
        }


        FairyGUI.Transition trans = words.GetTransition(anim);
        trans.Play();
        trans.SetHook("over", () => {
            words.Dispose();
        });
    }
    //文本参数解析 同fairygui
    public static string ParseTemplate(string template, Dictionary<string, string> _templateVars)
    {
        int pos1 = 0, pos2 = 0;
        int pos3;
        string tag;
        string value;
        StringBuilder buffer = new StringBuilder();

        while ((pos2 = template.IndexOf('{', pos1)) != -1)
        {
            if (pos2 > 0 && template[pos2 - 1] == '\\')
            {
                buffer.Append(template, pos1, pos2 - pos1 - 1);
                buffer.Append('{');
                pos1 = pos2 + 1;
                continue;
            }

            buffer.Append(template, pos1, pos2 - pos1);
            pos1 = pos2;
            pos2 = template.IndexOf('}', pos1);
            if (pos2 == -1)
                break;

            if (pos2 == pos1 + 1)
            {
                buffer.Append(template, pos1, 2);
                pos1 = pos2 + 1;
                continue;
            }

            tag = template.Substring(pos1 + 1, pos2 - pos1 - 1);
            pos3 = tag.IndexOf('=');
            if (pos3 != -1)
            {
                if (!_templateVars.TryGetValue(tag.Substring(0, pos3), out value))
                    value = tag.Substring(pos3 + 1);
            }
            else
            {
                if (!_templateVars.TryGetValue(tag, out value))
                    value = "";
            }
            buffer.Append(value);
            pos1 = pos2 + 1;
        }
        if (pos1 < template.Length)
            buffer.Append(template, pos1, template.Length - pos1);

        return buffer.ToString();
    }

    //public static string WordsUBB(string text, Dictionary<string, string> p)
    //{
    //    GTextField WordsUBBText = new GTextField(); //LayaAir平台用new GBasicTextField
    //    WordsUBBText.UBBEnabled = true;
    //    WordsUBBText.text = text;
    //    int index = 1;
    //    foreach (var item in p)
    //    {
    //        WordsUBBText.SetVar("p" + index, item);
    //        Debug.Log("---WordsUBB---:" + item);
    //        index++;
    //    }
    //    WordsUBBText.FlushVars();
    //    Debug.Log("---WordsUBB---:" + WordsUBBText.text);
    //    return WordsUBBText.text;
    //}

    public static void NoticeWords(string word,Google.Protobuf.Collections.RepeatedField<string> p)
    {
        NoticeWordsAnim(word, p, "warning");
    }


    public static bool IsChineseOrNumberOrWord(string value)
    {
        
        Regex rg = new Regex("^[\u4e00-\u9fa5_a-zA-Z0-9]+$");
        bool re = rg.IsMatch(value);
        Debug.Log("IsChineseOrNumberOrWord:" + value+" :"+re);
        return re;
    }
    //^1[3-9]\d{9}$
    public static bool IsPhoneNumber(string value)
    {
        //return System.Text.RegularExpressions.Regex.IsMatch(value, @"^[1]+[3,5]+\d{9}");
        Regex rg = new Regex(@"^1[3-9]\d{9}$");
        bool re = rg.IsMatch(value);
        Debug.Log("IsPhoneNumber:" + value + " :" + re);
        return re;
    }
    //公会成员品级对应
    public static string[] GuildPinLevelWords = { "无", "一品", "二品", "三品", "四品", "五品", "六品", "七品", "八品", "九品", "大宗师" };
    //公会职位对应
    public static string[] GuildPostWords = { "普通会员", "普通会员", "普通会员", "普通会员", "普通会员", "普通会员", "普通会员", "普通会员", "普通会员", "普通会员", "会长" };

    //通过频道类型获取频道文字
    public static string GetTextFromChatChanel(int chanel)
    {
        //////聊天频道 1附近 2全服 3私聊 4队伍
        switch (chanel)
        {
            case 1:
                if(GameScene.Singleton.m_MyMainUnit != null && GameScene.Singleton.m_MyMainUnit.GroupID > 0)
                {
                    return "队伍";
                }
                return "地图";
            case 2:
                return "全服";
            case 3:
                return "私聊";
            case 4:
                return "队伍";
            case 5:
                return "公会";
        }

        return "附近";
    }
    public delegate void ClickDo();
    public static void AddClick(GObject obj, ClickDo cd)
    {
        if(obj == null)
        {
            return;
        }
        var touchbeginpos = new Vector2(0, 0);
        var touchbegintime = GetTime();
        obj.onTouchBegin.Add((EventContext context) =>
        {
            InputEvent inputEvent = (InputEvent)context.data;
            touchbegintime = GetTime();
            touchbeginpos = inputEvent.position;
        });
        obj.onTouchEnd.Add((EventContext context) =>
        {
            InputEvent inputEvent = (InputEvent)context.data;
            var endpos = inputEvent.position;
            var t1 = GetTime();
            if(t1 - touchbegintime > 0.2)
            {
                return;
            }
            if(Vector2.Distance(endpos, touchbeginpos) > 15)
            {
                return;
            }
            cd();
        });
    }
    public static string getMemory(object o) // 获取引用类型的内存地址方法  
    {
        GCHandle h = GCHandle.Alloc(o, GCHandleType.WeakTrackResurrection);

        System.IntPtr addr = GCHandle.ToIntPtr(h);

        return "0x" + addr.ToString("X");
    }
    private const double DOUBLE_DELTA = 1E-06;
    public static bool IsZero(double value)
    {
        return (Math.Abs(value) < DOUBLE_DELTA);
    }

//通过频道类型获取频道内容颜色
public static string GetContetColorFromChatChanel(int chanel)
    {
        //////聊天频道 1附近 2全服 3私聊 4队伍 5公会
        switch (chanel)
        {
            case 1:
                return "[color=#FFFFFF]";
            case 2:
                return "[color=#FFFFFF]";
            case 3:
                return "[color=#ff00ff]";
            case 4:
                return "[color=#FFFF33]";
            case 5:
                return "[color=#FFFF33]";
        }

        return "[color=#FFFFFF]";
    }

    //创建道具节点sellable

    //发送验证码
    public static void SendSMS(string phone, string code)
    {
        Debug.Log("SendSMS11111111111111");
        IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", "LTAI4G3c5QBfDPC8ztnGZtZz", "FmO4kM4FNwsIt2YFU0qc892vedm039");
        
        DefaultAcsClient client = new DefaultAcsClient(profile);
        CommonRequest request = new CommonRequest();
        request.Method = MethodType.POST;
        request.Domain = "dysmsapi.aliyuncs.com";
        request.Version = "2017-05-25";
        request.Action = "SendSms";
        // request.Protocol = ProtocolType.HTTP;
        request.AddQueryParameters("PhoneNumbers", phone);
        request.AddQueryParameters("SignName", "daolegame");
        request.AddQueryParameters("TemplateCode", "SMS_195873160");
        var codejson = "{\'code\':\'" + code + "\'}";
        request.AddQueryParameters("TemplateParam", codejson);

        Aliyun.Acs.Core.Auth.AcsURLEncoder.Encode("PhoneNumbers");
        Debug.Log("1111111111111");
        Aliyun.Acs.Core.Auth.AcsURLEncoder.Encode("SignName");
        Debug.Log("22222222222222");
        Aliyun.Acs.Core.Auth.AcsURLEncoder.Encode("daolegame");
        Debug.Log("333333333333333");
        Aliyun.Acs.Core.Auth.AcsURLEncoder.Encode(phone);
        Debug.Log("44444444444444");
        Aliyun.Acs.Core.Auth.AcsURLEncoder.Encode("TemplateCode");
        Debug.Log("5555555555555");
        Aliyun.Acs.Core.Auth.AcsURLEncoder.Encode("SMS_195873160");
        Debug.Log("666666666666");
        Aliyun.Acs.Core.Auth.AcsURLEncoder.Encode("TemplateParam");
        Debug.Log("777777777777:"+ codejson);
        Aliyun.Acs.Core.Auth.AcsURLEncoder.Encode(codejson);
        Debug.Log("8888888888888888");
        try
        {
            CommonResponse response = client.GetCommonResponse(request);
            Debug.Log(System.Text.Encoding.Default.GetString(response.HttpResponse.Content));
        }
        catch (ServerException e)
        {
            Debug.Log(e);
        }
        catch (ClientException e)
        {
            Debug.Log(e);
        }
    }
}
