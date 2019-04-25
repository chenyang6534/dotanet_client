using System.Collections;
using System.Collections.Generic;
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

}
