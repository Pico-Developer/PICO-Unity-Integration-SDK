#if XR_COMPOSITION_LAYERS
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Unity.Profiling;
using Unity.XR.CompositionLayers.Extensions;
using Unity.XR.CompositionLayers.Layers;
using Unity.XR.CompositionLayers.Provider;
using Unity.XR.CompositionLayers.Services;
using UnityEngine;

namespace Unity.XR.PXR
{
    /// <summary>
    /// Manages communication of changes between an application and the UnityPXR_ lib for all
    /// <see cref="Unity.XR.CompositionLayers.Layers.LayerData"/> objects.
    /// </summary>
    /// <remarks>
    /// PXR_ providers or extensions that create custom composition layer types or that override how the built-in
    /// layer types are handled, must implement the <see cref="ILayerProvider"/> interface and register instances of
    /// these implementations with the <c>PXR_LayerProvider</c> via <see cref="RegisterLayerHandler(Type, PXR_LayerProvider.ILayerHandler)"/>.
    /// </remarks>
    public class PXR_LayerProvider : ILayerProvider, IDisposable
    {
        /// <summary>
        /// An interface used by the <see cref="PXR_LayerProvider"/> to communicate layer data changes to
        /// registered layer handlers.
        /// </summary>
        /// <remarks>
        /// <c>ILayerHandler</c> instances must register themselves via
        /// <see cref="PXR_LayerProvider.RegisterLayerHandler(Type, PXR_LayerProvider.ILayerHandler)"/>
        /// to specify the <see cref="LayerData"/> type to handle.
        /// If more than one object registers itself as a handler for a specific <see cref="LayerData"/>
        /// type, the last registered handler is used.
        ///
        /// The <see cref="PXR_CustomLayerHandler{T}"/> class provides a partial, base implementation of this interface that you can
        /// use to create custom layer handlers.
        /// </remarks>
        public interface ILayerHandler
        {

            /// <summary>
            /// Called by the <see cref="PXR_LayerProvider"/> during the Unity Update loop.
            /// All implementations must call <see cref="PXR_LayerUtility.AddActiveLayersToEndFrame(void*,void*,int,int)"/> every frame
            /// to add their native layer structs to the <c>endFrameInfo</c> struct inside the UnityPXR_ lib.
            /// </summary>
            public void OnUpdate();

            /// <summary>
            /// Called by the <see cref="PXR_LayerProvider"/> when a new <see cref="LayerData"/>
            /// object of the type registered to this <c>ILayerHandler</c> instance has been created.
            /// </summary>
            /// <param name="layerInfo"> Container for the instance id and CompositionLayer component of the composition layer
            /// that was just created.</param>
            public void CreateLayer(CompositionLayerManager.LayerInfo layerInfo);

            /// <summary>
            /// Called by the <see cref="PXR_LayerProvider"/> when a <see cref="LayerData"/> object
            /// of the type registered to this <c>ILayerHandler</c> instance has been destroyed or disabled.
            /// </summary>
            /// <param name="removedLayerId"> The instance id of the CompositionLayer component that was removed.</param>
            public void RemoveLayer(int id);

            /// <summary>
            /// Called by the <see cref="PXR_LayerProvider"/> when a <see cref="LayerData"/> object
            /// or any attached extension components have had a member modified.
            /// </summary>
            /// <param name="layerInfo"> Container for the instance id and CompositionLayer component of the composition layer
            /// that was modified.</param>
            public void ModifyLayer(CompositionLayerManager.LayerInfo layerInfo);

            /// <summary>
            /// Called every frame by the <see cref="PXR_LayerProvider"/> for all currently active <see cref="LayerData"/> objects
            /// of the type registered to this <c>ILayerHandler</c> instance.
            /// </summary>
            /// <param name="layerInfo"> Container for the instance id and CompositionLayer component of the composition layer
            /// being set to active.</param>
            public void SetActiveLayer(CompositionLayerManager.LayerInfo layerInfo);
        }

        /// <summary>
        /// Initializes and returns an instance of <c>PXR_LayerProvider</c>.
        /// Initializes and registers all the default, built-in layer handlers.
        /// </summary>
        /// <remarks>
        /// The <c>PXR_LayerProvider</c> is created and disposed by the <see cref="Management.XRLoader"/>.
        /// You do not need to create an instance of <c>PXR_LayerProvider</c> yourself. Layer handlers
        /// should only use the static methods and properties of this class
        /// </remarks>
        public PXR_LayerProvider() => InitializeAndRegisterBuiltInHandlers();

