using Unity.XR.PXR;
using UnityEngine;

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
    public Texture2D lutTex;
    [HideInInspector]
    public float lutRowValue;
    [HideInInspector]
    public float lutColValue;

    private int row = 0;
    private int col = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (enableCameraEffect)
        {
            Camera camera = Camera.main;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0, 0, 0, 0);
            PXR_MixedReality.EnableVideoSeeThroughEffect(true);
            PXR_MixedReality.SetVideoSeeThroughEffect(PxrLayerEffect.Colortemp, colortempValue, 1);
            PXR_MixedReality.SetVideoSeeThroughEffect(PxrLayerEffect.Brightness, brightnessValue, 1);
            PXR_MixedReality.SetVideoSeeThroughEffect(PxrLayerEffect.Saturation, saturationValue, 1);
            PXR_MixedReality.SetVideoSeeThroughEffect(PxrLayerEffect.Contrast, contrastValue, 1);

            if (lutTex)
            {
                row = (int)(lutTex?.width * lutRowValue);
                col = (int)(lutTex?.height * lutColValue);
                PXR_MixedReality.SetVideoSeeThroughLut(lutTex, row, col);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDisable()
    {
        PXR_MixedReality.EnableVideoSeeThroughEffect(false);
    }
}
