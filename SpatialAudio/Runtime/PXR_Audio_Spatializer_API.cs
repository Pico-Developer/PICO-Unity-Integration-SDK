//  Copyright © 2015-2022 Pico Technology Co., Ltd. All Rights Reserved.

using System;
using System.Runtime.InteropServices;
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

            public abstract Result RemoveMesh(IntPtr ctx, int geometryId);

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
                bool isAsync);

            public abstract Result SetSourceConfig(IntPtr ctx, int sourceId, ref SourceConfig sourceConfig,
                uint propertyMask);

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
                ref MeshConfig config, ref int geometryId);

            public override Result SubmitMeshWithConfig(IntPtr ctx, float[] vertices, int verticesCount, int[] indices,
                int indicesCount,
                ref MeshConfig config, ref int geometryId)
            {
                return SubmitMeshWithConfigImport(ctx, vertices, verticesCount, indices, indicesCount, ref config,
                    ref geometryId);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_remove_mesh")]
            private static extern Result RemoveMeshImport(IntPtr ctx, int geometryId);

            public override Result RemoveMesh(IntPtr ctx, int geometryId)
            {
                return RemoveMeshImport(ctx, geometryId);
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
                bool isAsync
            )
            {
                return AddSourceWithConfigImport(ctx, ref sourceConfig, ref sourceId, isAsync);
            }

            [DllImport(DLLNAME, EntryPoint = "yggdrasil_audio_set_source_config")]
            private static extern Result SetSourceConfigImport(IntPtr ctx, int sourceId, ref SourceConfig sourceConfig,
                uint propertyMask);

            public override Result SetSourceConfig(IntPtr ctx, int sourceId, ref SourceConfig sourceConfig,
                uint propertyMask)
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
                return SubmitMeshWithConfigImport(ctx, vertices, verticesCount, indices, indicesCount, ref config,
                    ref geometryId);
            }

            [DllImport(DLLNAME, EntryPoint = "CSharp_PicoSpatializerWwise_RemoveMesh")]
            private static extern Result RemoveMeshImport(IntPtr ctx, int geometryId);

            public override Result RemoveMesh(IntPtr ctx, int geometryId)
            {
                return RemoveMeshImport(ctx, geometryId);
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
                bool isAsync
            )
            {
                Debug.LogWarning("Unexpected API calling.");
                return Result.Error;
            }

            public override Result SetSourceConfig(IntPtr ctx, int sourceId, ref SourceConfig sourceConfig,
                uint propertyMask)
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
    }
}