#if PICO_OPENXR_SDK
using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR;

namespace Unity.XR.OpenXR.Features.PICOSupport
{
    public class LayerBase : MonoBehaviour
    {
        public static int ID = 0;
        private Transform overlayTransform;
        private Camera xrRig;

        private Vector3 modelTranslations;
        private Quaternion modelRotations;
        private Vector3 modelScales ;
        private XROrigin cameraRig;
        private XROrigin lastcameraRig;
        public bool isXROriginChange = false;
        private float offsetY = 0;
        bool isUpdateOffsetY= false;
        private Vector3 cameraPosOri;
        private  TrackingOriginModeFlags lastTrackingOriginMod = TrackingOriginModeFlags.Unknown;
        public void Awake()
        {
            ID++;
            lastcameraRig=cameraRig=FindActiveXROrigin();
            overlayTransform = GetComponent<Transform>();
            PXR_Plugin.System.RecenterSuccess+=()=>
            {
                 isUpdateOffsetY = true;
            };
         
            
#if UNITY_ANDROID && !UNITY_EDITOR
            if (overlayTransform != null)
            {
                MeshRenderer render = overlayTransform.GetComponent<MeshRenderer>();
                if (render != null)
                {
                    render.enabled = false;
                }
            }
#endif
        }
        
        XROrigin FindActiveXROrigin()
        {
            XROrigin[] xrOrigins = FindObjectsOfType<XROrigin>();
            foreach (XROrigin xrOrigin in xrOrigins)
            {
                if (xrOrigin.gameObject.activeInHierarchy)
                {
                    return xrOrigin;
                }
            }
            return null;
        }
      
        private void OnDestroy()
        {
            ID--;
        }
      
        public void UpdateCoords(bool isCreate = false)
        {
            if (isXROriginChange)
            {
                cameraRig=FindActiveXROrigin();
                isUpdateOffsetY=cameraRig!= lastcameraRig;
                lastcameraRig = cameraRig;
            }

            if (isCreate)
            {
                cameraPosOri=cameraRig.transform.position;
            }
            if (isCreate||cameraRig.CurrentTrackingOriginMode != lastTrackingOriginMod ||isUpdateOffsetY)
            {
                
                if (cameraRig.CurrentTrackingOriginMode == TrackingOriginModeFlags.Floor)
                {
                    offsetY= cameraRig.Camera.transform.position.y;
                }
                Debug.Log("CurrentTrackingOriginMode:"+cameraRig.CurrentTrackingOriginMode+" offsetY:"+offsetY);
                isUpdateOffsetY=false;
                lastTrackingOriginMod = cameraRig.CurrentTrackingOriginMode;
            }

           
            var worldInsightModel = GetTransformMatrixForPassthrough(overlayTransform.localToWorldMatrix);
            modelTranslations=worldInsightModel.GetPosition();
            modelRotations = worldInsightModel.rotation;
            modelScales = overlayTransform.lossyScale;
        }

        private Matrix4x4 GetTransformMatrixForPassthrough(Matrix4x4 worldFromObj)
        {
            Matrix4x4 trackingSpaceFromWorld =
                (cameraRig != null) ? cameraRig.CameraFloorOffsetObject.transform.worldToLocalMatrix : Matrix4x4.identity;
            
            return trackingSpaceFromWorld * worldFromObj;
        }
        public void GetCurrentTransform(ref GeometryInstanceTransform geometryInstanceTransform)
        {
            geometryInstanceTransform.pose.position.x = modelTranslations.x;
            geometryInstanceTransform.pose.position.y = modelTranslations.y-offsetY+ (cameraRig.CurrentTrackingOriginMode == TrackingOriginModeFlags.Floor
                ? (cameraRig.transform.position.y - cameraPosOri.y)
                : 0);
            geometryInstanceTransform.pose.position.z = -modelTranslations.z;
            geometryInstanceTransform.pose.orientation.x = -modelRotations.x;
            geometryInstanceTransform.pose.orientation.y = -modelRotations.y;
            geometryInstanceTransform.pose.orientation.z = modelRotations.z;
            geometryInstanceTransform.pose.orientation.w = modelRotations.w;

            geometryInstanceTransform.scale.x = modelScales.x;
            geometryInstanceTransform.scale.y = modelScales.y;
            geometryInstanceTransform.scale.z = 1;

            geometryInstanceTransform.isFloor = cameraRig.CurrentTrackingOriginMode == TrackingOriginModeFlags.Floor;
        }
    }
}
#endif