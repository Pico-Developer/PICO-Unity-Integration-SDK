using System;

namespace Unity.XR.PICO.TOBSupport
{
    public class OffLineSystemUpdateConfig
    {
        //OTA压缩包路径
        public String otaFilePath = null;
        //升级完成后是否自动重启
        public Boolean autoReboot = true;
        //升级过程中是否显示进度
        public Boolean showProgress = false;
        public OffLineSystemUpdateConfig()
        {
        }

        public OffLineSystemUpdateConfig(string otaFilePath, bool autoReboot, bool showProgress)
        {
            this.otaFilePath = otaFilePath;
            this.autoReboot = autoReboot;
            this.showProgress = showProgress;
        }
    }
}