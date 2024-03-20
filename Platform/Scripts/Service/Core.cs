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
using System.Runtime.CompilerServices;
using Pico.Platform.Framework;
using Unity.XR.PXR;
using UnityEngine;

[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]

namespace Pico.Platform
{
    /**
     * \defgroup Platform Services
     */
    /**
     * \ingroup Platform
     *
     */
    public static class CoreService
    {
        public static bool Initialized = false;
        public static string NotInitializedError = "Platform SDK has not been initialized!";

        /// <summary>Gets whether the Platform SDK has been initialized.</summary>
        /// <returns>
        /// * `true`: initialized
        /// * `false`: not initialized
        /// </returns>
        public static bool IsInitialized()
        {
            return Initialized;
        }

        /// <summary>
        /// Gets the app ID for the current app.
        /// </summary>
        /// <returns>The app ID.</returns>
        /// <exception cref="UnityException">If the app ID cannot be found, this exception will be thrown.</exception>
        public static string GetAppID(string appId = null)
        {
            string configAppID = PXR_PlatformSetting.Instance.appID.Trim();
            if (!string.IsNullOrWhiteSpace(appId) && !string.IsNullOrWhiteSpace(configAppID) && appId != configAppID)
            {
                throw new UnityException("The parameter appId is inconsistent with the configured appId");
            }

            if (!string.IsNullOrWhiteSpace(appId))
            {
                return appId;
            }

            if (!string.IsNullOrWhiteSpace(configAppID))
            {
                return configAppID;
            }

            throw new UnityException("Cannot find appId");
        }

        /// <summary>
        /// Initializes the Platform SDK asynchronously. 
        /// </summary>
        /// <param name="appId">The app ID for the Platform SDK. If not provided, Unity editor configuration will be applied.</param>
        /// <returns>The initialization result.</returns>
        /// <exception cref="UnityException">If the input app ID is null or empty or if the initialization fails, this exception will be thrown.</exception>
        /// <exception cref="NotImplementedException">If the current platform is not supported, this exception will be thrown.</exception>
        public static Task<PlatformInitializeResult> AsyncInitialize(string appId = null)
        {
            if (Initialized)
            {
                return new Task<PlatformInitializeResult>(0);
            }

            appId = GetAppID(appId);
            if (String.IsNullOrWhiteSpace(appId))
            {
                throw new UnityException("AppID cannot be null or empty");
            }

            Task<PlatformInitializeResult> task;
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
                var requestId = CLIB.ppf_InitializeAndroidAsynchronous(appId, activity.GetRawObject(), IntPtr.Zero);
                if (requestId == 0)
                {
                    throw new Exception("PICO PlatformSDK failed to initialize");
                }

                task = new Task<PlatformInitializeResult>(requestId);
            }
            else if ((Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor))
            {
                var config = Resources.Load<TextAsset>("PicoSdkPCConfig");
                var logDirectory = Path.GetFullPath("Logs");
                if (config == null)
                {
                    throw new UnityException($"cannot find PC config file Resources/PicoSdkPCConfig");
                }

                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                var requestId = CLIB.ppf_PcInitAsynchronousWrapper(appId, config.text, logDirectory);
                if (requestId == 0)
                {
                    throw new Exception("PICO PlatformSDK failed to initialize");
                }
                else
                {
                    task = new Task<PlatformInitializeResult>(requestId);
                }
            }
            else
            {
                throw new NotImplementedException("PICO platform is not implemented on this platform yet.");
            }

            Initialized = true;
            Runner.RegisterGameObject();
            return task;
        }

        /// <summary>
        /// Initializes the Platform SDK synchronously.
        /// </summary>
        /// <param name="appId">The app ID for the Platform SDK. If not provided, Unity editor configuration will be applied.</param>
        /// <exception cref="NotImplementedException">If the current platform is not supported, this exception will be thrown.</exception>
        /// <exception cref="UnityException">If the initialization fails, this exception will be thrown.</exception>
        public static void Initialize(string appId = null)
        {
            if (Initialized)
            {
                return;
            }

            appId = GetAppID(appId);
            if (String.IsNullOrWhiteSpace(appId))
            {
                throw new UnityException("AppID must not be null or empty");
            }

            PlatformInitializeResult initializeResult;
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");

                initializeResult = CLIB.ppf_InitializeAndroid(appId, activity.GetRawObject(), IntPtr.Zero);

                if (initializeResult == PlatformInitializeResult.Success ||
                    initializeResult == PlatformInitializeResult.AlreadyInitialized)
                {
                    Initialized = true;
                }
            }
            else if ((Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor))
            {
                var config = Resources.Load<TextAsset>("PicoSdkPCConfig");
                if (config == null)
                {
                    throw new UnityException($"cannot find PC config file Resources/PicoSdkPCConfig");
                }

                var logDirectory = Path.GetFullPath("Logs");
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                initializeResult = CLIB.ppf_PcInitWrapper(appId, config.text, logDirectory);
                if (initializeResult == PlatformInitializeResult.Success ||
                    initializeResult == PlatformInitializeResult.AlreadyInitialized)
                {
                    Initialized = true;
                }
            }
            else
            {
                throw new NotImplementedException("PICO platform is not implemented on this platform yet.");
            }

            if (!Initialized)
            {
                throw new UnityException($"PICO Platform failed to initialize：{initializeResult}.");
            }

            Runner.RegisterGameObject();
        }

        /**
         * \overload Task<GameInitializeResult> GameInitialize(string accessToken)
         */
        /// <summary>
        /// Initializes game-related modules, such as room, matchmaking, and network.
        /// </summary>
        /// <param name="accessToken">The access token of Platform SDK. You can get the access token by calling `UserService.GetAccessToken()`.</param>
        public static Task<GameInitializeResult> GameInitialize(string accessToken)
        {
            if (Initialized)
            {
                return new Task<GameInitializeResult>(CLIB.ppf_Game_InitializeWithToken(accessToken));
            }

            Debug.LogError(NotInitializedError);
            return null;
        }

        /**
         * \overload Task<GameInitializeResult> GameInitialize()
         */
        /// <summary>
        /// Initializes modules without token related with game, such as room, matchmaking, and net.
        /// </summary>
        public static Task<GameInitializeResult> GameInitialize()
        {
            if (Initialized)
            {
                return new Task<GameInitializeResult>(CLIB.ppf_Game_InitializeAuto());
            }

            Debug.LogError(NotInitializedError);
            return null;
        }

        /// <summary>
        /// Uninitializes game-related modules, such as room, matchmaking, and network.
        /// </summary>
        /// <returns>
        /// * `true`: success
        /// * `false`: failure
        /// </returns>
        public static bool GameUninitialize()
        {
            if (Initialized)
            {
                return CLIB.ppf_Game_UnInitialize();
            }

            return false;
        }
    }
}