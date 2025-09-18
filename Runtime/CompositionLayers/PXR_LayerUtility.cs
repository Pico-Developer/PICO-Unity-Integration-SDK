#if XR_COMPOSITION_LAYERS
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.XR.CompositionLayers;
using Unity.XR.CompositionLayers.Extensions;
using Unity.XR.CompositionLayers.Services;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.XR.CompositionLayers.CompositionLayersRuntimeSettings;

namespace Unity.XR.PXR
{
    /// <summary>
    /// A general-purpose helper class for composition layer support.
    /// </summary>
    public static class PXR_LayerUtility
    {
        internal unsafe delegate void LayerCallbackDelegate(int layerId, XrCompositionLayerBaseHeader* layer);

        private static Material textureM;
        private static Material cubeM;
        static Dictionary<UInt32, Texture> _textureMap = new Dictionary<UInt32, Texture>();
        static Dictionary<ValueTuple<int, int>, Texture> _textureCache = new Dictionary<ValueTuple<int, int>, Texture>();

        /// <summary>
        /// Calls the methods in its invocation list when a swapchain is created on the graphics thread inside the UnityPXR_ lib.
        /// </summary>
        /// <param name="layerId">The instance id of the composition layer object.</param>
        /// <param name="swapchainHandle">The handle to the native swapchain that was just created.</param>
        public unsafe delegate void SwapchainCallbackDelegate(int layerId, ulong swapchainHandle);

        /// <summary>
        /// Calls the methods in its invocation list when a stereo swapchain is created on the graphics thread inside the UnityPXR_ lib.
        /// </summary>
        /// <param name="layerId">The instance id of the composition layer object.</param>
        /// <param name="swapchainHandleLeft">The handle to one of the stereo swapchains that was just created.</param>
        /// <param name="swapchainHandleRight">The handle to one of the stereo swapchains that was just created.</param>
        public unsafe delegate void StereoSwapchainCallbackDelegate(int layerId, ulong swapchainHandleLeft, ulong swapchainHandleRight);

        /// <summary>
        /// Helper method used to gather the extension components attached to a CompositionLayer GameObject.
        /// This method chains the native extension struct pointers of those extension components to initialize an PXR_ native object's Next pointer struct chain.
        /// </summary>
        /// <param name="layerInfo"> Container for the instance id and CompositionLayer component of the composition layer.</param>
        /// <param name="extensionTarget"> Represents what part of the composition layer to retrieve extensions for.</param>
        /// <returns>A pointer to the head of an array of native extension objects that will be associated with a composition layer.</returns>
        public static unsafe void* GetExtensionsChain(CompositionLayerManager.LayerInfo layerInfo, CompositionLayerExtension.ExtensionTarget extensionTarget)
        {
            void* extensionsChainHead = null;
            void* extensionsChain = null;

            foreach (var extension in layerInfo.Layer.Extensions)
            {
                // Skip extension if not enabled or not the intended target.
                if (!extension.enabled || extension.Target != extensionTarget)
                    continue;

                var extensionNativeStructPtr = extension.GetNativeStructPtr();

                // Skip extension if no native pointer is provided.
                if (extensionNativeStructPtr == null)
                    continue;

                // Initialize pointer chain if head has not been set.
                if (extensionsChainHead == null)
                {
                    extensionsChainHead = extensionNativeStructPtr;
                    extensionsChain = extensionsChainHead;
                }
                // Chain pointer if head has been initialized.
                else
                {
                    ((XrBaseInStructure*)extensionsChain)->Next = extensionNativeStructPtr;
                    extensionsChain = extensionNativeStructPtr;
                }
            }

            return extensionsChainHead;
        }

        /// <summary>
        /// Helper method used get the current app space for any native composition layer structs that may require an associated XrSpace.
        /// </summary>
        /// <returns>A handle to the current app space.</returns>
        /// <remarks>Normally used when creating native composition layers.</remarks>
        //public static ulong GetCurrentAppSpace() => Features.PXR_Feature.Internal_GetAppSpace(out ulong appSpaceId) ? appSpaceId : 0; // TODO
        public static ulong GetCurrentAppSpace() => 0;

        /// <summary>
        /// Helper method used get the XR session handle for any native composition layer structs that may require an associated XrSession.
        /// </summary>
        /// <returns>A handle to the current xr session.</returns>
        //public static ulong GetXRSession() => Features.PXR_Feature.Internal_GetXRSession(out ulong xrSessionHandle) ? xrSessionHandle : 0; // TODO
        public static ulong GetXRSession() => 0;

