This article records the changes to the PICO Unity Integration SDK in version 3.2.0.
## What's new
Released on: May 29, 2025
Required PICO device's system version: 5.13.0 or later (PICO Neo3 and PICO 4 series devices are currently not supported)
The newly added content is as follows:
| **Module** | **Description** |
| --- | --- |
| General | Added the [PICO XR Portal](https://developer.picoxr.com/document/unity/pico-xr-portal/) - a developer portal consisting of four sections: Configs, Tools, Samples, and About. |
|  | [Project Validation](https://developer.picoxr.com/document/unity/project-validation/) supported checking the following settings: <br>  <br> * When using Unity6, check if the current project has the **Run In Background** option checked. If not, report an error. <br> * Check if the **MRC** checkbox is checked in the **PXR_Manager (Script)** panel. If not, report an error. <br> *  Verify if the **Display Refresh Rates** parameter is set to **Default**. If not, report an error. <br> *  When using Vulkan, check if the **Optimize Buffer Discards** option is checked. If not, report an error. |
|  | [PICO Building Blocks](https://developer.picoxr.com/document/unity/pico-building-blocks) supported setting up the following: <br>  <br> * Spatial Audio: free field <br> * Spatial Audio: ambisonics |
| Rendering | Added the `UPxr_SetSuperResolutionOrSharpening` API for dynamically enabling and disabling the [Super Resolution](https://developer.picoxr.com/document/unity/super-resolution) and [Sharpening](https://developer.picoxr.com/document/unity/sharpening) functionalities. |
| Sense Pack | Added the following APIs for getting the progress of uploading and downloading shared spatial anchors: <br>  <br> * `UploadSpatialAnchorWithProgressAsync`: Upload spatial anchors and get the upload progress. <br> * `DownloadSharedSpatialAnchorWithProgressAsync`: Download shared spatial anchors and get the download progress. |
| SecureMR | Added [SecureMR](https://developer.picoxr.com/document/unity/securemr-overview) capabilities, which enables secure, AI-powered mixed reality use cases while maintaining rigorous protection of user data and privacy. |
| Enterprise services | Added the following APIs: <br>  <br> * `SetDeviceOwner`: Sets an app as the device owner app. <br> * `GetDeviceOwner`: Gets the device owner app. <br> * `SetBrowserHomePage`: Sets a home page for the browser. <br> * `GetBrowserHomePage`: Gets the home page of the browser. <br> * `SetMotionTrackerAutoStart`: Sets the capability for the PICO Motion Tracker to automatically power on when plugged in. <br> * `AllowWifiAutoJoin`: Enables the device to automatically join WiFi. <br> * `GetLargeSpaceBoundsInfoWithType`: Gets the bound information of the large space. |
Below are the changes to APIs:
| **Class** | **API** | **What's changed** |
| --- | --- | --- |
| [PXR_Boundary](https://developer.picoxr.com/reference/unity/client-api/PXR_Boundary) | GetDimensions | Currently, only supported to be called at the StageLevel. |
|  | GetSeeThroughTrackingState | Deprecated. |
|  | UseGlobalPose | Deprecated. |
| [PXR_FoveationRendering](https://developer.picoxr.com/reference/unity/client-api/PXR_FoveationRendering/) | SetFoveationParameters | Deprecated. |
| [PXR_HandTracking](https://developer.picoxr.com/reference/unity/client-api/PXR_HandTracking/) | GetSettingState | Deprecated. |
| [PXR_Input](https://developer.picoxr.com/reference/unity/client-api/PXR_Input/) | GetDominantHand | Deprecated. |
|  | SetDominantHand | Deprecated. |
|  | SetControllerVibration | Deprecated. |
|  | SetControllerVibrationEvent | Deprecated. |
|  | StopControllerVCMotor | Deprecated. |
|  | StartControllerVCMotor | Deprecated. |
|  | SetControllerAmp | Deprecated. |
|  | StartVibrateBySharem | Deprecated. |
|  | SaveVibrateByCache | Deprecated. |
|  | StartVibrateByCache | Deprecated. |
|  | ClearVibrateByCache | Deprecated. |
|  | StartVibrateByPHF | Deprecated. |
|  | PauseVibrate | Deprecated. |
|  | ResumeVibrate | Deprecated. |
|  | UpdateVibrateParams | Deprecated. |
|  | GetBodyTrackingPose | Deprecated. |
|  | GetMotionTrackerConnectStateWithID | Deprecated. |
|  | GetMotionTrackerBattery | Deprecated. |
|  | GetMotionTrackerCalibState | Deprecated. |
|  | SetBodyTrackingMode | Deprecated. |
|  | SetBodyTrackingBoneLength | Deprecated. |
|  | ResetController | Deprecated. |
|  | SetArmModelParameters | Deprecated. |
|  | CreateHapticStream | Deprecated. |
|  | WriteHapticStream | Deprecated. |
|  | SetHapticStreamSpeed | Deprecated. |
|  | GetHapticStreamSpeed | Deprecated. |
|  | GetHapticStreamCurrentFrameSequence | Deprecated. |
|  | StartHapticStream | Deprecated. |
|  | StopHapticStream | Deprecated. |
|  | RemoveHapticStream | Deprecated. |
|  | AnalysisHapticStreamPHF | Deprecated. |
| [PXR_MotionTracking](https://developer.picoxr.com/reference/unity/client-api/PXR_MotionTracking/) | WantEyeTrackingService | Deprecated. |
|  | WantFaceTrackingService | Deprecated. |
|  | GetFaceTrackingSupported | Deprecated. |
|  | StartFaceTracking | Deprecated. |
|  | StopFaceTracking | Deprecated. |
|  | GetFaceTrackingState | Deprecated. |
|  | GetFaceTrackingData | Deprecated. |
|  | BodyTrackingAbnormalCalibrationData | Deprecated. Please use `GetBodyTrackingState` instead. |
|  | BodyTrackingStateError | Deprecated. Please use `GetBodyTrackingState` instead. |
|  | BodyTrackingAction | Deprecated. |
|  | RequestMotionTrackerCompleteAction | New callback event. |
|  | MotionTrackerConnectionAction | New callback event. |
|  | MotionTrackerPowerKeyAction | New callback event. |
|  | MotionTrackerNumberOfConnections | Deprecated. You can use `MotionTrackerConnectionAction` to be notified when the connection state of PICO Motion Tracker changes. |
|  | MotionTrackerBatteryLevel | Deprecated. You can use `GetMotionTrackerBattery` to get the battery of a PICO Motion Tracker. |
|  | MotionTrackerKeyAction | Deprecated. You can use `MotionTrackerPowerKeyAction` to receive events for the Power key of PICO Motion Trackers. |
|  | MotionTrackingModeChangedAction | Deprecated. |
|  | CheckMotionTrackerNumber | Newly added. |
|  | GetMotionTrackerLocation | Newly added. |
|  | GetMotionTrackerBattery | Newly added. |
|  | GetMotionTrackerConnectStateWithSN | Not supported. You can use `MotionTrackerConnectionAction` to be notified when the connection state of PICO Motion Tracker changes. |
|  | GetMotionTrackerDeviceType | Deprecated. |
|  | CheckMotionTrackerModeAndNumber | Deprecated. Please use `CheckMotionTrackerNumber` instead. <br>  |
|  | GetMotionTrackerMode | Deprecated. |
|  | GetMotionTrackerLocations | Deprecated. Please use `GetMotionTrackerLocation` instead. |
|  | ExpandDeviceConnectionAction | New callback event. |
|  | ExpandDeviceBatteryAction | New callback event. |
|  | ExtDevConnectAction | Deprecated. Please use `ExpandDeviceConnectionAction` instead. |
|  | ExtDevBatteryAction | Deprecated. Please use `ExtDevBatteryAction` instead. |
|  | GetExtDevTrackerConnectState | Deprecated. You can use `ExpandDeviceConnectionAction` to get notified when the state of the connection between the PICO Motion Tracker and an external device changes. |
|  | SetExpandDeviceVibrate | Newly added. |
|  | GetExpandDevice | Newly added. |
|  | SetExpandDeviceCustomData | Newly added. |
|  | GetExpandDeviceCustomData | Newly added. |
|  | GetExpandDeviceBattery | Newly added. |
|  | SetExtDevTrackerMotorVibrate | Deprecated. Please use `SetExpandDeviceVibrate` instead. |
|  | SetExtDevTrackerByPassData | Deprecated. Please use `SetExpandDeviceCustomData` instead. |
|  | GetExtDevTrackerByPassData | Deprecated. Please use `GetExpandDeviceCustomData` instead. |
|  | GetExtDevTrackerBattery | Deprecated. Please use `GetExpandDeviceBattery` instead. |
|  | GetExtDevTrackerKeyData | Not supported. |
| [PXR_System](https://developer.picoxr.com/reference/unity/client-api/PXR_System/) | SetExtraLatencyMode | Not supported. |
|  | EnableFaceTracking | Not supported. |
|  | EnableLipSync | Not supported. |
|  | GetFaceTrackingData | Not supported. |
|  | SetEyeFOV | Not supported. |
|  | SetFaceTrackingStatus | Not supported. |
|  | SetCommonBrightness | Not supported. |
|  | GetCommonBrightness | Not supported. |
|  | GetScreenBrightnessLevel | Not supported. |
|  | SetScreenBrightnessLevel | Not supported. |
|  | *GetDisplayFrequenciesAvailable* | Newly added. You can use it to get the available display refresh rates. |
|  | Action<int> SessionStateChanged; | Modified to the enumeration type Action<XrSessionState> SessionStateChanged. |

