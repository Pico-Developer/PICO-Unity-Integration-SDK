using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PICO.TOBSupport
{

    // 包装 Pose 列表的类，用于 JsonUtility 解析
    [Serializable]
    public class PoseListWrapper
    {
        public List<Pose> Poses;
    }
    [Serializable]
    public class Pose
    {
        public long timestamp;
        public double x;
        public double y;
        public double z;
        public double rw;
        public double rx;
        public double ry;
        public double rz;
        public int type;
        public int confidence;
        public int poseError;
        public  List<int> reservedInt;
        public List<double> reservedDouble;
        
        
        public Pose()
        {
            timestamp = 0;
            x = 0.0;
            y = 0.0;
            z = 0.0;
            rw = 0.0;
            rx = 0.0;
            ry = 0.0;
            rz = 0.0;
            type = 0;
            confidence = 0;
            poseError = 0;
            reservedInt = new List<int>();
            reservedDouble = new List<double>();
        }
        
    }

    public partial class JsonParser
    {
        public static Pose ParsePoseFromJson(string json)
        {
            try
            {
                // 使用 Unity 的 JsonUtility 解析 JSON 字符串为 Pose 对象
                return JsonUtility.FromJson<Pose>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"JSON 解析出错: {ex.Message}");
                return null;
            }
        }
        public static List<Pose> ParsePoseArrayFromJson(string json)
        {
            try
            {
                // 先解析到包装类
                PoseListWrapper wrapper = JsonUtility.FromJson<PoseListWrapper>(json);
                if (wrapper != null && wrapper.Poses != null)
                {
                    return wrapper.Poses;
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogError($"JSON 解析出错: {ex.Message}");
                return null;
            }
        }
        // 新增方法：将 Pose 对象转换为 JSON 字符串
        public static string PoseToJson(Pose pose)
        {
            try
            {
                // 使用 JsonUtility.ToJson 方法将 Pose 对象转换为 JSON 字符串
                return JsonUtility.ToJson(pose);
            }
            catch (Exception ex)
            {
                Debug.LogError($"对象转 JSON 出错: {ex.Message}");
                return null;
            }
        }

        // 新增方法：将 Pose 数组转换为 JSON 字符串
        public static string PoseArrayToJson(List<Pose> poses)
        {
            try
            {
                // 创建包装类实例
                PoseListWrapper wrapper = new PoseListWrapper
                {
                    Poses =poses
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