        /// <summary>
        /// Create the <see cref="XrSwapchainCreateInfo"/> struct that is passed to PXR_ SDK to create a swapchain.
        /// </summary>
        /// <param name="layerId">The instance id of the composition layer object.</param>
        /// <param name="createInfo">The struct used to create the swapchain.</param>
        /// <param name="isExternalSurface"> Optional parameter that can be used when an external surface will be used, like when using the Android Surface feature.</param>
        /// <param name="callback"> Optional parameter that can be used if your composition layer needs to know the handle after swapchain creation.</param>
        public static void CreateSwapchain(int layerId, XrSwapchainCreateInfo createInfo, bool isExternalSurface = false, SwapchainCallbackDelegate callback = null)
        {
            Pxr_CompositorLayersCreateSwapchain(layerId, createInfo, isExternalSurface, callback);
        }

        /// <summary>
        /// Create the <see cref="XrSwapchainCreateInfo"/> struct that is passed to PXR_ SDK to create a swapchain for stereo projection, like Projection layer type.
        /// </summary>
        /// <param name="layerId">The instance id of the composition layer object.</param>
        /// <param name="createInfo">The struct used to create the swapchain.</param>
        /// <param name="callback"> Optional parameter that can be used if your composition layer needs to know the handles after swapchain creation.</param>
        public static void CreateStereoSwapchain(int layerId, XrSwapchainCreateInfo createInfo, StereoSwapchainCallbackDelegate callback = null)
        {
            Pxr_CompositorLayersCreateStereoSwapchain(layerId, createInfo, callback);
        }

        /// <summary>
        /// Release swapchain according to the id provided.
        /// </summary>
        /// <param name="layerId">The instance id of the composition layer object.</param>
        public static void ReleaseSwapchain(int layerId)
        {
            Pxr_CompositorLayersReleaseSwapchain(layerId);
        }

        /// <summary>
        /// Return swapchain supported color format.
        /// </summary>
        /// <returns>The color format the swapchains will be using.</returns>
        public static Int64 GetDefaultColorFormat()
        {
            if (GraphicsDeviceType.Vulkan == SystemInfo.graphicsDeviceType)
            {
                return (long)PXR_CompositionLayer.ColorForamt.VK_FORMAT_R8G8B8A8_SRGB;
            }
            else
            {
                return (long)PXR_CompositionLayer.ColorForamt.GL_SRGB8_ALPHA8;
            }
        }

        /// <summary>
        /// Finds the render texture of the give texture id.
        /// </summary>
        /// <param name="texId">The id of the render texture to find.</param>
        /// <returns>The render texture with the provided id or null if no render textrue with that id was found.</returns>
        public static Texture FindRenderTexture(int id, UInt32 texId)
        {
            // texId will be 0 if swapchain has no images.
            if (texId == 0)
                return null;

            if (!_textureMap.TryGetValue(texId, out var renderTexture))
            {
                var objs = Resources.FindObjectsOfTypeAll<RenderTexture>();
                var name = $"XR Texture [{texId}]";
                // for (int i = 0; i < objs.Length; i++)
                // {
                //     Debug.Log($"FindRenderTexture 2 objs[i]={objs[i].name}");

                // }
                bool found = false;
                foreach (var rt in objs)
                {
                    if (rt.name == name)
                    {
                        renderTexture = rt;
                        _textureMap[texId] = rt;
                        found = true;
                        break;
                    }
                }
            }
            return renderTexture;
        }

        /// <summary>
        /// Finds the render texture of the layer id.
        /// </summary>
        /// <param name="layerInfo"> Container for the instance id and CompositionLayer component of the composition layer.</param>
        /// <returns>The render texture with the provided id or null if no render textrue with that id was found.</returns>
        public static Texture FindRenderTexture(CompositionLayerManager.LayerInfo layerInfo)
        {
            UInt32 texId = Pxr_CompositorLayersCreateOrGetRenderTextureId(layerInfo.Id);
            return FindRenderTexture(layerInfo.Id, texId);
        }

