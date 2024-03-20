//  Copyright © 2015-2022 Pico Technology Co., Ltd. All Rights Reserved.

using System;
using System.Collections;
using PXR_Audio.Spatializer;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PXR_Audio_Spatializer_AudioSource : MonoBehaviour
{
    [SerializeField] [Range(0.0f, 24.0f)] private float sourceGainDB = 0.0f;
    private float sourceGainAmplitude = 1.0f;

    [SerializeField] [Range(-120.0f, 48.0f)] private float reflectionGainDB = 0.0f;
    private float reflectionGainAmplitude = 1.0f;

    [SerializeField] [Range(0.0f, 100000.0f)]
    private float sourceSize = 0.0f;

    [SerializeField] private bool enableDoppler = true;

    [SerializeField] public SourceAttenuationMode sourceAttenuationMode = SourceAttenuationMode.InverseSquare;
    [SerializeField] public float minAttenuationDistance = 1.0f;
    [SerializeField] public float maxAttenuationDistance = 100.0f;
    [SerializeField] [Range(0.0f, 1.0f)] private float directivityAlpha = 0.0f;

    [SerializeField] [Range(0.0f, 1000.0f)]
    private float directivityOrder = 1.0f;

#if UNITY_EDITOR
    private Mesh directivityDisplayMesh;
#endif

    private SourceConfig sourceConfig;
    private uint sourcePropertyMask = 0;

    private bool isActive;
    private bool isAudioDSPInProgress = false;

    public bool IsAudioDSPInProgress
    {
        get { return isAudioDSPInProgress; }
    }

    private PXR_Audio_Spatializer_Context context;

    private PXR_Audio_Spatializer_Context Context
    {
        get
        {
            if (context == null)
                context = PXR_Audio_Spatializer_Context.Instance;
            return context;
        }
    }

    private AudioSource nativeSource;

    private int sourceId = -1;

    private int currentContextUuid = -2;

    private float[] positionArray = new float[3] { 0.0f, 0.0f, 0.0f };

    private float playheadPosition = 0.0f;
    private bool wasPlaying = false;

    private void OnEnable()
    {
        if (Context != null && Context.Initialized)
        {
            if (Context.UUID == currentContextUuid)
                isActive = true;
            else
                RegisterInternal();
        }
        else
        {
            sourceId = -1;
            currentContextUuid = -2;
        }
    }

    /// <summary>
    /// Register this audio source in spatializer
    /// </summary>
    internal void RegisterInternal()
    {
        nativeSource = GetComponent<AudioSource>();

        positionArray[0] = transform.position.x;
        positionArray[1] = transform.position.y;
        positionArray[2] = -transform.position.z;

        sourceConfig = new SourceConfig(PXR_Audio.Spatializer.SourceMode.Spatialize);
        sourcePropertyMask = 0;

        sourceConfig.position.x = positionArray[0];
        sourceConfig.position.y = positionArray[1];
        sourceConfig.position.z = positionArray[2];
        sourceConfig.front.x = transform.forward.x;
        sourceConfig.front.y = transform.forward.y;
        sourceConfig.front.z = -transform.forward.z;
        sourceConfig.up.x = transform.up.x;
        sourceConfig.up.y = transform.up.y;
        sourceConfig.up.z = -transform.up.z;
        sourceConfig.enableDoppler = enableDoppler;
        sourceGainAmplitude = DB2Mag(sourceGainDB);
        sourceConfig.sourceGain = sourceGainAmplitude;
        reflectionGainAmplitude = DB2Mag(reflectionGainDB);
        sourceConfig.reflectionGain = reflectionGainAmplitude;
        sourceConfig.radius = sourceSize;
        sourceConfig.attenuationMode = sourceAttenuationMode;
        sourceConfig.minAttenuationDistance = minAttenuationDistance;
        sourceConfig.maxAttenuationDistance = maxAttenuationDistance;
        sourceConfig.directivityAlpha = directivityAlpha;
        sourceConfig.directivityOrder = directivityOrder;

        PXR_Audio.Spatializer.Result ret = Context.AddSourceWithConfig(
            ref sourceConfig,
            ref sourceId,
            true);
        if (ret != PXR_Audio.Spatializer.Result.Success)
        {
            Debug.LogError("Failed to add source.");
            return;
        }

        isActive = true;
        currentContextUuid = Context.UUID;

        Debug.Log("Source #" + sourceId + " is added.");
    }

    /// <summary>
    /// Resume playing status of this source
    /// </summary>
    public void Resume()
    {
        nativeSource.time = playheadPosition;
        if (wasPlaying)
        {
            nativeSource.Play();
        }
    }

    /// <summary>
    /// Setup source gain in dB
    /// </summary>
    /// <param name="gainDB">Gain in dB</param>
    public void SetGainDB(float gainDB)
    {
        // if (Mathf.Abs(gainDB - sourceGainDB) < 1e-7) return;
        sourceGainDB = gainDB;
        sourceConfig.sourceGain = sourceGainAmplitude = DB2Mag(gainDB);
        sourcePropertyMask |= (uint)SourceProperty.SourceGain;
    }

    /// <summary>
    /// Setup source gain in Amplitude
    /// </summary>
    /// <param name="gainAmplitude">Gain in Amplitude</param>
    public void SetGainAmplitude(float gainAmplitude)
    {
        sourceConfig.sourceGain = sourceGainAmplitude = gainAmplitude;
        sourceGainDB = Mag2DB(gainAmplitude);
        sourcePropertyMask |= (uint)SourceProperty.SourceGain;
    }

    /// <summary>
    /// Setup source reflection gain in dB
    /// </summary>
    /// <param name="gainDB">Gain in dB</param>
    public void SetReflectionGainDB(float gainDB)
    {
        reflectionGainDB = gainDB;
        sourceConfig.reflectionGain = reflectionGainAmplitude = DB2Mag(gainDB);
        sourcePropertyMask |= (uint)SourceProperty.ReflectionGain;
    }

    /// <summary>
    /// Setup source radius in meters
    /// </summary>
    /// <param name="radius">source radius in meter</param>
    public void SetSize(float radius)
    {
        sourceConfig.radius = sourceSize = radius;
        sourcePropertyMask |= (uint)SourceProperty.VolumetricRadius;
    }

    /// <summary>
    /// Turn on/off in-engine doppler effect
    /// </summary>
    /// <param name="on">Turn doppler effect on/off </param>
    public void SetDopplerStatus(bool on)
    {
        sourceConfig.enableDoppler = enableDoppler = on;
        sourcePropertyMask |= (uint)SourceProperty.DopplerOnOff;
    }

    /// <summary>
    /// Setup min attenuation range
    /// </summary>
    /// <param name="min"> Minimum attenuation range. Source loudness would stop increasing when source-listener
    /// distance is shorter than this </param>
    public void SetMinAttenuationRange(float min)
    {
        sourceConfig.minAttenuationDistance = minAttenuationDistance = min;
        sourcePropertyMask |= (uint)SourceProperty.RangeMin;
    }

    /// <summary>
    /// Setup max attenuation range
    /// </summary>
    /// <param name="max"> Maximum attenuation range. Source loudness would stop decreasing when source-listener
    /// distance is further than this </param>
    public void SetMaxAttenuationRange(float max)
    {
        sourceConfig.maxAttenuationDistance = maxAttenuationDistance = max;
        sourcePropertyMask |= (uint)SourceProperty.RangeMax;
    }

    /// <summary>
    /// Setup the radiation polar pattern of source, which describes the gain of initial sound wave radiated towards
    /// different directions. The relation between sound emission direction, alpha, and order can be described as
    /// follows: Let theta equals the angle between radiation direction and source front direction, the directivity
    /// gain g is:
    ///     g = (|1 - alpha| + alpha * cos(theta)) ^ order;
    /// </summary>
    /// <param name="alpha"> Define the shape of the directivity pattern.
    /// <param name="order"> Indicates how sharp the source polar pattern is.
    public void SetDirectivity(float alpha, float order)
    {
        sourceConfig.directivityAlpha = directivityAlpha = alpha;
        sourceConfig.directivityOrder = directivityOrder = order;
        sourcePropertyMask |= (uint)SourceProperty.Directivity;
    }

    void Update()
    {
        if (isActive && sourceId >= 0 && context != null && context.Initialized)
        {
            if (transform.hasChanged)
            {
                sourceConfig.position.x = transform.position.x;
                sourceConfig.position.y = transform.position.y;
                sourceConfig.position.z = -transform.position.z;
                sourceConfig.front.x = transform.forward.x;
                sourceConfig.front.y = transform.forward.y;
                sourceConfig.front.z = -transform.forward.z;
                sourceConfig.up.x = transform.up.x;
                sourceConfig.up.y = transform.up.y;
                sourceConfig.up.z = -transform.up.z;
                
                sourcePropertyMask |= (uint)SourceProperty.Position | (uint)SourceProperty.Orientation;
                transform.hasChanged = false;
            }

            if (sourcePropertyMask != 0)
            {
                var ret = Context.SetSourceConfig(sourceId, ref sourceConfig, sourcePropertyMask);
                if (ret == Result.Success)
                    sourcePropertyMask = 0;
            }

            if (nativeSource.isPlaying)
                playheadPosition = nativeSource.time;
            wasPlaying = nativeSource.isPlaying;
        }
    }

    private void OnDisable()
    {
        isActive = false;
        isAudioDSPInProgress = false;
    }

    private void OnDestroy()
    {
        DestroyInternal();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (EditorApplication.isPlaying)
        {
            SetGainDB(sourceGainDB);
            SetReflectionGainDB(reflectionGainDB);
            SetSize(sourceSize);
            SetDopplerStatus(enableDoppler);
            SetDirectivity(directivityAlpha, directivityOrder);
        }
    }
#endif
    private void DestroyInternal()
    {
        isActive = false;
        if (context != null && context.Initialized)
        {
            var ret = context.RemoveSource(sourceId);
            if (ret != PXR_Audio.Spatializer.Result.Success)
            {
                Debug.LogError("Failed to delete source #" + sourceId + ", error code is: " + ret);
            }
            else
            {
                Debug.Log("Source #" + sourceId + " is deleted.");
            }
        }

        isAudioDSPInProgress = false;
        sourceId = -1;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (!isActive || sourceId < 0 || context == null || !context.Initialized)
        {
            //  Mute Original signal
            for (int i = 0; i < data.Length; ++i)
                data[i] = 0.0f;
            return;
        }

        isAudioDSPInProgress = true;
        int numFrames = data.Length / channels;
        float oneOverChannelsF = 1.0f / ((float)channels);

        //  force to mono
        if (channels > 1)
        {
            for (int frame = 0; frame < numFrames; ++frame)
            {
                float sample = 0.0f;
                for (int channel = 0; channel < channels; ++channel)
                {
                    sample += data[frame * channels + channel];
                }

                data[frame] = sample * oneOverChannelsF;
            }
        }

        Context.SubmitSourceBuffer(sourceId, data, (uint)numFrames);

        //  Mute Original signal
        for (int i = 0; i < data.Length; ++i)
            data[i] = 0.0f;
        isAudioDSPInProgress = false;
    }

    private float DB2Mag(float db)
    {
        return Mathf.Pow(10.0f, db / 20.0f);
    }

    private float Mag2DB(float mag)
    {
        return 20 * Mathf.Log10(mag);
    }

    void OnDrawGizmos()
    {
        Color c;
        const float colorSolidAlpha = 0.1f;

        // VolumetricRadius (purple)
        c.r = 1.0f;
        c.g = 0.0f;
        c.b = 1.0f;
        c.a = 1.0f;
        Gizmos.color = c;
        Gizmos.DrawWireSphere(transform.position, sourceSize);
        c.a = colorSolidAlpha;
        Gizmos.color = c;
        Gizmos.DrawSphere(transform.position, sourceSize);

        //  Attenuation distance (min && max)
        if (sourceAttenuationMode == SourceAttenuationMode.InverseSquare)
        {
            //  min
            c.r = 1.0f;
            c.g = 0.35f;
            c.b = 0.0f;
            c.a = 1.0f;
            Gizmos.color = c;
            Gizmos.DrawWireSphere(transform.position, minAttenuationDistance);
            c.a = colorSolidAlpha;
            Gizmos.color = c;
            Gizmos.DrawSphere(transform.position, minAttenuationDistance);

            //  max
            c.r = 0.0f;
            c.g = 1.0f;
            c.b = 1.0f;
            c.a = 1.0f;
            Gizmos.color = c;
            Gizmos.DrawWireSphere(transform.position, maxAttenuationDistance);
            c.a = colorSolidAlpha;
            Gizmos.color = c;
            Gizmos.DrawSphere(transform.position, maxAttenuationDistance);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        //  Draw directivity mesh
        GeneratePolarPatternMesh(directivityDisplayMesh, directivityAlpha, directivityOrder);
    }
    
    private void GeneratePolarPatternMesh(Mesh mesh, float alpha, float order)
    {
        if (mesh == null)
            mesh = new Mesh();
        Vector2[] cardioidVertices2D = GeneratePolarPatternVertices2D(alpha, order, 90);
        int numVertices = cardioidVertices2D.Length * 2;
        Vector3[] vertices = new Vector3[numVertices];
        for (int i = 0; i < cardioidVertices2D.Length; ++i)
        {
            var vertex2D = cardioidVertices2D[i];
            vertices[i] = new Vector3(vertex2D.x, 0.0f, vertex2D.y);
            vertices[cardioidVertices2D.Length + i] = Quaternion.AngleAxis(45, Vector3.forward) *
                                                      new Vector3(vertex2D.x, 0.0f, vertex2D.y);
        }

        int[] indices = new int[cardioidVertices2D.Length * 2 * 3];
        int idx = 0;
        for (idx = 0; idx < cardioidVertices2D.Length - 1; ++idx)
        {
            indices[idx * 6 + 0] = idx;
            indices[idx * 6 + 1] = idx + 1;
            indices[idx * 6 + 2] = idx + cardioidVertices2D.Length;
            indices[idx * 6 + 3] = idx + 1;
            indices[idx * 6 + 4] = idx + cardioidVertices2D.Length + 1;
            indices[idx * 6 + 5] = idx + cardioidVertices2D.Length;
        }

        // Construct a new mesh for the gizmo.
        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        // Draw the mesh.
        Vector3 scale = 2.0f * Mathf.Max(transform.lossyScale.x, transform.lossyScale.z) * Vector3.one;
        Color c;
        c.r = 0.2f;
        c.g = 0.5f;
        c.b = 0.7f;
        c.a = 0.5f;
        Gizmos.color = c;
        Gizmos.DrawMesh(mesh, transform.position, transform.rotation, scale);
        Gizmos.DrawMesh(mesh, transform.position, transform.rotation * Quaternion.AngleAxis(45, Vector3.forward),
            scale);
        Gizmos.DrawMesh(mesh, transform.position, transform.rotation * Quaternion.AngleAxis(90, Vector3.forward),
            scale);
        Gizmos.DrawMesh(mesh, transform.position, transform.rotation * Quaternion.AngleAxis(135, Vector3.forward),
            scale);
        Gizmos.DrawMesh(mesh, transform.position, transform.rotation * Quaternion.AngleAxis(180, Vector3.forward),
            scale);
        Gizmos.DrawMesh(mesh, transform.position, transform.rotation * Quaternion.AngleAxis(225, Vector3.forward),
            scale);
        Gizmos.DrawMesh(mesh, transform.position, transform.rotation * Quaternion.AngleAxis(270, Vector3.forward),
            scale);
        Gizmos.DrawMesh(mesh, transform.position, transform.rotation * Quaternion.AngleAxis(315, Vector3.forward),
            scale);
    }

    private Vector2[] GeneratePolarPatternVertices2D(float alpha, float order, int numVertices)
    {
        Vector2[] points = new Vector2[numVertices];
        float interval = Mathf.PI / (numVertices - 1);
        for (int i = 0; i < numVertices; ++i)
        {
            float theta = 0.0f;
            if (i != numVertices - 1)
                theta = i * interval;
            else
                theta = Mathf.PI;
            // Magnitude |r| for |theta| in radians.
            float r = Mathf.Pow(Mathf.Abs((1 - alpha) + alpha * Mathf.Cos(theta)), order);
            points[i] = new Vector2(r * Mathf.Sin(theta), r * Mathf.Cos(theta));
        }

        return points;
    }
#endif
}