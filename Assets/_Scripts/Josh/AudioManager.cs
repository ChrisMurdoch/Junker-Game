using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum AudioChannel
    {
        Master = 0, SFX = 1, Music = 2 
    }

    [Range(0f, 1f)]
    public float masterVolume = 1;
    [Range(0f, 1f)]
    public float sfxVolume = 1;
    [Range(0f, 1f)]
    public float musicVolume = 1;

    AudioSource[] musicSources;
    int activeMusicSourceIndex;

    public static AudioManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            musicSources = new AudioSource[2];
            for (int i = 0; i < 2; i++)
            {
                GameObject newMusicSource = new GameObject("Music Source " + (i + 1));
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;
            }
        }
    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolume * masterVolume);
        }

    }

    public void setVolume(float volume, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                masterVolume = volume;
                break;
            case AudioChannel.SFX:
                sfxVolume = volume;
                break;
            case AudioChannel.Music:
                musicVolume = volume;
                break;

        }

        foreach (AudioSource s in musicSources)
        {
            s.volume = musicVolume * masterVolume;
        }

    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();
        StartCoroutine(MusicCrossFade(fadeDuration));

    }

    IEnumerator MusicCrossFade(float duration)
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolume * masterVolume, percent);
            musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(musicVolume * masterVolume, 0, percent);
            yield return null;
        }
    }
}
