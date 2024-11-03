using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance { get; private set; }

    private static CinemachineBasicMultiChannelPerlin _noise;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        _noise = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>(); ;
    }
    public void AddShake(float amount, float duration)
    {
        StartCoroutine(AddShakeDelayed(amount, duration));
    }
    public void AddShake(float amount)
    {
        _noise.m_AmplitudeGain += amount;
        Debug.Log(amount);
    }
    private IEnumerator AddShakeDelayed(float amount, float duration)
    {
        AddShake(amount);
        yield return new WaitForSeconds(duration);
        AddShake(-amount);
    }
}