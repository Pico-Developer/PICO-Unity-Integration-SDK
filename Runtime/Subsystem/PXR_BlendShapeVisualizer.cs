#if AR_FOUNDATION
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR
{
    public class PXR_BlendShapeVisualizer : MonoBehaviour
    {
        [SerializeField]
        float m_CoefficientScale = 100.0f;

        public float coefficientScale
        {
            get { return m_CoefficientScale; }
            set { m_CoefficientScale = value; }
        }

        [SerializeField]
        SkinnedMeshRenderer m_SkinnedMeshRenderer;

        public SkinnedMeshRenderer skinnedMeshRenderer
        {
            get
            {
                return m_SkinnedMeshRenderer;
            }
            set
            {
                m_SkinnedMeshRenderer = value;
                CreateFeatureBlendMapping();
            }
        }

        private PXR_FaceSubsystem m_PICOFaceSubsystem;
        private Dictionary<BlendShapeIndex, int> m_FaceBlendShapeIndexMap;
        private PxrFaceTrackingInfo ftInfo = new PxrFaceTrackingInfo();


        void Awake()
        {
            CreateFeatureBlendMapping();
        }

        void CreateFeatureBlendMapping()
        {
            if (skinnedMeshRenderer == null || skinnedMeshRenderer.sharedMesh == null)
            {
                return;
            }

            const string strPrefix = "blendShape2.";
            m_FaceBlendShapeIndexMap = new Dictionary<BlendShapeIndex, int>();

            m_FaceBlendShapeIndexMap[BlendShapeIndex.EyeLookDown_L] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookDown_L");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.NoseSneer_L] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "noseSneer_L");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.EyeLookIn_L] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookIn_L");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.BrowInnerUp] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "browInnerUp");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.BrowDown_R] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "browDown_R");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthClose] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthClose");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthLowerDown_R] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthLowerDown_R");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.JawOpen] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "jawOpen");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthUpperUp_R] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthUpperUp_R");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthShrugUpper] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthShrugUpper");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthFunnel] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthFunnel");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.EyeLookIn_R] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookIn_R");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.EyeLookDown_R] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookDown_R");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.NoseSneer_R] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "noseSneer_R");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthRollUpper] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthRollUpper");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.JawRight] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "jawRight");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.BrowDown_L] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "browDown_L");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthShrugLower] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthShrugLower");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthRollLower] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthRollLower");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthSmile_L] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthSmile_L");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthPress_L] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthPress_L");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthSmile_R] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthSmile_R");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthPress_R] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthPress_R");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthDimple_R] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthDimple_R");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthLeft] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthLeft");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.JawForward] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "jawForward");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.EyeSquint_L] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeSquint_L");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthFrown_L] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthFrown_L");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.EyeBlink_L] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeBlink_L");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.CheekSquint_L] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "cheekSquint_L");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.BrowOuterUp_L] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "browOuterUp_L");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.EyeLookUp_L] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookUp_L");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.JawLeft] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "jawLeft");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthStretch_L] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthStretch_L");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthPucker] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthPucker");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.EyeLookUp_R] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookUp_R");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.BrowOuterUp_R] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "browOuterUp_R");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.CheekSquint_R] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "cheekSquint_R");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.EyeBlink_R] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeBlink_R");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthUpperUp_L] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthUpperUp_L");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthFrown_R] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthFrown_R");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.EyeSquint_R] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeSquint_R");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthStretch_R] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthStretch_R");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.CheekPuff] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "cheekPuff");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.EyeLookOut_L] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookOut_L");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.EyeLookOut_R] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeLookOut_R");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.EyeWide_R] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeWide_R");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.EyeWide_L] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "eyeWide_L");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthDimple_L] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthDimple_L");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthLowerDown_L] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthLowerDown_L");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.MouthRight] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "mouthRight");
            m_FaceBlendShapeIndexMap[BlendShapeIndex.TongueOut] = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "tongueOut");
        }

        void Update()
        {
            if (skinnedMeshRenderer == null || !skinnedMeshRenderer.enabled || skinnedMeshRenderer.sharedMesh == null)
            {
                return;
            }

            UpdateBlendShapeWeight();
        }

        unsafe private void UpdateBlendShapeWeight()
        {
            PXR_FaceSubsystem.GetBlendShapeCoefficients(ref ftInfo);
            if (ftInfo.videoInputValid[0] == 1)
            {
                for (int i = 0; i < PXR_FaceSubsystem.FACE_COUNT; i++)
                {
                    int mappedBlendShapeIndex;
                    if (m_FaceBlendShapeIndexMap.TryGetValue((BlendShapeIndex)i, out mappedBlendShapeIndex))
                    {
                        if (mappedBlendShapeIndex >= 0)
                        {
                            skinnedMeshRenderer.SetBlendShapeWeight(mappedBlendShapeIndex, ftInfo.blendShapeWeight[i] * coefficientScale);
                        }
                    }
                }
            }
        }

    }
}
#endif