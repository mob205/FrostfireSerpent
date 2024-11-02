using UnityEngine;

public class Destructible : MonoBehaviour, IEnclosable, IChargeable
{
    public bool CanEnclose { get; private set; } = true;
    public void Enclose()
    {
        Destroy(gameObject);
        CanEnclose = false;
    }

    public void OnCharge()
    {
        Destroy(gameObject);
    }
}
