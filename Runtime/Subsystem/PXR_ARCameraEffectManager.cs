using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.UI;

public class PXR_ARCameraEffectManager : MonoBehaviour
{
    public bool enableCameraEffect = false;
    [HideInInspector]
    public float colortempValue;
    [HideInInspector]
    public float brightnessValue;
    [HideInInspector]
    public float saturationValue;
    [HideInInspector]
    public float contrastValue;
    [HideInInspector]
    public Texture2D lutTex1;
    [HideInInspector]
    public Texture2D lutTex2;
    [HideInInspector]
    public Texture2D lutTex3;
    [HideInInspector]
    public Texture2D lutTex4;
    [HideInInspector]
    public Texture2D lutTex5;

    private const string TAG = "PXR_ARCameraEffectManager";

    // Start is called before the first frame update
    void Start()
    {
        Camera camera = Camera.main;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0, 0, 0, 0);
        if (enableCameraEffect)
        {
            PXR_MixedReality.EnableVideoSeeThroughEffect(true);
            PXR_MixedReality.SetVideoSeeThroughEffect(PxrLayerEffect.Colortemp, colortempValue, 1);
            PXR_MixedReality.SetVideoSeeThroughEffect(PxrLayerEffect.Brightness, brightnessValue, 1);
            PXR_MixedReality.SetVideoSeeThroughEffect(PxrLayerEffect.Saturation, saturationValue, 1);
            PXR_MixedReality.SetVideoSeeThroughEffect(PxrLayerEffect.Contrast, contrastValue, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetColortemp(float x)
    {
        PXR_MixedReality.EnableVideoSeeThroughEffect(true);
        PXR_MixedReality.SetVideoSeeThroughEffect(PxrLayerEffect.Colortemp, x, 1);
    }

    public void SetBrightness(float x)
    {
        PXR_MixedReality.EnableVideoSeeThroughEffect(true);
        PXR_MixedReality.SetVideoSeeThroughEffect(PxrLayerEffect.Brightness, x, 1);
    }

    public void SetSaturation(float x)
    {
        PXR_MixedReality.EnableVideoSeeThroughEffect(true);
        PXR_MixedReality.SetVideoSeeThroughEffect(PxrLayerEffect.Saturation, x, 1);
    }

    public void SetContrast(float x)
    {
        PXR_MixedReality.EnableVideoSeeThroughEffect(true);
        PXR_MixedReality.SetVideoSeeThroughEffect(PxrLayerEffect.Contrast, x, 1);
    }

    public void EnableLut(bool enable)
    {
        PLog.d(TAG, $"SetLutRow lutTex={lutTex1}, enable={enable} ");
        PXR_MixedReality.EnableVideoSeeThroughEffect(enable);
        if (lutTex1 && enable)
        {
            PLog.d(TAG, $"SetLutRow lutTex={lutTex1}");
            PXR_MixedReality.SetVideoSeeThroughLut(lutTex1, 8, 8);
        }
    }

    public void SetLut(int index)
    {
        PLog.d(TAG, $"SetLutRow index={index}");
        switch (index)
        {
            case 0:
                PXR_MixedReality.EnableVideoSeeThroughEffect(false);
                break;
            case 1:
                PXR_MixedReality.EnableVideoSeeThroughEffect(true);
                PXR_MixedReality.SetVideoSeeThroughLut(lutTex1, 8, 8);
                break;
            case 2:
                PXR_MixedReality.EnableVideoSeeThroughEffect(true);
                PXR_MixedReality.SetVideoSeeThroughLut(lutTex2, 8, 8);
                break;
            case 3:
                PXR_MixedReality.EnableVideoSeeThroughEffect(true);
                PXR_MixedReality.SetVideoSeeThroughLut(lutTex3, 8, 8);
                break;
            case 4:
                PXR_MixedReality.EnableVideoSeeThroughEffect(true);
                PXR_MixedReality.SetVideoSeeThroughLut(lutTex4, 8, 8);
                break;
            case 5:
                PXR_MixedReality.EnableVideoSeeThroughEffect(true);
                PXR_MixedReality.SetVideoSeeThroughLut(lutTex5, 8, 8);
                break;
            default:
                break;
        }
    }

    private void OnDisable()
    {
        PXR_MixedReality.EnableVideoSeeThroughEffect(false);
    }
}