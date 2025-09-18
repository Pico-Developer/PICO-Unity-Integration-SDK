
/*******************************************************************************
Copyright © 2015-2022 PICO Technology Co., Ltd.All rights reserved.  

NOTICE：All information contained herein is, and remains the property of 
PICO Technology Co., Ltd. The intellectual and technical concepts 
contained herein are proprietary to PICO Technology Co., Ltd. and may be 
covered by patents, patents in process, and are protected by trade secret or 
copyright law. Dissemination of this information or reproduction of this 
material is strictly forbidden unless prior written permission is obtained from
PICO Technology Co., Ltd. 
*******************************************************************************/
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
namespace Unity.XR.PXR.Debugger
{
    
[CreateAssetMenu(fileName = "MainConfig", menuName = "ScriptableObjects / PXR_PicoDebuggerSO", order = 1)]
public class PXR_PicoDebuggerSO : ScriptableObject
{
    private static PXR_PicoDebuggerSO _instance;
    public static PXR_PicoDebuggerSO Instance
    {
        get
        {
            if (_instance == null)
            {
                GetAsset(out PXR_PicoDebuggerSO picoDebuggerSO, "PXR_PicoDebuggerSO");
                _instance = picoDebuggerSO;
            }

            return _instance;
        }
    }
    [Header("default")]
    public bool isOpen;
    public LauncherButton debuggerLauncherButton;
    public StartPosiion startPosition;
    [Header("console")]
    [Range(500,1000)]public int maxInfoCount;
    [Header("tools")]
    public LauncherButton rulerClearButton;
    internal static void GetAsset<T>(out T asset, string name) where T : PXR_PicoDebuggerSO
    {
        asset = null;
#if UNITY_EDITOR
        string path = GetPath(name);
        asset = AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;

        if (asset == null )
        {
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
        }
#else
        asset = Resources.Load<T>(name);
#endif
    }
    #if UNITY_EDITOR
    internal static string GetPath(string name)
    {
        string resourcesPath = Path.Combine(Application.dataPath, "Resources");
        if (!Directory.Exists(resourcesPath))
        {
            Directory.CreateDirectory(resourcesPath);
        }
        string assetPath = Path.GetRelativePath(Application.dataPath, Path.GetFullPath(Path.Combine(resourcesPath, $"{name}.asset")));
        // Unity's AssetDatabase path requires a slash before "Assets"
        return "Assets/" + assetPath.Replace('\\', '/');
    }

    public void AddToPreloadedAssets()
    {
        var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();

        if (!preloadedAssets.Contains(this))
        {
            preloadedAssets.Add(this);
            PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
        }
    }
#endif
#if UNITY_EDITOR
    public static PXR_PicoDebuggerSO GetSerializedObject(string path)
    {
        var config = AssetDatabase.LoadAssetAtPath<PXR_PicoDebuggerSO>(path);
        if (config == null)
        {
            Debug.LogError("Failed to load PXR_PicoDebuggerSO at path: " + path);
        }
        return config;
    }
#endif
}
}
#endif