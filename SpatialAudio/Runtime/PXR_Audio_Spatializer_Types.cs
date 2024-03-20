//  Copyright © 2015-2022 Pico Technology Co., Ltd. All Rights Reserved.

using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace PXR_Audio
{
    namespace Spatializer
    {
        public enum Result
        {
            Error = -1,
            Success = 0,
            SourceNotFound = -1001,
            SourceDataNotFound = -1002,
            SceneNotFound = -1003,
            SceneMeshNotFound = -1004,
            IllegalValue = -1005,
            ContextNotCreated = -1006,
            ContextNotReady = -1007,
            ContextRepeatedInitialization = -1008,
            EnvironmentalAcousticsDisabled = -1009,
            ApiDisabled = -1010,

            ///< API is disabled in current build
            SourceInuse = -1011,
        };

        public enum PlaybackMode
        {
            BinauralOut,
            LoudspeakersOut,
        };

        public enum LateReverbUpdatingMode
        {
            RealtimeLateReverb = 0,
            BakedLateReverb = 1,
            SharedSpectralLateReverb = 2,
        };

        public enum LateReverbRenderingMode
        {
            IrLateReverb = 0,
            SpectralLateReverb = 1,
        };

        public enum RenderingMode
        {
            LowQuality = 0, // 1st order ambisonic
            MediumQuality = 1, // 3rd order ambisonic
            HighQuality = 2, // 5th order ambisonic
            AmbisonicFirstOrder,
            AmbisonicSecondOrder,
            AmbisonicThirdOrder,
            AmbisonicFourthOrder,
            AmbisonicFifthOrder,
            AmbisonicSixthOrder,
            AmbisonicSeventhOrder,
        };

        public enum SourceMode
        {
            Spatialize = 0,
            Bypass = 1,
        };

        public enum IRUpdateMethod
        {
            PerPartitionSwapping = 0,
            InterPartitionLinearCrossFade = 1,
            InterPartitionPowerComplementaryCrossFade = 2
        };

        public enum AcousticsMaterial
        {
            AcousticTile,
            Brick,
            BrickPainted,
            Carpet,
            CarpetHeavy,
            CarpetHeavyPadded,
            CeramicTile,
            Concrete,
            ConcreteRough,
            ConcreteBlock,
            ConcreteBlockPainted,
            Curtain,
            Foliage,
            Glass,
            GlassHeavy,
            Grass,
            Gravel,
            GypsumBoard,
            PlasterOnBrick,
            PlasterOnConcreteBlock,
            Soil,
            SoundProof,
            Snow,
            Steel,
            Water,
            WoodThin,
            WoodThick,
            WoodFloor,
            WoodOnConcrete,
            Custom
        };

        public enum AmbisonicNormalizationType
        {
            SN3D,
            N3D
        };

        public enum SourceAttenuationMode
        {
            None = 0, // 引擎不依据距离计算衰减
            Fixed = 1, // 与None完全一致
            InverseSquare = 2, // 引擎 InverseSquare Law 计算距离衰减
            Customized = 3, // 依据外部传入的 Callback 计算距离衰减
        };

        public enum SpatializerApiImpl
        {
            unity,
            wwise,
        }

        public delegate float DistanceAttenuationCallback(float distance, float rangeMin, float rangeMax);

        [StructLayout(LayoutKind.Sequential)]
        public struct NativeVector3f
        {
            public float x; //float[3]
            public float y;
            public float z;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SourceConfig
        {
            [MarshalAs(UnmanagedType.U4)] public SourceMode mode;
            public NativeVector3f position;
            public NativeVector3f front;
            public NativeVector3f up;

            public float directivityAlpha; // Weighting balance between figure of eight pattern and circular pattern for

            // source emission in range [0, 1].
            // A value of 0 results in a circular pattern.
            // A value of 0.5 results in a cardioid pattern.
            // A value of 1 results in a figure of eight pattern.
            public float
                directivityOrder; // Order applied to computed directivity. Higher values will result in narrower and

            public float radius;

            // sharper directivity patterns. Range [1, inf).
            [MarshalAs(UnmanagedType.U1)] public bool useDirectPathSpread;

            public float directPathSpread; // Alternatively, we could use spread param directly.

            // This is useful when audio middleware specifies spread value by itself.
            public float sourceGain; // Master gain of sound source.
            public float reflectionGain; // Reflection gain relative to default (master gain).

            [MarshalAs(UnmanagedType.U1)] public bool enableDoppler;

            [MarshalAs(UnmanagedType.U4)] public SourceAttenuationMode attenuationMode;

            public IntPtr
                directDistanceAttenuationCallback; // Native function pointer of direct sound distance attenuation

            public IntPtr indirectDistanceAttenuationCallback;

            // Attenuation range
            public float minAttenuationDistance; // When distance < minAttenuationDistance, no attenuation.

            public float
                maxAttenuationDistance; // When distance > maxAttenuationDistance, attenuation = AttenuationFunc(range_max).

            public SourceConfig(SourceMode inMode)
            {
                mode = inMode;
                position.x = 0.0f;
                position.y = 0.0f;
                position.z = 0.0f;
                front.x = 0.0f;
                front.y = 0.0f;
                front.z = -1.0f;
                up.x = 0.0f;
                up.y = 1.0f;
                up.z = 0.0f;
                radius = 0.1f;
                directivityAlpha = 0.0f;
                directivityOrder = 1.0f;
                useDirectPathSpread = false;
                directPathSpread = 0.0f;
                sourceGain = 1.0f;
                reflectionGain = 1.0f;
                enableDoppler = false;
                attenuationMode = SourceAttenuationMode.InverseSquare;
                directDistanceAttenuationCallback = IntPtr.Zero;
                indirectDistanceAttenuationCallback = IntPtr.Zero;
                minAttenuationDistance = 0.25f;
                maxAttenuationDistance = 250f;
            }
        }

        public enum SourceProperty : uint
        {
            Mode = 1u,
            Position = (1u << 1),

            ///< float[3]
            Orientation = (1u << 2),

            ///< float[6]
            Directivity = (1u << 3),

            ///< float[2], directivity alpha and directivity order
            VolumetricRadius = (1u << 4),
            VolumetricSpread = (1u << 5),
            SourceGain = (1u << 6),
            ReflectionGain = (1u << 7),
            DopplerOnOff = (1u << 8),
            AttenuationMode = (1u << 9),

            ///< Only after setting AttenuationMode will AttenuationCallback be applied
            DirectAttenuationCallback = (1u << 10),
            IndirectAttenuationCallback = (1u << 11),
            RangeMin = (1u << 12),
            RangeMax = (1u << 13),
            All = ~0u,
            None = 0u,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NativeVector4f
        {
            public float v0; //float[4]
            public float v1;
            public float v2;
            public float v3;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NativeMatrix4x4f
        {
            public float v0; //float[16]
            public float v1;
            public float v2;
            public float v3;
            public float v4; //float[16]
            public float v5;
            public float v6;
            public float v7;
            public float v8; //float[16]
            public float v9;
            public float v10;
            public float v11;
            public float v12;
            public float v13;
            public float v14;
            public float v15;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MeshConfig
        {
            [MarshalAs(UnmanagedType.U1)] public bool enabled;
            [MarshalAs(UnmanagedType.U4)] public AcousticsMaterial materialType;

            ///< Material preset; If this equal to YGG_MATERIAL_Custom, the absorption,
            ///< scattering, and transmission coefficients below will be used
            public NativeVector4f absorption;

            ///< Absorption of 4 bands
            public float scattering;

            ///< Wide-band scattering
            public float transmission;

            ///< Wide-band transmission
            public NativeMatrix4x4f toWorldTransform;

            ///< Column-major 4x4 to-world transform matrix of this mesh, which
            ///< describes the position, rotation, and scale of it's default to
            ///< identity matrix, which represents a scene mesh positioned at
            ///< world origin, with no rotation and no scaling what's so ever
            public MeshConfig(bool enabled, PXR_Audio_Spatializer_SceneMaterial material, Matrix4x4 toWorldMatrix4X4)
            {
                this.enabled = enabled;
                materialType = material.materialPreset;
                absorption.v0 = material.absorption[0];
                absorption.v1 = material.absorption[1];
                absorption.v2 = material.absorption[2];
                absorption.v3 = material.absorption[3];
                scattering = material.scattering;
                transmission = material.transmission;
                toWorldTransform.v0 = toWorldMatrix4X4[0];
                toWorldTransform.v1 = toWorldMatrix4X4[1];
                toWorldTransform.v2 = -toWorldMatrix4X4[2];
                toWorldTransform.v3 = toWorldMatrix4X4[3];
                toWorldTransform.v4 = toWorldMatrix4X4[4];
                toWorldTransform.v5 = toWorldMatrix4X4[5];
                toWorldTransform.v6 = -toWorldMatrix4X4[6];
                toWorldTransform.v7 = toWorldMatrix4X4[7];
                toWorldTransform.v8 = toWorldMatrix4X4[8];
                toWorldTransform.v9 = toWorldMatrix4X4[9];
                toWorldTransform.v10 = -toWorldMatrix4X4[10];
                toWorldTransform.v11 = toWorldMatrix4X4[11];
                toWorldTransform.v12 = toWorldMatrix4X4[12];
                toWorldTransform.v13 = toWorldMatrix4X4[13];
                toWorldTransform.v14 = -toWorldMatrix4X4[14];
                toWorldTransform.v15 = toWorldMatrix4X4[15];
            }

            public void SetMaterial(PXR_Audio_Spatializer_SceneMaterial material)
            {
                materialType = material.materialPreset;
                absorption.v0 = material.absorption[0];
                absorption.v1 = material.absorption[1];
                absorption.v2 = material.absorption[2];
                absorption.v3 = material.absorption[3];
                scattering = material.scattering;
                transmission = material.transmission;
            }
            
            public void SetTransformMatrix4x4(Matrix4x4 toWorldMatrix4X4)
            {
                toWorldTransform.v0 = toWorldMatrix4X4[0];
                toWorldTransform.v1 = toWorldMatrix4X4[1];
                toWorldTransform.v2 = -toWorldMatrix4X4[2];
                toWorldTransform.v3 = toWorldMatrix4X4[3];
                toWorldTransform.v4 = toWorldMatrix4X4[4];
                toWorldTransform.v5 = toWorldMatrix4X4[5];
                toWorldTransform.v6 = -toWorldMatrix4X4[6];
                toWorldTransform.v7 = toWorldMatrix4X4[7];
                toWorldTransform.v8 = toWorldMatrix4X4[8];
                toWorldTransform.v9 = toWorldMatrix4X4[9];
                toWorldTransform.v10 = -toWorldMatrix4X4[10];
                toWorldTransform.v11 = toWorldMatrix4X4[11];
                toWorldTransform.v12 = toWorldMatrix4X4[12];
                toWorldTransform.v13 = toWorldMatrix4X4[13];
                toWorldTransform.v14 = -toWorldMatrix4X4[14];
                toWorldTransform.v15 = toWorldMatrix4X4[15];
            }
        }

        enum MeshProperty : uint
        {
            Enabled = 1u,
            Material = (1u << 1),
            Absorption = (1u << 2),
            Scattering = (1u << 3),
            Transmission = (1u << 4),
            ToWorldTransform = (1u << 5),
            All = ~0u,
            None = 0u,
        }
    }
}