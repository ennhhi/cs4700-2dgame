using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sfx : MonoBehaviour
{
    public static Sfx I;

    [Header("Clips")]
    public AudioClip jump;
    public AudioClip collect;
    public AudioClip win;
    public AudioClip die;

    AudioSource src;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        src = GetComponent<AudioSource>();
        src.playOnAwake = false;
        src.spatialBlend = 0f; // 2D
        src.volume = 1f;
    }

    public void PlayJump()    { if (jump)    src.PlayOneShot(jump); }
    public void PlayCollect() { if (collect) src.PlayOneShot(collect); }
    public void PlayWin()     { if (win)     src.PlayOneShot(win); }
    public void PlayDie()     { if (die)     src.PlayOneShot(die); }
}
