This article records the changes to the PICO Unity Integration SDK in version 3.3.0.
## Download the SDK
Download the latest version of the PICO Unity Integration SDK from the [Download](https://developer-global.pico-interactive.com/resources/#sdk) screen.
## What's new
Released on: September 18, 2025
Required PICO device's system version: 5.13.0 for PICO 4 series, 5.14.0 or later for PICO 4 Ultra series
**Add**
| **Module** | **Description** |
| --- | --- |
| General | Supported the Unity OpenXR Plugin, and you can use this plugin to integrate XR functionalities into your app. For more information, refer to [Add support for the Unity OpenXR Plugin](https://developer.picoxr.com/document/unity/support-for-the-unity-openxr-plugin/). |
| PICO Building Blocks | Added the following blocks: <br>  <br> * PICO Spatial Anchor Sample <br> * PICO Spatial Mesh <br> * PICO Scene Capture <br> * PICO Composition Layer Overlay <br> * PICO Composition Layer Underlay <br>  <br> For more information, refer to [PICO Building Blocks](https://developer.picoxr.com/document/unitypico-building-blocks/). |
|  | Compatible with the Unity OpenXR Plugin. |
| Enterprise services | Added the following APIs: <br>  <br> * `GetHeadTrackingStatus`: Gets the status of HMD tracking. <br> * `GetHeadPose`: Gets the pose of the HMD. <br> * `GetControllerPose`: Gets the pose of the controller. <br> * `GetSwiftPose`: Gets the pose of a motion tracker. <br> * `GetSwiftTrackerDevices`: Gets the information of motion trackers. <br> * `GetHeadIMUData`: Gets the IMU data of the HMD. <br> * `GetControllerIMUData`: Gets the IMU data of the controller. <br> * `GetSwiftIMUData`: Gets the IMU data of a motion tracker. <br> * `StartSwiftTrackerPairing`: Starts pairing motion tracker(s). <br> * `UnBondSwiftTracker`: Unbonds motion tracker(s). <br> * `ResetTracking`: Resets tracking. <br> * `SetFenceColor`: Sets the color of the fence. <br> * `GetFenceColor`: Gets the color of the fence. <br> * `SetUsbTetheringStaticIP`: Sets the static IP for USB tethering. <br> * `GetUsbTetheringStaticIPLocal`: Gets the local static IP for USB tethering. <br> * `GetUsbTetheringStaticIPClient`: Gets the client static IP for USB tethering. <br> * `SetLargeSpaceMapScale`: Sets the scale of the large-space map. <br> * `GetPredictedMainSensorState2`: Gets the predicted pose and status of the main sensor when the VST image is being displayed. <br> * `UseGlobalPose`: Uses the global pose for HMD and controller tracking. <br> * `ConvertPoseCoordinate`: Converts the coordinate of a pose. |
| Developer Tools | Added the PICO Debugger, which is a debugging tool that allows you not only to view logs and scene information, but also to use its built-in tools to optimize your application in a more targeted way. For more information, refer to "[PICO Debugger](https://developer.picoxr.com/document/unity/pico-debugger)". |
**Optimize**
| **Module** | **Description** |
| --- | --- |
| Rendering | Optimized the "Use Premultiplied Alpha" parameter used for configuring composition layers: <br>  <br> * Enabled the premultiplied alpha effect for content, which multiplies the RGB color channels by the alpha value `(R×A, G×A, B×A)`. <br> * Improved the rendering performance of transparent elements, such as UI, particles, and more. <br> * Fixed visual defects such as color fringing at the edges of semi-transparent objects. <br> * Supported the specification for OpenXR and GPU hybrid rendering. |
**Change**
| **Module** | **Description** |
| --- | --- |
| Rendering | Renamed the PXR_Over Lay component used for configuring composition layer parameters to PXR_Composition Layer. |

