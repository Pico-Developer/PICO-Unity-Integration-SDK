using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PICO.TOBSupport
{
    public class Intent
    {
        private string Componentpkg = "";
        private string Componentcls = "";
        private string Action = "";
        private string type = "";
        private string url = "";

        List<string> categoryList = new List<string>();
        Dictionary<string, string> stringPairs = new Dictionary<string, string>();
        Dictionary<string, bool> boolPairs = new Dictionary<string, bool>();
        Dictionary<string, int> intPairs = new Dictionary<string, int>();
        Dictionary<string, float> floatPairs = new Dictionary<string, float>();
        Dictionary<string, double> doublePairs = new Dictionary<string, double>();

        public void setComponent(string pkg, string cls)
        {
            Componentpkg = pkg;
            Componentcls = cls;
        }

        public void setAction(string _Action)
        {
            Action = _Action;
        }

        public void setType(string _type)
        {
            type = _type;
        }

        public void setData(string _url)
        {
            url = _url;
        }

        public void addCategory(string _category)
        {
            categoryList.Add(_category);
        }

        public void putExtra(string name, string value)
        {
            stringPairs.Add(name, value);
        }

        public void putExtra(string name, int value)
        {
            intPairs.Add(name, value);
        }

        public void putExtra(string name, float value)
        {
            floatPairs.Add(name, value);
        }

        public void putExtra(string name, double value)
        {
            doublePairs.Add(name, value);
        }

        public void putExtra(string name, bool value)
        {
            boolPairs.Add(name, value);
        }

        public AndroidJavaObject getIntent()
        {
            AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
            if (!string.IsNullOrEmpty(Componentpkg) && !string.IsNullOrEmpty(Componentcls))
            {
                AndroidJavaObject componentName =
                    new AndroidJavaObject("android.content.ComponentName", Componentpkg, Componentcls);
                intent.Call<AndroidJavaObject>("setComponent", componentName);
            }

            if (!string.IsNullOrEmpty(Action))
            {
                intent.Call<AndroidJavaObject>("setAction", Action);
            }

            if (!string.IsNullOrEmpty(type))
            {
                intent.Call<AndroidJavaObject>("setType", type);
            }

            // mIntent.setData(Uri.parse(""));
            if (!string.IsNullOrEmpty(url))
            {
                AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri"); // 对应的安卓调用函数是Uri.parse()
                AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", url);
                intent.Call<AndroidJavaObject>("setData", uriObject);
            }

            if (categoryList.Count != 0)
            {
                for (int i = 0; i < categoryList.Count; i++)
                {
                    intent.Call<AndroidJavaObject>("addCategory", categoryList[i]);
                }
            }

            foreach (KeyValuePair<string, string> kvp in stringPairs)
            {
                intent.Call<AndroidJavaObject>("putExtra", kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<string, int> kvp in intPairs)
            {
                intent.Call<AndroidJavaObject>("putExtra", kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<string, bool> kvp in boolPairs)
            {
                intent.Call<AndroidJavaObject>("putExtra", kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<string, float> kvp in floatPairs)
            {
                intent.Call<AndroidJavaObject>("putExtra", kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<string, double> kvp in doublePairs)
            {
                intent.Call<AndroidJavaObject>("putExtra", kvp.Key, kvp.Value);
            }

            return intent;
        }
    }
}