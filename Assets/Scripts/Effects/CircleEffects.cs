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

    private AudioSource _audio;
    private PlayerAbilities _abilities;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _abilities = GetComponentInParent<PlayerAbilities>();
    }
    public void PlayEffects(Vector3 position)
    {
        var particles = Instantiate(_particles, position, Quaternion.identity);
        var gradient = _inactiveGradient;
        if(_abilities.IsAbilityActive)
        {
            Debug.Log("Active");
            gradient = _activeGradient;
        }

        var colorModule = particles.colorOverLifetime;
        colorModule.color = gradient;

        if (CameraShakeManager.Instance)
        {
            CameraShakeManager.Instance.AddShake(_camShakeIntensity, _camShakeDuration);
        }

        if (_circleCompleteSFX)
        {
            _circleCompleteSFX.Play(_audio);
        }
    }
}
