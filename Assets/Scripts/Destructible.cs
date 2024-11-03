using DG.Tweening;
using UnityEngine;

public class Destructible : MonoBehaviour, IEnclosable, IChargeable
{
    [SerializeField] private bool _destroyByEnclose = true;
    [SerializeField] private bool _destroyByCharge = true;
    [SerializeField] private float _shrinkDuration;
    [SerializeField] private GameObject _ruinsPrefab;
    public bool CanEnclose { get; private set; } = true;

    public delegate void DestroyedDel();
    public DestroyedDel destroyedDel;
    public void Start()
    {
        CanEnclose = _destroyByEnclose;
    }
    public void Enclose()
    {
        CanEnclose = false;
        HandleDestruction();
    }

    public void OnCharge()
    {
        if(_destroyByCharge)
        {
            HandleDestruction();
        }
    }
    private void HandleDestruction()
    {
        var tween = transform.DOScale(0, _shrinkDuration);
        tween.SetEase(Ease.InBounce);
        tween.onComplete += FinishDestruction;
        destroyedDel?.Invoke();
    }
    private void FinishDestruction()
    {
        if(_ruinsPrefab)
        {
            Instantiate(_ruinsPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
