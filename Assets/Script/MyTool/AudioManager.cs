using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class AudioManager : MonoBehaviour
{
    //将声音管理器写成单例模式
    public static AudioManager Am;
    //音乐播放器
    //public AudioSource MusicPlayer;
    //音效播放器
    public AudioSource SoundPlayer;

    protected Dictionary<AudioClip, double> PlaySoundTime = new Dictionary<AudioClip, double>();
    void Start()
    {
        Am = this;
        //AudioSource.PlayClipAtPoint
    }

    // Update is called once per frame
    void Update()
    {

    }

    //播放音乐
    public void PlayMusic(string path)
    {
        //if (MusicPlayer.isPlaying == false)
        {
            AudioClip clip = Resources.Load<AudioClip>(path);
            SoundPlayer.clip = clip;
            SoundPlayer.Play();
        }

    }
    //播放音效
    public void Play3DSound(string path,Vector3 pos)
    {

        if (path.Length <= 0)
        {
            return;
        }
        string[] allpath = path.Split(',');
        int index = Random.Range(0, allpath.Length);
        Debug.Log("Play3DSound:" + index + "  :" + allpath[index]);
        //Debug.Log("Play3DSound:" + allpath[index]);

        AudioClip clip = Resources.Load<AudioClip>(allpath[index]);
        AudioSource.PlayClipAtPoint(clip, pos);

        
    }
    //播放音效
    public void Play3DSound(AudioClip ac, Vector3 pos)
    {
        if (PlaySoundTime.ContainsKey(ac))
        {
            if(Tool.GetTime()- PlaySoundTime[ac] < 0.1)
            {
                return;
            }
        }
        PlaySoundTime[ac] = Tool.GetTime();
        AudioSource.PlayClipAtPoint(ac, pos);

    }


    //
    //static public string Sound_Gold = "sound/other/gold1,sound/other/gold2,sound/other/gold3";
    static public string Sound_Gold = "sound/other/gold1";
    static public string Sound_LevelUp = "sound/other/levelup";
    static public string Sound_OpenUI = "sound/menu/filter_open";
    static public string Sound_CloseUI = "sound/menu/filter_close";
    static public string Sound_OpenLittleUI = "sound/menu/chat_open";
    static public string Sound_CloseLittleUI = "sound/menu/chat_close";
    static public string Sound_Click = "sound/menu/browser_click_common";

    
    //播放音效
    public void Play2DSound(string path)
    {

        if(path.Length <= 0)
        {
            return;
        }
        string[] allpath = path.Split(',');
        int index = Random.Range(0, allpath.Length);
        AudioClip clip = Resources.Load<AudioClip>(allpath[index]);
        SoundPlayer.clip = clip;
        SoundPlayer.PlayOneShot(clip, 0.5f);
        
        Debug.Log("Play2DSound:" + path + "   " + SoundPlayer.volume);
    }

    public void Play2DSound(AudioClip ac)
    {
        //AudioClip clip = Resources.Load<AudioClip>(path);
        //SoundPlayer.clip = clip;
        SoundPlayer.PlayOneShot(ac);
    }
}