using UnityEngine;

public class Destructible : MonoBehaviour, IEnclosable, IChargeable
{
    public bool CanEnclose { get; private set; } = true;

    public delegate void DestroyedDel();
    public DestroyedDel destroyedDel;
    public void Enclose()
    {
        Destroy(gameObject);
        CanEnclose = false;
        destroyedDel.Invoke();
    }

    public void OnCharge()
    {
        Destroy(gameObject);
        destroyedDel.Invoke();
    }
}
