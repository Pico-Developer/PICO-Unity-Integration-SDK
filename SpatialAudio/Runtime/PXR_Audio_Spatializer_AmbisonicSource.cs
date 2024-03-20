//  Copyright © 2015-2021 Pico Technology Co., Ltd. All Rights Reserved.

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PXR_Audio_Spatializer_AmbisonicSource : MonoBehaviour
{
    private AudioSource nativeSource;

    private float playheadPosition = 0.0f;
    private bool wasPlaying = false;
    
    /// <summary>
    /// Resume audio playing status.
    /// </summary>
    public void Resume()
    {
        if (nativeSource)
        {
            nativeSource.time = playheadPosition;
            if (wasPlaying)
            {
                nativeSource.Play();
            }
        }
    }

    void Awake()
    {
        nativeSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (nativeSource.isPlaying)
            playheadPosition = nativeSource.time;
        wasPlaying = nativeSource.isPlaying;
    }
}
