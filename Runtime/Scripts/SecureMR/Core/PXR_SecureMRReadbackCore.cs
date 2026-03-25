#if !PICO_OPENXR_SDK
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.XR.PXR.SecureMR
{
    public class ReadbackTexture : IDisposable
    {
        public ulong Handle { get; private set; }
        bool released;
        public Texture2D Texture { get; private set; }

        internal ReadbackTexture(ulong handle, Texture2D texture)
        {
            Handle = handle;
            Texture = texture;
        }

        public PxrResult Release()
        {
            if (released) return PxrResult.SUCCESS;
            released = true;
            var r = PXR_SecureMRPlugin.UPxr_ReleaseReadbackTexture(Handle);
            if (Texture != null)
            {
                UnityEngine.Object.Destroy(Texture);
                Texture = null;
            }
            return r;
        }

        public void Dispose()
        {
            Release();
            GC.SuppressFinalize(this);
        }

        ~ReadbackTexture()
        {
            Release();
        }
    }
    
    public static class Readback
    {
        public static PxrResult RequestBuffer(Tensor globalTensor, out ulong future)
        {
            future = 0;
            if (!globalTensor.IsGlobalTensor)
            {
                return PxrResult.ERROR_RUNTIME_FAILURE;
            }
            return PXR_SecureMRPlugin.UPxr_CreateBufferFromGlobalTensorAsync(globalTensor.TensorHandle, out future);
        }

        public static PxrResult CompleteBuffer(Tensor globalTensor, ulong future, out byte[] buffer)
        {
            buffer = Array.Empty<byte>();
            if (!globalTensor.IsGlobalTensor)
            {
                return PxrResult.ERROR_RUNTIME_FAILURE;
            }
            return PXR_SecureMRPlugin.UPxr_CreateBufferFromGlobalTensorComplete(globalTensor.TensorHandle, future, out buffer);
        }

        public static PxrResult RequestTexture(Tensor globalTensor, out ulong future)
        {
            future = 0;
            if (!globalTensor.IsGlobalTensor)
            {
                return PxrResult.ERROR_RUNTIME_FAILURE;
            }
            return PXR_SecureMRPlugin.UPxr_CreateTextureFromGlobalTensorAsync(globalTensor.TensorHandle, out future);
        }

        public static PxrResult CompleteTexture(Tensor globalTensor, ulong future, out ulong handle)
        {
            if (!globalTensor.IsGlobalTensor)
            {
                handle = 0;
                return PxrResult.ERROR_RUNTIME_FAILURE;
            }

            var result =
                PXR_SecureMRPlugin.UPxr_CreateTextureFromGlobalTensorComplete(globalTensor.TensorHandle, future,
                    out handle);
            return result;
        }

        static T[] ConvertBytesTo<T>(byte[] bytes) where T : struct
        {
            if (bytes == null || bytes.Length == 0) return Array.Empty<T>();
            var size = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
            var count = bytes.Length / size;
            if (typeof(T) == typeof(byte)) return (T[])(object)bytes;
            var dest = new T[count];
            Buffer.BlockCopy(bytes, 0, dest, 0, count * size);
            return dest;
        }

        public static async Task<T[]> ReadbackBufferAsync<T>(Tensor globalTensor, int pollIntervalMs = 5)
            where T : struct
        {
            var req = RequestBuffer(globalTensor, out var future);
            if (req != PxrResult.SUCCESS) return Array.Empty<T>();
            PxrFutureState state;
            do
            {
                PXR_Plugin.MixedReality.UPxr_PollFuture(future, out state);
                if (state != PxrFutureState.Ready) await Task.Delay(pollIntervalMs);
            } while (state != PxrFutureState.Ready);

            CompleteBuffer(globalTensor, future, out var buffer);
            return ConvertBytesTo<T>(buffer);
        }

        public static async Task<ReadbackTexture> ReadbackTextureAsync(Tensor globalTensor, int pollIntervalMs = 5)
        {
            var req = RequestTexture(globalTensor, out var future);
            if (req != PxrResult.SUCCESS) return null;
            PxrFutureState state;
            do
            {
                PXR_Plugin.MixedReality.UPxr_PollFuture(future, out state);
                if (state != PxrFutureState.Ready) await Task.Delay(pollIntervalMs);
            } while (state != PxrFutureState.Ready);

            CompleteTexture(globalTensor, future, out var handle);
            if (handle == 0) return null;

            int width = globalTensor.Dimensions != null && globalTensor.Dimensions.Length >= 2
                ? globalTensor.Dimensions[0]
                : 0;
            int height = globalTensor.Dimensions != null && globalTensor.Dimensions.Length >= 2
                ? globalTensor.Dimensions[1]
                : 0;
            var device = SystemInfo.graphicsDeviceType;
            TextureFormat format = TextureFormat.RGBA32;
            if (globalTensor.DataType == SecureMRTensorDataType.DynamicTextureFloat)
            {
                format = TextureFormat.RGBAFloat;
            }
            else if (globalTensor.Channels == 1)
            {
                format = TextureFormat.R8;
            }

            Texture2D tex = null;
            if (device == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 ||
                device == UnityEngine.Rendering.GraphicsDeviceType.OpenGLCore)
            {
                PXR_SecureMRPlugin.UPxr_GetReadbackTextureImageOpenGL(handle, out var texId);
                tex = Texture2D.CreateExternalTexture(width, height, format, false, false, new IntPtr(texId));
            }
            else if (device == UnityEngine.Rendering.GraphicsDeviceType.Vulkan)
            {
                PXR_SecureMRPlugin.UPxr_GetReadbackTextureImageVulkan(handle, out var vkImage);
                //tex = Texture2D.CreateExternalTexture(width, height, format, false, false, vkImage);
            }

            return new ReadbackTexture(handle, tex);
        }
    }
}
#endif

