#if !PICO_XR
using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.XR.OpenXR.Features.PICOSupport;
using Unity.XR.PICO.TOBSupport;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_XR_COMPOSITIONLAYERS
using Unity.XR.CompositionLayers;
using Unity.XR.OpenXR.Features.PICOSupport;
using UnityEngine.XR.OpenXR.CompositionLayers;
#endif
public class EnterpriseAPI_OpenXR : MonoBehaviour
{  
    private string tag = "CaptureLib ----";
#if UNITY_XR_COMPOSITIONLAYERS
    private CompositionLayer _overlay = null;
#endif
    private PXRCaptureRenderMode Mode = PXRCaptureRenderMode.PXRCapture_RenderMode_LEFT;
    byte[] imgByte ;
    private int width=2048;        
    private int height=1536; 
    public Material videoMaterial;
    public Transform FrameTarget;
    public Transform RenderTarget;
    public Text CanshuText;
    public Toggle showtime;
    public Text showtimeText;
    private bool isRuning=false;
    // 将视频帧转换为 Unity 纹理
    Texture2D texture;
    public Text fpsText;
    private void Awake()
    {
        Debug.Log($"{tag}  Awake ");
        PassthroughFeature.EnableVideoSeeThrough=true;
#if UNITY_XR_COMPOSITIONLAYERS
        _overlay = GetComponent<CompositionLayer>();
        if (_overlay == null)
        {
            _overlay = gameObject.AddComponent<CompositionLayer>();
        }
#endif
        imgByte = new byte[width*height*4];
        texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
        videoMaterial.SetTexture("_MainTex", texture);
        PXR_Enterprise.Configurefor4U();

        PXR_Enterprise.OpenCameraAsyncfor4U(ret =>
        {
            Debug.Log($"{tag}  OpenCameraAsync ret=  {ret}");
        });
    }
    public void SetTrackingMode(int listChoice)
    {
        Debug.Log($"{tag}  SetMode ret=  {listChoice}");
        // if (listChoice > 0)
        {
            switch (listChoice)
            {
                case 0:
                    Mode = PXRCaptureRenderMode.PXRCapture_RenderMode_LEFT;
                    break;
                case 1:
                    Mode = PXRCaptureRenderMode.PXRCapture_RenderMode_RIGHT;
                    break;
                case 2:
                    Mode = PXRCaptureRenderMode.PXRCapture_RenderMode_3D;
                    break;
                case 3:
                    Mode = PXRCaptureRenderMode.PXRCapture_RenderMode_Interlace;
                    break;
            }
           
        }
    }
    float outputFovHorizontal=76.35f;
    float outputFovVertical=61.05f;
    public void getCameraParameters()
    {
        Debug.Log($"{tag} getCamera ");
        double[] CameraIntrinsics=PXR_Enterprise.GetCameraIntrinsicsfor4U(width, height, outputFovHorizontal, outputFovVertical);
        PXR_Enterprise.GetCameraExtrinsicsfor4U(out Matrix4x4 leftExtrinsics, out Matrix4x4 rightExtrinsics);
        Debug.Log($"getCamera-- GetCameraIntrinsics:[{CameraIntrinsics[0]},{CameraIntrinsics[1]},{CameraIntrinsics[2]},{CameraIntrinsics[3]}]");
        Debug.Log(tag+"getCamera-- GetCameraExtrinsics leftExtrinsics :\n"+leftExtrinsics);
        Debug.Log(tag+"getCamera-- GetCameraExtrinsics rightExtrinsics :\n"+rightExtrinsics);
       
        RGBCameraParamsNew param = PXR_Enterprise.GetCameraParametersNewfor4U(width, height);
        // Debug.Log($"getCamera GetCameraIntrinsics:[{param.cx},{param.cy},{param.fx},{param.fy}]");
        // Debug.Log($"getCamera GetCameraExtrinsics leftExtrinsics::[{param.l_pos} ------ {param.l_rot}]");
        // Debug.Log($"getCamera GetCameraExtrinsics rightExtrinsics::[{param.r_pos} ------ {param.r_rot}]");
        CanshuText.text = $"外参::[{param.l_pos} ------ {param.l_rot}]\n"+$" [{param.r_pos} ------ {param.r_rot}]";
    }
    public void StartPreview()
    {
        Debug.Log($"{tag} StartPreview ");
        FrameTarget.position = new Vector3(0,0,0);
        FrameTarget.rotation = Quaternion.Euler(0,0,0);
        RenderTarget.position = new Vector3(0,0,0);
        RenderTarget.rotation = Quaternion.Euler(0,0,0);
#if UNITY_XR_COMPOSITIONLAYERS
        _overlay.enabled = true;
        StartCoroutine(_StartPreview());
#endif
    }
    
#if UNITY_XR_COMPOSITIONLAYERS
    private IEnumerator _StartPreview()
    {
        IntPtr surface = IntPtr.Zero;
        yield return new WaitUntil(() =>
        {
            surface = OpenXRLayerUtility.GetLayerAndroidSurfaceObject(_overlay.GetInstanceID());
            PXR_Enterprise.StartPreviewfor4U(surface,Mode);
            return (surface != IntPtr.Zero);
        });
    }
#endif
    
