This article records the changes to the PICO Unity Integration SDK in version 3.0.5.
## Download the SDK
Download the latest version of the PICO Unity Integration SDK from the [Download](https://developer-global.pico-interactive.com/resources/#sdk) screen.
## What's new
Released on: November 05, 2024
Required PICO device's system version: 5.12.0 or later
**Add**
| **Module** | **Description** |
| --- | --- |
| General | Added the PICO Building Blocks system, which can help you quickly set up features with one click. The features are either provided by the PICO Unity Integration SDK or Unity itself. |
| Sense Pack | Added the following new semantic labels for Scene Capture: `Curtain`, `Cabinet`, `Bed`, `Plant`, `Screen`, `Refrigerator`, `WashingMachine`, `AirConditioner`, `Lamp`, `WallArt`. |
| Project Validation | Project Validation supports checking whether your project has correctly set up the keystore and key. |
| Input | * Added the `GetControllerStatus` API for retrieving the connection status of controller. <br> * Added the `InputDeviceChanged` event for receiving the notification when the input device (hand poses / controllers) has changed. |
| Spatial Audio | The PICO Spatial Audio Plugin combines the Spatial Mesh capability with spatial audio rendering, allowing virtual sound sources to interact with the user's real environment. |
| Enterprise Service | * Added value `LARGESPACE_MAP_INFO ` to enumeration`SystemInfoEnum`, which is used to get the information of the large space map. <br> * Added the following APIs: <br>    * `GetControllerVibrateAmplitude`: Gets the vibration amplitude of controllers; <br>    * `SetHMDVolumeKeyFunc`: Sets a functionality for the volume button of the HMD; <br>    * `GetHMDVolumeKeyFunc`: Gets the functionality of the volume button of the HMD; <br>    * `GetPowerManageMode`: Gets the device's power management mode; <br>    * `GetEyeTrackRate`: Gets the frame rate of eye tracking; <br>    * `GetTrackFrequency`: Gets the tracking frequency; <br>    * `GetDistanceSensitivity`: Gets the device's distance sensing sensitivity; <br>    * `GetSpeedSensitivity`: Gets the device's speed sensing sensitivity; <br>    * `SetMRCollisionAlertSensitivity`: Sets the device's collision alert sensitivity;l <br>    * `GetMRCollisionAlertSensitivity`: Gets the device's collision alert sensitivity; <br>    * `ConnectWifi`: Connects to a WiFi; <br>    * `SetStaticIpConfigurationtoConnectWifi`: Sets up WifiConfiguration and connects to the WiFi; <br>    * `GetSingleEyeSource`: Gets the eye that serves as the image source; <br>    * `GetViewVisual`: Gets the device's view mode; <br>    * `GetAcceptCastMode`: Gets whether the device accepts screen sharing from the external device; <br>    * `GetScreenCastMode`: Gets whether the device allows the sharing of its screen to the external device; <br>    * `GetScreenRecordShotRatio`: Gets the aspect ratio for screen recording and screenshots; <br>    * `GetScreenResolution`: Gets the resolution for screen recording and screenshots; <br>    * `GetScreenRecordFrameRate`:Gets the frame rate for screen recording. |
**Bugfix**

* Fixed the issue that the controllers' battery level couldn't be updated immediately after switching between controllers and hand poses for input.
* Fixed the issue that the battery level of the left controller was incorrectly displayed as that of the right controller.
