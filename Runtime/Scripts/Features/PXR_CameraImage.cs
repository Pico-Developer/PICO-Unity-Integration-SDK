using System;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.XR.PXR
{
    public static class PXR_CameraImage
    {
        /// <summary>
        /// Gets available camera device identifiers supported by the runtime.
        /// </summary>
        /// <param name="cameraIds">Output. Array of camera IDs detected and available for use.</param>
        /// <returns>Operation result.</returns>
        public static PxrResult GetAvailableCameras(out XrCameraIdPICO[] cameraIds)
        {
            return PXR_CameraImagePlugin.UPxr_GetAvailableCameras(out cameraIds);
        }

        /// <summary>
        /// Gets which property types are supported for a specific camera.
        /// </summary>
        /// <param name="cameraId">Input. Target camera ID.</param>
        /// <param name="propertyTypes">Output. Supported property types.</param>
        /// <returns>Operation result.</returns>
        public static PxrResult GetCameraPropertyTypesAvailable(XrCameraIdPICO cameraId,out XrCameraPropertyTypePICO[] propertyTypes)
        {
            return PXR_CameraImagePlugin.UPxr_GetCameraPropertyTypesAvailable(cameraId,out propertyTypes);
        }

        /// <summary>
        /// Gets the camera facing configuration for the given camera.
        /// </summary>
        /// <param name="cameraId">Input. Target camera ID.</param>
        /// <param name="propertyValue">Output. Camera facing value.</param>
        /// <returns>Operation result.</returns>
        public static PxrResult GetCameraFacingProperties(XrCameraIdPICO cameraId, out XrCameraFacingPICO propertyValue)
        {
            propertyValue = XrCameraFacingPICO.XR_CAMERA_FACING_WORLD_PICO;
            int propertyValueInt = -1;
            PxrResult result = PXR_CameraImagePlugin.UPxr_GetCameraProperties(cameraId,
                XrCameraPropertyTypePICO.XR_CAMERA_PROPERTY_TYPE_FACING_PICO, ref propertyValueInt);
            if (result == PxrResult.SUCCESS)
            {
                propertyValue = (XrCameraFacingPICO)propertyValueInt;
            }

            return result;
        }

        /// <summary>
        /// Gets the camera position property for the given camera.
        /// </summary>
        /// <param name="cameraId">Input. Target camera ID.</param>
        /// <param name="propertyValue">Output. Camera position value.</param>
        /// <returns>Operation result.</returns>
        public static PxrResult GetCameraPositionProperties(XrCameraIdPICO cameraId,
            out XrCameraPositionPICO propertyValue)
        {
            propertyValue = XrCameraPositionPICO.XR_CAMERA_POSITION_UNSPECIFIED_PICO;
            int propertyValueInt = -1;
            PxrResult result = PXR_CameraImagePlugin.UPxr_GetCameraProperties(cameraId,
                XrCameraPropertyTypePICO.XR_CAMERA_PROPERTY_TYPE_POSITION_PICO, ref propertyValueInt);
            if (result == PxrResult.SUCCESS)
            {
                propertyValue = (XrCameraPositionPICO)propertyValueInt;
            }

            return result;
        }

        /// <summary>
        /// Gets the camera type property for the given camera.
        /// </summary>
        /// <param name="cameraId">Input. Target camera ID.</param>
        /// <param name="propertyValue">Output. Camera type value.</param>
        /// <returns>Operation result.</returns>
        public static PxrResult GetCameraCameraTypeProperties(XrCameraIdPICO cameraId,
            out XrCameraTypePICO propertyValue)
        {
            propertyValue = XrCameraTypePICO.XR_CAMERA_TYPE_PASSTHROUGH_COLOR_PICO;
            int propertyValueInt = -1;
            PxrResult result = PXR_CameraImagePlugin.UPxr_GetCameraProperties(cameraId,
                XrCameraPropertyTypePICO.XR_CAMERA_PROPERTY_TYPE_CAMERA_TYPE_PICO, ref propertyValueInt);
            if (result == PxrResult.SUCCESS)
            {
                propertyValue = (XrCameraTypePICO)propertyValueInt;
            }

            return result;
        }

        /// <summary>
        /// Gets capability types supported by the given camera.
        /// </summary>
        /// <param name="cameraId">Input. Target camera ID.</param>
        /// <param name="capabilitys">Output. Supported capability types.</param>
        /// <returns>Operation result.</returns>
        public static PxrResult GetCameraCapabilityAvailable(XrCameraIdPICO cameraId,out XrCameraCapabilityTypePICO[] capabilitys)
        {
            return PXR_CameraImagePlugin.UPxr_GetCameraCapabilityAvailable(cameraId,out capabilitys);
        }

        /// <summary>
        /// Gets supported image frame rates for the given camera.
        /// </summary>
        /// <param name="cameraId">Input. Target camera ID.</param>
        /// <param name="imageFps">Output. Supported image frame rates.</param>
        /// <returns>Operation result.</returns>
        public static PxrResult GetCameraImageFpsCapability(XrCameraIdPICO cameraId,
            out XrCameraImageFpsPICO[] imageFps)
        {
            imageFps = null;
            int[] values = null;
            PxrResult ret = PXR_CameraImagePlugin.UPxr_GetCameraCapability(cameraId,
                XrCameraCapabilityTypePICO.XR_CAMERA_CAPABILITY_TYPE_IMAGE_FPS_PICO, out values);
            if (ret != PxrResult.SUCCESS)
            {
                return ret;
            }

            if (values == null || values.Length == 0)
            {
                return PxrResult.ERROR_RUNTIME_FAILURE;
            }

            imageFps = new XrCameraImageFpsPICO[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                imageFps[i] = (XrCameraImageFpsPICO)values[i];
            }

            return ret;
        }

        /// <summary>
        /// Gets supported camera models for the given camera.
        /// </summary>
        /// <param name="cameraId">Input. Target camera ID.</param>
        /// <param name="cameraModels">Output. Supported camera models.</param>
        /// <returns>Operation result.</returns>
        public static PxrResult GetCameraCameraModelCapability(XrCameraIdPICO cameraId,
            out XrCameraModelPICO[] cameraModels)
        {
            cameraModels = null;
            int[] values = null;
            PxrResult ret = PXR_CameraImagePlugin.UPxr_GetCameraCapability(cameraId,
                XrCameraCapabilityTypePICO.XR_CAMERA_CAPABILITY_TYPE_CAMERA_MODEL_PICO, out values);
            if (ret != PxrResult.SUCCESS)
            {
                return ret;
            }

            if (values == null || values.Length == 0)
            {
                return PxrResult.ERROR_RUNTIME_FAILURE;
            }

            cameraModels = new XrCameraModelPICO[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                cameraModels[i] = (XrCameraModelPICO)values[i];
            }

            return ret;
        }

        /// <summary>
        /// Gets supported data transfer types for the given camera.
        /// </summary>
        /// <param name="cameraId">Input. Target camera ID.</param>
        /// <param name="dataTransferTypes">Output. Supported data transfer types.</param>
        /// <returns>Operation result.</returns>
        public static PxrResult GetCameraDataTransferTypeCapability(XrCameraIdPICO cameraId,
            out XrCameraDataTransferTypePICO[] dataTransferTypes)
        {
            dataTransferTypes = null;
            int[] values = null;
            PxrResult ret = PXR_CameraImagePlugin.UPxr_GetCameraCapability(cameraId,
                XrCameraCapabilityTypePICO.XR_CAMERA_CAPABILITY_TYPE_DATA_TRANSFER_TYPE_PICO, out values);
            if (ret != PxrResult.SUCCESS)
            {
                return ret;
            }

            if (values == null || values.Length == 0)
            {
                return PxrResult.ERROR_RUNTIME_FAILURE;
            }

            dataTransferTypes = new XrCameraDataTransferTypePICO[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                dataTransferTypes[i] = (XrCameraDataTransferTypePICO)values[i];
            }

            return ret;
        }

        /// <summary>
        /// Gets supported image formats for the given camera.
        /// </summary>
        /// <param name="cameraId">Input. Target camera ID.</param>
        /// <param name="formats">Output. Supported image formats.</param>
        /// <returns>Operation result.</returns>
        public static PxrResult GetCameraImageFormatCapability(XrCameraIdPICO cameraId,
            out XrCameraImageFormatPICO[] formats)
        {
            formats = null;
            int[] values = null;
            PxrResult ret = PXR_CameraImagePlugin.UPxr_GetCameraCapability(cameraId,
                XrCameraCapabilityTypePICO.XR_CAMERA_CAPABILITY_TYPE_IMAGE_FORMAT_PICO, out values);
            if (ret != PxrResult.SUCCESS)
            {
                return ret;
            }

            if (values == null || values.Length == 0)
            {
                return PxrResult.ERROR_RUNTIME_FAILURE;
            }

            formats = new XrCameraImageFormatPICO[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                formats[i] = (XrCameraImageFormatPICO)values[i];
            }

            return ret;
        }

        /// <summary>
        /// Gets supported image resolutions for the given camera.
        /// </summary>
        /// <param name="cameraId">Input. Target camera ID.</param>
        /// <param name="resolutions">Output. Supported image resolutions (width, height in pixels).</param>
        /// <returns>Operation result.</returns>
        public static PxrResult GetCameraImageResolutionCapability(XrCameraIdPICO cameraId,
            out PxrExtent2Di[] resolutions)
        {
            resolutions = null;
            int[] values = null;
            PxrResult ret = PXR_CameraImagePlugin.UPxr_GetCameraCapability(cameraId,
                XrCameraCapabilityTypePICO.XR_CAMERA_CAPABILITY_TYPE_IMAGE_RESOLUTION_PICO, out values);
            if (ret != PxrResult.SUCCESS)
            {
                return ret;
            }

            if (values == null || values.Length == 0)
            {
                return PxrResult.ERROR_RUNTIME_FAILURE;
            }

            resolutions = new PxrExtent2Di[values.Length / 2];
            for (int i = 0; i < values.Length; i += 2)
            {
                resolutions[i / 2] = new PxrExtent2Di();
                resolutions[i / 2].width = values[i];
                resolutions[i / 2].height = values[i + 1];
            }

            return ret;
        }

        /// <summary>
        /// Asynchronously creates a camera device handle for the specified camera.
        /// </summary>
        /// <param name="cameraId">Input. Target camera ID.</param>
        /// <param name="token">Input. Cancellation token to abort the operation.</param>
        /// <returns>Task that completes with the operation result.</returns>
        public static async Task<PxrResult> CreateCameraDeviceAsync(XrCameraIdPICO cameraId,
            CancellationToken token = default)
        {
            return await Task.Run(async () =>
            {
                var startResult = PXR_CameraImagePlugin.UPxr_CreateCameraDevice((int)cameraId, out var future);

                if (startResult == PxrResult.SUCCESS)
                {
                    while (true)
                    {
                        var pollResult = PXR_Plugin.MixedReality.UPxr_PollFuture(future, out var futureState);
                        if (pollResult == PxrResult.SUCCESS)
                        {
                            if (futureState == PxrFutureState.Ready)
                            {
                                var completeResult =
                                    PXR_CameraImagePlugin.UPxr_CreateCameraDeviceComplete((int)cameraId, future,
                                        out var completion);
                                if (completeResult == PxrResult.SUCCESS)
                                {
                                    return completion.futureResult;
                                }
                                else
                                {
                                    return completeResult;
                                }
                            }
                        }
                        else
                        {
                            return pollResult;
                        }

                        await Task.Delay(11, token);
                    }
                }
                else
                {
                    return startResult;
                }
            }, token);
        }

        /// <summary>
        /// Asynchronously creates a camera capture session configured with the specified parameters.
        /// </summary>
        /// <param name="cameraId">Input. Target camera ID.</param>
        /// <param name="width">Input. Image width in pixels.</param>
        /// <param name="height">Input. Image height in pixels.</param>
        /// <param name="fps">Input. Target image frame rate.</param>
        /// <param name="format">Input. Image pixel format.</param>
        /// <param name="transferType">Input. Data transfer type.</param>
        /// <param name="model">Input. Camera model.</param>
        /// <param name="token">Input. Cancellation token to abort the operation.</param>
        /// <returns>Task that completes with the operation result.</returns>
        public static async Task<PxrResult> CreateCameraCaptureSessionAsync(XrCameraIdPICO cameraId, int width, int height, XrCameraImageFpsPICO fps,
            XrCameraImageFormatPICO format,
            XrCameraDataTransferTypePICO transferType,
            XrCameraModelPICO model,
            CancellationToken token = default)
        {
            return await Task.Run(async () =>
            {
                var startResult = PXR_CameraImagePlugin.UPxr_CreateCameraCaptureSession((int)cameraId, width, height, fps, format, transferType, model, out var future);

                if (startResult == PxrResult.SUCCESS)
                {
                    while (true)
                    {
                        var pollResult = PXR_Plugin.MixedReality.UPxr_PollFuture(future, out var futureState);
                        if (pollResult == PxrResult.SUCCESS)
                        {
                            if (futureState == PxrFutureState.Ready)
                            {
                                var completeResult =
                                    PXR_CameraImagePlugin.UPxr_CreateCameraCaptureSessionComplete((int)cameraId, future,
                                        out var completion);
                                if (completeResult == PxrResult.SUCCESS)
                                {
                                    return completion.futureResult;
                                }
                                else
                                {
                                    return completeResult;
                                }
                            }
                        }
                        else
                        {
                            return pollResult;
                        }

                        await Task.Delay(11, token);
                    }
                }
                else
                {
                    return startResult;
                }
            }, token);
        }

        /// <summary>
        /// Destroys the camera device associated with the specified camera.
        /// </summary>
        /// <param name="cameraId">Input. Target camera ID.</param>
        /// <returns>Operation result.</returns>
        public static PxrResult DestroyCameraDevice(XrCameraIdPICO cameraId)
        {
            return PXR_CameraImagePlugin.UPxr_DestroyCameraDevice((int)cameraId);
        }
        /// <summary>
        /// Destroys the active camera capture session for the specified camera.
        /// </summary>
        /// <param name="cameraId">Input. Target camera ID.</param>
        /// <returns>Operation result.</returns>
        public static PxrResult DestroyCameraCaptureSession(XrCameraIdPICO cameraId)
        {
            return PXR_CameraImagePlugin.UPxr_DestroyCameraCaptureSession((int)cameraId);
        }

        /// <summary>
        /// Gets camera intrinsics (focal length, principal point, and distortion parameters).
        /// </summary>
        /// <param name="cameraId">Input. Target camera ID.</param>
        /// <param name="intrinsics">Output. Camera intrinsics.</param>
        /// <returns>Operation result.</returns>
        public static PxrResult GetCameraIntrinsics(XrCameraIdPICO cameraId, out XrCameraIntrinsics intrinsics)
        {
            return PXR_CameraImagePlugin.UPxr_GetCameraIntrinsics((int)cameraId, out intrinsics);
        }

        /// <summary>
        /// Gets camera extrinsics (pose of the camera in the tracking space).
        /// </summary>
        /// <param name="cameraId">Input. Target camera ID.</param>
        /// <param name="extrinsics">Output. Camera extrinsics.</param>
        /// <returns>Operation result.</returns>
        public static PxrResult GetCameraExtrinsics(XrCameraIdPICO cameraId, out XrCameraExtrinsics extrinsics)
        {
            PxrResult ret=PXR_CameraImagePlugin.UPxr_GetCameraExtrinsic((int)cameraId, out extrinsics);
            if(ret==PxrResult.SUCCESS)
            {
                extrinsics.pose.Position.Z=-extrinsics.pose.Position.Z;
                extrinsics.pose.Orientation.Z=-extrinsics.pose.Orientation.Z;
                extrinsics.pose.Orientation.W=-extrinsics.pose.Orientation.W;
            }
            return ret;
        }

        /// <summary>
        /// Begins capturing images for the specified camera.
        /// </summary>
        /// <param name="cameraId">Input. Target camera ID.</param>
        /// <returns>Operation result.</returns>
        public static PxrResult BeginCameraCapture(XrCameraIdPICO cameraId)
        {
            return PXR_CameraImagePlugin.UPxr_BeginCameraCapture((int)cameraId);
        }

        /// <summary>
        /// Ends image capture for the specified camera.
        /// </summary>
        /// <param name="cameraId">Input. Target camera ID.</param>
        /// <returns>Operation result.</returns>
        public static PxrResult EndCameraCapture(XrCameraIdPICO cameraId)
        {
            return PXR_CameraImagePlugin.UPxr_EndCameraCapture((int)cameraId);
        }

   
        /// <summary>
        /// Acquires a camera image.The runtime must return the newest image captured after lastCaptureTime. If no new image is available,
        /// the runtime must return XR_CAMERA_IMAGE_NO_UPDATE_PICO when xrAcquireCameraImagePICO
        /// </summary>
        /// <param name="deviceId">Input. Target camera ID.</param>
        /// <param name="lastCaptureTime">Input. Timestamp of the last acquired image (use 0 to acquire the earliest available image).</param>
        /// <param name="imageId">Output. Identifier of the acquired image.</param>
        /// <param name="captureTime">Output. Timestamp of the acquired image.</param>
        /// <returns>Operation result.</returns>
        public static PxrResult AcquireCameraImage(XrCameraIdPICO deviceId, Int64 lastCaptureTime, out ulong imageId, out Int64 captureTime)
        {
            XrCameraImageAcquireInfo acquireInfo = new XrCameraImageAcquireInfo();
            acquireInfo.lastCaptureTime = lastCaptureTime;
            imageId=0;
            captureTime=0;
            PxrResult ret=PXR_CameraImagePlugin.UPxr_AcquireCameraImage((int)deviceId, acquireInfo, out var image);
            if(ret==PxrResult.SUCCESS)
            {
                imageId=image.imageId;
                captureTime=image.captureTime;
            }
            return ret;
        }

        /// <summary>
        /// Releases resources associated with a previously acquired camera image.
        /// </summary>
        /// <param name="deviceId">Input. Target camera ID.</param>
        /// <param name="imageId">Input. Identifier of the acquired image to release.</param>
        /// <returns>Operation result.</returns>
        public static PxrResult ReleaseCameraImage(XrCameraIdPICO deviceId, ulong imageId)
        {
            return PXR_CameraImagePlugin.UPxr_ReleaseCameraImageData((int)deviceId, imageId);
        }
        
        /// <summary>
        /// Gets raw buffer data for a previously acquired camera image.
        /// </summary>
        /// <param name="deviceId">Input. Target camera ID.</param>
        /// <param name="imageId">Input. Identifier of the acquired image.</param>
        /// <param name="rawBufferData">Output. Raw image buffer data and metadata.</param>
        /// <returns>Operation result.</returns>
        public static PxrResult GetCameraImageData(XrCameraIdPICO deviceId, ulong imageId, out XrCameraImageDataRawBuffer rawBufferData)
        {
            rawBufferData=new XrCameraImageDataRawBuffer();
            return PXR_CameraImagePlugin.UPxr_GetCameraImageDataPICO((int)deviceId, imageId, ref rawBufferData);
        }
    }
}