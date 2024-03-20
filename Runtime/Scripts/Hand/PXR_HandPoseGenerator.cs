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

using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR
{
    public class PXR_HandPoseGenerator : MonoBehaviour
    {

        public PXR_HandPoseConfig config;
        public PXR_HandPosePreview preview;

        //Shapes
        public ShapesRecognizer.Finger thumb = new ShapesRecognizer.Finger(HandFinger.Thumb);
        public ShapesRecognizer.Finger index = new ShapesRecognizer.Finger(HandFinger.Index);
        public ShapesRecognizer.Finger middle = new ShapesRecognizer.Finger(HandFinger.Middle);
        public ShapesRecognizer.Finger ring = new ShapesRecognizer.Finger(HandFinger.Ring);
        public ShapesRecognizer.Finger pinky = new ShapesRecognizer.Finger(HandFinger.Pinky);

        public float shapesholdDuration = 0.09f;

        //Bones
        public List<BonesRecognizer.BonesGroup> Bones = new List<BonesRecognizer.BonesGroup>();

        public float bonesHoldDuration = 0.022f;

        //Trans
        public TransRecognizer.TrackAxis trackAxis;
        public TransRecognizer.SpaceType spaceType;
        public TransRecognizer.TrackTarget trackTarget;

        public float angleThreshold = 35f;
        public float thresholdWidth = 10f;
        public float transHoldDuration = 0.022f;
    }
}
