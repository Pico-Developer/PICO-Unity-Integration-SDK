#if XR_COMPOSITION_LAYERS
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.XR.CompositionLayers.Extensions;
using Unity.XR.CompositionLayers.Services;
using UnityEngine;
#if UNITY_VIDEO
using UnityEngine.Video;
#endif


namespace Unity.XR.PXR
{
    /// <summary>
    /// Provides a base implementation for the <see cref="PXR_LayerProvider.ILayerHandler"/> interface.
    /// You can implement the required methods of this abstract class to create a concrete layer handler.
    /// </summary>
    /// <remarks>
    /// The <see cref="PXR_LayerProvider.ILayerHandler"/> methods that this class implements handle adding
    /// and removing composition layers from native arrays, swap chain creation dispatching and other tasks
    /// required by the Unity side of the API.
    ///
    /// The abstract methods that you must implement handle the custom aspects of your layer. These methods include:
    ///
    /// * <see cref="CreateSwapchain(CompositionLayerManager.LayerInfo, out SwapchainCreateInfo)"/>
    /// * <see cref="CreateNativeLayer(CompositionLayerManager.LayerInfo, PXR_CustomLayerHandler{T}.SwapchainCreatedOutput, out T)"/>
    /// * <see cref="ModifyNativeLayer(CompositionLayerManager.LayerInfo, ref T)"/>
    ///
    /// You are not required to implement a custom layer handler based on this abstract class, but doing so should be
    /// easier than implementing the <see cref="PXR_LayerProvider.ILayerHandler"/> interface in its entirety.
    ///
    /// You must register your concrete layer handler object with
    /// <see cref="PXR_LayerProvider.RegisterLayerHandler(Type, PXR_LayerProvider.ILayerHandler)"/>.
    /// </remarks>
    /// <typeparam name="T">The native PXR_ structure of the composition layer to handle.</typeparam>
    public abstract class PXR_CustomLayerHandler<T> : PXR_LayerProvider.ILayerHandler, IDisposable where T : struct
    {
        /// <summary>
        /// Container for swapchain related information that may be needed during the creation of the native PXR_ composition layer struct.
        /// </summary>
        protected struct SwapchainCreateInfo
        {
            /// <summary>
            /// Native structure for the swapchain creation info.
            /// </summary>
            public XrSwapchainCreateInfo nativeStruct;

            /// <summary>
            /// Tells if swapchain is using an external surface.
            /// </summary>
            [MarshalAs(UnmanagedType.I1)]
            public bool isExternalSurface;

            /// <summary>
            /// Tells if swapchain should be stereo.
            /// </summary>
            public bool isStereo;

            /// <summary>
            /// Initializes and returns an instance of SwapchainCreateInfo with the provided parameters.
            /// </summary>
            /// <param name="xrSwapchainCreateInfo">Native structure for the swapchain creation info.</param>
            /// <param name="isExternalSurface">Tells if swapchain is using an external surface.</param>
            /// <param name="isStereo">Tells if swapchain should be stereo.</param>
            public SwapchainCreateInfo(XrSwapchainCreateInfo xrSwapchainCreateInfo, bool isExternalSurface = false, bool isStereo = false)
            {
                this.nativeStruct = xrSwapchainCreateInfo;
                this.isExternalSurface = isExternalSurface;
                this.isStereo = isStereo;
            }

            /// <summary>
            /// Implicit conversion with just a native XrSwapchainCreateInfo struct.
            /// </summary>
            /// <param name="createInfo">The native struct to convert.</param>
            public static implicit operator SwapchainCreateInfo(XrSwapchainCreateInfo createInfo) => new SwapchainCreateInfo(createInfo);
        }

        /// <summary>
        /// Container for swapchain related information that may be needed during the creation of the native PXR_ composition layer struct.
        /// </summary>
        protected struct SwapchainCreatedOutput
        {
            /// <summary>
            /// The handle of the created swapchain.
            /// Can be used to initialize the swapchain member of a native PXR_ composition layer struct.
            /// </summary>
            public ulong handle;

