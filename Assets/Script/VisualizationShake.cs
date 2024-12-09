using UnityEngine;
using Cinemachine;
using System.Collections;

//Untuk saat ini skrip ini dipanggil ketika musuh ngehit player
//kedepannya masih perlu fleksibilitas dan kustomisasi lebih tinggi lagi

//dipanggil dari skrip Fighting.cs di gameobject karakter
//cara kerja: mengubah value amplitude dari component VirtualMachineCamera didalam segmen noise
//set noise to: Basic Multi Channel Perlin
//set noise profile to: 6D Shake

public class VisualizationShake : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float shakeDuration = 1f;
    public float shakeAmplitude = 1f;
    public float shakeFrequency = 2f;

    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeTimer;

    void Start()
    {
        noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = 0f; // Set initial amplitude to 0
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            noise.m_AmplitudeGain = shakeAmplitude * (shakeTimer / shakeDuration);
        }
        else
        {
            noise.m_AmplitudeGain = 0f; // Reset amplitude when shake is done
        }
    }

    public void TriggerShake()
    {
        shakeTimer = shakeDuration; // Reset timer
        noise.m_AmplitudeGain = shakeAmplitude; // Set amplitude to desired value
    }
}