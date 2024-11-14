namespace Unity.XR.PICO.TOBSupport
{
    public class WifiConfiguration
    {
        public WifiConfiguration(string ssid, string password, bool isClient=true)
        {
            this.ssid = ssid;
            this.password = password;
            this.isClient = isClient;
        }
        public WifiConfiguration()
        {
            this.ssid = "";
            this.password = "";
            this.isClient = true;
        }
        public override string ToString()
        {
            return $"{nameof(ssid)}: {ssid}, {nameof(password)}: {password}, {nameof(isClient)}: {isClient}";
        }

        public string ssid;
        public string password;
        public bool isClient;
    }
}