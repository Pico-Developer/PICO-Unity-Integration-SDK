//  Copyright © 2015-2022 Pico Technology Co., Ltd. All Rights Reserved.

using System;
using System.Collections;
using PXR_Audio.Spatializer;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class PXR_Audio_Spatializer_Context : MonoBehaviour
{
    [SerializeField] public SpatializerApiImpl spatializerApiImpl = SpatializerApiImpl.unity;

    private static PXR_Audio.Spatializer.Api _api = null;

#if UNITY_EDITOR
    private static SpatializerApiImpl _lastSpatializerApiImpl;
#endif
    public PXR_Audio.Spatializer.Api PXR_Audio_Spatializer_Api
    {
        get
        {
#if UNITY_EDITOR
            if (_api == null ||
                (_lastSpatializerApiImpl != spatializerApiImpl && !EditorApplication.isPlaying))
#else
            if (_api == null)
#endif
            {
                if (spatializerApiImpl == SpatializerApiImpl.unity)
                    _api = new ApiUnityImpl();
                else if (spatializerApiImpl == SpatializerApiImpl.wwise)
                    _api = new ApiWwiseImpl();
#if UNITY_EDITOR
                _lastSpatializerApiImpl = spatializerApiImpl;
#endif
            }

            return _api;
        }
    }

    private static PXR_Audio_Spatializer_Context _instance;

    public static PXR_Audio_Spatializer_Context Instance => _instance;

    private IntPtr context = IntPtr.Zero;

    private bool initialized = false;

    private bool isSceneDirty = false;

    public bool Initialized
    {
        get => initialized;
    }

    [SerializeField]
    private PXR_Audio.Spatializer.RenderingMode renderingQuality = PXR_Audio.Spatializer.RenderingMode.MediumQuality;

    #region EDITOR-ONLY SerializedFields

#if UNITY_EDITOR
    [SerializeField, HideInInspector] private LayerMask meshBakingLayerMask = ~0;
#endif

    #endregion

    public PXR_Audio.Spatializer.RenderingMode RenderingQuality => renderingQuality;

    [SerializeField] private UnityEvent preInitEvent;
    [SerializeField] private UnityEvent lateInitEvent;

    private AudioConfiguration audioConfig;

    public AudioConfiguration AudioConfig => audioConfig;

    private bool bypass = true;

    private bool Bypass => bypass;

    static int uuidCounter = 0;

    private static int GetUuid()
    {
        var temp = uuidCounter;
        uuidCounter = (uuidCounter == Int32.MaxValue) ? 0 : (uuidCounter + 1);
        return temp;
    }

    private int uuid = -1;
    public int UUID => uuid;

    public PXR_Audio.Spatializer.Result SubmitMesh(
        float[] vertices,
        int verticesCount,
        int[] indices,
        int indicesCount,
        PXR_Audio.Spatializer.AcousticsMaterial material,
        ref int geometryId)
    {
        isSceneDirty = true;
        return PXR_Audio_Spatializer_Api.SubmitMesh(
            context,
            vertices,
            verticesCount,
            indices,
            indicesCount,
            material,
            ref geometryId);
    }

    public PXR_Audio.Spatializer.Result SubmitMeshAndMaterialFactor(
        float[] vertices,
        int verticesCount,
        int[] indices,
        int indicesCount,
        float[] absorptionFactor,
        float scatteringFactor,
        float transmissionFactor,
        ref int geometryId)
    {
        isSceneDirty = true;
        return PXR_Audio_Spatializer_Api.SubmitMeshAndMaterialFactor(
            context,
            vertices,
            verticesCount,
            indices,
            indicesCount,
            absorptionFactor,
            scatteringFactor,
            transmissionFactor,
            ref geometryId);
    }

    public Result SubmitMeshWithConfig(float[] vertices, int verticesCount, int[] indices, int indicesCount,
        ref MeshConfig config, ref int geometryId)
    {
        isSceneDirty = true;
        return PXR_Audio_Spatializer_Api.SubmitMeshWithConfig(context, vertices, verticesCount, indices, indicesCount,
            ref config, ref geometryId);
    }

    public Result RemoveMesh(int geometryId)
    {
        isSceneDirty = true;
        return PXR_Audio_Spatializer_Api.RemoveMesh(context, geometryId);
    }

    public Result SetMeshConfig(int geometryId, ref MeshConfig config, uint propertyMask)
    {
        isSceneDirty = true;
        return PXR_Audio_Spatializer_Api.SetMeshConfig(context, geometryId, ref config, propertyMask);
    }

    public PXR_Audio.Spatializer.Result AddSource(
        PXR_Audio.Spatializer.SourceMode sourceMode,
        float[] position,
        ref int sourceId,
        bool isAsync = false)
    {
        return PXR_Audio_Spatializer_Api.AddSource(
            context,
            sourceMode,
            position,
            ref sourceId,
            isAsync);
    }

    public PXR_Audio.Spatializer.Result AddSourceWithOrientation(
        PXR_Audio.Spatializer.SourceMode mode,
        float[] position,
        float[] front,
        float[] up,
        float radius,
        ref int sourceId,
        bool isAsync)
    {
        return PXR_Audio_Spatializer_Api.AddSourceWithOrientation(
            context,
            mode,
            position,
            front,
            up,
            radius,
            ref sourceId,
            isAsync);
    }

    public PXR_Audio.Spatializer.Result AddSourceWithConfig(
        ref PXR_Audio.Spatializer.SourceConfig sourceConfig,
        ref int sourceId,
        bool isAsync)
    {
        return PXR_Audio_Spatializer_Api.AddSourceWithConfig(context, ref sourceConfig, ref sourceId, isAsync);
    }

    public Result SetSourceConfig(int sourceId, ref SourceConfig sourceConfig, uint propertyMask)
    {
        return PXR_Audio_Spatializer_Api.SetSourceConfig(context, sourceId, ref sourceConfig, propertyMask);
    }

    public PXR_Audio.Spatializer.Result SetSourceAttenuationMode(int sourceId,
        PXR_Audio.Spatializer.SourceAttenuationMode mode,
        PXR_Audio.Spatializer.DistanceAttenuationCallback directDistanceAttenuationCallback = null,
        PXR_Audio.Spatializer.DistanceAttenuationCallback indirectDistanceAttenuationCallback = null)
    {
        return PXR_Audio_Spatializer_Api.SetSourceAttenuationMode(context, sourceId, mode,
            directDistanceAttenuationCallback, indirectDistanceAttenuationCallback);
    }

    public PXR_Audio.Spatializer.Result SetSourceRange(int sourceId, float rangeMin, float rangeMax)
    {
        return PXR_Audio_Spatializer_Api.SetSourceRange(context, sourceId, rangeMin, rangeMax);
    }

    public PXR_Audio.Spatializer.Result RemoveSource(int sourceId)
    {
        return PXR_Audio_Spatializer_Api.RemoveSource(context, sourceId);
    }

    public PXR_Audio.Spatializer.Result SubmitSourceBuffer(
        int sourceId,
        float[] inputBufferPtr,
        uint numFrames)
    {
        return PXR_Audio_Spatializer_Api.SubmitSourceBuffer(
            context,
            sourceId,
            inputBufferPtr,
            numFrames);
    }

    public PXR_Audio.Spatializer.Result SubmitAmbisonicChannelBuffer(
        float[] ambisonicChannelBuffer,
        int order,
        int degree,
        PXR_Audio.Spatializer.AmbisonicNormalizationType normType,
        float gain)
    {
        return PXR_Audio_Spatializer_Api.SubmitAmbisonicChannelBuffer(
            context,
            ambisonicChannelBuffer,
            order,
            degree,
            normType,
            gain);
    }

    public PXR_Audio.Spatializer.Result SubmitInterleavedAmbisonicBuffer(
        float[] ambisonicBuffer,
        int ambisonicOrder,
        PXR_Audio.Spatializer.AmbisonicNormalizationType normType,
        float gain)
    {
        return PXR_Audio_Spatializer_Api.SubmitInterleavedAmbisonicBuffer(
            context,
            ambisonicBuffer,
            ambisonicOrder,
            normType,
            gain);
    }

    public PXR_Audio.Spatializer.Result SubmitMatrixInputBuffer(
        float[] inputBuffer,
        int inputChannelIndex)
    {
        return PXR_Audio_Spatializer_Api.SubmitMatrixInputBuffer(
            context,
            inputBuffer,
            inputChannelIndex);
    }

    public PXR_Audio.Spatializer.Result GetInterleavedBinauralBuffer(
        float[] outputBufferPtr,
        uint numFrames,
        bool isAccumulative)
    {
        return PXR_Audio_Spatializer_Api.GetInterleavedBinauralBuffer(
            context,
            outputBufferPtr,
            numFrames,
            isAccumulative);
    }

    public PXR_Audio.Spatializer.Result GetPlanarBinauralBuffer(
        float[][] outputBufferPtr,
        uint numFrames,
        bool isAccumulative)
    {
        return PXR_Audio_Spatializer_Api.GetPlanarBinauralBuffer(
            context,
            outputBufferPtr,
            numFrames,
            isAccumulative);
    }

    public PXR_Audio.Spatializer.Result GetInterleavedLoudspeakersBuffer(
        float[] outputBufferPtr,
        uint numFrames)
    {
        return PXR_Audio_Spatializer_Api.GetInterleavedLoudspeakersBuffer(
            context,
            outputBufferPtr,
            numFrames);
    }

    public PXR_Audio.Spatializer.Result GetPlanarLoudspeakersBuffer(
        float[][] outputBufferPtr,
        uint numFrames)
    {
        return PXR_Audio_Spatializer_Api.GetPlanarLoudspeakersBuffer(
            context,
            outputBufferPtr,
            numFrames);
    }

    public PXR_Audio.Spatializer.Result SetPlaybackMode(
        PXR_Audio.Spatializer.PlaybackMode playbackMode)
    {
        return PXR_Audio_Spatializer_Api.SetPlaybackMode(
            context,
            playbackMode);
    }

    public PXR_Audio.Spatializer.Result SetLoudspeakerArray(
        float[] positions,
        int numLoudspeakers)
    {
        return PXR_Audio_Spatializer_Api.SetLoudspeakerArray(
            context,
            positions,
            numLoudspeakers);
    }

    public PXR_Audio.Spatializer.Result SetMappingMatrix(
        float[] matrix,
        int numInputChannels,
        int numOutputChannels)
    {
        return PXR_Audio_Spatializer_Api.SetMappingMatrix(
            context,
            matrix,
            numInputChannels,
            numOutputChannels);
    }

    public PXR_Audio.Spatializer.Result SetListenerPosition(
        float[] position)
    {
        return PXR_Audio_Spatializer_Api.SetListenerPosition(
            context,
            position);
    }

    public PXR_Audio.Spatializer.Result SetListenerOrientation(
        float[] front,
        float[] up)
    {
        return PXR_Audio_Spatializer_Api.SetListenerOrientation(
            context,
            front,
            up);
    }

    public PXR_Audio.Spatializer.Result SetListenerPose(
        float[] position,
        float[] front,
        float[] up)
    {
        return PXR_Audio_Spatializer_Api.SetListenerPose(
            context,
            position,
            front,
            up);
    }

    public PXR_Audio.Spatializer.Result SetSourcePosition(
        int sourceId,
        float[] position)
    {
        return PXR_Audio_Spatializer_Api.SetSourcePosition(
            context,
            sourceId,
            position);
    }

    public PXR_Audio.Spatializer.Result SetSourceGain(
        int sourceId,
        float gain)
    {
        return PXR_Audio_Spatializer_Api.SetSourceGain(
            context,
            sourceId,
            gain);
    }

    public PXR_Audio.Spatializer.Result SetSourceSize(
        int sourceId,
        float volumetricSize)
    {
        return PXR_Audio_Spatializer_Api.SetSourceSize(
            context,
            sourceId,
            volumetricSize);
    }

    public PXR_Audio.Spatializer.Result UpdateSourceMode(
        int sourceId,
        PXR_Audio.Spatializer.SourceMode mode)
    {
        return PXR_Audio_Spatializer_Api.UpdateSourceMode(
            context,
            sourceId,
            mode);
    }

    public PXR_Audio.Spatializer.Result SetDopplerEffect(int sourceId, bool on)
    {
        return PXR_Audio_Spatializer_Api.SetDopplerEffect(context, sourceId, on);
    }

    void OnAudioConfigurationChangedEventHandler(bool deviceWasChanged)
    {
        audioConfig = AudioSettings.GetConfiguration();
        ResetContext(renderingQuality);
    }

    /// <summary>
    /// Setup Spatializer rendering quality.
    /// </summary>
    /// <param name="quality">Rendering quality preset.</param>
    public void SetRenderingQuality(PXR_Audio.Spatializer.RenderingMode quality)
    {
        renderingQuality = quality;
        AudioSettings.Reset(AudioSettings.GetConfiguration());
        Debug.Log("Pico Spatializer has set rendering quality to: " + renderingQuality);
    }

    private void OnEnable()
    {
        if (_instance == null)
        {
            _instance = this;

            AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChangedEventHandler;

            //  Create context
            StartInternal(renderingQuality);
            Debug.Log("Pico Spatializer Initialized.");

            DontDestroyOnLoad(this);
        }
        else if (_instance != this)
        {
            Destroy(this);
        }
    }

    private void StartInternal(PXR_Audio.Spatializer.RenderingMode quality)
    {
        preInitEvent.Invoke();
        uuid = GetUuid();
        PXR_Audio.Spatializer.Result ret = Result.Success;

        audioConfig = AudioSettings.GetConfiguration();
        ret = PXR_Audio_Spatializer_Api.CreateContext(
            ref context,
            quality,
            (uint)audioConfig.dspBufferSize,
            (uint)audioConfig.sampleRate);
        if (ret != PXR_Audio.Spatializer.Result.Success)
        {
            Debug.LogError("Failed to create context, error code: " + ret);
        }

        ret = PXR_Audio_Spatializer_Api.InitializeContext(context);
        if (ret != PXR_Audio.Spatializer.Result.Success)
        {
            Debug.LogError("Failed to initialize context, error code: " + ret);
        }

        //  Add all the geometries back
        PXR_Audio_Spatializer_SceneGeometry[] geometries = FindObjectsOfType<PXR_Audio_Spatializer_SceneGeometry>();
        for (int geoId = 0; geoId < geometries.Length; ++geoId)
        {
            //  For all found geometry and material pair, submit them into Pico spatializer
            geometries[geoId].SubmitMeshToContext();
            geometries[geoId].SubmitStaticMeshToContext();
            if (ret != PXR_Audio.Spatializer.Result.Success)
            {
                Debug.LogError("Failed to submit geometry #" + geoId + ", error code: " + ret);
            }
        }

        ret = PXR_Audio_Spatializer_Api.CommitScene(context);
        if (ret != PXR_Audio.Spatializer.Result.Success)
        {
            Debug.LogError("Failed to commit scene, error code: " + ret);
        }

        lateInitEvent.Invoke();

        initialized = true;
        if (spatializerApiImpl != SpatializerApiImpl.wwise)
        {
            //  Add all the sources back
            PXR_Audio_Spatializer_AudioSource[] sources = FindObjectsOfType<PXR_Audio_Spatializer_AudioSource>();
            for (int i = 0; i < sources.Length; ++i)
            {
                sources[i].RegisterInternal();
            }
        }

        //  Add listener back
        PXR_Audio_Spatializer_AudioListener listener = FindObjectOfType<PXR_Audio_Spatializer_AudioListener>();
        listener.RegisterInternal();
    }

    private void DestroyInternal()
    {
        initialized = false;
        uuid = -1;
        if (spatializerApiImpl == SpatializerApiImpl.wwise)
        {
            PXR_Audio_Spatializer_Api.Destroy(context);
            context = IntPtr.Zero;
            return;
        }

        //  Wait until all sources and listener's on-going audio DSP process had finished
        bool canContinue = true;
        do
        {
            canContinue = true;
            PXR_Audio_Spatializer_AudioListener[] listeners = FindObjectsOfType<PXR_Audio_Spatializer_AudioListener>();
            foreach (var listener in listeners)
            {
                if (listener != null && listener.IsAudioDSPInProgress)
                {
                    canContinue = false;
                    break;
                }
            }

            PXR_Audio_Spatializer_AudioSource[] sources = FindObjectsOfType<PXR_Audio_Spatializer_AudioSource>();
            foreach (var source in sources)
            {
                if (source != null && source.IsAudioDSPInProgress)
                {
                    canContinue = false;
                    break;
                }
            }
        } while (!canContinue);

        PXR_Audio_Spatializer_Api.Destroy(context);
        context = IntPtr.Zero;
    }

    private void OnDisable()
    {
        if (_instance != null && _instance == this)
        {
            _instance = null;

            //  Remove context reset handler when destructing context
            //  https://docs.microsoft.com/en-us/dotnet/desktop/winforms/controls/how-to-add-an-event-handler?view=netdesktop-6.0
            AudioSettings.OnAudioConfigurationChanged -= OnAudioConfigurationChangedEventHandler;
            DestroyInternal();
        }
    }

    void Update()
    {
        if (isSceneDirty)
        {
            PXR_Audio_Spatializer_Api.CommitScene(context);
            isSceneDirty = false;
        }

        PXR_Audio_Spatializer_Api.UpdateScene(context);
    }

    void ResetContext(PXR_Audio.Spatializer.RenderingMode quality)
    {
        DestroyInternal();
        StartInternal(quality);

        if (spatializerApiImpl == SpatializerApiImpl.wwise)
        {
            return;
        }

        //  Resume all sources playback
        var sources = FindObjectsOfType<PXR_Audio_Spatializer_AudioSource>();
        foreach (var source in sources)
        {
            source.Resume();
        }

        //  Resume all ambisonic sources playback
        var ambisonicSources =
            FindObjectsOfType<PXR_Audio_Spatializer_AmbisonicSource>();
        foreach (var source in ambisonicSources)
        {
            source.Resume();
        }

        Debug.Log("Pico Spatializer Context restarted.");
    }
}