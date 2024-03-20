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

using UnityEngine;
using UnityEngine.XR;

namespace Unity.XR.PXR
{
    public static class PXR_Usages
    {
        public static InputFeatureUsage<Vector3> combineEyePoint = new InputFeatureUsage<Vector3>("CombinedEyeGazePoint");
        public static InputFeatureUsage<Vector3> combineEyeVector = new InputFeatureUsage<Vector3>("CombinedEyeGazeVector");
        public static InputFeatureUsage<Vector3> leftEyePoint = new InputFeatureUsage<Vector3>("LeftEyeGazePoint");
        public static InputFeatureUsage<Vector3> leftEyeVector = new InputFeatureUsage<Vector3>("LeftEyeGazeVector");
        public static InputFeatureUsage<Vector3> rightEyePoint = new InputFeatureUsage<Vector3>("RightEyeGazePoint");
        public static InputFeatureUsage<Vector3> rightEyeVector = new InputFeatureUsage<Vector3>("RightEyeGazeVector");
        public static InputFeatureUsage<float> leftEyeOpenness = new InputFeatureUsage<float>("LeftEyeOpenness");
        public static InputFeatureUsage<float> rightEyeOpenness = new InputFeatureUsage<float>("RightEyeOpenness");
        public static InputFeatureUsage<uint> leftEyePoseStatus = new InputFeatureUsage<uint>("LeftEyePoseStatus");
        public static InputFeatureUsage<uint> rightEyePoseStatus = new InputFeatureUsage<uint>("RightEyePoseStatus");
        public static InputFeatureUsage<uint> combinedEyePoseStatus = new InputFeatureUsage<uint>("CombinedEyePoseStatus");
        public static InputFeatureUsage<float> leftEyePupilDilation = new InputFeatureUsage<float>("LeftEyePupilDilation");
        public static InputFeatureUsage<float> rightEyePupilDilation = new InputFeatureUsage<float>("RightEyePupilDilation");
        public static InputFeatureUsage<Vector3> leftEyePositionGuide = new InputFeatureUsage<Vector3>("LeftEyePositionGuide");
        public static InputFeatureUsage<Vector3> rightEyePositionGuide = new InputFeatureUsage<Vector3>("RightEyePositionGuide");
        public static InputFeatureUsage<Vector3> foveatedGazeDirection = new InputFeatureUsage<Vector3>("FoveatedGazeDirection");
        public static InputFeatureUsage<uint> foveatedGazeTrackingState = new InputFeatureUsage<uint>("FoveatedGazeTrackingState");
        public static InputFeatureUsage<bool> triggerTouch = new InputFeatureUsage<bool>("TriggerTouch");
        public static InputFeatureUsage<float> grip1DAxis = new InputFeatureUsage<float>("Grip1DAxis");
        public static InputFeatureUsage<bool> controllerStatus = new InputFeatureUsage<bool>("ControllerStatus");

    }
}

