//  Copyright © 2015-2022 Pico Technology Co., Ltd. All Rights Reserved.

using System;
using System.Runtime.InteropServices;
using Unity.XR.PXR;
using UnityEngine;

namespace PXR_Audio
{
    namespace Spatializer
    {
        public abstract class Api
        {
            public abstract string GetVersion(ref int major, ref int minor, ref int patch);

            public abstract Result CreateContext(ref IntPtr ctx, RenderingMode mode, uint framesPerBuffer,
                uint sampleRate);

            public abstract Result InitializeContext(IntPtr ctx);

            public abstract Result SubmitMesh(
                IntPtr ctx,
                float[] vertices,
                int verticesCount,
                int[] indices,
                int indicesCount,
                AcousticsMaterial material,
                ref int geometryId);

            public abstract Result SubmitMeshAndMaterialFactor(
                IntPtr ctx,
                float[] vertices,
                int verticesCount,
                int[] indices,
                int indicesCount,
                float[] absorptionFactor,
                float scatteringFactor,
                float transmissionFactor,
                ref int geometryId);

            public abstract Result SubmitMeshWithConfig(
                IntPtr ctx,
                float[] vertices,
                int verticesCount,
                int[] indices,
                int indicesCount,
                ref MeshConfig config,
                ref int geometryId);

            public abstract Result UpdateMesh(
                IntPtr ctx,
                int geometryId,
                float[] newVertices,
                int newVerticesCount,
                int[] newIndices,
                int newIndicesCount,
                ref MeshConfig config,
                ref int newGeometryId,
                bool isAsync = false);

            public abstract Result RemoveMesh(IntPtr ctx, int geometryId);

            public abstract int GetNumOfGeometries(IntPtr ctx);

            public abstract Result SetMeshConfig(
                IntPtr ctx,
                int geometryId,
                ref MeshConfig config,
                uint propertyMask);

            public abstract Result GetAbsorptionFactor(
                AcousticsMaterial material,
                float[] absorptionFactor);

            public abstract Result GetScatteringFactor(
                AcousticsMaterial material,
                ref float scatteringFactor);

            public abstract Result GetTransmissionFactor(
                AcousticsMaterial material,
                ref float transmissionFactor);

            public abstract Result CommitScene(IntPtr ctx);

            public abstract Result AddSource(
                IntPtr ctx,
                SourceMode sourceMode,
                float[] position,
                ref int sourceId,
                bool isAsync);

            public abstract Result AddSourceWithOrientation(
                IntPtr ctx,
                SourceMode mode,
                float[] position,
                float[] front,
                float[] up,
                float radius,
                ref int sourceId,
                bool isAsync);

            public abstract Result AddSourceWithConfig(
                IntPtr ctx,
                ref SourceConfig sourceConfig,
                ref int sourceId,
                bool isAsync,
                AudioSource nativeSource,
                bool enablePicoAttenuation = false);

            public abstract Result SetSourceConfig(IntPtr ctx, int sourceId, ref SourceConfig sourceConfig,
                uint propertyMask, AudioSource nativeSource, bool enabledPicoAttenuation = false);

            public abstract Result SetSourceAttenuationMode(
                IntPtr ctx,
                int sourceId,
                SourceAttenuationMode mode,
                DistanceAttenuationCallback directDistanceAttenuationCallback,
                DistanceAttenuationCallback indirectDistanceAttenuationCallback);

            public abstract Result SetSourceRange(IntPtr ctx, int sourceId, float rangeMin, float rangeMax);
            public abstract Result RemoveSource(IntPtr ctx, int sourceId);

            public abstract Result SubmitSourceBuffer(
                IntPtr ctx,
                int sourceId,
                float[] inputBufferPtr,
                uint numFrames);

            public abstract Result SubmitAmbisonicChannelBuffer(
                IntPtr ctx,
                float[] ambisonicChannelBuffer,
                int order,
                int degree,
                AmbisonicNormalizationType normType,
                float gain);

            public abstract Result SubmitInterleavedAmbisonicBuffer(
                IntPtr ctx,
                float[] ambisonicBuffer,
                int ambisonicOrder,
                AmbisonicNormalizationType normType,
                float gain);

            public abstract Result SubmitMatrixInputBuffer(
                IntPtr ctx,
                float[] inputBuffer,
                int inputChannelIndex);

            public abstract Result GetInterleavedBinauralBuffer(
                IntPtr ctx,
                float[] outputBufferPtr,
                uint numFrames,
                bool isAccumulative);

            public abstract Result GetPlanarBinauralBuffer(
                IntPtr ctx,
                float[][] outputBufferPtr,
                uint numFrames,
                bool isAccumulative);

            public abstract Result GetInterleavedLoudspeakersBuffer(
                IntPtr ctx,
                float[] outputBufferPtr,
                uint numFrames);

            public abstract Result GetPlanarLoudspeakersBuffer(
                IntPtr ctx,
                float[][] outputBufferPtr,
                uint numFrames);

            public abstract Result UpdateScene(IntPtr ctx);
            public abstract Result SetDopplerEffect(IntPtr ctx, int sourceId, bool on);
            public abstract Result SetPlaybackMode(IntPtr ctx, PlaybackMode playbackMode);

            public abstract Result SetLoudspeakerArray(
                IntPtr ctx,
                float[] positions,
                int numLoudspeakers);

            public abstract Result SetMappingMatrix(
                IntPtr ctx,
                float[] matrix,
                int numInputChannels,
                int numOutputChannels);

            public abstract Result SetListenerPosition(
                IntPtr ctx,
                float[] position);

            public abstract Result SetListenerOrientation(
                IntPtr ctx,
                float[] front,
                float[] up);

            public abstract Result SetListenerPose(
                IntPtr ctx,
                float[] position,
                float[] front,
                float[] up);