    public float showTime = 1f;

    private int count = 0;
    private float deltaTime = 0f;
    public void StartGetImageData()
    {
        Debug.Log($"{tag} StartGetImageData ");
#if UNITY_XR_COMPOSITIONLAYERS
        _overlay.enabled = false;
#endif
        IntPtr data=Marshal.UnsafeAddrOfPinnedArrayElement(imgByte,0);
        PXR_Enterprise.SetCameraFrameBufferfor4U(width,height,ref data, (Frame frame) =>
        {
            // Debug.Log($"{tag} sensorState position:[{frame.sensorState.globalPose.position.x},{frame.sensorState.globalPose.position.y},{frame.sensorState.globalPose.position.z}]," +
            //           $" orientation:[{frame.sensorState.globalPose.orientation.x},{frame.sensorState.globalPose.orientation.y},{frame.sensorState.globalPose.orientation.z},{frame.sensorState.globalPose.orientation.w}] ");
            // FrameTarget.position=frame.pose.position;
            // FrameTarget.rotation = frame.pose.rotation;
            // FrameTarget.position = new Vector3(frame.pose.position.x, frame.pose.position.y, -frame.pose.position.z);
            // FrameTarget.rotation = new Quaternion(frame.pose.rotation.x, frame.pose.rotation.y, -frame.pose.rotation.z, -frame.pose.rotation.w); 
            FrameTarget.position = frame.pose.position;
            FrameTarget.rotation = frame.pose.rotation;
            if (showtime.isOn)
            {
                showtimeText.text = frame.timestamp+"";
            }
            else
            {
                showtimeText.text = frame.timestamp+"";
                texture.LoadRawTextureData(imgByte);
                texture.Apply();
            }
           
            Debug.Log($"{tag} imageAvailable ");
            Debug.Log($"{tag} onImageAvailable cameraFramePredictedDisplayTime = "+frame.timestamp +"   Time.deltaTime:"+Time.deltaTime);
            Debug.Log($"{tag} onImageAvailable size = "+frame.datasize);
            count++;
        });
        Debug.Log($"{tag}  StartGetImageData Mode=  {Mode}");
        bool ret=PXR_Enterprise.StartGetImageDatafor4U(Mode, width, height);
        isRuning=true;
        Debug.Log($"{tag}  StartGetImageData ret=  {ret}");
    }
    
    
    public void Release()
    {
        PXR_Enterprise.CloseCamerafor4U();
        isRuning=false;
    }

    private double time = 0;
    private SensorState a;
    private void Update()
    {
        deltaTime += Time.deltaTime;
        if (deltaTime >= showTime) {
            if (count>0)
            {
                float fps = count / deltaTime;
                float milliSecond = deltaTime * 1000 / count;
                string strFpsInfo = string.Format("当前每帧渲染间隔：{0:0.0} ms ({1:0.} 帧每秒)", milliSecond, fps);
                fpsText.text = strFpsInfo;
            }
            count = 0;
            deltaTime = 0f;
        }
        if (reopen)
        {
            reopen = false;
            PXR_Enterprise.StartGetImageDatafor4U(Mode, width, height);  
            isRuning=true;
        }

       
        time= PXR_Enterprise.GetPredictedDisplayTime();
        a=PXR_Enterprise.GetPredictedMainSensorState(time,false);
        RenderTarget.position = a.pose.position;
        RenderTarget.rotation = a.pose.rotation; 
        // RenderTarget.position = new Vector3(a.pose.position.x, a.pose.position.y, -a.pose.position.z);
        // RenderTarget.rotation = new Quaternion(a.pose.rotation.x, a.pose.rotation.y, -a.pose.rotation.z, -a.pose.rotation.w); 
    }
    static bool  reopen = false;
    private void OnApplicationPause(bool pauseStatus)
    {
        if (isRuning)
        {
            // PXR_Enterprise.SetRunningState(pauseStatus);
            if (pauseStatus)
            {
                PXR_Enterprise.CloseCamerafor4U();
            }
            else
            {
                PXR_Enterprise.OpenCameraAsyncfor4U(ret =>
                {
                    Debug.Log($"{tag}  OpenCameraAsync reopen=  {ret}");
                    reopen = ret;
                });
            }
        }
        
    }
}
#endif