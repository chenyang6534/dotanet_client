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
        AudioClip clip = Resources.Load<AudioClip>(path);
        //SoundPlayer.clip = clip;
        //SoundPlayer.PlayOneShot(clip);
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

    //播放音效
    public void Play2DSound(string path)
    {
        AudioClip clip = Resources.Load<AudioClip>(path);
        //SoundPlayer.clip = clip;
        SoundPlayer.PlayOneShot(clip);
    }

    public void Play2DSound(AudioClip ac)
    {
        //AudioClip clip = Resources.Load<AudioClip>(path);
        //SoundPlayer.clip = clip;
        SoundPlayer.PlayOneShot(ac);
    }
}