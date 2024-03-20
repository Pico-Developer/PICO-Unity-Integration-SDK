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

using UnityEditor;

namespace Pico.Platform.Editor
{
    /// <summary>
    /// Unity Setting Getter and Setter
    /// </summary>
    public class Gs
    {
        public static string productName
        {
            get { return PlayerSettings.productName; }
            set { PlayerSettings.productName = value; }
        }

        public static string packageName
        {
            get { return PlayerSettings.GetApplicationIdentifier(EditorUserBuildSettings.selectedBuildTargetGroup); }
            set { PlayerSettings.SetApplicationIdentifier(EditorUserBuildSettings.selectedBuildTargetGroup, value); }
        }

        public static BuildTargetGroup buildTargetGroup
        {
            get { return EditorUserBuildSettings.selectedBuildTargetGroup; }
            set
            {
                EditorUserBuildSettings.selectedBuildTargetGroup = value;
                if (value == BuildTargetGroup.Android)
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                }
            }
        }

        public static BuildTarget buildTarget
        {
            get { return EditorUserBuildSettings.activeBuildTarget; }
        }

        public static BuildTarget selectedStandaloneTarget
        {
            get { return EditorUserBuildSettings.selectedStandaloneTarget; }
            set
            {
                EditorUserBuildSettings.selectedStandaloneTarget = value;
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, value);
            }
        }

        public static AndroidSdkVersions minimumApiLevel
        {
            get { return PlayerSettings.Android.minSdkVersion; }
            set { PlayerSettings.Android.minSdkVersion = value; }
        }

        public static AndroidSdkVersions targetSdkVersion
        {
            get { return PlayerSettings.Android.targetSdkVersion; }
            set { PlayerSettings.Android.targetSdkVersion = value; }
        }


        public static string bundleVersion
        {
            get { return PlayerSettings.bundleVersion; }
            set { PlayerSettings.bundleVersion = value; }
        }

        public static int bundleVersionCode
        {
            get { return PlayerSettings.Android.bundleVersionCode; }
            set { PlayerSettings.Android.bundleVersionCode = value; }
        }

        public static string keystoreName
        {
            get { return PlayerSettings.Android.keystoreName; }
            set { PlayerSettings.Android.keystoreName = value; }
        }


        public static string keystorePass
        {
            get { return PlayerSettings.Android.keystorePass; }
            set { PlayerSettings.Android.keystorePass = value; }
        }

        public static string keyaliasName
        {
            get { return PlayerSettings.Android.keyaliasName; }
            set { PlayerSettings.Android.keyaliasName = value; }
        }

        public static string keyaliasPass
        {
            get { return PlayerSettings.Android.keyaliasPass; }
            set { PlayerSettings.Android.keyaliasPass = value; }
        }

        public static bool useCustomKeystore
        {
            get { return PlayerSettings.Android.useCustomKeystore; }
            set { PlayerSettings.Android.useCustomKeystore = value; }
        }

        public static ScriptingImplementation scriptBackend
        {
            get { return PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup); }
            set
            {
                PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, value);
                if (value == ScriptingImplementation.Mono2x)
                {
                    //mono only support armv7
                    targetArchitectures = AndroidArchitecture.ARMv7;
                }
                else if (value == ScriptingImplementation.IL2CPP)
                {
                    //il2cpp use a reasonable default value
                    if (targetArchitectures != AndroidArchitecture.ARMv7 && targetArchitectures != AndroidArchitecture.ARM64)
                    {
                        targetArchitectures = AndroidArchitecture.ARM64;
                    }
                }
            }
        }

        public static AndroidArchitecture targetArchitectures
        {
            get { return PlayerSettings.Android.targetArchitectures; }
            set { PlayerSettings.Android.targetArchitectures = value; }
        }

        public static AndroidBuildType androidBuildType
        {
            get { return EditorUserBuildSettings.androidBuildType; }
            set { EditorUserBuildSettings.androidBuildType = value; }
        }

        public static UIOrientation UIOrientation
        {
            get { return PlayerSettings.defaultInterfaceOrientation; }
            set { PlayerSettings.defaultInterfaceOrientation = value; }
        }
    }
}