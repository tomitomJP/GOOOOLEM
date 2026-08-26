using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;
    [SerializeField] AudioSource se;
    [SerializeField] AudioSource bgm;

    public float bgmVolume = 1;
    public float seVolume = 1;

    Coroutine fadeCoroutine = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // SE再生（ピッチ指定）
    public static void PlaySEWithPitch(AudioClip clip, float pitch, float volumeScale = 1)
    {
        instance._PlaySEWithPitch(clip, pitch, volumeScale);
    }

    public void _PlaySEWithPitch(AudioClip clip, float pitch, float volumeScale)
    {
        GameObject tempGO = new GameObject("TempAudio");
        AudioSource tempSource = tempGO.AddComponent<AudioSource>();

        tempSource.clip = clip;
        tempSource.pitch = pitch;
        tempSource.volume = seVolume * volumeScale;
        tempSource.Play();

        Destroy(tempGO, clip.length / pitch);
    }

    // SE再生（通常）
    public static void PlaySE(AudioClip clip, float volumeScale = 1)
    {
        instance.se.PlayOneShot(clip, instance.seVolume * volumeScale);
    }

    // BGM再生
    public static void PlayBGM(AudioClip clip, bool loop = true)
    {
        instance._PlayBGM(clip, loop);
    }

    public void _PlayBGM(AudioClip clip, bool loop)
    {
        if (bgm.clip == clip && bgm.isPlaying) return;

        bgm.clip = clip;
        bgm.loop = loop;
        bgm.volume = bgmVolume;
        bgm.Play();
    }

    // BGM停止（即時）
    public static void StopBGM()
    {
        instance._StopBGM();
    }

    public void _StopBGM()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
        bgm.Stop();
    }

    // BGMのピッチ変更
    public static void BgmOption(float pitch)
    {
        instance._BgmOption(pitch);
    }

    public void _BgmOption(float pitch)
    {
        if (bgm.pitch == pitch && bgm.isPlaying) return;

        bgm.pitch = pitch;
    }

    // BGMをフェードアウトしながら停止
    public static void FadeOutBGM(float duration)
    {
        instance._FadeOutBGM(duration);
    }

    public void _FadeOutBGM(float duration)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeOutBGMCoroutine(duration));
    }

    private IEnumerator FadeOutBGMCoroutine(float duration)
    {
        float startVolume = bgm.volume;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            bgm.volume = Mathf.Lerp(startVolume, 0, time / duration);
            yield return null;
        }

        bgm.Stop();
        bgm.volume = bgmVolume; // 元の音量に戻す
        fadeCoroutine = null;
    }

    public static void FadeInBGM(AudioClip clip, float duration, bool loop = true)
    {
        instance._FadeInBGM(clip, duration, loop);
    }

    public void _FadeInBGM(AudioClip clip, float duration, bool loop)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeInBGMCoroutine(clip, duration, loop));
    }

    private IEnumerator FadeInBGMCoroutine(AudioClip clip, float duration, bool loop)
    {
        bgm.clip = clip;
        bgm.loop = loop;
        bgm.volume = 0;
        bgm.Play();

        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            bgm.volume = Mathf.Lerp(0, bgmVolume, time / duration);
            yield return null;
        }

        bgm.volume = bgmVolume;
        fadeCoroutine = null;
    }
}
