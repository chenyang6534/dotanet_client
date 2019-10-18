using FairyGUI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class Tool {

	public static double GetTime()
    {
        var t1 = (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
        //var t1 = Time.realtimeSinceStartup;
        return t1;
    }

    public static Vector2 Vec2Rotate(Vector2 v, float angle)
    {

        var radians = (Mathf.PI / 180) * angle;
            

        var sin = Mathf.Sin(radians);

        var cos = Mathf.Cos(radians);

        var re = new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);

        return re;
       
    }

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

    public static void NoticeWords(string word)
    {
        GComponent words = UIPackage.CreateObject("GameUI", "NoticeWords").asCom;
        //1，直接加到GRoot显示出来
        GRoot.inst.AddChild(words);
        GRoot.inst.SetChildIndex(words, 1);
        words.xy = Tool.GetPosition(0.5f, 0.25f);
        //var root = words.GetComponent<FairyGUI.UIPanel>().ui;
        words.GetChild("word").asTextField.text = word;// noticewords.Words;
        FairyGUI.Transition trans = words.GetTransition("anim1");
        trans.Play();
        trans.SetHook("over", () => {
            words.Dispose();
        });
    }


    public static bool IsChineseOrNumberOrWord(string value)
    {
        
        Regex rg = new Regex("^[\u4e00-\u9fa5_a-zA-Z0-9]+$");
        bool re = rg.IsMatch(value);
        Debug.Log("IsChineseOrNumberOrWord:" + value+" :"+re);
        return re;
    }

}
