using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.PICO.TOBSupport;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.UI;

public class VirtualDisplayDemo : MonoBehaviour
{
    private string tag = "VirtualDisplayDemo ----";
    private PXR_OverLay overlay = null;
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
        overlay = GetComponent<PXR_OverLay>();
        if (overlay == null)
        {
            Debug.LogError("PXRLog Overlay is null!");
            overlay = gameObject.AddComponent<PXR_OverLay>();
        }

        overlay.isExternalAndroidSurface = true;
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

            displayId = PXR_Enterprise.CreateVirtualDisplay("VirtualDisplayDemo", overlay.externalAndroidSurfaceObject,
                320, flags);
            showLog("CreateVirtualDisplay displayId：" + displayId);
        });
    }

    public void OpenApp()
    {
        showLog("StartApp ret：");
        Intent m = new Intent();
        m.setComponent("com.tencent.mm", "com.tencent.mm.ui.LauncherUI");
        int ret = PXR_Enterprise.StartApp(displayId, m);
        showLog("StartApp ret：" + ret);
    }


    public void ReleaseVirtualDisplay()
    {
        int ret = PXR_Enterprise.ReleaseVirtualDisplay(displayId);
        showLog("ReleaseVirtualDisplay ret：" + ret);
    }

    public void InjectEvent(int action, float x, float y)
    {
        int ret = PXR_Enterprise.InjectEvent(displayId, action, SOURCE_KEYBOARD, x, y);
    }

    public void bcak()
    {
        int ret = PXR_Enterprise.InjectEvent(displayId, ACTION_DOWN, SOURCE_KEYBOARD, KEYCODE_BACK);
    }
}