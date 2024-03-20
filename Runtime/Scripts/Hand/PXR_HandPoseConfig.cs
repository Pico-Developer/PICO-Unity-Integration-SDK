/*******************************************************************************
Copyright © 2015-2022 PICO Technology Co., Ltd.All rights reserved.  

NOTICE：All information contained herein is, and remains the property of 
PICO Technology Co., Ltd. The intellectual and technical concepts 
contained herein are proprietary to PICO Technology Co., Ltd. and may be 
covered by patents, patents in process, and are protected by trade secret or 
copyright law. Dissemination of this information or reproduction of this 
material is strictly forbidden unless prior written permission is obtained from
PICO Technology Co., Ltd. 
*******************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR
{
    [Serializable]
    public class PXR_HandPoseConfig : ScriptableObject
    {
        [DisplayOnly]
        public ShapesRecognizer shapesRecognizer;
        [DisplayOnly]
        public BonesRecognizer bonesRecognizer;
        [DisplayOnly]
        public TransRecognizer transRecognizer;
    }

    [Serializable]
    public class ShapesRecognizer
    {
        public Finger thumb = new Finger(HandFinger.Thumb);
        public Finger index = new Finger(HandFinger.Index);
        public Finger middle = new Finger(HandFinger.Middle);
        public Finger ring = new Finger(HandFinger.Ring);
        public Finger pinky = new Finger(HandFinger.Pinky);
        public float holdDuration = 0.09f;
        [Serializable]
        public class Finger
        {
            [HideInInspector]
            public HandFinger handFinger;
            public Flexion flexion;
            public Curl curl;
            public Abduction abduction;
            public FingerConfigs fingerConfigs;

            public Finger(HandFinger finger)
            {
                handFinger = finger;
                flexion = Flexion.Any;
                curl = Curl.Any;
                abduction = Abduction.Any;
                fingerConfigs = new FingerConfigs(finger);
            }
        }
        [Serializable]
        public class FingerConfigs
        {
            public RangeConfigs flexionConfigs;
            public RangeConfigs curlConfigs;
            public RangeConfigsAbduction abductionConfigs;

            public FingerConfigs(HandFinger finger)
            {
                flexionConfigs = new RangeConfigs(flexionMin, flexionMax, defaultFlexionWidth);
                if (finger == HandFinger.Thumb)
                {
                    curlConfigs = new RangeConfigs(curlThumbMin, curlThumbMax, defaultCurlWidth);
                    abductionConfigs = new RangeConfigsAbduction(abductionThumbMid, abductionThumbWidth);
                }
                else
                {
                    curlConfigs = new RangeConfigs(curlMin, curlMax, defaultCurlWidth);
                    abductionConfigs = new RangeConfigsAbduction(abductionMid, abductionWidth);
                }
            }
        }

        public enum ShapeType
        {
            flexion,
            curl,
            abduction
        }

        public enum Flexion
        {
            Any,
            Open,
            Close,
            //Custom
        }

        public enum Curl
        {
            Any,
            Open,
            Close,
            //Custom
        }

        public enum Abduction
        {
            Any,
            Open,
            Close,
        }
        [Serializable]
        public class RangeConfigs
        {
            public float min;
            public float max;
            public float width;
            public RangeConfigs(float n, float m, float w)
            {
                min = n;
                max = m;
                width =w;
            }
        }
        [Serializable]
        public class RangeConfigsAbduction
        {
            public float mid;
            public float width;
            public RangeConfigsAbduction(float m, float w)
            {
                mid = m;
                width = w;
            }
        }

        public const float defaultFlexionWidth = 10f;

        public const float flexionThumbOpenMin = 155f;
        public const float flexionThumbOpenMax = 180f;
        public const float flexionThumbCloseMin = 90f;
        public const float flexionThumbCloseMax = 120f;

        public const float flexionOpenMin = 144f;
        public const float flexionOpenMax = 180f;
        public const float flexionCloseMin = 90f;
        public const float flexionCloseMax = 126f;
        public const float flexionMin = 90f;
        public const float flexionMax = 180f;

        public const float defaultCurlWidth = 20f;

        public const float curlThumbOpenMin = 90f;
        public const float curlThumbOpenMax = 180f;
        public const float curlThumbCloseMin = 45f;
        public const float curlThumbCloseMax = 90f;
        public const float curlThumbMin = 45f;
        public const float curlThumbMax = 180f;

        public const float curlOpenMin = 107f;
        public const float curlOpenMax = 180f;
        public const float curlCloseMin = 0f;
        public const float curlCloseMax = 73f;
        public const float curlMin = 0f;
        public const float curlMax = 180f;

        public const float abductionThumbMid = 13f;
        public const float abductionThumbWidth = 6f;

        public const float abductionMid = 10f;
        public const float abductionWidth = 6f;
        public const float abductionMin = 0f;
        public const float abductionMax = 90f;
    }

    [Serializable]
    public class BonesRecognizer
    {
        public List<BonesGroup> Bones = new List<BonesGroup>();

        public float holdDuration = 0.022f;
        [Serializable]
        public class BonesGroup
        {
            [LabelAttribute("Joint 1")]
            public HandBones bone1 = HandBones.Wrist;
            [LabelAttribute("Joint 2")]
            public HandBones bone2 = HandBones.Wrist;
            public float distance = 0.025f;
            [LabelAttribute("Margin")]
            public float thresholdWidth = 0.003f;

            [HideInInspector]
            public bool activeState;
        }
        public enum HandBones
        {
            Palm = 0,
            Wrist = 1,

            Thumb_Metacarpal = 2,
            Thumb_Proximal = 3,
            Thumb_Distal = 4,
            Thumb_Tip = 5,

            Index_Metacarpal = 6,
            Index_Proximal = 7,
            Index_Intermediate = 8,
            Index_Distal = 9,
            Index_Tip = 10,

            Middle_Metacarpal = 11,
            Middle_Proximal = 12,
            Middle_Intermediate = 13,
            Middle_Distal = 14,
            Middle_Tip = 15,

            Ring_Metacarpal = 16,
            Ring_Proximal = 17,
            Ring_Intermediate = 18,
            Ring_Distal = 19,
            Ring_Tip = 20,

            Little_Metacarpal = 21,
            Little_Proximal = 22,
            Little_Intermediate = 23,
            Little_Distal = 24,
            Little_Tip = 25
        }
    }

    [Serializable]
    public class TransRecognizer
    {
        public TrackAxis trackAxis;
        public SpaceType spaceType;
        public TrackTarget trackTarget;

        public enum SpaceType
        {
            WorldSpace,
            LocalXY,
            LocalYZ,
            LocalXZ
        }

        public enum TrackAxis
        {
            Fingers, Palm, Thumb
        }

        public enum TrackTarget
        {
            TowardsFace,
            AwayFromFace,
            WorldUp,
            WorldDown,
        }

        public float angleThreshold = 35f;
        public float thresholdWidth = 10f;
        public float holdDuration = 0.022f;
    }

    public class DisplayOnly : PropertyAttribute { }
    public class LabelAttribute : PropertyAttribute
    {
        public string name;
        public LabelAttribute(string name)
        {
            this.name = name;
        }
    }
}


