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
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
#if UNITY_6000_0_OR_NEWER
using UnityEngine.XR.Interaction.Toolkit.Interactables;
#endif

namespace Unity.XR.PXR
{
    [DisallowMultipleComponent]
    public class PXR_SpatialAnchor : MonoBehaviour
    {
        private const string TAG = "[PXR_SpatialAnchor]";

        [HideInInspector]
        public bool Created = false;
        [HideInInspector]
        public ulong anchorHandle;
        [HideInInspector]
        public Guid anchorUuid;

        // Start is called before the first frame update
        void Start()
        {
            if (!Created)
            {
                CreateSpatialAnchor();
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Created)
            {
                var result = PXR_MixedReality.LocateAnchor(anchorHandle, out var position, out var rotation);
                if (result == PxrResult.SUCCESS)
                {
                    transform.SetPositionAndRotation(position, rotation);
                }
            }
        }

        void OnDestroy()
        {
            var result = PXR_MixedReality.DestroyAnchor(anchorHandle);
            if (result != PxrResult.SUCCESS)
            {
                PLog.e(TAG, "DestroySpatialAnchor Fail: " + result, false);
            }
        }

        private async void CreateSpatialAnchor()
        {
            var result = await PXR_MixedReality.CreateSpatialAnchorAsync(transform.position, transform.rotation);
            if (result.result == PxrResult.SUCCESS)
            {
                anchorHandle = result.anchorHandle;
                anchorUuid = result.uuid;
                Created = true;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public async Task<bool> Persist()
        {
            var result = await PXR_MixedReality.PersistSpatialAnchorAsync(anchorHandle);
            if (result == PxrResult.SUCCESS)
            {
                return true;
            }
            else
            {
                PLog.e(TAG, "PersistSpatialAnchor Fail: " + result, false);
                return false;
            }
        }

        public async Task<bool> UnPersist()
        {
            var result = await PXR_MixedReality.UnPersistSpatialAnchorAsync(anchorHandle);
            if (result == PxrResult.SUCCESS)
            {
                return true;
            }
            else
            {
                PLog.e(TAG, "UnPersistSpatialAnchor Fail: " + result, false);
                return false;
            }
        }

        public void DestroySpatialAnchor()
        {
            var result = PXR_MixedReality.DestroyAnchor(anchorHandle);
            if (result == PxrResult.SUCCESS)
            {
                Destroy(gameObject);
            }
            else
            {
                PLog.e(TAG, "DestroySpatialAnchor Fail: " + result, false);
            }
        }
    }
}