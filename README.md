This article records the changes in version 3.4.0 of the PICO Unity Integration SDK.
## What's new
Release date: February 27, 2026
**Add**
| **Module** | **Description** |
| --- | --- |
| Sense Pack | * Added plane detection functionality. With plane detection, MR applications can identify and track horizontal, vertical, or inclined surfaces (such as floors, desktops, walls, and sloped roofs). For details, refer to "[Plane detection](https://developer.picoxr.com/document/unity/plane-detection)". <br> * The following features are supported on PICO 4 series devices: Spatial Anchor, Shared Spatial Anchor, Spatial Mesh, Scene Capture, and MR Safeguard. <br> * The following features have been added to SecureMR: <br>    * Added JavaScript Operator for executing JavaScript scripts submitted by developers. For details, refer to "[Use different operators](https://developer.picoxr.com/document/unity/use-different-operators/)". <br>    * Added two new `TensorDataType`: `DynamicTextureByte = 8` and `DynamicTextureFloat = 9`, which allow `GlobalTensor` to be declared as dynamic texture, and then loaded as GPU texture in the pipeline with zero-copy for use by glTF Tensor. For details, refer to "[Use the dynamic texture](https://developer.picoxr.com/document/unity/use-dynamic-texture)". <br>    * Added Readback Tensor to grant permission to read camera-related data. For details, refer to "[Use the Readback tensor](https://developer.picoxr.com/document/unity/use-the-readback-tensor)" |
| Rendering | Supported using the APIs provided by the `PXR_CameraImage` class on the device to manage camera image data. For details, refer to "[Camera image data (user device)](https://developer.picoxr.com/document/unity/camera-image-data-management)". |

