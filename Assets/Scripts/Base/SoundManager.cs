using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Globals;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    AudioSource audioMusic, audioEffect;

    public static SoundManager instance;

    [SerializeField]
    AudioClip click;
    [SerializeField]
    AudioClip lobby;

    private List<AudioSource> listAudioSrc = new List<AudioSource>();
    private List<AudioSource> listCurrentAudioSrc = new List<AudioSource>();

    private void Awake()
    {
        SoundManager.instance = this;

    }

    public void playMusic()
    { //play Lobby Music 
        if (UIManager.instance.gameView != null)
        {
            playMusicInGame(UIManager.instance.gameView.soundBg);
        }
        else
        {
            if (Globals.Config.isMusic)
            {
                if (audioMusic.clip == lobby && !audioMusic.isPlaying)
                {
                    audioMusic.volume = 0.5f;
                    audioMusic.clip = lobby;
                    audioMusic.Play();
                }
                else if (audioMusic.clip != lobby)
                {
                    audioMusic.Stop();
                    audioMusic.clip = lobby;
                    audioMusic.volume = 0.5f;
                    audioMusic.Play();
                }
            }
            else
            {
                audioMusic.Stop();
            }
        }

    }
    public void playMusicInGame(string pathAudio)
    {
        var audioClip = Resources.Load(pathAudio) as AudioClip;
        if (Globals.Config.isMusic)
        {

            audioMusic.Stop();
            audioMusic.clip = audioClip;
            audioMusic.Play();
        }
        else
        {
            audioMusic.Stop();
        }
    }
    public void pauseMusic()
    {
        audioMusic.Pause();
    }

    public AudioSource playEffectFromPath(string pathAudio)
    {
        if (Globals.Config.isSound)
        {
            //Resources.Load(path) as GameObject;
            var audioClip = Resources.Load(pathAudio) as AudioClip;
            AudioSource audioSrc;
            if (listAudioSrc.Count > 0 && listAudioSrc[0].isPlaying == false)
            {
                audioSrc = listAudioSrc[0];
                listAudioSrc.RemoveAt(0);
            }
            else
            {
                audioSrc = Instantiate(audioEffect);
                audioSrc.transform.SetParent(transform);
            }
            audioSrc.Stop();
            audioSrc.clip = audioClip;
            audioSrc.Play();
            //audioSrc.
            listCurrentAudioSrc.Add(audioSrc);
            DOTween.Sequence()
              .AppendInterval(audioSrc.time).AppendCallback(() =>
              {
                  //audioSrc.Stop();
                  listAudioSrc.Add(audioSrc);
                  listCurrentAudioSrc.Remove(audioSrc);
              });
            return audioSrc;
        }
        return null;

    }
    public void stopAllCurrentEffect()
    {
        Debug.Log("stopAllCurrentEffect");
        listCurrentAudioSrc.ForEach(auSrc =>
        {

            if (auSrc.isPlaying)
            {
                auSrc.Stop();
                listAudioSrc.Add(auSrc);
            }
        });
        listAudioSrc.ForEach(auSrc =>
        {

            if (auSrc.isPlaying)
            {
                auSrc.Stop();
            }
        });
        listCurrentAudioSrc.Clear();
        audioEffect.Stop();
    }
    void playSound(AudioClip audioClip)
    {
        if (!Globals.Config.isSound) return;

        audioEffect.clip = audioClip;
        audioEffect.Play();
    }

    public void soundClick()
    {
        playSound(click);
    }
}
