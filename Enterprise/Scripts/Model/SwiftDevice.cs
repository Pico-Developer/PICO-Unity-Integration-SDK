using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PICO.TOBSupport
{
    [System.Serializable]
    public class SwiftDevice
    {
        public const int STATUS_OFFLINE = 0;
        public const int STATUS_ONLINE = 1;
        public const int POSITION_UNDEFINED = 0;
        public const int POSITION_LEFT = 1;
        public const int POSITION_RIGHT = 2;
        public const int POSITION_CENTER = 3;
        public const int BIND_NONE = 0;
        public const int BIND_DONE = 1;
        public const int ID_ALL = 0;
        public const int ID_T1 = 1;
        public const int ID_T2 = 2;
        public const int ID_T3 = 3;
        public const int CHARGE_STATUS_NONE = 0;
        public const int CHARGE_STATUS_PRE = 1;
        public const int CHARGE_STATUS_GOING = 2;
        public const int CHARGE_STATUS_DONE = 3;
        public const int BATTERY_LOW = 0;
        
        
        public int connectState;
        public int position;
        public int bindState;
        public int id;
        public string fwVersion;
        public string hwVersion;
        public string sn;
        public string addr;
        public int chargeStatus;
        public int battery;
        public int imuType;
        public int generation;
        public SwiftDevice()
        {
            connectState = 0;
            position = 0;
            bindState = 0;
            id = 0;
            fwVersion = string.Empty;
            hwVersion = string.Empty;
            sn = string.Empty;
            addr = string.Empty;
            chargeStatus = 0;
            battery = 0;
            imuType = 0;
            generation = 0;
        }
    }
    [Serializable]
    public class SwiftDeviceListWrapper
    {
        public List<SwiftDevice> SwiftDevices;
    }
    public partial class JsonParser
    {
        public static SwiftDevice ParseSwiftDeviceFromJson(string json)
        {
            try
            {
                // 使用 Unity 的 JsonUtility 解析 JSON 字符串为 Pose 对象
                return JsonUtility.FromJson<SwiftDevice>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"JSON 解析出错: {ex.Message}");
                return null;
            }
        }
        public static List<SwiftDevice> ParseSwiftDeviceArrayFromJson(string json)
        {
            try
            {
                // 先解析到包装类
                SwiftDeviceListWrapper wrapper = JsonUtility.FromJson<SwiftDeviceListWrapper>(json);
                if (wrapper != null && wrapper.SwiftDevices != null)
                {
                    return wrapper.SwiftDevices;
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"JSON 解析出错: {ex.Message}");
                return null;
            }
        }
        public static string SwiftDeviceArrayToJson(List<SwiftDevice> devices)
        {
            try
            {
                // 创建包装类实例
                SwiftDeviceListWrapper wrapper = new SwiftDeviceListWrapper
                {
                    SwiftDevices = devices
                };
                // 使用 JsonUtility.ToJson 方法将包装类对象转换为 JSON 字符串
                return JsonUtility.ToJson(wrapper);
            }
            catch (Exception ex)
            {
                Debug.LogError($"对象数组转 JSON 出错: {ex.Message}");
                return null;
            }
        }
    }
}