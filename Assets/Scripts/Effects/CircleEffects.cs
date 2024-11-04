using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleEffects : MonoBehaviour
{
    [Header("Particles")]
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private ParticleSystem.MinMaxGradient _activeGradient;
    [SerializeField] private ParticleSystem.MinMaxGradient _inactiveGradient;

    [Header("Camera Shake")]
    [SerializeField] private float _camShakeIntensity;
    [SerializeField] private float _camShakeDuration;

    [Header("SFX")]
    [SerializeField] private AudioEvent _circleCompleteSFX;
    [SerializeField] private AudioEvent _witchLaughSFX;

    private AudioSource[] _audio;
    private PlayerAbilities _abilities;

    private void Awake()
    {
        _audio = GetComponents<AudioSource>();
        _abilities = GetComponentInParent<PlayerAbilities>();
    }
    public void PlayEffects(Vector3 position)
    {
        var particles = Instantiate(_particles, position, Quaternion.identity);
        var gradient = _inactiveGradient;
        if(_abilities.IsAbilityActive)
        {
            gradient = _activeGradient;
        }

        var colorModule = particles.colorOverLifetime;
        colorModule.color = gradient;

        if (CameraShakeManager.Instance)
        {
            CameraShakeManager.Instance.AddShake(_camShakeIntensity, _camShakeDuration);
        }

        if (_circleCompleteSFX && _audio.Length >= 1)
        {
            _circleCompleteSFX.Play(_audio[0]);
        }
        if(_witchLaughSFX && _audio.Length >= 2)
        {
            _witchLaughSFX.Play(_audio[1]);
        }
    }
}