        public static Texture CreateExternalTexture(int id, int width, int height, bool isCube)
        {
            int imageIndex = 0;
            PXR_Plugin.Render.UPxr_GetLayerNextImageIndexByRender(id, ref imageIndex);

            var cacheKey = ValueTuple.Create(id, imageIndex);
            if (_textureCache.TryGetValue(cacheKey, out var cachedTexture))
            {
                return cachedTexture;
            }

            IntPtr ptr = IntPtr.Zero;
            PXR_Plugin.Render.UPxr_GetLayerImagePtr(id, (EyeType)0, imageIndex, ref ptr);
            if (IntPtr.Zero == ptr)
            {
                Debug.LogError($"WriteToRenderTexture id={id}, _textureMap, imageIndex={imageIndex}, IntPtr.Zero == ptr");
                return null;
            }

            Texture nativeTexture;

            if (isCube)
            {
                nativeTexture = Cubemap.CreateExternalTexture(width, TextureFormat.RGBA32, false, ptr);
            }
            else
            {
                nativeTexture = Texture2D.CreateExternalTexture(width, height, TextureFormat.RGBA32, false, true, ptr);
            }
            if (nativeTexture == null)
            {
                Debug.LogError($"WriteToRenderTexture id={id}, _textureMap, imageIndex={imageIndex}, nativeTexture == null");
            }
            if (nativeTexture != null)
            {
                nativeTexture.name = $"{id}+{imageIndex}";
                _textureCache[cacheKey] = nativeTexture;
            }
            Debug.Log($"WriteToRenderTexture id={id}, imageIndex={imageIndex}, cacheKey={cacheKey}, ptr={ptr}");
            return nativeTexture;
        }


        /// <summary>
        /// Handles transfering texture data to a render texture.
        /// </summary>
        /// <param name="texture">The source texture that will be written into the provided render texture.</param>
        /// <param name="renderTexture">The render texture that will be written to.</param>
        public static void WriteToRenderTexture(int id, Texture sourceTextures, Texture nativeTexture, bool isCube)
        {
            if (sourceTextures == null)
            {
                Debug.LogError($"WriteToRenderTexture sourceTextures == null!");
                return;
            }

            nativeTexture = CreateExternalTexture(id, sourceTextures.width, sourceTextures.height, isCube);
            if (nativeTexture == null)
            {
                Debug.LogError($"WriteToRenderTexture 11 id={id}  nativeTexture == null");
                return;
            }
            int eyeCount = 1;
            for (int i = 0; i < eyeCount; i++)
            {
                if (isCube && null == sourceTextures as Cubemap)
                {
                    Debug.LogError($"WriteToRenderTexture 11 id={id} isCube && null == sourceTextures as Cubemap");
                    return;
                }

                int faceCount = isCube ? 6:1;
                for (int f = 0; f < faceCount; f++)
                {
                    if (QualitySettings.activeColorSpace == ColorSpace.Gamma && sourceTextures != null)
                    {
                        Graphics.CopyTexture(sourceTextures, f, 0, nativeTexture, f, 0);
                    }
                    else
                    {
                        RenderTextureDescriptor rtDes = new RenderTextureDescriptor((int)sourceTextures.width, (int)sourceTextures.height, RenderTextureFormat.ARGB32, 0);
                        rtDes.msaaSamples = 1;
                        rtDes.useMipMap = true;
                        rtDes.autoGenerateMips = false;
                        rtDes.sRGB = true;

                        RenderTexture renderTexture = RenderTexture.GetTemporary(rtDes);

                        if (!renderTexture.IsCreated())
                        {
                            renderTexture.Create();
                        }
                        renderTexture.DiscardContents();

                        if (isCube)
                        {
                            if (cubeM == null)
                            {
                                Debug.Log($"WriteToRenderTexture id={id}, cubeM , f={f}, cubeM == null");
                                cubeM = new Material(Shader.Find("PXR_SDK/PXR_CubemapBlit"));
                            }
                            cubeM.SetInt("_d", f);
                            Graphics.Blit(sourceTextures, renderTexture, cubeM);
                        }
                        else
                        {
                            if (textureM == null)
                            {
                                Debug.Log($"WriteToRenderTexture id={id}, textureM, textureM == null");
                                textureM = new Material(Shader.Find("PXR_SDK/PXR_Texture2DBlit"));
                            }
                            textureM.mainTexture = renderTexture;
                            textureM.SetPass(0);
                            //textureM.SetInt("_premultiply", isPremultipliedAlpha ? 1 : 0);
                            Graphics.Blit(sourceTextures, renderTexture);
                        }
                        Graphics.CopyTexture(renderTexture, 0, 0, nativeTexture, f, 0);
                        RenderTexture.ReleaseTemporary(renderTexture);
                    }
                }
            }
        }

        /// <summary>
        /// Query the correct XR Textures for rendering and blit the layer textures.
        /// </summary>
        /// <param name="layerInfo"> Container for the instance id and CompositionLayer component of the composition layer.</param>
        /// <param name="texture">The source texture that will be written into the provided render texture.</param>
        /// <param name="renderTexture">The render texture that will be searched for and written to.
        /// Will be null if no render texture can be found for the provided layerInfo object.</param>
        /// <returns>True if a render texture was found and written to, false if the provided texture is null or if no render texture was found for the provided layerInfo object.</returns>
        public static bool FindAndWriteToRenderTexture(CompositionLayerManager.LayerInfo layerInfo, Texture texture, out Texture renderTexture, bool isCube)
        {
            if (texture == null)
            {
                Debug.Log($"FindAndWriteToRenderTexture texture == null");
                renderTexture = null;
                return false;
            }

            renderTexture = FindRenderTexture(layerInfo);
            WriteToRenderTexture(layerInfo.Id, texture, renderTexture, isCube);
            return renderTexture != null;
        }

