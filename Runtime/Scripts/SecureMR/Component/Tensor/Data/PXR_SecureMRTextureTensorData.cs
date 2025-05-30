using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR.SecureMR;
using UnityEngine;

public class PXR_SecureMRTextureTensorData : PXR_SecureMRTensorData
{
    //The texture that will be converted to color and float array
    //Note: you have to mark the texture as Read/Write in the Inspector
    [SerializeField]
    private Texture2D texture;

    private Color32[] _colorData;
    private float[] _rgbaFloatData;
    private int[] _rgbaIntData;
    private byte[] _rgbaByteData;

    public Texture2D Texture
    {
        get { return texture; }
        set 
        { 
            texture = value;
            UpdateArrayData();
        }
    }
    public override float[] ToFloatArray()
    {
        return _rgbaFloatData;
    }

    public override byte[] ToByteArray()
    {
        return _rgbaByteData;
    }

    public override int[] ToIntArray()
    {
        return _rgbaIntData;
    }

    private void UpdateArrayData()
    {
        if (texture == null)
        {
            _colorData = null;
            _rgbaFloatData = null;
            _rgbaByteData = null;
            _rgbaIntData = null;
            return;
        }

        // Get raw color data from texture
        _colorData = texture.GetPixels32();
        _rgbaIntData = texture.GetPixelData<int>(0).ToArray();
            
        // Convert to RGBA float array
        _rgbaFloatData = new float[_colorData.Length * 4];
        _rgbaByteData = new byte[_colorData.Length * 4];
        for (int i = 0; i < _colorData.Length; i++)
        {
            _rgbaFloatData[i * 4] = _colorData[i].r / 255f;     // R
            _rgbaFloatData[i * 4 + 1] = _colorData[i].g / 255f;  // G
            _rgbaFloatData[i * 4 + 2] = _colorData[i].b / 255f;  // B
            _rgbaFloatData[i * 4 + 3] = _colorData[i].a / 255f;  // A
            _rgbaByteData[i * 4] = _colorData[i].r;     // R
            _rgbaByteData[i * 4 + 1] = _colorData[i].g;  // G
            _rgbaByteData[i * 4 + 2] = _colorData[i].b;  // B
            _rgbaByteData[i * 4 + 3] = _colorData[i].a;  // A
        }
        
        
    }
}
