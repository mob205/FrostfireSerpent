using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Audio Event")]
public class AudioEvent : ScriptableObject
{
    public AudioClip[] Clips;
    public float MinPitch = 1;
    public float MaxPitch = 1;
    public float MinVolume = .5f;
    public float MaxVolume = .5f;

    public void Play(AudioSource audioSource)
    {
        if(!audioSource) { return; }

        AudioClip clip = Clips[Random.Range(0, Clips.Length)];
        float pitch = Random.Range(MinPitch, MaxPitch);
        float volume = Random.Range(MinVolume, MaxVolume);
        audioSource.clip = clip;
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.Play();
    }
}