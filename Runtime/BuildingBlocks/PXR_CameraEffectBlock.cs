using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;
#if PICO_OPENXR_SDK
using Unity.XR.OpenXR.Features.PICOSupport;
#endif

public class PXR_CameraEffectBlock : MonoBehaviour
{
    public Texture2D lutTex;

    private int row = 0;
    private int col = 0;

    private float brightness = 0;
    private float contrast = 0;
    private float saturation = 0;
    private PassthroughStyle passthroughStyle = new()
    {
        enableColorMap = true,
        enableEdgeColor = true,
        TextureOpacityFactor = 1.0f
    };

    private float r = 0;
    private float g = 0;
    private float b = 0;
    private float a = 0;
    private Color[] values;

    // Start is called before the first frame update
    void Start()
    {
#if PICO_OPENXR_SDK
        PassthroughFeature.EnableVideoSeeThrough = true;
        values = new Color[PassthroughFeature.XR_PASSTHROUGH_COLOR_MAP_MONO_SIZE_FB];
#else
        PXR_Manager.EnableVideoSeeThrough = true;
        PXR_MixedReality.EnableVideoSeeThroughEffect(true);
#endif
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetColortemp(float x)
    {
#if PICO_OPENXR_SDK
#else
        PXR_MixedReality.EnableVideoSeeThroughEffect(true);
        PXR_MixedReality.SetVideoSeeThroughEffect(PxrLayerEffect.Colortemp, x, 0);
#endif
    }
    public void SetBrightness(float x)
    {
#if PICO_OPENXR_SDK
        brightness = x;
        PassthroughFeature.SetBrightnessContrastSaturation(ref passthroughStyle, brightness, contrast, saturation);
        PassthroughFeature.SetPassthroughStyle(passthroughStyle);
#else
        PXR_MixedReality.EnableVideoSeeThroughEffect(true);
        PXR_MixedReality.SetVideoSeeThroughEffect(PxrLayerEffect.Brightness, x, 0);
#endif
    }
    public void SetSaturation(float x)
    {
#if PICO_OPENXR_SDK
        saturation = x;
        PassthroughFeature.SetBrightnessContrastSaturation(ref passthroughStyle, brightness, contrast, saturation);
        PassthroughFeature.SetPassthroughStyle(passthroughStyle);
#else
        PXR_MixedReality.EnableVideoSeeThroughEffect(true);
        PXR_MixedReality.SetVideoSeeThroughEffect(PxrLayerEffect.Saturation, x, 0);
#endif
    }
    public void SetContrast(float x)
    {
#if PICO_OPENXR_SDK
        contrast = x;
        PassthroughFeature.SetBrightnessContrastSaturation(ref passthroughStyle, brightness, contrast, saturation);
        PassthroughFeature.SetPassthroughStyle(passthroughStyle);
#else
        PXR_MixedReality.EnableVideoSeeThroughEffect(true);
        PXR_MixedReality.SetVideoSeeThroughEffect(PxrLayerEffect.Contrast, x, 0);
#endif
    }
    public void SetLut()
    {
        if (lutTex)
        {
#if PICO_OPENXR_SDK
#else
            PXR_MixedReality.EnableVideoSeeThroughEffect(true);
            PXR_MixedReality.SetVideoSeeThroughLut(lutTex, 8, 8);
#endif
        }
    }
    public void ClearAll()
    {
#if PICO_OPENXR_SDK
        passthroughStyle = new()
        {
            enableColorMap = true,
            enableEdgeColor = true,
            TextureOpacityFactor = 1.0f
        };
        PassthroughFeature.SetPassthroughStyle(passthroughStyle);
#else
        PXR_MixedReality.EnableVideoSeeThroughEffect(false);
#endif
    }



#if PICO_OPENXR_SDK
    public void MonoToMono()
    {
        var monOvalues = new int[PassthroughFeature.XR_PASSTHROUGH_COLOR_MAP_MONO_SIZE_FB];
        for (int i = 0; i < PassthroughFeature.XR_PASSTHROUGH_COLOR_MAP_MONO_SIZE_FB; ++i)
        {
            monOvalues[i] = i;
        }
        PassthroughFeature.SetColorMapbyMonoToMono(ref passthroughStyle, monOvalues);
        PassthroughFeature.SetPassthroughStyle(passthroughStyle);
    }

    public void SetEdgeColorToR(float x)
    {
        r = x;
        SetEdgeColorRGBA();
    }

    public void SetEdgeColorToG(float x)
    {
        g = x;
        SetEdgeColorRGBA();
    }

    public void SetEdgeColorToB(float x)
    {
        b = x;
        SetEdgeColorRGBA();
    }

    public void SetEdgeColorToA(float x)
    {
        a = x;
        SetEdgeColorRGBA();
    }

    public void SetEdgeColorRGBA()
    {
        passthroughStyle.EdgeColor = new Color(r, g, b, a);
        PassthroughFeature.SetPassthroughStyle(passthroughStyle);
    }

    public void SetColorMapR()
    {
        var values = new Color[PassthroughFeature.XR_PASSTHROUGH_COLOR_MAP_MONO_SIZE_FB];
        for (int i = 0; i < PassthroughFeature.XR_PASSTHROUGH_COLOR_MAP_MONO_SIZE_FB; ++i)
        {
            float colorValue = i / 255.0f;
            values[i] = new Color(colorValue, 0.0f, 0.0f, 1.0f);
        }
        PassthroughFeature.SetColorMapbyMonoToRgba(ref passthroughStyle, values);
        PassthroughFeature.SetPassthroughStyle(passthroughStyle);
    }
    public void SetColorMapG()
    {
        var values = new Color[PassthroughFeature.XR_PASSTHROUGH_COLOR_MAP_MONO_SIZE_FB];
        for (int i = 0; i < PassthroughFeature.XR_PASSTHROUGH_COLOR_MAP_MONO_SIZE_FB; ++i)
        {
            float colorValue = i / 255.0f;
            values[i] = new Color(0.0f, colorValue, 0.0f, 1.0f);
        }
        PassthroughFeature.SetColorMapbyMonoToRgba(ref passthroughStyle, values);
        PassthroughFeature.SetPassthroughStyle(passthroughStyle);
    }

    public void SetColorMapB()
    {
        var values = new Color[PassthroughFeature.XR_PASSTHROUGH_COLOR_MAP_MONO_SIZE_FB];
        for (int i = 0; i < PassthroughFeature.XR_PASSTHROUGH_COLOR_MAP_MONO_SIZE_FB; ++i)
        {
            float colorValue = i / 255.0f;
            values[i] = new Color(0.0f, 0.0f, colorValue, 1.0f);
        }
        PassthroughFeature.SetColorMapbyMonoToRgba(ref passthroughStyle, values);
        PassthroughFeature.SetPassthroughStyle(passthroughStyle);
    }
#endif
}