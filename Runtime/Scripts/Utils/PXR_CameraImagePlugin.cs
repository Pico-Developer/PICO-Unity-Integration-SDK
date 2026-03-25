using System;
using System.Runtime.InteropServices;

namespace Unity.XR.PXR
{
    public static class PXR_CameraImagePlugin
    {
        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_ReleaseCameraImageData(int deviceId, ulong imageId);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_GetCameraImageDataPICO(int deviceId, ulong imageId,
            out XrCameraImageDataRawBuffer rawBufferData);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_AcquireCameraImage(int deviceId, ref XrCameraImageAcquireInfo acquireInfo,
            out XrCameraImage image);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_BeginCameraCapture(int deviceId);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_EndCameraCapture(int deviceId);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_GetCameraExtrinsic(int deviceId, out XrCameraExtrinsics intrinsics);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_GetCameraIntrinsics(int deviceId, out XrCameraIntrinsics intrinsics);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern PxrResult Pxr_DestroyCameraDevice(int deviceId);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern PxrResult Pxr_DestroyCameraCaptureSession(int deviceId);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_CreateCameraCaptureSession(int deviceId, int width, int height,
            XrCameraImageFpsPICO fps,
            XrCameraImageFormatPICO format,
            XrCameraDataTransferTypePICO transferType,
            XrCameraModelPICO model, out ulong future);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_CreateCameraCaptureSessionComplete(int deviceId, ulong future,
            ref XrCreateCameraCaptureSessionCompletion completion);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_CreateCameraDevice(int deviceId, out ulong future);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        private static extern PxrResult Pxr_CreateCameraDeviceComplete(int deviceId, ulong future,
            ref XrCreateCameraDeviceCompletion completion);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern PxrResult Pxr_GetAvailableCameras(ref uint count, ref IntPtr cameras);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern PxrResult Pxr_GetCameraCapabilityAvailable(int cameraId, ref uint count, ref IntPtr types);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern PxrResult Pxr_GetCameraCapability(int cameraId, XrCameraCapabilityTypePICO capabilityType,
            ref uint count, ref IntPtr types);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern PxrResult Pxr_GetCameraPropertyTypesAvailable(int cameraId, ref uint count,
            ref IntPtr types);

        [DllImport(PXR_Plugin.PXR_PLATFORM_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern PxrResult Pxr_GetCameraProperties(int cameraId, XrCameraPropertyTypePICO propertyType,
            ref int propertyValue);



        public static  PxrResult UPxr_GetCameraImageDataPICO(int deviceId, ulong imageId,
            ref XrCameraImageDataRawBuffer rawBufferData)
        {
            var pxrResult = Pxr_GetCameraImageDataPICO(deviceId, imageId, out rawBufferData);
            return pxrResult;
        }
        public static PxrResult UPxr_ReleaseCameraImageData(int deviceId, ulong imageId)
        {
            var pxrResult = Pxr_ReleaseCameraImageData(deviceId, imageId);
            return pxrResult;
        }
        
 
        public static PxrResult UPxr_AcquireCameraImage(int deviceId,XrCameraImageAcquireInfo acquireInfo,out  XrCameraImage image)
        {
            image = new XrCameraImage();
            var pxrResult = Pxr_AcquireCameraImage(deviceId,ref acquireInfo, out image);
            return pxrResult;
        }
        public static PxrResult UPxr_EndCameraCapture(int deviceId)
        {
            var pxrResult = Pxr_EndCameraCapture(deviceId);
            return pxrResult;
        }
        public static PxrResult UPxr_BeginCameraCapture(int deviceId)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var pxrResult = Pxr_BeginCameraCapture(deviceId);
            return pxrResult;
#else
            return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
        }
        
        public static PxrResult UPxr_GetCameraExtrinsic(int deviceId,out XrCameraExtrinsics extrinsic)
        {
             
#if UNITY_ANDROID && !UNITY_EDITOR
            var pxrResult = Pxr_GetCameraExtrinsic(deviceId,out extrinsic);
            return pxrResult;
#else
            extrinsic = new XrCameraExtrinsics();
            return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
        }
        public static PxrResult UPxr_GetCameraIntrinsics(int deviceId,out XrCameraIntrinsics intrinsics)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var pxrResult = Pxr_GetCameraIntrinsics(deviceId,out intrinsics);
            return pxrResult;
#else
            intrinsics = new XrCameraIntrinsics();
            return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
        }
        
