using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.XR.PXR.SecureMR
{
    public class PXR_SecureMRRenderTextOperatorConfig : PXR_SecureMROperatorConfig
    {
        public SecureMRFontTypeface typeface = SecureMRFontTypeface.Default;
        public string languageAndLocale;
        public int width;
        public int height;
    }
}