            /// <summary>
            /// The second handle of the created stereo swapchain.
            /// Can be used to initialize the swapchain member of a native PXR_ composition layer struct.
            /// </summary>
            public ulong secondStereoHandle;
        }

        /// <summary>
        /// Container for grouping render information for each compostion layer.
        /// </summary>
        class LayerRenderInfo
        {
            public Texture RenderTexture;
            public Texture Texture;
#if UNITY_VIDEO
            public VideoPlayer videoPlayer;
#endif
            public MeshCollider meshCollider;
        }

        /// <summary>
        /// Initializes and returns an instance of this <c>PXR_CustomLayerHandler&lt;T&gt;</c> while also setting the singleton instance member.
        /// </summary>
        protected PXR_CustomLayerHandler() => Instance = this;

        /// <summary>
        /// Singleton instance of this specific handler.
        /// </summary>
        protected static PXR_CustomLayerHandler<T> Instance;

        /// <summary>
        /// Deinitializes this instance of c>PXR_CustomLayerHandler&lt;T&gt;</c>.
        /// </summary>
        ~PXR_CustomLayerHandler() => Dispose(false);

        /// <summary>
        /// Override this method to create the <see cref="XrSwapchainCreateInfo"/> struct that is passed to PXR_
        /// to create a swapchain.
        /// </summary>
        /// <remarks>
        /// To add extensions when constructing the <see cref="XrSwapchainCreateInfo"/> struct, initialize
        /// the <c>Next</c> pointer with
        /// <see cref="PXR_LayerUtility.GetExtensionsChain(CompositionLayerManager.LayerInfo, Unity.XR.CompositionLayers.CompositionLayerExtension.ExtensionTarget)"/>.
        /// </remarks>
        /// <param name="layerInfo"> Container for the instance id and CompositionLayer component of the composition layer
        /// that was just created.</param>
        /// <param name="swapchainCreateInfo"> An <c>XrSwapchainCreateInfo</c> object created and initialized by the concrete implementation of this method.</returns>
        /// <returns> A bool indicating success or failure.</returns>

        protected abstract bool CreateSwapchain(CompositionLayerManager.LayerInfo layerInfo, out SwapchainCreateInfo swapchainCreateInfo);

        /// <summary>
        /// Override this method to create the native composition layer struct of type T that is passed to PXR_.
        /// A swapchain info struct is provided so your layer handler has access to any needed swapchain information.
        /// </summary>
        /// <remarks>
        /// To add extensions when constructing the <see cref="XrSwapchainCreateInfo"/> struct, initialize
        /// the <c>Next</c> pointer with <see cref="PXR_LayerUtility.GetExtensionsChain(CompositionLayerManager.LayerInfo, Unity.XR.CompositionLayers.CompositionLayerExtension.ExtensionTarget)"/>.
        ///
        /// If your struct needs any XrSpace relative info you can use <see cref="PXR_LayerUtility.GetCurrentAppSpace"/>
        /// to get the current app space.
        /// </remarks>
        /// <param name="layerInfo"> Container for the instance id and CompositionLayer component of the composition layer
        /// that was just created.</param>
        /// <param name="swapchainOutput"> Information regarding the swapchain that was created for this layer,
        /// such as the associated swapchain handle.</param>
        /// <param name="nativeLayer"> An object of type T that is created and initialized by the concrete implementation of this method.</returns>
        /// <returns> A bool indicating success or failure.</returns>

        protected abstract bool CreateNativeLayer(CompositionLayerManager.LayerInfo layerInfo, SwapchainCreatedOutput swapchainOutput, out T nativeLayer);