            public abstract Result SetSourcePosition(
                IntPtr ctx,
                int sourceId,
                float[] position);

            public abstract Result SetSourceGain(
                IntPtr ctx,
                int sourceId,
                float gain);

            public abstract Result SetSourceSize(
                IntPtr ctx,
                int sourceId,
                float volumetricSize);

            public abstract Result UpdateSourceMode(
                IntPtr ctx,
                int sourceId,
                SourceMode mode);

            public abstract Result Destroy(IntPtr ctx);
        }

        public class ApiUnityImpl : Api
        {
#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
            private static string DLLNAME = "__Internal";
#else
            private const string DLLNAME = "PicoSpatializer";
#endif


            [DllImport(DLLNAME, EntryPoint = "yggdrasil_get_version")]
            private static extern string GetVersionImport(ref int major, ref int minor, ref int patch);

            public override string GetVersion(ref int major, ref int minor, ref int patch)
            {
                return GetVersionImport(ref major, ref minor, ref patch);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_create_context")]
            private static extern Result CreateContextImport(
                ref IntPtr ctx,
                RenderingMode mode,
                uint framesPerBuffer,
                uint sampleRate);

            public override Result
                CreateContext(
                    ref IntPtr ctx,
                    RenderingMode mode,
                    uint framesPerBuffer,
                    uint sampleRate
                )
            {
                PXR_Plugin.System.UPxr_LogSdkApi("pico_spatial_audio_create_context|unity_native");
                return CreateContextImport(ref ctx, mode, framesPerBuffer, sampleRate);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_initialize_context")]
            private static extern Result InitializeContextImport(IntPtr ctx);

            public override Result InitializeContext(IntPtr ctx)
            {
                return InitializeContextImport(ctx);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_submit_mesh")]
            private static extern Result SubmitMeshImport(
                IntPtr ctx,
                float[] vertices,
                int verticesCount,
                int[] indices,
                int indicesCount,
                AcousticsMaterial material,
                ref int geometryId);

            public override Result SubmitMesh(
                IntPtr ctx,
                float[] vertices,
                int verticesCount,
                int[] indices,
                int indicesCount,
                AcousticsMaterial material,
                ref int geometryId
            )
            {
                return SubmitMeshImport(ctx, vertices, verticesCount, indices, indicesCount, material, ref geometryId);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_submit_mesh_and_material_factor")]
            private static extern Result SubmitMeshAndMaterialFactorImport(
                IntPtr ctx,
                float[] vertices,
                int verticesCount,
                int[] indices,
                int indicesCount,
                float[] absorptionFactor,
                float scatteringFactor,
                float transmissionFactor,
                ref int geometryId);

            public override Result SubmitMeshAndMaterialFactor(
                IntPtr ctx,
                float[] vertices,
                int verticesCount,
                int[] indices,
                int indicesCount,
                float[] absorptionFactor,
                float scatteringFactor,
                float transmissionFactor,
                ref int geometryId)
            {
                return SubmitMeshAndMaterialFactorImport(ctx, vertices, verticesCount, indices, indicesCount,
                    absorptionFactor, scatteringFactor, transmissionFactor, ref geometryId);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_submit_mesh_with_config")]
            private static extern Result SubmitMeshWithConfigImport(IntPtr ctx, float[] vertices, int verticesCount,
                int[] indices,
                int indicesCount,
                ref MeshConfig config, ref int geometryId, bool isAsync);

            public override Result SubmitMeshWithConfig(IntPtr ctx, float[] vertices, int verticesCount, int[] indices,
                int indicesCount,
                ref MeshConfig config, ref int geometryId)
            {
                PXR_Plugin.System.UPxr_LogSdkApi("pico_spatial_audio_submit_mesh_with_config|unity_native");
                return SubmitMeshWithConfigImport(ctx, vertices, verticesCount, indices, indicesCount, ref config,
                    ref geometryId, false);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_update_mesh")]
            private static extern Result UpdateMeshImport(IntPtr ctx, int geometryId, float[] newVertices,
                int newVerticesCount, int[] newIndices,
                int newIndicesCount, ref MeshConfig config, ref int newGeometryId, bool isAsync = false);

            public override Result UpdateMesh(IntPtr ctx, int geometryId, float[] newVertices, int newVerticesCount,
                int[] newIndices,
                int newIndicesCount, ref MeshConfig config, ref int newGeometryId, bool isAsync = false)
            {
                PXR_Plugin.System.UPxr_LogSdkApi("pico_spatial_audio_update_mesh|unity_native");
                return UpdateMeshImport(ctx, geometryId, newVertices, newVerticesCount, newIndices, newIndicesCount,
                    ref config, ref newGeometryId, isAsync);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_remove_mesh")]
            private static extern Result RemoveMeshImport(IntPtr ctx, int geometryId);

            public override Result RemoveMesh(IntPtr ctx, int geometryId)
            {
                return RemoveMeshImport(ctx, geometryId);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_get_num_of_geometries")]
            private static extern int GetNumOfGeometriesImport(IntPtr ctx);

            public override int GetNumOfGeometries(IntPtr ctx)
            {
                return GetNumOfGeometriesImport(ctx);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_mesh_config")]
            private static extern Result SetMeshConfigImport(IntPtr ctx, int geometryId, ref MeshConfig config,
                uint propertyMask);

            public override Result SetMeshConfig(IntPtr ctx, int geometryId, ref MeshConfig config, uint propertyMask)
            {
                return SetMeshConfigImport(ctx, geometryId, ref config, propertyMask);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_get_absorption_factor")]
            private static extern Result GetAbsorptionFactorImport(
                AcousticsMaterial material,
                float[] absorptionFactor);

            public override Result GetAbsorptionFactor(
                AcousticsMaterial material,
                float[] absorptionFactor
            )
            {
                return GetAbsorptionFactorImport(material, absorptionFactor);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_get_scattering_factor")]
            private static extern Result GetScatteringFactorImport(
                AcousticsMaterial material,
                ref float scatteringFactor);

            public override Result GetScatteringFactor(
                AcousticsMaterial material,
                ref float scatteringFactor
            )
            {
                return GetScatteringFactorImport(material, ref scatteringFactor);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_get_transmission_factor")]
            private static extern Result GetTransmissionFactorImport(
                AcousticsMaterial material,
                ref float transmissionFactor);

            public override Result GetTransmissionFactor(
                AcousticsMaterial material,
                ref float transmissionFactor
            )
            {
                return GetTransmissionFactorImport(material, ref transmissionFactor);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_commit_scene")]
            private static extern Result CommitSceneImport(IntPtr ctx);

            public override Result CommitScene(IntPtr ctx)
            {
                return CommitSceneImport(ctx);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_add_source")]
            private static extern Result AddSourceImport(
                IntPtr ctx,
                SourceMode sourceMode,
                float[] position,
                ref int sourceId,
                bool isAsync);

            public override Result AddSource(
                IntPtr ctx,
                SourceMode sourceMode,
                float[] position,
                ref int sourceId,
                bool isAsync
            )
            {
                return AddSourceImport(ctx, sourceMode, position, ref sourceId, isAsync);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_add_source_with_orientation")]
            private static extern Result AddSourceWithOrientationImport(
                IntPtr ctx,
                SourceMode mode,
                float[] position,
                float[] front,
                float[] up,
                float radius,
                ref int sourceId,
                bool isAsync);

            public override Result AddSourceWithOrientation(
                IntPtr ctx,
                SourceMode mode,
                float[] position,
                float[] front,
                float[] up,
                float radius,
                ref int sourceId,
                bool isAsync
            )
            {
                return AddSourceWithOrientationImport(ctx, mode, position, front, up, radius, ref sourceId, isAsync);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_add_source_with_config")]
            private static extern Result AddSourceWithConfigImport(
                IntPtr ctx,
                ref SourceConfig sourceConfig,
                ref int sourceId,
                bool isAsync);

            public override Result AddSourceWithConfig(
                IntPtr ctx,
                ref SourceConfig sourceConfig,
                ref int sourceId,
                bool isAsync,
                AudioSource nativeSource,
                bool enablePicoAttenuation = false
            )
            {
                PXR_Plugin.System.UPxr_LogSdkApi("pico_spatial_audio_add_source_with_config|unity_native");
                return AddSourceWithConfigImport(ctx, ref sourceConfig, ref sourceId, isAsync);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_source_config")]
            private static extern Result SetSourceConfigImport(IntPtr ctx, int sourceId, ref SourceConfig sourceConfig,
                uint propertyMask);

            public override Result SetSourceConfig(IntPtr ctx, int sourceId, ref SourceConfig sourceConfig,
                uint propertyMask, AudioSource nativeSource, bool enabledPicoAttenuation = false)
            {
                return SetSourceConfigImport(ctx, sourceId, ref sourceConfig, propertyMask);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_source_attenuation_mode")]
            private static extern Result SetSourceAttenuationModeImport(IntPtr ctx,
                int sourceId,
                SourceAttenuationMode mode,
                DistanceAttenuationCallback directDistanceAttenuationCallback,
                DistanceAttenuationCallback indirectDistanceAttenuationCallback);

            public override Result SetSourceAttenuationMode(
                IntPtr ctx,
                int sourceId,
                SourceAttenuationMode mode,
                DistanceAttenuationCallback directDistanceAttenuationCallback,
                DistanceAttenuationCallback indirectDistanceAttenuationCallback
            )
            {
                return SetSourceAttenuationModeImport(ctx, sourceId, mode, directDistanceAttenuationCallback,
                    indirectDistanceAttenuationCallback);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_source_range")]
            private static extern Result SetSourceRangeImport(IntPtr ctx, int sourceId, float rangeMin, float rangeMax);

            public override Result SetSourceRange(IntPtr ctx, int sourceId, float rangeMin, float rangeMax)
            {
                return SetSourceRangeImport(ctx, sourceId, rangeMin, rangeMax);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_remove_source")]
            private static extern Result RemoveSourceImport(IntPtr ctx, int sourceId, bool is_async);

            public override Result RemoveSource(IntPtr ctx, int sourceId)
            {
                return RemoveSourceImport(ctx, sourceId, true);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_submit_source_buffer")]
            private static extern Result SubmitSourceBufferImport(
                IntPtr ctx,
                int sourceId,
                float[] inputBufferPtr,
                uint numFrames);

            public override Result SubmitSourceBuffer(
                IntPtr ctx,
                int sourceId,
                float[] inputBufferPtr,
                uint numFrames
            )
            {
                return SubmitSourceBufferImport(ctx, sourceId, inputBufferPtr, numFrames);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_submit_ambisonic_channel_buffer")]
            private static extern Result SubmitAmbisonicChannelBufferImport(
                IntPtr ctx,
                float[] ambisonicChannelBuffer,
                int order,
                int degree,
                AmbisonicNormalizationType normType,
                float gain);

            public override Result SubmitAmbisonicChannelBuffer(
                IntPtr ctx,
                float[] ambisonicChannelBuffer,
                int order,
                int degree,
                AmbisonicNormalizationType normType,
                float gain
            )
            {
                return SubmitAmbisonicChannelBufferImport(ctx, ambisonicChannelBuffer, order, degree, normType, gain);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_submit_interleaved_ambisonic_buffer")]
            private static extern Result SubmitInterleavedAmbisonicBufferImport(
                IntPtr ctx,
                float[] ambisonicBuffer,
                int ambisonicOrder,
                AmbisonicNormalizationType normType,
                float gain);

            public override Result SubmitInterleavedAmbisonicBuffer(
                IntPtr ctx,
                float[] ambisonicBuffer,
                int ambisonicOrder,
                AmbisonicNormalizationType normType,
                float gain
            )
            {
                return SubmitInterleavedAmbisonicBufferImport(ctx, ambisonicBuffer, ambisonicOrder, normType, gain);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_submit_matrix_input_buffer")]
            private static extern Result SubmitMatrixInputBufferImport(
                IntPtr ctx,
                float[] inputBuffer,
                int inputChannelIndex);

            public override Result SubmitMatrixInputBuffer(
                IntPtr ctx,
                float[] inputBuffer,
                int inputChannelIndex
            )
            {
                return SubmitMatrixInputBufferImport(ctx, inputBuffer, inputChannelIndex);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_get_interleaved_binaural_buffer")]
            private static extern Result GetInterleavedBinauralBufferImport(
                IntPtr ctx,
                float[] outputBufferPtr,
                uint numFrames,
                bool isAccumulative);

            public override Result GetInterleavedBinauralBuffer(
                IntPtr ctx,
                float[] outputBufferPtr,
                uint numFrames,
                bool isAccumulative
            )
            {
                return GetInterleavedBinauralBufferImport(ctx, outputBufferPtr, numFrames, isAccumulative);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_get_planar_binaural_buffer")]
            private static extern Result GetPlanarBinauralBufferImport(
                IntPtr ctx,
                float[][] outputBufferPtr,
                uint numFrames,
                bool isAccumulative);

            public override Result GetPlanarBinauralBuffer(
                IntPtr ctx,
                float[][] outputBufferPtr,
                uint numFrames,
                bool isAccumulative
            )
            {
                return GetPlanarBinauralBufferImport(ctx, outputBufferPtr, numFrames, isAccumulative);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_get_interleaved_loudspeakers_buffer")]
            private static extern Result GetInterleavedLoudspeakersBufferImport(
                IntPtr ctx,
                float[] outputBufferPtr,
                uint numFrames);

            public override Result GetInterleavedLoudspeakersBuffer(
                IntPtr ctx,
                float[] outputBufferPtr,
                uint numFrames
            )
            {
                return GetInterleavedLoudspeakersBufferImport(ctx, outputBufferPtr, numFrames);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_get_planar_loudspeakers_buffer")]
            private static extern Result GetPlanarLoudspeakersBufferImport(
                IntPtr ctx,
                float[][] outputBufferPtr,
                uint numFrames);

            public override Result GetPlanarLoudspeakersBuffer(
                IntPtr ctx,
                float[][] outputBufferPtr,
                uint numFrames
            )
            {
                return GetPlanarLoudspeakersBufferImport(ctx, outputBufferPtr, numFrames);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_update_scene")]
            private static extern Result UpdateSceneImport(IntPtr ctx);

            public override Result UpdateScene(IntPtr ctx)
            {
                AmbisonicDecoderUpdate();
                return UpdateSceneImport(ctx);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_doppler_effect")]
            private static extern Result SetDopplerEffectImport(IntPtr ctx, int sourceId, bool on);

            public override Result SetDopplerEffect(IntPtr ctx, int sourceId, bool on)
            {
                return SetDopplerEffectImport(ctx, sourceId, on);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_playback_mode")]
            private static extern Result SetPlaybackModeImport(
                IntPtr ctx,
                PlaybackMode playbackMode);

            public override Result SetPlaybackMode(IntPtr ctx, PlaybackMode playbackMode)
            {
                return SetPlaybackModeImport(ctx, playbackMode);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_loudspeaker_array")]
            private static extern Result SetLoudspeakerArrayImport(
                IntPtr ctx,
                float[] positions,
                int numLoudspeakers);

            public override Result SetLoudspeakerArray(
                IntPtr ctx,
                float[] positions,
                int numLoudspeakers
            )
            {
                return SetLoudspeakerArrayImport(ctx, positions, numLoudspeakers);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_mapping_matrix")]
            private static extern Result SetMappingMatrixImport(
                IntPtr ctx,
                float[] matrix,
                int numInputChannels,
                int numOutputChannels);

            public override Result SetMappingMatrix(
                IntPtr ctx,
                float[] matrix,
                int numInputChannels,
                int numOutputChannels
            )
            {
                return SetMappingMatrixImport(ctx, matrix, numInputChannels, numOutputChannels);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_listener_position")]
            private static extern Result SetListenerPositionImport(
                IntPtr ctx,
                float[] position);

            public override Result SetListenerPosition(
                IntPtr ctx,
                float[] position
            )
            {
                return SetListenerPositionImport(ctx, position);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_listener_orientation")]
            private static extern Result SetListenerOrientationImport(
                IntPtr ctx,
                float[] front,
                float[] up);

            public override Result SetListenerOrientation(
                IntPtr ctx,
                float[] front,
                float[] up
            )
            {
                return SetListenerOrientationImport(ctx, front, up);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_listener_pose")]
            private static extern Result SetListenerPoseImport(
                IntPtr ctx,
                float[] position,
                float[] front,
                float[] up);

            public override Result SetListenerPose(
                IntPtr ctx,
                float[] position,
                float[] front,
                float[] up
            )
            {
                return SetListenerPoseImport(ctx, position, front, up);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_source_position")]
            private static extern Result SetSourcePositionImport(
                IntPtr ctx,
                int sourceId,
                float[] position);

            public override Result SetSourcePosition(
                IntPtr ctx,
                int sourceId,
                float[] position
            )
            {
                return SetSourcePositionImport(ctx, sourceId, position);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_source_gain")]
            private static extern Result SetSourceGainImport(
                IntPtr ctx,
                int sourceId,
                float gain);

            public override Result SetSourceGain(
                IntPtr ctx,
                int sourceId,
                float gain
            )
            {
                return SetSourceGainImport(ctx, sourceId, gain);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_source_size")]
            private static extern Result SetSourceSizeImport(
                IntPtr ctx,
                int sourceId,
                float volumetricSize);

            public override Result SetSourceSize(
                IntPtr ctx,
                int sourceId,
                float volumetricSize
            )
            {
                return SetSourceSizeImport(ctx, sourceId, volumetricSize);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_update_source_mode")]
            private static extern Result UpdateSourceModeImport(
                IntPtr ctx,
                int sourceId,
                SourceMode mode);

            public override Result UpdateSourceMode(
                IntPtr ctx,
                int sourceId,
                SourceMode mode
            )
            {
                return UpdateSourceModeImport(ctx, sourceId, mode);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_destroy")]
            private static extern Result DestroyImport(IntPtr ctx);

            public override Result Destroy(IntPtr ctx)
            {
                return DestroyImport(ctx);
            }

            //  Call from Pico's unity native ambisonic decoder
            [DllImport("PicoAmbisonicDecoder", EntryPoint = "yggdrasil_audio_unity_ambisonic_decoder_update")]
            private static extern void AmbisonicDecoderUpdate();
        }

        public class ApiWwiseImpl : Api
        {
#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
            private static string DLLNAME = "__Internal";
#else
            private const string DLLNAME = "PicoSpatializerWwise";
#endif


            [DllImport(DLLNAME, EntryPoint = "yggdrasil_get_version")]
            private static extern string GetVersionImport(ref int major, ref int minor, ref int patch);

            public override string GetVersion(ref int major, ref int minor, ref int patch)
            {
                return GetVersionImport(ref major, ref minor, ref patch);
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoSpatializerWwise_CreateContext")]
            private static extern Result CreateContextImport(ref IntPtr ctx,
                RenderingMode mode,
                uint framesPerBuffer,
                uint sampleRate);

            public override Result
                CreateContext(
                    ref IntPtr ctx,
                    RenderingMode mode,
                    uint framesPerBuffer,
                    uint sampleRate
                )
            {
                PXR_Plugin.System.UPxr_LogSdkApi("pico_spatial_audio_create_context|wwise");
                return CreateContextImport(ref ctx, mode, framesPerBuffer, sampleRate);
            }

            public override Result InitializeContext(IntPtr ctx)
            {
                Debug.Log("Wwise plugin will automatically initialize context after creating.");
                return Result.Success;
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoSpatializerWwise_SubmitMesh")]
            private static extern Result SubmitMeshImport(
                IntPtr ctx,
                float[] vertices,
                int verticesCount,
                int[] indices,
                int indicesCount,
                AcousticsMaterial material,
                ref int geometryId);

            public override Result SubmitMesh(
                IntPtr ctx,
                float[] vertices,
                int verticesCount,
                int[] indices,
                int indicesCount,
                AcousticsMaterial material,
                ref int geometryId
            )
            {
                return SubmitMeshImport(ctx, vertices, verticesCount, indices, indicesCount, material, ref geometryId);
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoSpatializerWwise_SubmitMeshAndMaterialFactor")]
            private static extern Result SubmitMeshAndMaterialFactorImport(
                IntPtr ctx,
                float[] vertices,
                int verticesCount,
                int[] indices,
                int indicesCount,
                float[] absorptionFactor,
                float scatteringFactor,
                float transmissionFactor,
                ref int geometryId);

            public override Result SubmitMeshAndMaterialFactor(
                IntPtr ctx,
                float[] vertices,
                int verticesCount,
                int[] indices,
                int indicesCount,
                float[] absorptionFactor,
                float scatteringFactor,
                float transmissionFactor,
                ref int geometryId)
            {
                return SubmitMeshAndMaterialFactorImport(ctx, vertices, verticesCount, indices, indicesCount,
                    absorptionFactor, scatteringFactor, transmissionFactor, ref geometryId);
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoSpatializerWwise_SubmitMeshWithConfig")]
            private static extern Result SubmitMeshWithConfigImport(IntPtr ctx, float[] vertices, int verticesCount,
                int[] indices,
                int indicesCount,
                ref MeshConfig config, ref int geometryId);

            public override Result SubmitMeshWithConfig(IntPtr ctx, float[] vertices, int verticesCount, int[] indices,
                int indicesCount,
                ref MeshConfig config, ref int geometryId)
            {
                PXR_Plugin.System.UPxr_LogSdkApi("pico_spatial_audio_submit_mesh_with_config|wwise");
                return SubmitMeshWithConfigImport(ctx, vertices, verticesCount, indices, indicesCount, ref config,
                    ref geometryId);
            }

            public override Result UpdateMesh(IntPtr ctx, int geometryId, float[] newVertices, int newVerticesCount,
                int[] newIndices,
                int newIndicesCount, ref MeshConfig config, ref int newGeometryId, bool isAsync = false)
            {
                throw new NotImplementedException();
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoSpatializerWwise_RemoveMesh")]
            private static extern Result RemoveMeshImport(IntPtr ctx, int geometryId);

            public override Result RemoveMesh(IntPtr ctx, int geometryId)
            {
                return RemoveMeshImport(ctx, geometryId);
            }

            public override int GetNumOfGeometries(IntPtr ctx)
            {
                throw new NotImplementedException();
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoSpatializerWwise_SetMeshConfig")]
            private static extern Result SetMeshConfigImport(IntPtr ctx, int geometryId, ref MeshConfig config,
                uint propertyMask);

            public override Result SetMeshConfig(IntPtr ctx, int geometryId, ref MeshConfig config, uint propertyMask)
            {
                return SetMeshConfigImport(ctx, geometryId, ref config, propertyMask);
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoSpatializerWwise_GetAbsorptionFactor")]
            private static extern Result GetAbsorptionFactorImport(
                AcousticsMaterial material,
                float[] absorptionFactor);

            public override Result GetAbsorptionFactor(
                AcousticsMaterial material,
                float[] absorptionFactor
            )
            {
                return GetAbsorptionFactorImport(material, absorptionFactor);
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoSpatializerWwise_GetScatteringFactor")]
            private static extern Result GetScatteringFactorImport(
                AcousticsMaterial material,
                ref float scatteringFactor);

            public override Result GetScatteringFactor(
                AcousticsMaterial material,
                ref float scatteringFactor
            )
            {
                return GetScatteringFactorImport(material, ref scatteringFactor);
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoSpatializerWwise_GetTransmissionFactor")]
            private static extern Result GetTransmissionFactorImport(
                AcousticsMaterial material,
                ref float transmissionFactor);

            public override Result GetTransmissionFactor(
                AcousticsMaterial material,
                ref float transmissionFactor
            )
            {
                return GetTransmissionFactorImport(material, ref transmissionFactor);
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoSpatializerWwise_CommitScene")]
            private static extern Result CommitSceneImport(IntPtr ctx);

            public override Result CommitScene(IntPtr ctx)
            {
                return CommitSceneImport(ctx);
            }


            public override Result AddSource(
                IntPtr ctx,
                SourceMode sourceMode,
                float[] position,
                ref int sourceId,
                bool isAsync
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }


            public override Result AddSourceWithOrientation(
                IntPtr ctx,
                SourceMode mode,
                float[] position,
                float[] front,
                float[] up,
                float radius,
                ref int sourceId,
                bool isAsync
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }


            public override Result AddSourceWithConfig(
                IntPtr ctx,
                ref SourceConfig sourceConfig,
                ref int sourceId,
                bool isAsync,
                AudioSource nativeSource,
                bool enablePicoAttenuation = false
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result SetSourceConfig(IntPtr ctx, int sourceId, ref SourceConfig sourceConfig,
                uint propertyMask, AudioSource nativeSource, bool enabledPicoAttenuation = false)
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result SetSourceAttenuationMode(
                IntPtr ctx,
                int sourceId,
                SourceAttenuationMode mode,
                DistanceAttenuationCallback directDistanceAttenuationCallback,
                DistanceAttenuationCallback indirectDistanceAttenuationCallback
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result SetSourceRange(IntPtr ctx, int sourceId, float rangeMin, float rangeMax)
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result RemoveSource(IntPtr ctx, int sourceId)
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }


            public override Result SubmitSourceBuffer(
                IntPtr ctx,
                int sourceId,
                float[] inputBufferPtr,
                uint numFrames
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }


            public override Result SubmitAmbisonicChannelBuffer(
                IntPtr ctx,
                float[] ambisonicChannelBuffer,
                int order,
                int degree,
                AmbisonicNormalizationType normType,
                float gain
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }


            public override Result SubmitInterleavedAmbisonicBuffer(
                IntPtr ctx,
                float[] ambisonicBuffer,
                int ambisonicOrder,
                AmbisonicNormalizationType normType,
                float gain
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }


            public override Result SubmitMatrixInputBuffer(
                IntPtr ctx,
                float[] inputBuffer,
                int inputChannelIndex
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }


            public override Result GetInterleavedBinauralBuffer(
                IntPtr ctx,
                float[] outputBufferPtr,
                uint numFrames,
                bool isAccumulative
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }


            public override Result GetPlanarBinauralBuffer(
                IntPtr ctx,
                float[][] outputBufferPtr,
                uint numFrames,
                bool isAccumulative
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result GetInterleavedLoudspeakersBuffer(
                IntPtr ctx,
                float[] outputBufferPtr,
                uint numFrames
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }


            public override Result GetPlanarLoudspeakersBuffer(
                IntPtr ctx,
                float[][] outputBufferPtr,
                uint numFrames
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoSpatializerWwise_UpdateScene")]
            private static extern Result UpdateSceneImport(IntPtr ctx);

            public override Result UpdateScene(IntPtr ctx)
            {
                return UpdateSceneImport(ctx);
            }

            public override Result SetDopplerEffect(IntPtr ctx, int sourceId, bool on)
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result SetPlaybackMode(IntPtr ctx, PlaybackMode playbackMode)
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result SetLoudspeakerArray(
                IntPtr ctx,
                float[] positions,
                int numLoudspeakers
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result SetMappingMatrix(
                IntPtr ctx,
                float[] matrix,
                int numInputChannels,
                int numOutputChannels
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result SetListenerPosition(
                IntPtr ctx,
                float[] position
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }


            public override Result SetListenerOrientation(
                IntPtr ctx,
                float[] front,
                float[] up
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result SetListenerPose(
                IntPtr ctx,
                float[] position,
                float[] front,
                float[] up
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result SetSourcePosition(
                IntPtr ctx,
                int sourceId,
                float[] position
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result SetSourceGain(
                IntPtr ctx,
                int sourceId,
                float gain
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result SetSourceSize(
                IntPtr ctx,
                int sourceId,
                float volumetricSize
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result UpdateSourceMode(
                IntPtr ctx,
                int sourceId,
                SourceMode mode
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoSpatializerWwise_Destroy")]
            private static extern Result DestroyImport(IntPtr ctx);

            public override Result Destroy(IntPtr ctx)
            {
                return DestroyImport(ctx);
            }
        }

        public class ApiUnityNativeImpl : Api
        {
#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
            private static string DLLNAME = "__Internal";
#else
            private const string DLLNAME = "PicoAudioSDKUnityNativePlugin";
#endif


            [DllImport(DLLNAME, EntryPoint = "yggdrasil_get_version")]
            private static extern string GetVersionImport(ref int major, ref int minor, ref int patch);

            public override string GetVersion(ref int major, ref int minor, ref int patch)
            {
                return GetVersionImport(ref major, ref minor, ref patch);
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoAudioSDKUnityNativePlugin_ToggleInternalSceneUpdate")]
            private static extern Result CSharp_PicoAudioSDKUnityNativePlugin_ToggleInternalSceneUpdate(bool on);

            public override Result
                CreateContext(
                    ref IntPtr ctx,
                    RenderingMode mode,
                    uint framesPerBuffer,
                    uint sampleRate
                )
            {
                //  Turn off scene update inside native spatializer plugin
                return CSharp_PicoAudioSDKUnityNativePlugin_ToggleInternalSceneUpdate(false);
            }

            public override Result InitializeContext(IntPtr ctx)
            {
                Debug.Log("Unity native spatial audio plugin will automatically initialize context after creating.");
                return Result.Success;
            }

            public override Result SubmitMesh(IntPtr ctx, float[] vertices, int verticesCount, int[] indices,
                int indicesCount,
                AcousticsMaterial material, ref int geometryId)
            {
                throw new NotImplementedException();
            }

            public override Result SubmitMeshAndMaterialFactor(
                IntPtr ctx,
                float[] vertices,
                int verticesCount,
                int[] indices,
                int indicesCount,
                float[] absorptionFactor,
                float scatteringFactor,
                float transmissionFactor,
                ref int geometryId)
            {
                throw new NotImplementedException();
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoAudioSDKUnityNativePlugin_SubmitMeshWithConfig")]
            private static extern Result SubmitMeshWithConfigImport(float[] vertices, int verticesCount,
                int[] indices,
                int indicesCount,
                ref MeshConfig config, ref int geometryId);

            public override Result SubmitMeshWithConfig(IntPtr ctx, float[] vertices, int verticesCount, int[] indices,
                int indicesCount,
                ref MeshConfig config, ref int geometryId)
            {
                return SubmitMeshWithConfigImport(vertices, verticesCount, indices, indicesCount, ref config,
                    ref geometryId);
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoAudioSDKUnityNativePlugin_UpdateMesh")]
            private static extern Result UpdateMeshImport(int geometryId, float[] newVertices, int newVerticesCount,
                int[] newIndices,
                int newIndicesCount, ref MeshConfig config, ref int newGeometryId, bool isAsync = false);

            public override Result UpdateMesh(IntPtr ctx, int geometryId, float[] newVertices, int newVerticesCount,
                int[] newIndices,
                int newIndicesCount, ref MeshConfig config, ref int newGeometryId, bool isAsync = false)
            {
                return UpdateMeshImport(geometryId, newVertices, newVerticesCount, newIndices, newIndicesCount,
                    ref config, ref newGeometryId, isAsync);
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoAudioSDKUnityNativePlugin_RemoveMesh")]
            private static extern Result RemoveMeshImport(int geometryId);

            public override Result RemoveMesh(IntPtr ctx, int geometryId)
            {
                return RemoveMeshImport(geometryId);
            }

            public override int GetNumOfGeometries(IntPtr ctx)
            {
                throw new NotImplementedException();
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoAudioSDKUnityNativePlugin_SetMeshConfig")]
            private static extern Result SetMeshConfigImport(int geometryId, ref MeshConfig config,
                uint propertyMask);

            public override Result SetMeshConfig(IntPtr ctx, int geometryId, ref MeshConfig config, uint propertyMask)
            {
                return SetMeshConfigImport(geometryId, ref config, propertyMask);
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoAudioSDKUnityNativePlugin_GetAbsorptionFactor")]
            private static extern Result GetAbsorptionFactorImport(
                AcousticsMaterial material,
                float[] absorptionFactor);

            public override Result GetAbsorptionFactor(
                AcousticsMaterial material,
                float[] absorptionFactor
            )
            {
                return GetAbsorptionFactorImport(material, absorptionFactor);
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoAudioSDKUnityNativePlugin_GetScatteringFactor")]
            private static extern Result GetScatteringFactorImport(
                AcousticsMaterial material,
                ref float scatteringFactor);

            public override Result GetScatteringFactor(
                AcousticsMaterial material,
                ref float scatteringFactor
            )
            {
                return GetScatteringFactorImport(material, ref scatteringFactor);
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoAudioSDKUnityNativePlugin_GetTransmissionFactor")]
            private static extern Result GetTransmissionFactorImport(
                AcousticsMaterial material,
                ref float transmissionFactor);

            public override Result GetTransmissionFactor(
                AcousticsMaterial material,
                ref float transmissionFactor
            )
            {
                return GetTransmissionFactorImport(material, ref transmissionFactor);
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoAudioSDKUnityNativePlugin_CommitScene")]
            private static extern Result CommitSceneImport();

            public override Result CommitScene(IntPtr ctx)
            {
                return CommitSceneImport();
            }


            public override Result AddSource(
                IntPtr ctx,
                SourceMode sourceMode,
                float[] position,
                ref int sourceId,
                bool isAsync
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }


            public override Result AddSourceWithOrientation(
                IntPtr ctx,
                SourceMode mode,
                float[] position,
                float[] front,
                float[] up,
                float radius,
                ref int sourceId,
                bool isAsync
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }


            public override Result AddSourceWithConfig(
                IntPtr ctx,
                ref SourceConfig sourceConfig,
                ref int sourceId,
                bool isAsync,
                AudioSource nativeSource,
                bool enablePicoAttenuation = false
            )
            {
                //  Get CustomSpatializationData.kId using AudioSource.GetSpatializerFloat
                nativeSource.GetSpatializerFloat((int)CustomSpatializationData.kId, out var sourceIdFloat);

                sourceId = (int)sourceIdFloat;

                //  Setup initial config
                SetSourceConfig(ctx, sourceId, ref sourceConfig, (uint)SourceProperty.All, nativeSource,
                    enablePicoAttenuation);

                return Result.Success;
            }

            private enum CustomSpatializationData
            {
                //  Source id.
                kId = 0,

                //  Directivity
                ///  Directivity alpha
                kDirectivityAlpha = 1,

                ///  Directivity order
                kDirectivityOrder = 2,

                //  Volumetric Radius
                kVolumetricRadius = 3,

                //  Master gain
                kMasterGain = 4,

                //  Reflection gain
                kReflectionGain = 5,

                //  Enable Pico Doppler
                kEnableDoppler = 6,

                //  Attenuation
                kEnablePicoAttenuation = 7,

                //  Pico attenuation settings
                kPicoAttenuationMode = 8,
            };

            public override Result SetSourceConfig(IntPtr ctx, int sourceId, ref SourceConfig sourceConfig,
                uint propertyMask, AudioSource nativeSource, bool enabledPicoAttenuation = false)
            {
                if (nativeSource.clip != null && nativeSource.spatialize)
                {
                    nativeSource.SetSpatializerFloat((int)CustomSpatializationData.kDirectivityAlpha,
                        sourceConfig.directivityAlpha);
                    nativeSource.SetSpatializerFloat((int)CustomSpatializationData.kDirectivityOrder,
                        sourceConfig.directivityOrder);
                    nativeSource.SetSpatializerFloat((int)CustomSpatializationData.kVolumetricRadius,
                        sourceConfig.radius);
                    nativeSource.SetSpatializerFloat((int)CustomSpatializationData.kMasterGain,
                        sourceConfig.sourceGain);
                    nativeSource.SetSpatializerFloat((int)CustomSpatializationData.kReflectionGain,
                        sourceConfig.reflectionGain);
                    nativeSource.SetSpatializerFloat((int)CustomSpatializationData.kEnableDoppler,
                        sourceConfig.enableDoppler ? 1.0f : 0.0f);
                    nativeSource.SetSpatializerFloat((int)CustomSpatializationData.kEnablePicoAttenuation,
                        enabledPicoAttenuation ? 1.0f : 0.0f);
                    nativeSource.SetSpatializerFloat((int)CustomSpatializationData.kPicoAttenuationMode,
                        (float)sourceConfig.attenuationMode);
                    //  Min and max attenuation distance is passed via Unity native callbacks (in native plugin code)
                }

                return Result.Success;
            }

            public override Result SetSourceAttenuationMode(
                IntPtr ctx,
                int sourceId,
                SourceAttenuationMode mode,
                DistanceAttenuationCallback directDistanceAttenuationCallback,
                DistanceAttenuationCallback indirectDistanceAttenuationCallback
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result SetSourceRange(IntPtr ctx, int sourceId, float rangeMin, float rangeMax)
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result RemoveSource(IntPtr ctx, int sourceId)
            {
                return Result.Success;
            }


            public override Result SubmitSourceBuffer(
                IntPtr ctx,
                int sourceId,
                float[] inputBufferPtr,
                uint numFrames
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }


            public override Result SubmitAmbisonicChannelBuffer(
                IntPtr ctx,
                float[] ambisonicChannelBuffer,
                int order,
                int degree,
                AmbisonicNormalizationType normType,
                float gain
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }


            public override Result SubmitInterleavedAmbisonicBuffer(
                IntPtr ctx,
                float[] ambisonicBuffer,
                int ambisonicOrder,
                AmbisonicNormalizationType normType,
                float gain
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }


            public override Result SubmitMatrixInputBuffer(
                IntPtr ctx,
                float[] inputBuffer,
                int inputChannelIndex
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }


            public override Result GetInterleavedBinauralBuffer(
                IntPtr ctx,
                float[] outputBufferPtr,
                uint numFrames,
                bool isAccumulative
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }


            public override Result GetPlanarBinauralBuffer(
                IntPtr ctx,
                float[][] outputBufferPtr,
                uint numFrames,
                bool isAccumulative
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result GetInterleavedLoudspeakersBuffer(
                IntPtr ctx,
                float[] outputBufferPtr,
                uint numFrames
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }


            public override Result GetPlanarLoudspeakersBuffer(
                IntPtr ctx,
                float[][] outputBufferPtr,
                uint numFrames
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoAudioSDKUnityNativePlugin_UpdateScene")]
            private static extern Result UpdateSceneImport();

            public override Result UpdateScene(IntPtr ctx)
            {
                return UpdateSceneImport();
            }

            public override Result SetDopplerEffect(IntPtr ctx, int sourceId, bool on)
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result SetPlaybackMode(IntPtr ctx, PlaybackMode playbackMode)
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result SetLoudspeakerArray(
                IntPtr ctx,
                float[] positions,
                int numLoudspeakers
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result SetMappingMatrix(
                IntPtr ctx,
                float[] matrix,
                int numInputChannels,
                int numOutputChannels
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result SetListenerPosition(
                IntPtr ctx,
                float[] position
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }


            public override Result SetListenerOrientation(
                IntPtr ctx,
                float[] front,
                float[] up
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result SetListenerPose(
                IntPtr ctx,
                float[] position,
                float[] front,
                float[] up
            )
            {
                return Result.Success;
            }

            public override Result SetSourcePosition(
                IntPtr ctx,
                int sourceId,
                float[] position
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result SetSourceGain(
                IntPtr ctx,
                int sourceId,
                float gain
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result SetSourceSize(
                IntPtr ctx,
                int sourceId,
                float volumetricSize
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result UpdateSourceMode(
                IntPtr ctx,
                int sourceId,
                SourceMode mode
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoAudioSDKUnityNativePlugin_Destroy")]
            private static extern Result DestroyImport();

            public override Result Destroy(IntPtr ctx)
            {
                //  Turn on scene update inside native spatializer plugin
                CSharp_PicoAudioSDKUnityNativePlugin_ToggleInternalSceneUpdate(false);
                return DestroyImport();
            }
        }
    }
}