        /// <summary>
        /// Calls the methods in its invocation list when the <c>PXR_LayerProvider</c> has started and registered it's built-in layer handlers.
        /// </summary>
        /// <remarks>
        /// You can use this event to wait for the <c>PXR_LayerProvider</c> to finish registering its built-in layer handlers
        /// so that you can override them with your own custom layer handlers.
        /// </remarks>
        public static event Action Started;

        /// <summary>
        /// Calls the methods in its invocation list when the <c>PXR_LayerProvider</c> has stopped and is disposed.
        /// </summary>
        public static event Action Stopped;

        /// <summary>
        /// Reports whether the <c>PXR_LayerProvider</c> has already been created and started.
        /// </summary>
        public static bool isStarted { get; set; }

        static Dictionary<Type, ILayerHandler> LayerHandlers = new Dictionary<Type, ILayerHandler>();
        static readonly ProfilerMarker s_PXR_LayerProviderCreate = new ProfilerMarker("PXR_LayerProvider.Create");
        static readonly ProfilerMarker s_PXR_LayerProviderRemove = new ProfilerMarker("PXR_LayerProvider.Remove");
        static readonly ProfilerMarker s_PXR_LayerProviderModify = new ProfilerMarker("PXR_LayerProvider.Modify");
        static readonly ProfilerMarker s_PXR_LayerProviderActive = new ProfilerMarker("PXR_LayerProvider.Active");
        static readonly ProfilerMarker s_PXR_LayerProviderUpdate = new ProfilerMarker("PXR_LayerProvider.Update");

        /// <summary>
        /// Registers a concrete <see cref="ILayerHandler"/> object as the handler for all layers of a specific
        /// <see cref="LayerData"/> subclass.
        /// </summary>
        /// <remarks>
        /// If more than one object registers itself as a handler for a specific <see cref="LayerData"/>
        /// type, the last registered handler is used.
        ///
        /// The <c>PXR_LayerProvider</c> invokes the registered layer handler's <see cref="ILayerHandler"/> methods
        /// when any object of the associated <see cref="LayerData"/> type is updated in some way.
        /// </remarks>
        /// <param name="layerDataType">The <see cref="LayerData"/> subclass to handle.</param>
        /// <param name="handler">The concrete <c>ILayerHandler</c> instance> to register.</param>
        public static void RegisterLayerHandler(Type layerDataType, ILayerHandler handler)
        {
            if (handler == null)
            {
                LayerHandlers.Remove(layerDataType);
                return;
            }

            LayerHandlers[layerDataType] = handler;
        }

        /// <summary>
        /// Sets the layer provider state on first assignment to the <see cref="CompositionLayerManager" />.
        /// </summary>
        /// <param name="layers">The list of all currently known <see cref="CompositionLayer"/> instances, regardless of active state.</param>
        public void SetInitialState(List<CompositionLayerManager.LayerInfo> layers)
        {
            UpdateLayers(layers, null, null, null);
        }