        /// <summary>
        /// Override this method to modify a native composition layer struct in response to changes on the associated
        /// <see cref="Unity.XR.CompositionLayers.Layers.LayerData"/> object or any extension components on the
        /// <see cref="Unity.XR.CompositionLayers.CompositionLayer"/> GameObject.
        /// </summary>
        /// <remarks>
        /// You must reinitialize the Next pointer with <see cref="PXR_LayerUtility.GetExtensionsChain(CompositionLayerManager.LayerInfo, Unity.XR.CompositionLayers.CompositionLayerExtension.ExtensionTarget)"/>
        /// to get any potential updates from extension components.
        /// </remarks>
        /// <param name="layerInfo"> Container for the instance id and CompositionLayer component of the composition
        /// layer that was modified.</param>
        /// <param name="nativeLayer"> A reference to the native PXR_ structure of the composition layer that was modified.
        /// The concrete implementation of this method should update the values of the structure as appropriate.</param>
        /// <returns> A bool indicating success or failure.</returns>
        protected abstract bool ModifyNativeLayer(CompositionLayerManager.LayerInfo layerInfo, ref T nativeLayer);

        /// <summary>
        /// Mapping of instance ids and native layer structs to help determine what layers are currently set to be active.
        /// </summary>
        protected Dictionary<int, T> m_nativeLayers = new Dictionary<int, T>();

        /// <summary>
        /// Thread safe queue used to dispatch callbacks that may come from other threads such as the swapchain creation
        /// on the graphics thread.
        /// </summary>
        protected ConcurrentQueue<Action> actionsForMainThread = new ConcurrentQueue<Action>();

        Dictionary<int, LayerRenderInfo> m_renderInfos = new Dictionary<int, LayerRenderInfo>();
        Dictionary<int, CompositionLayerManager.LayerInfo> m_layerInfos = new Dictionary<int, CompositionLayerManager.LayerInfo>();
        NativeArray<T> m_ActiveNativeLayers;
        NativeArray<int> m_ActiveNativeLayerOrders;
        int m_ActiveNativeLayerCount;

        /// <summary>
        /// Implements the <see cref="PXR_LayerProvider.ILayerHandler"/> method that is called by the
        /// <see cref="PXR_LayerProvider"/> during the Unity update loop.
        /// </summary>
        /// <remarks>
        /// This implementation carries out two tasks. It dequeues actions for the main thread like dispatch when
        /// the swapchain has been
        /// created and it adds all the active layers to the <c>endFrameInfo</c> struct in the native UnityPXR_ lib.
        /// </remarks>
        public virtual void OnUpdate()
        {
            while (actionsForMainThread.Count > 0)
            {
                if (actionsForMainThread.TryDequeue(out Action action))
                    action();
            }

            unsafe
            {
                if (m_ActiveNativeLayerCount > 0)
                    PXR_LayerUtility.AddActiveLayersToEndFrame(m_ActiveNativeLayers.GetUnsafePtr(), m_ActiveNativeLayerOrders.GetUnsafePtr(), m_ActiveNativeLayerCount, UnsafeUtility.SizeOf<T>());
            }

            m_ActiveNativeLayerCount = 0;
        }

        /// <summary>
        /// Implements the <see cref="PXR_LayerProvider.ILayerHandler"/> method that is called by the
        /// <see cref="PXR_LayerProvider"/> when a new layer has been created.
        /// This implementation triggers the creation of a swapchain before the actual native layer struct is created.
        /// </summary>
        /// <param name="layerInfo"> Container for the instance id and CompositionLayer component of the composition layer
        /// being created.</param>
        public void CreateLayer(CompositionLayerManager.LayerInfo layerInfo)
        {
            CreateSwapchainAsync(layerInfo);
        }