        public static PxrResult UPxr_DestroyCameraDevice(int deviceId)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var pxrResult = Pxr_DestroyCameraDevice(deviceId);
            return pxrResult;
#else
            return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
        }
        public static PxrResult UPxr_DestroyCameraCaptureSession(int deviceId)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var pxrResult = Pxr_DestroyCameraCaptureSession(deviceId);
            return pxrResult;
#else
            return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
        }
        public static PxrResult UPxr_CreateCameraCaptureSession(int deviceId,int width, int height, XrCameraImageFpsPICO fps,
            XrCameraImageFormatPICO format,
            XrCameraDataTransferTypePICO transferType,
            XrCameraModelPICO model, out ulong future)
        {
            future = UInt64.MinValue;
#if UNITY_ANDROID && !UNITY_EDITOR
            var pxrResult = Pxr_CreateCameraCaptureSession(deviceId,width, height, fps, format,
                transferType,
                model, out future);
            return pxrResult;
#else
            return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
        }
        public static PxrResult UPxr_CreateCameraCaptureSessionComplete(int deviceId,ulong future,out XrCreateCameraCaptureSessionCompletion completion)
        {
            completion = new XrCreateCameraCaptureSessionCompletion();
           
#if UNITY_ANDROID && !UNITY_EDITOR
            var pxrResult = Pxr_CreateCameraCaptureSessionComplete(deviceId,future, ref completion);
            return pxrResult;
#else
            return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
        }
        public static PxrResult UPxr_CreateCameraDevice(int deviceId, out ulong future)
        {
            future = UInt64.MinValue;
#if UNITY_ANDROID && !UNITY_EDITOR
                var pxrResult = Pxr_CreateCameraDevice(deviceId, out future);
                return pxrResult;
#else
            return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
        }
        public static PxrResult UPxr_CreateCameraDeviceComplete(int deviceId,ulong future,out XrCreateCameraDeviceCompletion completion)
        {
            completion = new XrCreateCameraDeviceCompletion();
           
#if UNITY_ANDROID && !UNITY_EDITOR
                var pxrResult = Pxr_CreateCameraDeviceComplete(deviceId,future, ref completion);
                return pxrResult;
#else
            return PxrResult.ERROR_RUNTIME_FAILURE;
#endif
        }
        
        public static PxrResult UPxr_GetAvailableCameras( out XrCameraIdPICO[] capabilityValues)
        {
            capabilityValues = null;
            uint count = 0;
            IntPtr camerasHandle = IntPtr.Zero;
            PxrResult ret = Pxr_GetAvailableCameras(ref count, ref camerasHandle);
            if (ret == PxrResult.SUCCESS)
            {
                long[] values = new long[count];
                capabilityValues = new XrCameraIdPICO[count];
                Marshal.Copy(camerasHandle, values, 0, (int)count);
                for (int i = 0; i < count; i++)
                {
                    capabilityValues[i] = (XrCameraIdPICO)values[i];
                }
            }
            return ret;
        }

        public static PxrResult UPxr_GetCameraCapability(XrCameraIdPICO cameraId,XrCameraCapabilityTypePICO capabilityType,out int[] capabilityValues)
        {
            capabilityValues = null;
            uint count = 0;
            IntPtr capabilityHandle = IntPtr.Zero;
            PxrResult ret = Pxr_GetCameraCapability((int)cameraId, capabilityType, ref count, ref capabilityHandle);
            if (ret == PxrResult.SUCCESS)
            {
                if (capabilityType==XrCameraCapabilityTypePICO.XR_CAMERA_CAPABILITY_TYPE_IMAGE_RESOLUTION_PICO)
                {
                    capabilityValues = new int[count*2];
                    Marshal.Copy(capabilityHandle, capabilityValues, 0, (int)count*2);
                }
                else
                {
                    capabilityValues = new int[count];
                    Marshal.Copy(capabilityHandle, capabilityValues, 0, (int)count);
                }
            }
            return ret;
        }
        
        public static PxrResult UPxr_GetCameraProperties(XrCameraIdPICO cameraId,XrCameraPropertyTypePICO propertyType, ref int propertyValue)
        {
            return Pxr_GetCameraProperties((int)cameraId, propertyType, ref propertyValue);
        }

        public static PxrResult UPxr_GetCameraCapabilityAvailable(XrCameraIdPICO cameraId,
            out XrCameraCapabilityTypePICO[] propertyTypes)
        {
            propertyTypes = null;
            uint count = 0;
            IntPtr typesHandle = IntPtr.Zero;
            PxrResult ret = Pxr_GetCameraCapabilityAvailable((int)cameraId, ref count, ref typesHandle);
            if (ret == PxrResult.SUCCESS)
            {
                int[] typeArray = new int[count];
                Marshal.Copy(typesHandle, typeArray, 0, (int)count);
                propertyTypes = new XrCameraCapabilityTypePICO[count];
                for (int i = 0; i < count; i++)
                {
                    propertyTypes[i] = (XrCameraCapabilityTypePICO)typeArray[i];
                }

            }
            return ret;
        }

        public static PxrResult UPxr_GetCameraPropertyTypesAvailable(XrCameraIdPICO cameraId,out XrCameraPropertyTypePICO[] propertyTypes)
        {
            propertyTypes = null;
            uint count = 0;
            IntPtr typesHandle = IntPtr.Zero;
            PxrResult ret = Pxr_GetCameraPropertyTypesAvailable((int)cameraId, ref count, ref typesHandle);
            if (ret == PxrResult.SUCCESS)
            {
                int[] typeArray = new int[count];
                Marshal.Copy(typesHandle, typeArray, 0, (int)count);
                propertyTypes = new XrCameraPropertyTypePICO[count];
                for (int i = 0; i < count; i++)
                {
                    propertyTypes[i] = (XrCameraPropertyTypePICO)typeArray[i];
                }

            }
            return ret;
        }
        

        public static XrCameraPropertyTypePICO[] UPxr_GetCameraPropertyTypesAvailable(XrCameraIdPICO cameraId)
        {
            int[] typeArray = { 0 };
            uint configCount = 0;
            IntPtr configHandle = IntPtr.Zero;
            PxrResult ret = PxrResult.Unknown;
#if UNITY_ANDROID && !UNITY_EDITOR
            ret = Pxr_GetCameraPropertyTypesAvailable((int)cameraId, ref configCount, ref configHandle);
#endif
            if (ret == PxrResult.SUCCESS)
            {
                typeArray = new int[configCount];
                Marshal.Copy(configHandle, typeArray, 0, (int)configCount);
                XrCameraPropertyTypePICO[] retArray = new XrCameraPropertyTypePICO[configCount];
                for (int i = 0; i < configCount; i++)
                {
                    retArray[i] = (XrCameraPropertyTypePICO)typeArray[i];
                }

                return retArray;
            }

            return null;
        }
    }

        

    public enum XrCameraPropertyTypePICO
    {
        XR_CAMERA_PROPERTY_TYPE_FACING_PICO = 1, // The camera's facing orientation.
        XR_CAMERA_PROPERTY_TYPE_POSITION_PICO = 2, // The camera's position.
        XR_CAMERA_PROPERTY_TYPE_CAMERA_TYPE_PICO = 3 // The camera'stype.
    }
    
    public enum XrCameraFacingPICO {
        XR_CAMERA_FACING_WORLD_PICO = 1, // The camera is facing the world.
    }
    
    
    public enum XrCameraPositionPICO {
        XR_CAMERA_POSITION_UNSPECIFIED_PICO = 1, // The camera's position is unspecified.
        XR_CAMERA_POSITION_LEFT_PICO = 2, // Left-eye camera.
        XR_CAMERA_POSITION_RIGHT_PICO = 3, // Right-eye camera.
    } 

    public enum XrCameraTypePICO {
        XR_CAMERA_TYPE_PASSTHROUGH_COLOR_PICO = 1, // Color passthrough camera.
    } 
    public enum XrCameraIdPICO {
        XR_CAMERA_ID_RGB_LEFT_PICO = 1, // Left-eye RGB camera.
        XR_CAMERA_ID_RGB_RIGHT_PICO = 2, // Right-eye RGB camera.
    }

    public enum XrCameraCapabilityTypePICO
    {
        XR_CAMERA_CAPABILITY_TYPE_IMAGE_RESOLUTION_PICO = 1, // The supported image resolutions.
        XR_CAMERA_CAPABILITY_TYPE_IMAGE_FORMAT_PICO = 2, // The supported image formats.
        XR_CAMERA_CAPABILITY_TYPE_DATA_TRANSFER_TYPE_PICO = 3, // The supported data transfer typeS.
        XR_CAMERA_CAPABILITY_TYPE_CAMERA_MODEL_PICO = 4, // The supported camera models.
        XR_CAMERA_CAPABILITY_TYPE_IMAGE_FPS_PICO = 5, // The supported image FPS.
    }

    public enum XrCameraDataTransferTypePICO
    {
        XR_CAMERA_DATA_TRANSFER_TYPE_RAW_BUFFER_PICO = 1, // The data transfer type is raw cpu buffer.
    }

    public enum XrCameraImageFormatPICO
    {
        XR_CAMERA_IMAGE_FORMAT_RGBA_8888_PICO = 1, // The image format is RGBA8888.
    }

    public enum XrCameraModelPICO
    {
        XR_CAMERA_MODEL_PINHOLE_PICO = 1, // The camera model is pinhole.
    }
    public enum XrCameraImageFpsPICO {
        XR_CAMERA_IMAGE_FPS_30_PICO = 1, // The FPS is 30.
        XR_CAMERA_IMAGE_FPS_60_PICO = 2, // The FPS is 60.
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct XrCreateCameraDeviceCompletion
    {
        public XrStructureType type;
        public IntPtr next;
        public PxrResult futureResult;
        public ulong device;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XrCreateCameraCaptureSessionCompletion
    {
        public XrStructureType type;
        public IntPtr next;
        public PxrResult futureResult;
        public ulong captureSession;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XrCameraIntrinsics
    {
        public XrStructureType type;
        public IntPtr next;
        public XrVector2f focalLength; // The camera's focal length in pixels, represented as a 2D value (fx, fy).
        public XrVector2f principalPoint; // The camera's principal point in pixels, represented as a 2D value (cx, cy).
        public XrVector2f fov; // The camera's field of view (FOV) in degrees, represented as a 2D value (fx, fy).
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct XrCameraExtrinsics
    {
        public XrStructureType type;
        public IntPtr next;
        public XrPosef pose; // The camera's position and rotation relative to the XR device.
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XrCameraImageAcquireInfo
    {
        public XrStructureType type;
        public IntPtr next;
        public Int64 lastCaptureTime;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XrCameraImage
    {
        public XrStructureType type;
        public IntPtr next;
        public Int64 captureTime;
        public ulong imageId;
    } 
    [StructLayout(LayoutKind.Sequential)]
    public struct XrCameraImageDataRawBuffer {
        public XrStructureType type;
        public IntPtr next;
        public UInt32 width; // The image width in pixels.
        public UInt32 height; // The image height in pixels.
        public UInt32 stride; // The number of bytes between the start of consecutive rows.
        public UInt32 bytesPerPixel; // The number of bytes per pixel.
        public UInt32 pixelStride; // The number of bytes between consecutive pixels (for packed formats, this equals bytesPerPixel).
        public UInt32 bufferSize; // The total size of the image buffer in bytes.
        public IntPtr buffer; // A pointer to the raw image data.
    } 
}