        /// <summary>
        /// Called by the <see cref="CompositionLayerManager" /> to tell the instance of <see cref="ILayerProvider" /> about
        /// the current state of layers it is managing.
        /// </summary>
        ///
        /// <param name="createdLayers">The list of layers that were just created. Any layer in
        /// this list may be in the <paramref name="activeLayers" /> list if it is activated in the same frame.
        /// Any layer in this list should not be in <paramref name="modifiedLayers" /> or <paramref name="removedLayers" />.
        /// This list is ephemeral and cleared after each call.</param>
        ///
        /// <param name="removedLayers">The list of layers that are no longer being managed. Any layer in
        /// this list should not be in the <paramref name="createdLayers" />, <paramref name="modifiedLayers" />, or
        /// <paramref name="activeLayers" /> lists.
        /// This list is ephemeral and cleared after each call.</param>
        ///
        /// <param name="modifiedLayers">The list of layers that have been recently modified. Any layer in
        /// this list may also be in the <paramref name="activeLayers" /> list. Any layer in this list should not
        /// be in <paramref name="createdLayers" /> or <paramref name="removedLayers" />.
        /// This list is ephemeral and cleared after each call.</param>
        ///
        /// <param name="activeLayers">The list of layers currently active within the scene.
        /// Layers in this list may also be in the <paramref name="createdLayers" /> or <paramref name="modifiedLayers" /> lists
        /// if they became active in the same frame.</param>
        public void UpdateLayers(List<CompositionLayerManager.LayerInfo> createdLayers, List<int> removedLayers, List<CompositionLayerManager.LayerInfo> modifiedLayers, List<CompositionLayerManager.LayerInfo> activeLayers)
        {
            if (removedLayers != null && removedLayers.Count != 0)
            {
                foreach (var handler in LayerHandlers.Values)
                {
                    foreach (var removed in removedLayers)
                    {
                        s_PXR_LayerProviderRemove.Begin();
                        handler?.RemoveLayer(removed);
                        s_PXR_LayerProviderRemove.End();
                    }
                }
            }

            if (createdLayers != null && createdLayers.Count != 0)
            {
                foreach (var created in createdLayers)
                {
                    if (created.Layer == null)
                        continue;

                    var layerDataType = created.Layer.LayerData.GetType();
                    if (LayerHandlers.TryGetValue(layerDataType, out ILayerHandler handler))
                    {
                        s_PXR_LayerProviderCreate.Begin();
                        handler?.CreateLayer(created);
                        s_PXR_LayerProviderCreate.End();
                    }
                }
            }

            if (modifiedLayers != null && modifiedLayers.Count != 0)
            {
                foreach (var modified in modifiedLayers)
                 {
                    if (modified.Layer == null)
                        continue;

                    var layerDataType = modified.Layer.LayerData.GetType();

                    if (LayerHandlers.TryGetValue(layerDataType, out ILayerHandler handler))
                    {
                        s_PXR_LayerProviderModify.Begin();
                        handler?.ModifyLayer(modified);
                        s_PXR_LayerProviderModify.End();
                    }
                }
            }

            if (activeLayers != null && activeLayers.Count != 0)
            {
                foreach (var active in activeLayers)
                {
                    if (active.Layer == null)
                        continue;

                    var layerDataType = active.Layer.LayerData.GetType();
                    if (LayerHandlers.TryGetValue(layerDataType, out ILayerHandler handler))
                    {
                        s_PXR_LayerProviderActive.Begin();
                        handler?.SetActiveLayer(active);
                        s_PXR_LayerProviderActive.End();
                    }
                }
            }

            foreach (var handler in LayerHandlers.Values)
            {
                s_PXR_LayerProviderUpdate.Begin();
                handler?.OnUpdate();
                s_PXR_LayerProviderUpdate.End();
            }
        }

        /// <summary>
        /// Used for cleanup and to call Dispose() on registered layer handlers.
        /// </summary>
        /// <remarks>This is called by the PXR_Loader class when StopInternal() is invoked.</remarks>
        public void Dispose()
        {
            foreach (var handler in LayerHandlers.Values)
            {
                if (handler is IDisposable)
                {
                    ((IDisposable)handler)?.Dispose();
                }
            }

            LayerHandlers.Clear();
            isStarted = false;
            Stopped?.Invoke();
        }

        public void CleanupState()
        {
        }

        public void LateUpdate()
        {
        }

        void InitializeAndRegisterBuiltInHandlers()
        {
            // var defaultLayerHandler = new PXR_DefaultLayer();
            var quadLayerHandler = new PXR_QuadLayer();
            //var projectionLayerHandler = new PXR_ProjectionLayer();
            var cylinderLayerHandler = new PXR_CylinderLayer();
            var cubeLayerHandler = new PXR_CubeLayer() ;
            ILayerHandler equirectLayerHandler = new PXR_Equirect2Layer() ;

            // RegisterLayerHandler(typeof(DefaultLayerData), defaultLayerHandler);
            RegisterLayerHandler(typeof(QuadLayerData), quadLayerHandler);
            RegisterLayerHandler(typeof(CylinderLayerData), cylinderLayerHandler);
            //RegisterLayerHandler(typeof(ProjectionLayerData), projectionLayerHandler);
            //RegisterLayerHandler(typeof(ProjectionLayerRigData), projectionLayerHandler);
            RegisterLayerHandler(typeof(CubeProjectionLayerData), cubeLayerHandler);
            RegisterLayerHandler(typeof(EquirectMeshLayerData), equirectLayerHandler);

            isStarted = true;
            Started?.Invoke();
        }
    }
}

#endif