        /// <summary>
        /// Implements the <see cref="PXR_LayerProvider.ILayerHandler"/> method that is called by the
        /// <see cref="PXR_LayerProvider"/> when a layer or attached extension has been modified.
        /// This implementation asks the subclass for any changes that must be made to the layer via
        /// <see cref="ModifyNativeLayer(CompositionLayerManager.LayerInfo, ref T)"/>
        /// by sending a reference to the native layer struct.
        /// </summary>
        /// <param name="layerInfo"> Container for the instance id and CompositionLayer component of the composition layer
        /// that was modified.</param>
        public virtual void ModifyLayer(CompositionLayerManager.LayerInfo layerInfo)
        {
            var texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();

            if (!m_nativeLayers.TryGetValue(layerInfo.Id, out var nativeLayer))
            {
                if (texturesExtension != null && texturesExtension.TextureAdded)
                {
                    texturesExtension.TextureAdded = false;
                    CreateLayer(layerInfo);
                }
                return;
            }

            var success = ModifyNativeLayer(layerInfo, ref nativeLayer);
            if (success)
                m_nativeLayers[layerInfo.Id] = nativeLayer;
        }

        /// <summary>
        /// Implements the <see cref="PXR_LayerProvider.ILayerHandler"/> method that is called by the
        /// <see cref="PXR_LayerProvider"/> when a layer is destroyed or disabled.
        /// </summary>
        /// <param name="removedLayerId"> The instance id of the CompositionLayer component that was removed.</param>
        public virtual void RemoveLayer(int removedLayerId)
        {
            PXR_LayerUtility.ReleaseSwapchain(removedLayerId);
            m_nativeLayers.Remove(removedLayerId);
            m_layerInfos.Remove(removedLayerId);
            m_renderInfos.Remove(removedLayerId);
        }

        /// <summary>
        /// Implements the <see cref="PXR_LayerProvider.ILayerHandler"/> method that is called by the
        /// <see cref="PXR_LayerProvider"/> when a layer is considered to be currently active.
        /// </summary>
        /// <param name="layerInfo"> Container for the instance id and CompositionLayer component of the composition layer
        /// being set to active.</param>
        public virtual void SetActiveLayer(CompositionLayerManager.LayerInfo layerInfo)
        {
            if (!m_nativeLayers.TryGetValue(layerInfo.Id, out var nativeLayer))
                return;
            var success = ActiveNativeLayer(layerInfo, ref nativeLayer);
            if (!success)
                return;
            m_nativeLayers[layerInfo.Id] = nativeLayer;
            ResizeNativeArrays();
            m_ActiveNativeLayers[m_ActiveNativeLayerCount] = m_nativeLayers[layerInfo.Id];
            m_ActiveNativeLayerOrders[m_ActiveNativeLayerCount] = layerInfo.Layer.Order;
            ++m_ActiveNativeLayerCount;
        }

        /// <summary>
        /// Implements method from <see cref="IDisposable"/> that is called by the <see cref="PXR_LayerProvider"/>
        /// when this custom layer handler instance is disposed.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Clears all maps and disposes any created native arrays.
        /// </summary>
        /// <param name="disposing">Determines if this method was called from the Dispose() method or the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_layerInfos.Clear();
                m_nativeLayers.Clear();
                m_renderInfos.Clear();
            }

