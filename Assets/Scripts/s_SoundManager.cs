using UnityEngine;
using System.Collections;

public class s_SoundManager : MonoBehaviour
{
    public AudioSource sfxAudio;
    public AudioSource musicAudio;

    private bool muted = false;

    public void PlaySound(AudioClip sfx)
    {
        sfxAudio.clip = sfx;
        sfxAudio.Play();
    }

    public void PlayMusic(AudioClip music)
    {
        musicAudio.clip = music;
        musicAudio.Play();
    }

    public void Mute_Sounds_And_Music()
    {
        if (muted)
        {
            sfxAudio.volume = 1;
            musicAudio.volume = 1;
            muted = false;
        }
        else
        {
            sfxAudio.volume = 0;
            musicAudio.volume = 0;
            muted = true;
        }
    }
}
