using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _intro;
    [SerializeField] private AudioSource _loop;

    private void Start()
    {
        _intro.Play();
        _loop.PlayDelayed(_intro.clip.length);
    }
}