        /// <summary>
        /// Add native layer structs to the <c>endFrameInfo</c> struct inside the UnityPXR_ lib - for custom layer type support
        /// </summary>
        /// <param name="layers">Pointer to the native array of currently active composition layers.</param>
        /// <param name="orders">Pointer to the native array of order values for the currently active composition layers.</param>
        /// <param name="count">Indicates the size of the layers and orders arrays.</param>
        /// <param name="layerByteSize">Indicates the size in bytes of a single element of the given array of composition layers.</param>
        /// <remarks>Layers sent must all be of the same type.Demonstrated in the PXR_CustomLayerHandler class.</remarks>
        public static unsafe void AddActiveLayersToEndFrame(void* layers, void* orders, int count, int layerByteSize)
        {
            IntPtr ptrLayers = new IntPtr(layers);
            IntPtr ptrOrders = new IntPtr(orders);
            Pxr_CompositorLayersAddActiveLayers(layers, orders, count, layerByteSize);
        }

        /// <summary>
        /// Return the Surface object for Android External Surface support (Android only).
        /// </summary>
        /// <param name="layerId">The instance id of the composition layer object.</param>
        /// <returns>Pointer to the android surface object.</returns>
        public static System.IntPtr GetLayerAndroidSurfaceObject(int layerId)
        {
            IntPtr surfaceObject = IntPtr.Zero;
            if (Pxr_CompositorLayersGetLayerAndroidSurfaceObject(layerId, ref surfaceObject))
            {
                return surfaceObject;
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Sends an array of extensions to be attached to the native default compostion layer.
        /// </summary>
        /// <param name="extensions">Pointer to the array of extensions to attach to the default compostion layer.</param>
        /// <remarks>Currently only called by the PXR_DefautLayer class.</remarks>
        public static unsafe void SetDefaultSceneLayerExtensions(void* extensions)
        {
            // IntPtr ptr = new IntPtr(extensions);
            // ext_composition_layers_SetDefaultSceneLayerExtensions(extensions);
        }

        /// <summary>
        /// Sends what flags are to be added to the native default compostion layer.
        /// </summary>
        /// <param name="flags">Flags to be added to the native default compostion layer.</param>
        /// <remarks>Currently only called by the PXR_DefautLayer class.</remarks>
        public static unsafe void SetDefaultLayerFlags(XrCompositionLayerFlags flags)
        {
            // ext_composition_layers_SetDefaultSceneLayerFlags(flags);
        }

        const string LibraryName = "PxrPlatform";

        [DllImport(LibraryName)]
        internal static extern UInt32 Pxr_CompositorLayersCreateOrGetRenderTextureId(int id); // Down

        [DllImport(LibraryName)]
        [return: MarshalAs(UnmanagedType.U1)]
        internal static extern bool Pxr_CompositorLayersCreateOrGetStereoRenderTextureIds(int id, out UInt32 leftId, out UInt32 rightId); // Down

        [DllImport(LibraryName)]
        internal static extern void Pxr_CompositorLayersCreateSwapchain(int id, XrSwapchainCreateInfo createInfo, [MarshalAs(UnmanagedType.I1)]bool isExternalSurface = false, SwapchainCallbackDelegate callback = null); // Down

        [DllImport(LibraryName)]
        internal static extern void Pxr_CompositorLayersCreateStereoSwapchain(int id, XrSwapchainCreateInfo createInfo, StereoSwapchainCallbackDelegate callback = null); // Down

        [DllImport(LibraryName)]
        internal static extern void Pxr_CompositorLayersReleaseSwapchain(int id); // Down

        [DllImport(LibraryName)]
        internal static extern unsafe void Pxr_CompositorLayersAddActiveLayers(void* layers, void* orders, int count, int size); // Down

        [DllImport(LibraryName)]
        [return: MarshalAs(UnmanagedType.U1)]
        internal static extern bool Pxr_CompositorLayersGetLayerAndroidSurfaceObject(int layerId, ref IntPtr surfaceObject); // Down

        [DllImport(LibraryName)]
        internal static extern unsafe void ext_composition_layers_SetDefaultSceneLayerExtensions(void* extensions);

        [DllImport(LibraryName)]
        internal static extern void ext_composition_layers_SetDefaultSceneLayerFlags(XrCompositionLayerFlags flags);
    }
}

#endif