            if (m_ActiveNativeLayers.IsCreated)
                m_ActiveNativeLayers.Dispose();
            if (m_ActiveNativeLayerOrders.IsCreated)
                m_ActiveNativeLayerOrders.Dispose();
        }

        /// <summary>
        /// Calls <see cref="CreateSwapchain(CompositionLayerManager.LayerInfo, out SwapchainCreateInfo)"/> to create a
        /// <see cref="SwapchainCreateInfo"/> struct that is then passed to the
        /// UnityPXR_ lib to actually create the swapchain on the graphics thread.
        /// The static <see cref="OnCreatedSwapchainCallback(int, ulong)"/> method is passed as a callback and invoked when
        /// the swapchain has been created.
        /// </summary>
        /// <param name="layerInfo"> Container for the instance id and CompositionLayer component of the composition layer
        /// that was just created.</param>
        protected virtual void CreateSwapchainAsync(CompositionLayerManager.LayerInfo layerInfo)
        {
            m_layerInfos[layerInfo.Id] = layerInfo;
            var success = CreateSwapchain(layerInfo, out var swapChainInfo);
            if (!success)
                return;

            if (swapChainInfo.isStereo)
                PXR_LayerUtility.CreateStereoSwapchain(layerInfo.Id, swapChainInfo.nativeStruct, OnCreatedStereoSwapchainCallback);
            else
                PXR_LayerUtility.CreateSwapchain(layerInfo.Id, swapChainInfo.nativeStruct, swapChainInfo.isExternalSurface, OnCreatedSwapchainCallback);
        }

        /// <summary>
        /// This method is dispatched to the main thread inside <see cref="OnCreatedSwapchainCallback(int, ulong)"/>
        /// and asks this subclass to create the native layer struct by invoking
        /// <see cref="CreateNativeLayer(CompositionLayerManager.LayerInfo, SwapchainCreatedOutput, out T)"/>.
        /// </summary>
        /// <param name="layerInfo"> Container for the instance id and CompositionLayer component of the composition layer
        /// that was just created.</param>
        /// <param name="swapchainOutput"> Information regarding the swapchain that was created for this layer, such as
        /// the associated swapchain handle.</param>
        protected virtual void OnCreatedSwapchain(CompositionLayerManager.LayerInfo layerInfo, SwapchainCreatedOutput swapchainOutput)
        {
            var success = CreateNativeLayer(layerInfo, swapchainOutput, out var nativeLayer);
            if (success){

                m_nativeLayers[layerInfo.Id] = nativeLayer;
            }
        }

        /// <summary>
        /// Ensures that the native arrays are of the same size as the m_nativeLayers map.
        /// </summary>
        protected virtual void ResizeNativeArrays()
        {
            if (!m_ActiveNativeLayers.IsCreated && !m_ActiveNativeLayerOrders.IsCreated)
            {
                m_ActiveNativeLayers = new NativeArray<T>(m_nativeLayers.Count, Allocator.Persistent);
                m_ActiveNativeLayerOrders = new NativeArray<int>(m_nativeLayers.Count, Allocator.Persistent);
                return;
            }

            UnityEngine.Assertions.Assert.AreEqual(m_ActiveNativeLayers.Length, m_ActiveNativeLayerOrders.Length);

            if (m_ActiveNativeLayers.Length < m_nativeLayers.Count)
            {
                var newLayerArray = new NativeArray<T>(m_nativeLayers.Count, Allocator.Persistent);
                NativeArray<T>.Copy(m_ActiveNativeLayers, newLayerArray, m_ActiveNativeLayers.Length);
                m_ActiveNativeLayers.Dispose();
                m_ActiveNativeLayers = newLayerArray;

                var newOrderArray = new NativeArray<int>(m_nativeLayers.Count, Allocator.Persistent);
                NativeArray<int>.Copy(m_ActiveNativeLayerOrders, newOrderArray, m_ActiveNativeLayerOrders.Length);
                m_ActiveNativeLayerOrders.Dispose();
                m_ActiveNativeLayerOrders = newOrderArray;
            }
        }

        /// <summary>
        /// Override this method to modify a native composition layer struct in response to when it is active.
        /// An active compositon layer will invoke this every frame.
        /// </summary>
        /// <param name="layerInfo"> Container for the instance id and CompositionLayer component of the composition
        /// layer that is active.</param>
        /// <param name="nativeLayer"> A reference to the native PXR_ structure of the composition layer that is active.</param>
        /// <returns>Bool indicating success or failure. A failure case will result in the native composition layer struct not being added into the final XrFrameEndInfo struct.</returns>
        protected virtual bool ActiveNativeLayer(CompositionLayerManager.LayerInfo layerInfo, ref T nativeLayer)
        {
            var texturesExtension = layerInfo.Layer.GetComponent<TexturesExtension>();
            if (texturesExtension == null || texturesExtension.LeftTexture == null || texturesExtension.sourceTexture == TexturesExtension.SourceTextureEnum.AndroidSurface)
                return true;

            if (m_renderInfos.TryGetValue(layerInfo.Id, out var container))
            {
                PXR_LayerUtility.WriteToRenderTexture(layerInfo.Id, container.Texture, container.RenderTexture, nativeLayer.GetType() == typeof(XrCompositionLayerCubeKHR));
                bool isNewTexture = container.Texture != texturesExtension.LeftTexture;

                if (isNewTexture)
                {
                    // If we have a new texture with different dimensions then we need to release the current swapchain and create another.
                    // This is an async procedure that also creates a new native layer object.
                    if (container.Texture.width != texturesExtension.LeftTexture.width || container.Texture.height != texturesExtension.LeftTexture.height)
                    {
                        RemoveLayer(layerInfo.Id);
                        CreateSwapchainAsync(layerInfo);
                        return false;
                    }
                    else
                        container.Texture = texturesExtension.LeftTexture;

#if UNITY_VIDEO
                    container.videoPlayer = layerInfo.Layer.GetComponent<VideoPlayer>();
#endif
                    container.meshCollider = layerInfo.Layer.GetComponent<MeshCollider>();
                }

                bool isVideo = false;
#if UNITY_VIDEO
                isVideo = container.videoPlayer != null && container.videoPlayer.enabled;
#endif
                bool isUI = container.meshCollider != null && container.meshCollider.enabled;

#if UNITY_EDITOR
                // Layers with a video or ui component in editor may have multiple native render textures associated with the layer id so we must find them.
                if (isVideo || isUI){
                    PXR_LayerUtility.FindAndWriteToRenderTexture(layerInfo, container.Texture, out container.RenderTexture, nativeLayer.GetType() == typeof(XrCompositionLayerCubeKHR));
                }
                else if (isNewTexture){
                    PXR_LayerUtility.WriteToRenderTexture(layerInfo.Id, container.Texture, container.RenderTexture, nativeLayer.GetType() == typeof(XrCompositionLayerCubeKHR));
                }
#else
                // We only need to write continuously to the native render texture if our texture is changing.
                if (isVideo || isUI || isNewTexture)
                    PXR_LayerUtility.WriteToRenderTexture(layerInfo.Id, container.Texture, container.RenderTexture, nativeLayer.GetType() == typeof(XrCompositionLayerCubeKHR));
#endif

            }
            else
            {
                bool isRenderTextureWritten = PXR_LayerUtility.FindAndWriteToRenderTexture(layerInfo, texturesExtension.LeftTexture, out Texture renderTexture, nativeLayer.GetType() == typeof(XrCompositionLayerCubeKHR));
                if (isRenderTextureWritten)
                {
                    var layerRenderInfo = new LayerRenderInfo()
                        { Texture = texturesExtension.LeftTexture, RenderTexture = renderTexture,
#if UNITY_VIDEO
                            videoPlayer = layerInfo.Layer.GetComponent<VideoPlayer>(),
#endif
                            meshCollider = layerInfo.Layer.GetComponent<MeshCollider>() };

                    m_renderInfos.Add(layerInfo.Id, layerRenderInfo);
                };

            }
            return true;
        }

        [AOT.MonoPInvokeCallback(typeof(PXR_LayerUtility.SwapchainCallbackDelegate))]
        static void OnCreatedSwapchainCallback(int layerId, ulong swapchainHandle)
        {
            if (Instance == null)
                return;

            Instance.actionsForMainThread.Enqueue(() => { Instance.OnCreatedSwapchain(Instance.m_layerInfos[layerId], new SwapchainCreatedOutput { handle = swapchainHandle });});
        }

        [AOT.MonoPInvokeCallback(typeof(PXR_LayerUtility.StereoSwapchainCallbackDelegate))]
        static void OnCreatedStereoSwapchainCallback(int layerId, ulong swapchainHandleLeft, ulong swapchainHandleRight)
        {            
            if (Instance == null)
                return;

            Instance.actionsForMainThread.Enqueue(() => { Instance.OnCreatedSwapchain(Instance.m_layerInfos[layerId], new SwapchainCreatedOutput { handle = swapchainHandleLeft, secondStereoHandle = swapchainHandleRight}); });
        }
    }
}
#endif
