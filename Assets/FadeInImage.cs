using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeInImage : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private float _fadeDuration;

    private void OnEnable()
    {
        if(_image)
        {
            DOTween.To(SetColor, 0, 1, _fadeDuration);
        }
    }

    private void SetColor(float a)
    {
        var color = _image.color;
        color.a = a;
        _image.color = color;
    }
}
