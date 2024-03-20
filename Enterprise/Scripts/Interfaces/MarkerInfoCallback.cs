using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.XR;
#if PICO_XR
using Unity.XR.PXR;
#else
using Unity.XR.OpenXR.Features.PICOSupport;
#endif

namespace Unity.XR.PICO.TOBSupport
{
    public class MarkerInfoCallback : AndroidJavaProxy
    {
        public Action<List<MarkerInfo>> mCallback;
        private List<MarkerInfo> mlist = new List<MarkerInfo>();
        private TrackingOriginModeFlags TrackingMode;
        private float YOffset;

        public MarkerInfoCallback(TrackingOriginModeFlags trackingMode, float cameraYOffset,
            Action<List<MarkerInfo>> callback) : base("com.picoxr.tobservice.interfaces.StringCallback")
        {
            TrackingMode = trackingMode;
            YOffset = cameraYOffset;
            mCallback = callback;
            mlist.Clear();
#if PICO_XR
#else
            OpenXRExtensions.SetMarkMode();
#endif
        }

        public void CallBack(string var1)
        {
            Debug.Log("ToBService MarkerInfo Callback 回调:" + var1);
            List<MarkerInfo> tmp = JsonToMarkerInfos(var1);
            PXR_EnterpriseTools.QueueOnMainThread(() =>
            {
                if (mCallback != null)
                {
                    mCallback(tmp);
                }
            });
        }

        public List<MarkerInfo> JsonToMarkerInfos(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            List<MarkerInfo> ModelList = new List<MarkerInfo>();
            JsonData jsonData = JsonMapper.ToObject(json);
            IDictionary dictionary = jsonData as IDictionary;
            for (int i = 0; i < dictionary.Count; i++)
            {
                Debug.Log("TOB TestDemo---- MarkerInfo Callback 回调:1");

                float OriginHeight = 0;
                if (TrackingMode == TrackingOriginModeFlags.Device || TrackingMode == TrackingOriginModeFlags.Floor)
                {
              
#if PICO_XR
                    OriginHeight = PXR_Plugin.System.UPxr_GetConfigFloat(ConfigType.ToDelaSensorY);
#else
                    float trackingorigin_height = PXR_EnterprisePlugin.oxr_get_trackingorigin_height();
                    float locationheight = OpenXRExtensions.GetLocationHeight();
                    if (TrackingMode == TrackingOriginModeFlags.Floor)
                    {
                        YOffset = 0;
                        OriginHeight = -trackingorigin_height;
                    }
                    else
                    {
                        // OriginHeight = trackingorigin_height + locationheight;
                        // OriginHeight = locationheight;
                        OriginHeight = -trackingorigin_height;
                    }
#endif
                }
                else
                {
                    OriginHeight = 0;
                    YOffset = 0;
                } 
   
                Debug.Log("TOB TestDemo---- MarkerInfo Callback 回调:OriginHeight："+OriginHeight );
                MarkerInfo model = new MarkerInfo();
                model.posX = double.Parse(jsonData[i]["posX"].ToString());
                model.posY = double.Parse(jsonData[i]["posY"].ToString()) + OriginHeight + YOffset;
                model.posZ = -double.Parse(jsonData[i]["posZ"].ToString());

                model.rotationX = -double.Parse(jsonData[i]["rotationX"].ToString());
                model.rotationY = -double.Parse(jsonData[i]["rotationY"].ToString());
                model.rotationZ = double.Parse(jsonData[i]["rotationZ"].ToString());
                model.rotationW = double.Parse(jsonData[i]["rotationW"].ToString());

                model.validFlag = int.Parse(jsonData[i]["validFlag"].ToString());
                model.markerType = int.Parse(jsonData[i]["markerType"].ToString());
                model.iMarkerId = int.Parse(jsonData[i]["iMarkerId"].ToString());
                model.dTimestamp = double.Parse(jsonData[i]["dTimestamp"].ToString());

                IDictionary dictionaryReserve = jsonData[i]["reserve"] as IDictionary;
                model.reserve = new float[dictionaryReserve.Count];
                for (int j = 0; j < dictionaryReserve.Count; j++)
                {
                    model.reserve[j] = float.Parse(jsonData[i]["reserve"][j].ToString());
                }

                ModelList.Add(model);
            }

            return ModelList;
        }
    }
}