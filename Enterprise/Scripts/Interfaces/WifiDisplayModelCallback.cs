using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace Unity.XR.PICO.TOBSupport
{
    public class WifiDisplayModelCallback : AndroidJavaProxy
    {
        public Action<List<WifiDisplayModel>> mCallback;

        public WifiDisplayModelCallback(Action<List<WifiDisplayModel>> callback) : base("com.picoxr.tobservice.interfaces.StringCallback")
        {
            mCallback = callback;
        }

        public void CallBack(string var1)
        {
            Debug.Log("ToBService WifiDisplayModelCallback 回调:" + var1);
            List<WifiDisplayModel> tmp = JsonToWifiDisplayModel(var1);
            PXR_EnterpriseTools.QueueOnMainThread(() =>
            {
                if (mCallback != null)
                {
                    mCallback(tmp);
                }
            });
        }
        
        public List<WifiDisplayModel> JsonToWifiDisplayModel(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            List<WifiDisplayModel> ModelList = new List<WifiDisplayModel>();
            JsonData jsonData = JsonMapper.ToObject(json);
            IDictionary dictionary = jsonData as IDictionary;
            for (int i = 0; i < dictionary.Count; i++)
            {
                WifiDisplayModel model = new WifiDisplayModel();
                model.deviceAddress = jsonData[i]["deviceAddress"].ToString();
                model.deviceName = jsonData[i]["deviceName"].ToString();
                model.isAvailable = bool.Parse(jsonData[i]["isAvailable"].ToString());
                model.canConnect = bool.Parse(jsonData[i]["canConnect"].ToString());
                model.isRemembered = bool.Parse(jsonData[i]["isRemembered"].ToString());
                model.statusCode = int.Parse(jsonData[i]["statusCode"].ToString());
                model.status = jsonData[i]["status"].ToString();
                model.description = jsonData[i]["description"].ToString();

                ModelList.Add(model);
            }

            return ModelList;
        }
    }
}