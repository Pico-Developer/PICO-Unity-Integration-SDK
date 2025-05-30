#if !PICO_XR
using System;
using System.Collections;
#if UNITY_XR_COMPOSITIONLAYERS
using Unity.XR.CompositionLayers;
using Unity.XR.CompositionLayers.Extensions;
using UnityEngine.XR.OpenXR.CompositionLayers;
#endif
using Unity.XR.PICO.TOBSupport;
using UnityEngine;
using UnityEngine.UI;


public class VirtualDisplayDemo_OpenXR : MonoBehaviour
{
    private string tag = "VirtualDisplayDemo ----";
#if UNITY_XR_COMPOSITIONLAYERS
    private CompositionLayer _overlay = null;
    private TexturesExtension _sourceTextures = null;
#endif
    
    public Text mylog;
    private bool isBind = false;
    private int displayId = -1;

    public const int VIRTUAL_DISPLAY_FLAG_AUTO_MIRROR = 16;
    public const int VIRTUAL_DISPLAY_FLAG_OWN_CONTENT_ONLY = 8;
    public const int VIRTUAL_DISPLAY_FLAG_PRESENTATION = 2;
    public const int VIRTUAL_DISPLAY_FLAG_PUBLIC = 1;
    public const int VIRTUAL_DISPLAY_FLAG_SECURE = 4;
    public const int SOURCE_KEYBOARD = 257;
    public const int ACTION_DOWN = 0;
    public const int ACTION_UP = 1;
    public const int ACTION_MOVE = 2;
    int KEYCODE_BACK = 4;

    private void Awake()
    {
#if UNITY_XR_COMPOSITIONLAYERS

        _overlay = GetComponent<CompositionLayer>();
        if (_overlay == null)
        {
            _overlay = gameObject.AddComponent<CompositionLayer>();
        }
#endif
       
        PXR_Enterprise.InitEnterpriseService();
    }

    public void showLog(string log)
    {
        Debug.Log(tag + log);
        mylog.text = log;
    }

    // Start is called before the first frame update
    void Start()
    {
        showLog("tobDemo:start");
        PXR_Enterprise.BindEnterpriseService(b =>
        {
            showLog("Bind绑定的返回值测试：" + b);
            isBind = true;

            PXR_Enterprise.SwitchSystemFunction(
                (int)SystemFunctionSwitchEnum.SFS_BASIC_SETTING_SHOW_APP_QUIT_CONFIRM_DIALOG, (int)SwitchEnum.S_OFF,
                b =>
                {
                    // showLog("SFS_BASIC_SETTING_SHOW_APP_QUIT_CONFIRM_DIALOG：" + b);
                });

            int flags = VIRTUAL_DISPLAY_FLAG_PUBLIC;
            flags |= 1 << 6; //DisplayManager.VIRTUAL_DISPLAY_FLAG_SUPPORTS_TOUCH
            flags |= 1 << 7; //DisplayManager.VIRTUAL_DISPLAY_FLAG_ROTATES_WITH_CONTENT
            flags |= 1 << 8; //DisplayManager.VIRTUAL_DISPLAY_FLAG_DESTROY_CONTENT_ON_REMOVAL
            flags |= VIRTUAL_DISPLAY_FLAG_OWN_CONTENT_ONLY;
#if UNITY_XR_COMPOSITIONLAYERS
            StartCoroutine(CreateVirtualDisplay(flags));
#endif
            showLog("CreateVirtualDisplay displayId：" + displayId);
        });
    }
#if UNITY_XR_COMPOSITIONLAYERS
    private IEnumerator CreateVirtualDisplay(int flags)
    {
        IntPtr surface = IntPtr.Zero;
        yield return new WaitUntil(() =>
        {
            surface = OpenXRLayerUtility.GetLayerAndroidSurfaceObject(_overlay.GetInstanceID());

            displayId = PXR_Enterprise.CreateVirtualDisplay("VirtualDisplayDemo", surface,
                320, flags,720,1280);
            return (surface != IntPtr.Zero);
        });
    }
#endif
   
    public void OpenApp()
    {
        showLog("StartApp ret：");
        Intent m = new Intent();
        m.setComponent("com.pico.myapplication", "com.pico.myapplication.MainActivity");
        int ret = PXR_Enterprise.StartApp(displayId, m);
        showLog("StartApp ret：" + ret);
    }

    public void KillApp()
    {
        int[] args1 = {};
        string[] args2 = {"com.pico.myapplication"};
        PXR_Enterprise.KillAppsByPidOrPackageName(args1, args2);
    }

    public void ReleaseVirtualDisplay()
    {
        int ret = PXR_Enterprise.ReleaseVirtualDisplay(displayId);
        showLog("ReleaseVirtualDisplay ret：" + ret);
    }

    public void InjectEvent(int action, float x, float y)
    {
        int ret = PXR_Enterprise.InjectEvent(displayId, action, SOURCE_KEYBOARD, 720*x,1280*y);
    }

    public void bcak()
    {
        int ret = PXR_Enterprise.InjectEvent(displayId, ACTION_DOWN, SOURCE_KEYBOARD, KEYCODE_BACK);
    }
}
#endif