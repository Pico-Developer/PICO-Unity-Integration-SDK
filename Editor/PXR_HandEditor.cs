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
using UnityEditor;
using Unity.XR.PXR;

[CustomEditor(typeof(PXR_Hand))]
public class PXR_HandEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        serializedObject.ApplyModifiedProperties();

        PXR_Hand hand = (PXR_Hand)target;

        EditorGUILayout.LabelField("Hand Joints", EditorStyles.boldLabel);

        for (int i = 0; i < (int)HandJoint.JointMax; i++)
        {
            string jointName = ((HandJoint)i).ToString();
            hand.handJoints[i] = (Transform)EditorGUILayout.ObjectField(jointName, hand.handJoints[i], typeof(Transform), true);
        }
    }
}

