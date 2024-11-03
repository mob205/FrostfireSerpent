using DG.Tweening;
using UnityEngine;

public class FadeOnDetach : MonoBehaviour
{
    [SerializeField] private float _particleDecayDuration;
    [SerializeField] private float _spriteDecayDuration;

    private ParticleSystem _particles;
    private SpriteRenderer _sprite;

    private Tweener _emissionTween;
    private Tweener _alphaTween;

    private void Awake()
    {
        _particles = GetComponentInChildren<ParticleSystem>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }
    public void StartFade()
    {
        _emissionTween = DOTween.To(SetEmissionRate, _particles.emission.rateOverTime.constant, 0, _particleDecayDuration);
        _alphaTween = DOTween.To(SetAlpha, 1, 0, _spriteDecayDuration);
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
    private void OnDisable()
    {
        _emissionTween.Kill();
        _alphaTween.Kill();
    }
}
