using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager I;

    public AudioClip bgm;
    [Range(0f,1f)] public float volume = 0.5f;

    AudioSource src;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        src = GetComponent<AudioSource>();
        src.loop = true;
        src.playOnAwake = false;
        src.spatialBlend = 0f; // 2D
        src.volume = volume;

        if (bgm) { src.clip = bgm; src.Play(); }
    }

    public void SetVolume(float v)
    {
        volume = Mathf.Clamp01(v);
        if (src) src.volume = volume;
    }
}
