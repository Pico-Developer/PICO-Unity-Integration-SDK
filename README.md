# PICO Unity Integration SDK
The PICO Unity Integration SDK is a Unity-based software development kit developed by PICO. The SDK packages a series of features covering rendering, input, tracking, mixed reality, platform services, etc.
What's in the PICO Integration folder?
## Quick Start
https://developer-cn.picoxr.com/en/document/unity/chapter-overview/

## What's new
Released on: January 24, 2024
Required PICO device's system version: 5.9.0 or later
### Add
| **Module** | **Description** |
| --- | --- |
| Rendering | Compositor layers of the "External Surface" texture type supported playing back HDR videos and dynamically setting the HDR type (`HDRFlags`) based on the video's type during playback.  |
| Interaction | * Supported enabling the interaction between hands and 3D objects using the XR Interaction Toolkit. <br> * Supported previewing the hand tracking effect in the PICO Developer Center (PDC). |
| Enterprise Service <br>  | Added the following APIs: <br>  <br> * `SetWifiP2PDeviceName`: Set a name for the WiFi P2P device. <br> * `GetWifiP2PDeviceName`: Get the name of the WiFi P2P device. <br> * `SetScreenBrightness`: Set the brightness of the screen. <br> * `SwitchSystemFunction`: Enable or disable a specified system function. <br> * `SetSystemKeyUsability`: Enable or disable a system key. <br> * `SetLauncher`: Set an app as the launcher. <br> * `SetSystemAutoSleepTime`: Sets a time after which the device automatically enters the sleep mode <br> * `OpenTimingStartup`: Schedule auto startup for the device. <br> * `CloseTimingStartup`: Disable scheduled auto startup for the device. <br> * `OpenTimingShutdown`: Schedule auto shutdown for the device. <br> * `CloseTimingShutdown`: Disable scheduled auto shutdown for the device. <br> * `SetTimeZone`: Sets a time zone for the device. <br> * `AppCopyrightVerify`: Check whether the user has the entitlement to use the app. <br> * `GotoEnvironmentTextureCheck`: Go to the environment texture check page. |
### Optimize
| **Module** | **Description** |
| --- | --- |
| Interaction <br>  | Optimized the stability, performance, and interaction capabilities of hand tracking. Below are specific details: <br>  <br> * Improved the stability of wrist tracking, making the tracking of complete hand more stable and reducing the jitter and loss of hand model. <br> * Improved the success rate of pinch. To enhance the accuracy of pinch recognition, PICO has studied the different ways in which users pinch and conduct corresponding training. <br> * Decreased the latency in initiating hand poses, allowing virtual hand models to closely mirror users' hand movements in real time. <br> * Improved the stability of fingers, reducing finger jitter. |
### Bugfix

* Fixed the issue that using super resolution and adaptive resolution simultaneously may lead to misalignment of the super-resolutioned areas. 
* Fixed the issue where continuous frame drops and eventual crashes occur when using dynamic overlay layers in non-full-frame scenarios.
* Fixed the issue where the SDK only provides the vertex coordinates of the ceiling and floor saved by the Room Capture app, and the provided length and width are default values rather than the actual values.

