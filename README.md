This article records the changes to the PICO Unity Integration SDK in version 3.0.0.
## Download the SDK
Download the latest version of the PICO Unity Integration SDK from the [Download](https://developer-global.pico-interactive.com/resources/#sdk) screen.
## What's new
Released on: September 02, 2024
Required PICO device's system version: 5.11.0 or later
Since version 3.0.0, the PICO Unity Integration SDK supports Unity 2023.

**Add**
| **Module** | **Description** |
| --- | --- |
| Sense Pack | Added the Shared Spatial Anchor feature. In the same physical environment, when different users play the same app, they can share content using shared spatial anchors. |
|  | Added the Spatial Mesh feature. It supports dynamically scanning the physical environment in real time and then converting the scene content into spatial meshes. You can PICO SDK to retrieve and use spatial meshed in your app. |
|  | Added the MR Safeguard feature. When the distance between the objects in the virtual scene and the PICO headset or controllers is within a certain range, the virtual scene will become semi-transparent, revealing the real-world scene. |
|  | Added control over spatial data permission. For PICO apps, users can decide whether to authorize your app to use their spatial data. |
| Interaction <br>  | Below are the updates to the Hand Tracking feature: <br>  <br> * Added the High Frequency Tracking mode. It supports tracking a user's hands at 60Hz to capture faster hand movements. This improves the accuracy of hand tracking data. <br> * Added the Adaptive Hand Model feature. It supports automatically adjusting the size of virtual hand models with the change to the size of a user's hands. <br> * Provided new hand models, which provide a more realistic representation of the human hand. |
|  | Below are the updates to motion tracking: <br>  <br> * Added the Object Tracking feature. It supports tracking PICO Motion Trackers and outputs 6DoF data for them. You can use this feature to track PICO Motion Trackers or the objects they attach to.  <br> * Added the callback function for the button event of the PICO Motion Tracker. <br> * Supported dynamically switching the motion tracking mode. <br> * Supported stepping recognition. <br> * Supported passing through data between PICO devices and external devices. |
| Enterprise Service <br>  | * Added a number of APIs. For details, refer to the PXR_Enterprise class (from `SetSystemDate` to `OfflineSystemUpdate`) in the API reference. <br> * Added new enumeration values for `SwitchSystemFunction` and `GetSwitchSystemFunctionStatus`. For details, refer to the enumeration values list from `SFS_SYSTEM_VIBRATION_ENABLED` to `SFS_RETRIEVE_MAP_BY_MARKER_FIRST` in the descriptions of these two APIs.  |
| Developer Tools | The PICO Unity Live Preview Plugin has supported previewing Unity XR Hands in real time. |
| Others | Added the Project Validation feature. This feature can display the validation rules required by the installed XR package. For any validation rules that are not properly set up, you can use this feature to automatically fix them with a single click. |

**Modify & Optimize**
| **Module** | **Description** |
| --- | --- |
| Sense Pack | Refactorred spatial anchor APIs, thereby providing a more easy-to-use spatial anchor system. For details, refer to "[Compatibility & porting guide for MR features](/document/unity/compatibility-and-porting-guide-for-mr-features/)". |
|  | Refactorred scene capture APIs, thereby providing a quicker workflow for you to retrieve scene anchor data. For details, refer to "[Compatibility & porting guide for MR features](/document/unity/compatibility-and-porting-guide-for-mr-features/)". |
|  | Once video seethrough is enabled, it works throughout the lifecycle of the app. |
| Interaction | Optimized body tracking APIs. For details, refer to "[Motion tracking API compatibility information](/document/unity/motion-tracker-api-compatibility/)". |

**Known issues**

In Unity 2023, when using overlay layers in the Multiview Rendering mode, the layers will not be displayed or be incorrectly displayed. This issue is expected to be resolved in future engine updates.
