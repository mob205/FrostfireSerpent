using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FadeOnDetach : MonoBehaviour
{
    [SerializeField] private float _particleDecayDuration;
    [SerializeField] private float _spriteDecayDuration;
    [SerializeField] private float _lightDecayDuration;

    private ParticleSystem _particles;
    private SpriteRenderer _sprite;
    private Light2D _light;

    private Tweener _emissionTween;
    private Tweener _alphaTween;
    private Tweener _lightTween;

    private float _startingEmission;
    private float _startingIntensity;

    private void Awake()
    {
        _particles = GetComponentInChildren<ParticleSystem>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _light = GetComponentInChildren<Light2D>();

        _startingEmission = _particles.emission.rateOverTime.constant;
        _startingIntensity = _light.intensity;
    }
    private void OnEnable()
    {
        SetAlpha(1);
        SetEmissionRate(_startingEmission);
        SetIntensity(_startingIntensity);
    }
    public void StartFade()
    {
        _emissionTween = DOTween.To(SetEmissionRate, _particles.emission.rateOverTime.constant, 0, _particleDecayDuration);
        _alphaTween = DOTween.To(SetAlpha, 1, 0, _spriteDecayDuration);
        _lightTween = DOTween.To(SetIntensity, _light.intensity, 0, _lightDecayDuration);
    }
    private void SetEmissionRate(float rate)
    {
        var emission = _particles.emission;
        emission.rateOverTime = rate;
    }
    private void SetAlpha(float a)
    {
        var color = _sprite.color;
        color.a = a;
        _sprite.color = color;
    }
    private void SetIntensity(float intensity)
    {
        _light.intensity = intensity;
    }
    private void OnDisable()
    {
        _emissionTween.Kill();
        _alphaTween.Kill();
        _lightTween.Kill();
    }
}
